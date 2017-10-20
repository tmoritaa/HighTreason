using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HighTreasonGame.GameStates;

namespace HighTreasonGame
{
    public abstract class CardTemplate
    {
        public delegate void CardEffect(Game game, BoardChoices choices);

        public delegate BoardChoices CardChoice(Game game, ChoiceHandler choiceHandler);

        public string Name {
            get; private set;
        }

        // TODO: for now until we have enough cards.
        public void SetName(string name)
        {
            Name = name;
        }
            
        public int ActionPts {
            get; private set;
        }

        public List<CardEffect> SelectionEvents {
            get; private set;
        }

        public List<CardChoice> SelectionEventChoices {
            get; private set;
        }

        public List<CardEffect> TrialEvents {
            get; private set;
        }

        public List<CardChoice> TrialEventChoices {
            get; private set;
        }

        public List<CardEffect> SummationEvents {
            get; private set;
        }

        public List<CardChoice> SummationEventChoices {
            get; private set;
        }

        public CardTemplate(string _name, int _actionPts)
        {
            Name = _name;
            ActionPts = _actionPts;

            SelectionEvents = new List<CardEffect>();
            TrialEvents = new List<CardEffect>();
            SummationEvents = new List<CardEffect>();

            SelectionEventChoices = new List<CardChoice>();
            TrialEventChoices = new List<CardChoice>();
            SummationEventChoices = new List<CardChoice>();

            addSelectionEventsAndChoices();
            addTrialEventsAndChoices();
            addSummationEventsAndChoices();
        }

        public bool PlayAsEvent(Game game, int idx, ChoiceHandler choiceHandler)
        {
            GameState.GameStateType curStateType = game.CurState.StateType;

            CardChoice cardChoice = null;
            CardEffect cardEffect = null;

            if (curStateType == GameState.GameStateType.JurySelection)
            {
                cardChoice = SelectionEventChoices[idx];
                cardEffect = SelectionEvents[idx];
            }
            else if (curStateType == GameState.GameStateType.TrialInChief)
            {
                cardChoice = TrialEventChoices[idx];
                cardEffect = TrialEvents[idx];
            }
            else if (curStateType == GameState.GameStateType.Summation)
            {
                cardChoice = SummationEventChoices[idx];
                cardEffect = SummationEvents[idx];
            }

            System.Diagnostics.Debug.Assert(cardChoice != null && cardEffect != null, "Card choice or card effect is null. Should never happen");

            BoardChoices choices = cardChoice(game, choiceHandler);

            if (choices.NotCancelled)
            {
                cardEffect(game, choices);
            }

            return choices.NotCancelled;
        }

        public bool PlayAsAction(Game game, ChoiceHandler choiceHandler)
        {
            bool isSummation = game.CurState.GetType() == typeof(SummationState);

            int modValue = (game.CurPlayer.Side == Player.PlayerSide.Prosecution) ? 1 : -1;
            List<BoardObject> choices = game.FindBO(
                (Type t) =>
                {
                    return (t != typeof(Card));
                },
                (BoardObject bo) =>
                {
                    return (bo.Properties.Contains(Property.Track) &&
                    ((bo.Properties.Contains(Property.Jury) && bo.Properties.Contains(Property.Sway))
                    || (!isSummation && bo.Properties.Contains(Property.Aspect))))
                    && ((Track)bo).CanModifyByAction(modValue);
                }).ToList();

            int actionPtsForState = isSummation ? 2 : ActionPts;

            BoardChoices boardChoices;
            choiceHandler.ChooseBoardObjects(choices,
                HTUtility.GenActionValidateChoicesFunc(actionPtsForState, null),
                HTUtility.GenActionFilterChoicesFunc(actionPtsForState, null),
                HTUtility.GenActionChoicesCompleteFunc(actionPtsForState, null),
                game,
                out boardChoices);

            if (boardChoices.NotCancelled)
            {
                foreach (BoardObject bo in boardChoices.SelectedObjs.Keys)
                {
                    if (bo.GetType() == typeof(AspectTrack))
                    {
                        ((AspectTrack)bo).ModTrackByAction(modValue * boardChoices.SelectedObjs[bo]);
                    }
                    else
                    {
                        ((Track)bo).AddToValue(modValue * boardChoices.SelectedObjs[bo]);
                    }
                }
            }

            return boardChoices.NotCancelled;
        }

        public int GetNumberOfEventsInState(GameState.GameStateType type)
        {
            int num = 0;

            if (type == GameState.GameStateType.JurySelection)
            {
                num = SelectionEventChoices.Count;
            }
            else if (type == GameState.GameStateType.TrialInChief)
            {
                num = TrialEventChoices.Count;
            }
            else if (type == GameState.GameStateType.Summation)
            {
                num = SummationEventChoices.Count;
            }

            return num;
        }

        protected abstract void addSelectionEventsAndChoices();
        protected abstract void addTrialEventsAndChoices();
        protected abstract void addSummationEventsAndChoices();

        #region Choice Utility

        protected BoardChoices doNothingChoice(Game game, ChoiceHandler choiceHandler)
        {
            BoardChoices choices = new BoardChoices();
            return choices;
        }

