using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.GameStates
{
    public abstract class CardPlayState : GameState
    {
        public Action PlayerActionResponse = null;
        protected class PlayerActionChoiceSubstate : GameSubState
        {
            public PlayerActionChoiceSubstate(GameState parentState) : base(parentState)
            { }

            public override void PreRun(Game game, Player curPlayer)
            {
                if (game.NotifyStartOfTurn != null)
                {
                    game.NotifyStartOfTurn();
                }
            }

            public override Action RequestAction(Game game, Player curPlayer)
            {
                bool canPlay = game.CurState.StateType == GameStateType.JurySelection
                                || (game.CurState.StateType == GameStateType.TrialInChief && curPlayer.Hand.SelectableCards.Count > 2)
                                || (game.CurState.StateType == GameStateType.Summation && curPlayer.Hand.SelectableCards.Count > 0);

                if (canPlay)
                {
                    return
                            new Action(
                                ChoiceHandler.ChoiceType.PlayerAction,
                                curPlayer.ChoiceHandler,
                                curPlayer.Hand.SelectableCards,
                                game,
                                curPlayer);
                }
                else
                {
                    return null;
                }
            }

            public override void HandleRequestAction(Action action)
            {
                if (action != null)
                {
                    ((CardPlayState)parentState).PlayerActionResponse = action;
                }
            }

            public override void RunRest(Game game, Player curPlayer)
            {
                if (((CardPlayState)parentState).PlayerActionResponse != null)
                {
                    PlayerActionParams actionParams = (PlayerActionParams)((CardPlayState)parentState).PlayerActionResponse.ChoiceResult;
                    FileLogger.Instance.Log(actionParams.ToString(curPlayer, game.CurState.StateType));
                }
            }

            public override void PrepareNextSubstate()
            {
                if (((CardPlayState)parentState).PlayerActionResponse != null)
                {
                    PlayerActionParams actionParams = (PlayerActionParams)((CardPlayState)parentState).PlayerActionResponse.ChoiceResult;
                    if (actionParams.usage == PlayerActionParams.UsageType.Cancelled)
                    {
                        parentState.SetNextSubstate(typeof(PlayerActionChoiceSubstate));
                    }
                    else
                    {
                        parentState.SetNextSubstate(typeof(ObjectionSubstate));
                    }
                }
                else
                {
                    parentState.SetNextSubstate(typeof(PlayerActionResolutionSubstate));
                }
            }
        }

        protected class ObjectionSubstate : GameSubState
        {
            private bool objected = false;
            private Action objectionResponse = null;

            public ObjectionSubstate(GameState parentState) : base(parentState)
            { }

            public override void PreRun(Game game, Player curPlayer)
            {
                objected = false;
                objectionResponse = null;
            }

            public override Action RequestAction(Game game, Player curPlayer)
            {
                PlayerActionParams actionParams = (PlayerActionParams)((CardPlayState)parentState).PlayerActionResponse.ChoiceResult;
                List<Card> attorneyCards = curPlayer.Hand.Cards.FindAll(c => c.Template.CanBeUsedToObject(curPlayer));
                if ((actionParams.usage != PlayerActionParams.UsageType.Event
                    || (game.CurState.StateType != GameState.GameStateType.TrialInChief && game.CurState.StateType != GameState.GameStateType.Summation)
                    || attorneyCards.Count == 0))
                {
                    return null;
                }

                return
                    new Action(
                        ChoiceHandler.ChoiceType.Cards,
                        curPlayer.ChoiceHandler,
                        attorneyCards,
                        (Func<Dictionary<Card, int>, bool>)((Dictionary<Card, int> selected) => { return true; }),
                        (Func<List<Card>, Dictionary<Card, int>, List<Card>>)((List<Card> choices, Dictionary<Card, int> selected) => { return choices; }),
                        (Func<Dictionary<Card, int>, bool, bool>)((Dictionary<Card, int> selected, bool isDone) => { return selected.Count == 1 || isDone; }),
                        true,
                        game,
                        curPlayer,
                        "Select attorney card to object " + actionParams.card.Template.Name + ", or press done to pass");
            }

            public override void HandleRequestAction(Action action)
            {
                objectionResponse = action;
            }

            public override void RunRest(Game game, Player curPlayer)
            {
                if (objectionResponse != null)
                {
                    BoardChoices boardChoices = (BoardChoices)objectionResponse.ChoiceResult;

                    if (boardChoices.SelectedCards.Count > 0)
                    {
                        Card objectCard = boardChoices.SelectedCards.Keys.First();

                        FileLogger.Instance.Log(curPlayer + " objected with " + objectCard.Template.Name + "\n");

                        game.Discards.MoveCard(objectCard);
                        objected = true;
                    }
                }

                return;
            }

            public override void PrepareNextSubstate()
            {
                Type nextSubstateType = (objected) ? typeof(PlayerTurnCompleteSubstate) : typeof(PlayerActionResolutionSubstate);

                parentState.SetNextSubstate(nextSubstateType);
            }
        }

        protected class PlayerActionResolutionSubstate : GameSubState
        {
            public Action CardUsageResponse = null;
            public bool notCancelled = false;

            public PlayerActionResolutionSubstate(GameState _parent) : base(_parent)
            {}

            public override void PreRun(Game game, Player curPlayer)
            {
                CardUsageResponse = null;
                notCancelled = false;
            }

            public override Action RequestAction(Game game, Player curPlayer)
            {
                PlayerActionParams playerAction = (PlayerActionParams)((CardPlayState)parentState).PlayerActionResponse.ChoiceResult;

                if (playerAction.usage == PlayerActionParams.UsageType.Event)
                {
                    playerAction.card.BeingPlayed = true;
                    CardTemplate.CardEffectPair effectPair = playerAction.card.GetEventEffectPair(game, (int)playerAction.misc[0]);

                    return effectPair.CardChoice(game, curPlayer);
                }
                else if (playerAction.usage == PlayerActionParams.UsageType.Action)
                {
                    playerAction.card.BeingPlayed = true;
                    return playerAction.card.PerformActionChoice(game, curPlayer);
                }

                return null;
            }

            public override void HandleRequestAction(Action action)
            {
                CardUsageResponse = action;
            }

            public override void RunRest(Game game, Player curPlayer)
            {
                PlayerActionParams playerAction = (PlayerActionParams)((CardPlayState)parentState).PlayerActionResponse.ChoiceResult;

                bool actionPerformed = false;
                bool cardPlayed = false;
                if (playerAction.usage == PlayerActionParams.UsageType.Mulligan
                    && game.CurState.StateType == GameState.GameStateType.TrialInChief
                    && !curPlayer.PerformedMulligan)
                {
                    curPlayer.PerformMulligan();
                    actionPerformed = true;
                }
                else if (CardUsageResponse != null)
                {
                    BoardChoices boardChoices = (BoardChoices)CardUsageResponse.ChoiceResult;
                    notCancelled = boardChoices.NotCancelled;
                    if (notCancelled)
                    {
                        actionPerformed = true;
                        if (playerAction.usage == PlayerActionParams.UsageType.Event)
                        {
                            FileLogger.Instance.Log(boardChoices.ToStringForEvent());

                            CardTemplate.CardEffectPair effectPair = playerAction.card.GetEventEffectPair(game, (int)playerAction.misc[0]);
                            effectPair.CardEffect(game, curPlayer, boardChoices);
                            cardPlayed = true;
                        }
                        else if (playerAction.usage == PlayerActionParams.UsageType.Action)
                        {
                            playerAction.card.PerformAction(curPlayer, boardChoices);
                            cardPlayed = true;
                        }
                    }
                }

                if (actionPerformed)
                {
                    if (game.NotifyPlayerActionPerformed != null)
                    {
                        game.NotifyPlayerActionPerformed(playerAction);
                    }

                    if (cardPlayed)
                    {
                        // Move used card to discard.
                        game.Discards.MoveCard(playerAction.card);
                    }
                }

                if (cardPlayed)
                {
                    playerAction.card.BeingPlayed = false;
                }
            }

            public override void PrepareNextSubstate()
            {
                if (!notCancelled)
                {
                    parentState.SetNextSubstate(typeof(PlayerActionChoiceSubstate));
                }
                else
                {
                    parentState.SetNextSubstate(typeof(PlayerTurnCompleteSubstate));
                }

            }
        }

        protected class PlayerTurnCompleteSubstate : GameSubState
        {
            private int numPlayersFinished = 0;

            public PlayerTurnCompleteSubstate(GameState _parent) : base(_parent)
            {
            }

            public override void Init()
            {
                numPlayersFinished = 0;
            }

            public override void PreRun(Game game, Player curPlayer)
            { }

            public override Action RequestAction(Game game, Player curPlayer)
            {
                return null;
            }

            public override void HandleRequestAction(Action action)
            { }

            public override void RunRest(Game game, Player curPlayer)
            {
                if (game.CurState.StateType == GameStateType.JurySelection)
                {
                    if (curPlayer.Hand.Cards.Count == 2)
                    {
                        numPlayersFinished += 1;
                        curPlayer.AddHandToSummation();
                        parentState.PassToNextPlayer();
                    }
                }
                else if (game.CurState.StateType == GameStateType.TrialInChief)
                {
                    if (curPlayer.Hand.Cards.Count == 2)
                    {
                        numPlayersFinished += 1;
                        curPlayer.AddHandToSummation();
                    }

                    if (game.GetOtherPlayer(curPlayer).Hand.Cards.Count >= 2)
                    {
                        parentState.PassToNextPlayer();
                    }
                }
                else if (game.CurState.StateType == GameStateType.Summation)
                {
                    if (curPlayer.Hand.Cards.Count == 0)
                    {
                        numPlayersFinished += 1;
                    }

                    if ((curPlayer.Side == Player.PlayerSide.Prosecution && curPlayer.Hand.Cards.Count == 3)
                    || curPlayer.Hand.Cards.Count == 0)
                    {
                        parentState.PassToNextPlayer();
                    }
                }
            }

            public override void PrepareNextSubstate()
            {
                if (numPlayersFinished < 2)
                {
                    parentState.SetNextSubstate(typeof(PlayerActionChoiceSubstate));
                }
                else
                {
                    parentState.SignifyStateEnd();
                }
            }
        }

        public CardPlayState(GameStateType _stateType, Game _game) 
            : base(_stateType, _game)
        {}

        public override void InitState()
        {
            base.InitState();

            foreach (Player.PlayerSide side in new Player.PlayerSide[] { Player.PlayerSide.Prosecution, Player.PlayerSide.Defense })
            {
                game.GetPlayerOfSide(side).Hand.SetupHand(game.Deck.DealCards(GameConstants.NUM_HAND_SIZE));
            }

            curPlayer = game.GetPlayerOfSide(Player.PlayerSide.Prosecution);

            stateEnded = false;

            CurSubstate = substates[typeof(PlayerActionChoiceSubstate)];

            if (game.NotifyStateStart != null)
            {
                game.NotifyStateStart();
            }
        }

        protected override void initSubStates(GameState parent)
        {
            substates.Add(typeof(PlayerActionChoiceSubstate), new PlayerActionChoiceSubstate(this));
            substates.Add(typeof(ObjectionSubstate), new ObjectionSubstate(this));
            substates.Add(typeof(PlayerActionResolutionSubstate), new PlayerActionResolutionSubstate(this));
            substates.Add(typeof(PlayerTurnCompleteSubstate), new PlayerTurnCompleteSubstate(this));
        }
    }
}
