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

        public delegate BoardChoices CardChoice(Game game, IChoiceHandler choiceHandler);

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

        public bool PlayAsEvent(Game game, int idx, IChoiceHandler choiceHandler)
        {
            Type curStateType = game.CurState.GetType();

            CardChoice cardChoice = null;
            CardEffect cardEffect = null;

            if (curStateType == typeof(JurySelectionState))
            {
                cardChoice = SelectionEventChoices[idx];
                cardEffect = SelectionEvents[idx];
            }
            else if (curStateType == typeof(TrialInChiefState))
            {
                cardChoice = TrialEventChoices[idx];
                cardEffect = TrialEvents[idx];
            }
            else if (curStateType == typeof(SummationState))
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

        public bool PlayAsAction(Game game, IChoiceHandler choiceHandler)
        {
            bool isSummation = game.CurState.GetType() == typeof(SummationState);

            int modValue = (game.CurPlayer.Side == Player.PlayerSide.Prosecution) ? 1 : -1;
            List<Track> choices = game.GetHTGOFromCondition(
                (HTGameObject htgo) =>
                {
                    return (htgo.Properties.Contains(Property.Track) &&
                    ((htgo.Properties.Contains(Property.Jury) && htgo.Properties.Contains(Property.Sway))
                    || (!isSummation && htgo.Properties.Contains(Property.Aspect))))
                    && ((Track)htgo).CanModifyByAction(modValue);
                }).Cast<Track>().ToList();

            int actionPtsForState = isSummation ? 2 : ActionPts;
            Dictionary<Track, int> affectedTracks;
            bool choiceMade = choiceHandler.ChooseActionUsage(choices, actionPtsForState, game, out affectedTracks);

            if (choiceMade)
            {
                foreach (Track track in affectedTracks.Keys)
                {
                    if (track.GetType() == typeof(AspectTrack))
                    {
                        ((AspectTrack)track).ModTrackByAction(modValue * affectedTracks[track]);
                    }
                    else
                    {
                        track.AddToValue(modValue * affectedTracks[track]);
                    }
                }
            }

            return choiceMade;
        }

        public int GetNumberOfEventsInState(Type type)
        {
            int num = 0;

            if (type == typeof(JurySelectionState))
            {
                num = SelectionEventChoices.Count;
            }
            else if (type == typeof(TrialInChiefState))
            {
                num = TrialEventChoices.Count;
            }
            else if (type == typeof(SummationState))
            {
                num = SummationEventChoices.Count;
            }

            return num;
        }

        protected abstract void addSelectionEventsAndChoices();
        protected abstract void addTrialEventsAndChoices();
        protected abstract void addSummationEventsAndChoices();

        #region Choice Utility

        protected bool handleMomentOfInsightChoice(List<Player.PlayerSide> supportedSides, Game game, IChoiceHandler choiceHandler, out BoardChoices.MomentOfInsightInfo moiInfo)
        {
            moiInfo = new BoardChoices.MomentOfInsightInfo();
            if (!supportedSides.Contains(game.CurPlayer.Side))
            {
                moiInfo.Use = BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.NotChosen;
                return true;
            }

            return choiceHandler.ChooseMomentOfInsightUse(game, out moiInfo);
        }

        #endregion

        #region Effect Utility
        protected void handleMomentOfInsight(Game game, BoardChoices choices)
        {
            if (choices.MoIInfo.Use == BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Reveal)
            {
                game.GetOtherPlayer().RevealCardInSummation();
            }
            else if (choices.MoIInfo.Use == BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Swap)
            {
                Player player = game.CurPlayer;

                player.Hand.Remove(choices.MoIInfo.HandCard);
                player.SummationDeck.RemoveCard(choices.MoIInfo.SummationCard);

                player.Hand.Add(choices.MoIInfo.SummationCard);
                player.SummationDeck.AddCard(choices.MoIInfo.HandCard);
            }
        }
        #endregion
    }
}
