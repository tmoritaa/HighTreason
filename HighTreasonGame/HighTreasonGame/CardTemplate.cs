using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame
{
    public abstract class CardTemplate
    {
        public delegate HTAction CardChoice(Game game, Player choosingPlayer);
        public delegate void CardEffect(Game game, Player choosingPlayer, BoardChoices choices);

        public class CardEffectPair
        {
            public CardChoice CardChoice
            {
                get; private set;
            }

            public CardEffect CardEffect
            {
                get; private set;
            }

            public Func<Game, Player, bool> Selectable
            {
                get; private set;
            }

            public CardEffectPair(CardChoice choiceFunc, CardEffect effectFunc, Func<Game, Player, bool> selectable = null)
            {
                CardChoice = choiceFunc;
                CardEffect = effectFunc;

                Selectable = (Game game, Player player) => { return true; };
                if (selectable != null)
                {
                    Selectable = selectable;
                }
            }
        }

        public string Name {
            get; private set;
        }

        public int ActionPts {
            get; private set;
        }

        public CardInfo CardInfo
        {
            get; private set;
        }

        public List<CardEffectPair> SelectionEvents
        {
            get; private set;
        }

        public List<CardEffectPair> TrialEvents
        {
            get; private set;
        }

        public List<CardEffectPair> SummationEvents
        {
            get; private set;
        }

        protected Player.PlayerSide side;
        protected bool isAttorney;

        public CardTemplate(string _name, int _actionPts, Player.PlayerSide _side, bool _isAttorney = false)
        {
            Name = _name;
            ActionPts = _actionPts;
            side = _side;
            isAttorney = _isAttorney;

            SelectionEvents = new List<CardEffectPair>();
            TrialEvents = new List<CardEffectPair>();
            SummationEvents = new List<CardEffectPair>();
        }

        public int GetNumberOfEventsInState(GameState.GameStateType type)
        {
            int num = 0;

            if (type == GameState.GameStateType.JurySelection)
            {
                num = SelectionEvents.Count;
            }
            else if (type == GameState.GameStateType.TrialInChief)
            {
                num = TrialEvents.Count;
            }
            else if (type == GameState.GameStateType.Summation)
            {
                num = SummationEvents.Count;
            }

            return num;
        }

        public bool CanBeUsedToObject(Player player)
        {
            return player.Side == side && isAttorney;
        }

        public void Init(JObject json)
        {
            fillCardInfo(json);

            addSelectionEventsAndChoices();
            addTrialEventsAndChoices();
            addSummationEventsAndChoices();
        }

        protected abstract void addSelectionEventsAndChoices();
        protected abstract void addTrialEventsAndChoices();
        protected abstract void addSummationEventsAndChoices();

        private void fillCardInfo(JObject json)
        {
            JObject cardJson = json.Value<JObject>(Name);

            string typing = cardJson.Value<string>("typing");

            JToken notesToken;
            string notes = cardJson.TryGetValue("notes", out notesToken) ? (string)notesToken : "";

            List<CardInfo.EffectInfo> jurySelectionPairs = new List<CardInfo.EffectInfo>();
            List<CardInfo.EffectInfo> trialInChiefPairs = new List<CardInfo.EffectInfo>();
            List<CardInfo.EffectInfo> summationPairs = new List<CardInfo.EffectInfo>();

            foreach (JObject eo in cardJson.Value<JArray>("jury_selection"))
            {
                jurySelectionPairs.Add(new CardInfo.EffectInfo(CardInfo.EffectInfo.EffectType.JurySelect, eo.Value<string>("text"), eo.Value<string>("desc")));
            }

            foreach (JObject eo in cardJson.Value<JArray>("trial_in_chief"))
            {
                trialInChiefPairs.Add(new CardInfo.EffectInfo(eo.Value<string>("type"), eo.Value<string>("text"), eo.Value<string>("desc")));
            }

            foreach (JObject eo in cardJson.Value<JArray>("summation"))
            {
                summationPairs.Add(new CardInfo.EffectInfo(eo.Value<string>("type"), eo.Value<string>("text"), eo.Value<string>("desc")));
            }

            CardInfo = new CardInfo(Name, typing, notes, jurySelectionPairs, trialInChiefPairs, summationPairs);
        }

        #region Choice Utility

        protected HTAction doNothingChoice(Game game, Player choosingPlayer)
        {
            return new HTAction(choosingPlayer.ChoiceHandler).InitForDoNothing(new BoardChoices(), choosingPlayer);
        }

        protected HTAction handleMomentOfInsightChoice(Player.PlayerSide[] supportedSides, Game game, Player choosingPlayer)
        {
            if (!supportedSides.Contains(choosingPlayer.Side))
            {
                BoardChoices choices = new BoardChoices();
                choices.MoIInfo.Use = BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.NotChosen;

                return new HTAction(choosingPlayer.ChoiceHandler).InitForDoNothing(new BoardChoices(), choosingPlayer);
            }

            return new HTAction(choosingPlayer.ChoiceHandler).InitForMoI(
                game,
                choosingPlayer);
        }

        protected CardChoice genAspectTrackForModCardChoice(HashSet<Property> optionProps, int numChoices, int modValue, bool affectedByPlayerSide, string desc)
        {
            return
                (Game game, Player choosingPlayer) =>
                {
                    List<BoardObject> options = game.FindBO(
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

                            int mod = affectedByPlayerSide ? calcModValueBasedOnSide(modValue, choosingPlayer) : modValue;
                            return valid && ((Track)htgo).CanModify(mod);
                        });

                    return new HTAction(choosingPlayer.ChoiceHandler).InitForChooseBOs(
                        (List<BoardObject> choices) =>
                        {
                            return HTUtility.FindAllCombOfBoardObjs(choices, numChoices);
                        },
                        options,
                        (Dictionary<BoardObject, int> selected) => { return true; },
                        (List<BoardObject> remainingChoices, Dictionary<BoardObject, int> selected) =>
                        {
                            return remainingChoices.Where(obj => !selected.ContainsKey(obj)).ToList();
                        },
                        (Dictionary<BoardObject, int> selected) => { return selected.Keys.Count == numChoices; },
                        game,
                        choosingPlayer,
                        desc);
                };
        }

        protected CardChoice genRevealOrPeakCardChoice(HashSet<Property> optionProps,
            int numChoices,
            bool isReveal,
            string desc,
            Func<Dictionary<BoardObject, int>, bool> validateChoices = null,
            Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>> filterChoices = null,
            Func<IEnumerable<IEnumerable<BoardObject>>, IEnumerable<IEnumerable<BoardObject>>> calcCombFilter = null)
        {
            return
                (Game game, Player choosingPlayer) =>
                {
                    List<BoardObject> options = game.FindBO(
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

                            return valid && (isReveal ? !((Jury.JuryAspect)htgo).IsRevealed : !((Jury.JuryAspect)htgo).IsVisibleToPlayer(choosingPlayer.Side));
                        });

                    return new HTAction(choosingPlayer.ChoiceHandler).InitForChooseBOs(
                        (List<BoardObject> choices) =>
                        {
                            return HTUtility.FindAllCombOfBoardObjs(choices, numChoices, calcCombFilter);
                        },
                        options,
                        validateChoices != null ? validateChoices :
                        (Dictionary<BoardObject, int> selected) => { return true; },
                        filterChoices != null ? filterChoices :
                        (List<BoardObject> remainingChoices, Dictionary<BoardObject, int> selected) =>
                        {
                            return remainingChoices.Where(obj => !selected.ContainsKey(obj)).ToList();
                        },
                        ((Dictionary<BoardObject, int> selected) => { return selected.Keys.Count == numChoices; }),
                        game,
                        choosingPlayer,
                        desc);
                };
        }

        #endregion

        #region Effect Utility

        protected void doNothingEffect(Game game, Player choosingPlayer, BoardChoices choices)
        {
            // Do nothing.
        }

        // Common enough in cards that we'll just simplify it for ourselves.
        protected void raiseGuiltAndOneAspectEffect(Game game, Player choosingPlayer, BoardChoices choices)
        {
            game.Board.GetGuiltTrack().AddToValue(1);
            choices.SelectedObjs.Keys.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(1));
        }

        protected List<AspectTrack> findAspectTracksWithProp(Game game, params Property[] aspectProp)
        {
            return game.FindBO(
                (BoardObject bo) =>
                {
                    bool valid = aspectProp.Length == 0;
                    if (!valid)
                    {
                        foreach (Property prop in aspectProp)
                        {
                            if (bo.Properties.Contains(prop))
                            {
                                valid = true;
                                break;
                            }
                        }
                    }

                    return valid && bo.Properties.Contains(Property.Aspect) && bo.Properties.Contains(Property.Track);
                }).Cast<AspectTrack>().ToList();
        }

        protected void revealAllAspects(Game game, Player choosingPlayer, BoardChoices choices)
        {
            choices.SelectedObjs.Keys.Cast<Jury.JuryAspect>().ToList().ForEach(a => a.Reveal());
        }

        protected void peekAllAspects(Game game, Player choosingPlayer, BoardChoices choices)
        {
            choices.SelectedObjs.Keys.Cast<Jury.JuryAspect>().ToList().ForEach(a => a.Peek(choosingPlayer.Side));
        }

        protected int calcModValueBasedOnSide(int value, Player player)
        {
            return value * ((player.Side == Player.PlayerSide.Prosecution) ? 1 : -1);
        }

        protected void handleMomentOfInsight(Game game, Player choosingPlayer, BoardChoices choices)
        {
            if (choices.MoIInfo.Use == BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Reveal)
            {
                game.GetOtherPlayer(choosingPlayer).RevealCardInSummation();
            }
            else if (choices.MoIInfo.Use == BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Swap)
            {
                Player player = choosingPlayer;

                player.SummationDeck.MoveCard(choices.MoIInfo.HandCard);
                player.Hand.MoveCard(choices.MoIInfo.SummationCard);
            }
        }
        #endregion

        #region Attorney CardEffect Utility

        protected CardEffectPair genAttorneyJurySelectPeekEffectPair(int sameSideVal, int oppSideVal, int infoIdx)
        {
            return new CardEffectPair(
                    (Game game, Player choosingPlayer) =>
                    {
                        int numChoices = (choosingPlayer.Side == side) ? sameSideVal : oppSideVal;

                        CardChoice func = genRevealOrPeakCardChoice(new HashSet<Property>() { }, numChoices, false, this.CardInfo.JurySelectionInfos[infoIdx].Description);
                        return func(game, choosingPlayer);
                    },
                    peekAllAspects);
        }

        protected CardEffectPair genAttorneyTrialAspectClearEffectPair(int modVal, int infoIdx)
        {
            return new CardEffectPair(
                    (Game game, Player choosingPlayer) =>
                    {
                        List<AspectTrack> tracks = findAspectTracksWithProp(game);

                        return new HTAction(choosingPlayer.ChoiceHandler).InitForChooseBOs(
                            (List<BoardObject> choices) =>
                            {
                                return HTUtility.FindAllCombOfBoardObjs(choices, 1);
                            },
                            tracks.Cast<BoardObject>().ToList(),
                            (Dictionary<BoardObject, int> selected) => { return true; },
                            (List<BoardObject> choices, Dictionary<BoardObject, int> selected) =>
                            {
                                return choices;
                            },
                            (Dictionary<BoardObject, int> selected) =>
                            {
                                return selected.Count == 1;
                            },
                            game,
                            choosingPlayer,
                            this.CardInfo.TrialInChiefInfos[infoIdx].Description);
                    },
                    (Game game, Player choosingPlayer, BoardChoices choices) =>
                    {
                        AspectTrack track = (AspectTrack)choices.SelectedObjs.Keys.First();

                        track.ResetTimesAffected();
                        track.AddToValue(calcModValueBasedOnSide(modVal, choosingPlayer));
                    });
        }

        protected CardEffectPair genAttorneyTrialAddSwayEffectPair(int infoIdx)
        {
            return new CardEffectPair(
                    (Game game, Player choosingPlayer) =>
                    {
                        List<BoardObject> bos = game.FindBO(
                            (BoardObject bo) =>
                            {
                                return
                                    bo.Properties.Contains(Property.Sway)
                                    && bo.Properties.Contains(Property.Track)
                                    && bo.Properties.Contains(Property.Jury);
                            });

                        return new HTAction(choosingPlayer.ChoiceHandler).InitForChooseBOs(
                            genAttorneyAddSwayFilterCombFunc(choosingPlayer),
                            bos,
                            (Dictionary<BoardObject, int> selected) =>
                            {
                                int actionPtsLeft = ActionPts - HTUtility.CalcActionPtUsage(selected);
                                return (actionPtsLeft >= 0);
                            },
                            (List<BoardObject> choicesLeft, Dictionary<BoardObject, int> selected) =>
                            {
                                return choicesLeft.FindAll(t =>
                                    (choosingPlayer.Side == Player.PlayerSide.Prosecution) ? !((SwayTrack)t).IsLockedByProsecution : !((SwayTrack)t).IsLockedByDefense);
                            },                            
                            (Dictionary<BoardObject, int> selected) =>
                            {
                                int actionPtsLeft = ActionPts - HTUtility.CalcActionPtUsage(selected);
                                return (actionPtsLeft == 0);
                            },
                            game,
                            choosingPlayer,
                            this.CardInfo.TrialInChiefInfos[infoIdx].Description);
                    },
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        int sideMod = (choosingPlayer.Side == Player.PlayerSide.Prosecution) ? 1 : -1;
                        foreach (KeyValuePair<BoardObject, int> kv in boardChoices.SelectedObjs)
                        {
                            SwayTrack track = (SwayTrack)kv.Key;
                            track.AddToValue(sideMod * kv.Value);
                        }
                    });
        }

        protected CardEffectPair genAttorneySummationAddSwayEffectPair(int infoIdx)
        {
            return new CardEffectPair(
                    (Game game, Player choosingPlayer) =>
                    {
                        List<BoardObject> bos = game.FindBO(
                            (BoardObject bo) =>
                            {
                                return bo.Properties.Contains(Property.Sway)
                                    && bo.Properties.Contains(Property.Track)
                                    && bo.Properties.Contains(Property.Jury)
                                    && !((SwayTrack)bo).IsLocked;
                            });

                        return new HTAction(choosingPlayer.ChoiceHandler).InitForChooseBOs(
                            genAttorneyAddSwayFilterCombFunc(choosingPlayer),
                            bos,
                            (Dictionary<BoardObject, int> selected) =>
                            {
                                int actionPtsLeft = ActionPts - HTUtility.CalcActionPtUsage(selected);
                                return (actionPtsLeft >= 0);
                            },
                            (List<BoardObject> choicesLeft, Dictionary<BoardObject, int> selected) =>
                            {
                                return choicesLeft.FindAll(t => !((SwayTrack)t).IsLocked);
                            },
                            (Dictionary<BoardObject, int> selected) =>
                            {
                                int actionPtsLeft = ActionPts - HTUtility.CalcActionPtUsage(selected);
                                return (actionPtsLeft == 0);
                            },
                            game,
                            choosingPlayer,
                            this.CardInfo.SummationInfos[infoIdx].Description);
                    },
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        int sideMod = (choosingPlayer.Side == Player.PlayerSide.Prosecution) ? 1 : -1;
                        foreach (KeyValuePair<BoardObject, int> kv in boardChoices.SelectedObjs)
                        {
                            SwayTrack track = (SwayTrack)kv.Key;
                            track.AddToValue(sideMod * kv.Value);
                        }
                    });
        }

        protected CardEffectPair genAttorneySummationClearSwayEffectPair(int infoIdx)
        {
            return new CardEffectPair(
                    (Game game, Player choosingPlayer) =>
                    {
                        List<BoardObject> bos = game.FindBO(
                            (BoardObject bo) =>
                            {
                                return bo.Properties.Contains(Property.Sway)
                                    && bo.Properties.Contains(Property.Track)
                                    && bo.Properties.Contains(Property.Jury);
                            });

                        return new HTAction(choosingPlayer.ChoiceHandler).InitForChooseBOs(
                            (List<BoardObject> choices) =>
                            {
                                return HTUtility.FindAllCombOfBoardObjs(choices, 1);
                            },
                            bos,
                            (Dictionary<BoardObject, int> selected) => { return true; },
                            (List<BoardObject> choicesLeft, Dictionary<BoardObject, int> selected) => { return choicesLeft; },
                            (Dictionary<BoardObject, int> selected) => { return selected.Count == 1; },
                            game,
                            choosingPlayer,
                            this.CardInfo.SummationInfos[infoIdx].Description);
                    },
                    (Game game, Player choosingPlayer, BoardChoices boardChoices) =>
                    {
                        SwayTrack track = (SwayTrack)boardChoices.SelectedObjs.Keys.First();
                        track.ResetValue();
                        track.AddToValue((choosingPlayer.Side == Player.PlayerSide.Prosecution) ? 1 : -1);
                    });
        }

        #endregion

        #region Combination Calculation Utility

        public Func<IEnumerable<IEnumerable<BoardObject>>, IEnumerable<IEnumerable<BoardObject>>> LimitNumAspectFilterComb(int numCap)
        {
            return
                (IEnumerable<IEnumerable<BoardObject>> allCombs) =>
                {
                    // Filter combinations to only ones with one of each aspect.
                    List<List<BoardObject>> filteredCombs = new List<List<BoardObject>>();
                    foreach (var objs in allCombs)
                    {
                        int[] numProps = new int[] { 0, 0, 0 };
                        Property[] props = new Property[] { Property.Religion, Property.Occupation, Property.Language };

                        bool filter = false;
                        foreach (var obj in objs)
                        {
                            for (int i = 0; i < 3; ++i)
                            {
                                if (obj.Properties.Contains(props[i]))
                                {
                                    numProps[i] += 1;

                                    if (numProps[i] > numCap)
                                    {
                                        filter = true;
                                    }

                                    break;
                                }
                            }

                            if (filter)
                            {
                                break;
                            }
                        }

                        if (!filter)
                        {
                            filteredCombs.Add(new List<BoardObject>(objs));
                        }

                        filter = false;
                    }

                    return (IEnumerable<IEnumerable<BoardObject>>)filteredCombs;
                };
        }

        public Func<List<BoardObject>, List<object>> genAttorneyAddSwayFilterCombFunc(Player choosingPlayer)
        {
            return
                ((List<BoardObject> choices) =>
                {
                    List<BoardObject> selectables = new List<BoardObject>();
                    foreach (var choice in choices)
                    {
                        SwayTrack track = (SwayTrack)choice;
                        int targetValue = choosingPlayer.Side == Player.PlayerSide.Prosecution ? track.MaxValue : track.MinValue;
                        int timesSelectable = Math.Abs(targetValue - track.Value);

                        int itNum = Math.Min(ActionPts, timesSelectable);
                        for (int i = 0; i < itNum; ++i)
                        {
                            selectables.Add(choice);
                        }
                    }

                    return HTUtility.FindAllCombOfBoardObjs(selectables, ActionPts);
                });
        }

        public Func<List<BoardObject>, List<object>> genDoNothingCombFunc(Player choosingPlayer)
        {
            return
                ((List<BoardObject> choices) =>
                {
                    return new List<object>() { new BoardChoices() };
                });
        }

        #endregion
    }
}