        protected bool handleMomentOfInsightChoice(List<Player.PlayerSide> supportedSides, Game game, ChoiceHandler choiceHandler, out BoardChoices.MomentOfInsightInfo moiInfo)
        {
            moiInfo = new BoardChoices.MomentOfInsightInfo();
            if (!supportedSides.Contains(game.CurPlayer.Side))
            {
                moiInfo.Use = BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.NotChosen;
                return true;
            }

            return choiceHandler.ChooseMomentOfInsightUse(game, out moiInfo);
        }

        protected CardChoice genAspectTrackForModCardChoice(HashSet<Property> optionProps, int numChoices, int modValue, bool affectedByPlayerSide)
        {
            return
                (Game game, ChoiceHandler choiceHandler) =>
                {
                    List<BoardObject> options = game.FindBO(
                        (Type t) =>
                        {
                            return (t == typeof(AspectTrack));
                        },
                        (BoardObject htgo) =>
                        {
                            HashSet<Property> props = new HashSet<Property>(optionProps);
                            props.Add(Property.Aspect);
                            props.Add(Property.Track);

                            bool valid = true;
                            foreach (Property prop in props)
                            {
                                valid &= htgo.Properties.Contains(prop);
                            }

                            int mod = affectedByPlayerSide ? calcModValueBasedOnSide(modValue, game) : modValue;
                            return valid && ((Track)htgo).CanModify(mod);
                        });

                    BoardChoices boardChoices;
                    choiceHandler.ChooseBoardObjects(options,
                        (Dictionary<BoardObject, int> selected) => { return true; },
                        (List<BoardObject> remainingChoices, Dictionary<BoardObject, int> selected) =>
                        {
                            return remainingChoices.Where(obj => !selected.ContainsKey(obj)).ToList();
                        },
                        (Dictionary<BoardObject, int> selected) => { return selected.Keys.Count == numChoices; },
                        game,
                        out boardChoices);

                    return boardChoices;
                };
        }

        protected CardChoice genRevealOrPeakCardChoice(HashSet<Property> optionProps, 
            int numChoices, 
            bool isReveal,
            Func<Dictionary<BoardObject, int>, bool> validateChoices = null,
            Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>> filterChoices = null)
        {
            return
                (Game game, ChoiceHandler choiceHandler) =>
                {
                    List<Jury.JuryAspect> juryAspects = new List<Jury.JuryAspect>();

                    List<BoardObject> options = game.FindBO(
                        (Type t) =>
                        {
                            return (t == typeof(Jury.JuryAspect));
                        },
                        (BoardObject htgo) =>
                        {
                            HashSet<Property> props = new HashSet<Property>(optionProps);
                            props.Add(Property.Jury);
                            props.Add(Property.Aspect);

                            bool valid = true;
                            foreach (Property prop in props)
                            {
                                valid &= htgo.Properties.Contains(prop);
                            }

                            return valid && (isReveal ? !((Jury.JuryAspect)htgo).IsRevealed : !((Jury.JuryAspect)htgo).IsVisibleToPlayer(game.CurPlayer.Side));
                        });

                    BoardChoices boardChoices;
                    choiceHandler.ChooseBoardObjects(options,
                        validateChoices != null ? validateChoices : 
                        (Dictionary<BoardObject, int> selected) => { return true; },
                        filterChoices != null ? filterChoices : 
                        (List<BoardObject> remainingChoices, Dictionary<BoardObject, int> selected) =>
                        {
                            return remainingChoices.Where(obj => !selected.ContainsKey(obj)).ToList();
                        },
                        (Dictionary<BoardObject, int> selected) => { return selected.Keys.Count == numChoices; },
                        game,
                        out boardChoices);

                    return boardChoices;
                };
        }

        #endregion

        #region Effect Utility

        // Common enough in cards that we'll just simplify it for ourselves.
        protected void raiseGuiltAndOneAspectEffect(Game game, BoardChoices choices)
        {
            game.GetGuiltTrack().AddToValue(1);
            choices.SelectedObjs.Keys.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(1));
        }

        protected void revealAllAspects(Game game, BoardChoices choices)
        {
            choices.SelectedObjs.Keys.Cast<Jury.JuryAspect>().ToList().ForEach(a => a.Reveal());
        }

        protected void peekAllAspects(Game game, BoardChoices choices)
        {
            choices.SelectedObjs.Keys.Cast<Jury.JuryAspect>().ToList().ForEach(a => a.Peek(game.CurPlayer.Side));
        }

        protected int calcModValueBasedOnSide(int value, Game game)
        {
            return value * ((game.CurPlayer.Side == Player.PlayerSide.Prosecution) ? 1 : -1);
        }

        protected void handleMomentOfInsight(Game game, BoardChoices choices)
        {
            if (choices.MoIInfo.Use == BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Reveal)
            {
                game.GetOtherPlayer().RevealCardInSummation();
            }
            else if (choices.MoIInfo.Use == BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Swap)
            {
                Player player = game.CurPlayer;

                player.SummationDeck.MoveCard(choices.MoIInfo.HandCard);
                player.Hand.MoveCard(choices.MoIInfo.SummationCard);
            }
        }
        #endregion
    }
}
