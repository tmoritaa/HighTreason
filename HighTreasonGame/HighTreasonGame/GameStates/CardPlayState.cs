using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.GameStates
{
    public abstract class CardPlayState : GameState
    {
        public PlayerActionParams PlayerActionResponse = null;

        protected class PlayerActionChoiceSubstate : GameSubState
        {
            public PlayerActionChoiceSubstate(GameState parentState) : base(parentState)
            { }

            // Copy constructor
            public PlayerActionChoiceSubstate(PlayerActionChoiceSubstate substate, GameState parentState, Game game) : base(parentState)
            {}

            public override void PreRun(Game game, Player curPlayer)
            {
                if (game.NotifyStartOfTurn != null)
                {
                    game.NotifyStartOfTurn();
                }
            }

            public override HTAction RequestAction(Game game, Player curPlayer)
            {
                bool canPlay = game.CurState.StateType == GameStateType.JurySelection
                                || (game.CurState.StateType == GameStateType.TrialInChief && curPlayer.Hand.SelectableCards.Count > 2)
                                || (game.CurState.StateType == GameStateType.Summation && curPlayer.Hand.SelectableCards.Count > 0);

                if (canPlay)
                {
                    return
                            new HTAction(
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

            public override void HandleRequestAction(HTAction action, Game game, Player curPlayer)
            {
                if (action != null)
                {
                    PlayerActionParams actionParams = new PlayerActionParams((PlayerActionParams)action.ChoiceResult, game);

                    ((CardPlayState)parentState).PlayerActionResponse = actionParams;

                    FileLogger.Instance.Log(actionParams.ToString(curPlayer, game.CurState.StateType));
                }
            }

            public override void SetNextSubstate(Game game, Player curPlayer)
            {
                if (((CardPlayState)parentState).PlayerActionResponse != null)
                {
                    PlayerActionParams actionParams = ((CardPlayState)parentState).PlayerActionResponse;
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
            private BoardChoices objectionResponse = null;

            public ObjectionSubstate(GameState parentState) : base(parentState)
            {}

            // Copy constructor
            public ObjectionSubstate(ObjectionSubstate substate, GameState parentState, Game game) : base(parentState)
            {
                objected = substate.objected;
            }

            public override bool CheckCloneEquality(GameSubState substate)
            {
                bool equal = base.CheckCloneEquality(substate);

                ObjectionSubstate os = (ObjectionSubstate)substate;

                equal &= objected == os.objected;
                
                return equal;
            }

            public override void PreRun(Game game, Player curPlayer)
            {
                objected = false;
                objectionResponse = null;
            }

            public override HTAction RequestAction(Game game, Player curPlayer)
            {
                PlayerActionParams actionParams = ((CardPlayState)parentState).PlayerActionResponse;

                Player objectingPlayer = game.GetOtherPlayer(curPlayer);

                List<Card> attorneyCards = objectingPlayer.Hand.SelectableCards.FindAll(c => c.Template.CanBeUsedToObject(objectingPlayer));
                if ((actionParams.usage != PlayerActionParams.UsageType.Event
                    || (game.CurState.StateType != GameState.GameStateType.TrialInChief && game.CurState.StateType != GameState.GameStateType.Summation)
                    || attorneyCards.Count == 0))
                {
                    return null;
                }

                return
                    new HTAction(
                        ChoiceHandler.ChoiceType.Cards,
                        curPlayer.ChoiceHandler,
                        attorneyCards,
                        (Func<Dictionary<Card, int>, bool>)((Dictionary<Card, int> selected) => { return true; }),
                        (Func<List<Card>, Dictionary<Card, int>, List<Card>>)((List<Card> choices, Dictionary<Card, int> selected) => { return choices; }),
                        (Func<Dictionary<Card, int>, bool, bool>)((Dictionary<Card, int> selected, bool isDone) => { return selected.Count == 1 || isDone; }),
                        true,
                        game,
                        objectingPlayer,
                        "Select attorney card to object " + actionParams.card.Template.Name + ", or press done to pass");
            }

            public override void HandleRequestAction(HTAction action, Game game, Player curPlayer)
            {
                if (action != null)
                {
                    objectionResponse = new BoardChoices((BoardChoices)action.ChoiceResult, game);

                    BoardChoices boardChoices = objectionResponse;
                    if (boardChoices.SelectedCards.Count > 0)
                    {
                        Card objectCard = boardChoices.SelectedCards.Keys.First();

                        Player objectingPlayer = game.GetOtherPlayer(curPlayer);
                        FileLogger.Instance.Log(objectingPlayer + " objected with " + objectCard.Template.Name + "\n");

                        game.Discards.MoveCard(objectCard);

                        Card objectedCard = ((CardPlayState)parentState).PlayerActionResponse.card;
                        objectedCard.BeingPlayed = false;
                        game.Discards.MoveCard(objectedCard);

                        objected = true;
                    }
                }
            }

            public override void SetNextSubstate(Game game, Player curPlayer)
            {
                Type nextSubstateType = (objected) ? typeof(PlayerTurnCompleteSubstate) : typeof(PlayerActionResolutionSubstate);
                parentState.SetNextSubstate(nextSubstateType);
            }
        }

        protected class PlayerActionResolutionSubstate : GameSubState
        {
            public BoardChoices CardUsageResponse = null;
            public bool notCancelled = false;

            public PlayerActionResolutionSubstate(GameState _parent) : base(_parent)
            {}

            // Copy constructor
            public PlayerActionResolutionSubstate(PlayerActionResolutionSubstate substate, GameState parentState, Game game) : base(parentState)
            {
                notCancelled = substate.notCancelled;
            }

            public override bool CheckCloneEquality(GameSubState substate)
            {
                bool equal = base.CheckCloneEquality(substate);

                PlayerActionResolutionSubstate sub = (PlayerActionResolutionSubstate)substate;
                equal &= notCancelled == sub.notCancelled;

                return equal;
            }

            public override void PreRun(Game game, Player curPlayer)
            {
                CardUsageResponse = null;
                notCancelled = false;
            }

            public override HTAction RequestAction(Game game, Player curPlayer)
            {
                PlayerActionParams playerAction = ((CardPlayState)parentState).PlayerActionResponse;

                if (playerAction.usage == PlayerActionParams.UsageType.Event)
                {
                    playerAction.card.BeingPlayed = true;
                    CardTemplate.CardEffectPair effectPair = playerAction.card.GetEventEffectPair(game, playerAction.eventIdx);

                    return effectPair.CardChoice(game, curPlayer);
                }
                else if (playerAction.usage == PlayerActionParams.UsageType.Action)
                {
                    playerAction.card.BeingPlayed = true;
                    return playerAction.card.PerformActionChoice(game, curPlayer);
                }

                return null;
            }

            public override void HandleRequestAction(HTAction action, Game game, Player curPlayer)
            {
                if (action != null)
                {
                    CardUsageResponse = new BoardChoices((BoardChoices)action.ChoiceResult, game);
                }

                PlayerActionParams playerAction = ((CardPlayState)parentState).PlayerActionResponse;

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
                    BoardChoices boardChoices = CardUsageResponse;
                    notCancelled = boardChoices.NotCancelled;
                    if (notCancelled)
                    {
                        actionPerformed = true;
                        if (playerAction.usage == PlayerActionParams.UsageType.Event)
                        {
                            FileLogger.Instance.Log(boardChoices.ToStringForEvent());

                            CardTemplate.CardEffectPair effectPair = playerAction.card.GetEventEffectPair(game, playerAction.eventIdx);
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

            public override void SetNextSubstate(Game game, Player curPlayer)
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

            // Copy constructor
            public PlayerTurnCompleteSubstate(PlayerTurnCompleteSubstate substate, GameState parentState, Game game) : base(parentState)
            {
                numPlayersFinished = substate.numPlayersFinished;
            }

            public override bool CheckCloneEquality(GameSubState substate)
            {
                bool equal = base.CheckCloneEquality(substate);

                PlayerTurnCompleteSubstate sub = (PlayerTurnCompleteSubstate)substate;
                equal &= numPlayersFinished == sub.numPlayersFinished;

                return equal;
            }

            public override void Init()
            {
                numPlayersFinished = 0;
            }

            public override void PreRun(Game game, Player curPlayer)
            { }

            public override HTAction RequestAction(Game game, Player curPlayer)
            {
                return null;
            }

            public override void HandleRequestAction(HTAction action, Game game, Player curPlayer)
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

            public override void SetNextSubstate(Game game, Player curPlayer)
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

        // Copy constructor
        public CardPlayState(CardPlayState state, Game _game)
            : base(state, _game)
        {
            if (state.PlayerActionResponse != null)
            {
                PlayerActionResponse = new PlayerActionParams(state.PlayerActionResponse, _game);
            }
        }

        public override bool CheckCloneEquality(GameState state)
        {
            bool equal = base.CheckCloneEquality(state);

            if (PlayerActionResponse != null)
            {
                equal &= PlayerActionResponse.CheckCloneEquality(((CardPlayState)state).PlayerActionResponse);
            }
            else
            {
                equal &= PlayerActionResponse == ((CardPlayState)state).PlayerActionResponse;
            }

            if (!equal)
            {
                Console.WriteLine("PlayerActionResponse was not equal");
                return equal;
            }

            return equal;
        }

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

        protected override void cleanup()
        {
            PlayerActionResponse = null;
        }

        protected override void initSubStates(GameState parent)
        {
            substates.Add(typeof(PlayerActionChoiceSubstate), new PlayerActionChoiceSubstate(this));
            substates.Add(typeof(ObjectionSubstate), new ObjectionSubstate(this));
            substates.Add(typeof(PlayerActionResolutionSubstate), new PlayerActionResolutionSubstate(this));
            substates.Add(typeof(PlayerTurnCompleteSubstate), new PlayerTurnCompleteSubstate(this));
        }

        protected override void copySubstates(GameState state, Game _game)
        {
            foreach (var kv in ((CardPlayState)state).substates)
            {
                if (kv.Key == typeof(PlayerActionChoiceSubstate))
                {
                    substates[kv.Key] = new PlayerActionChoiceSubstate((PlayerActionChoiceSubstate)kv.Value, this, _game);
                }
                else if (kv.Key == typeof(ObjectionSubstate))
                {
                    substates[kv.Key] = new ObjectionSubstate((ObjectionSubstate)kv.Value, this, _game);
                }
                else if (kv.Key == typeof(PlayerActionResolutionSubstate))
                {
                    substates[kv.Key] = new PlayerActionResolutionSubstate((PlayerActionResolutionSubstate)kv.Value, this, _game);
                }
                else if (kv.Key == typeof(PlayerTurnCompleteSubstate))
                {
                    substates[kv.Key] = new PlayerTurnCompleteSubstate((PlayerTurnCompleteSubstate)kv.Value, this, _game);
                }
            }
        }
    }
}
