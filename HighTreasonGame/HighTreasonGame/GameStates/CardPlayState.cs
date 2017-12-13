using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.GameStates
{
    public abstract class CardPlayState : GameState
    {
        public CardPlayState(GameStateType _stateType, Game _game) 
            : base(_stateType, _game)
        {}

        public override void StartState()
        {
            foreach (Player.PlayerSide side in new Player.PlayerSide[] { Player.PlayerSide.Prosecution, Player.PlayerSide.Defense })
            {
                game.GetPlayerOfSide(side).Hand.SetupHand(game.Deck.DealCards(GameConstants.NUM_HAND_SIZE));
            }

            curPlayer = game.GetPlayerOfSide(Player.PlayerSide.Prosecution);

            if (game.NotifyStateStart != null)
            {
                game.NotifyStateStart();
            }

            mainLoop();
        }

        protected abstract void mainLoop();

        protected void performPlayerAction(Player curPlayer)
        {
            bool actionPerformed = false;
            while (!actionPerformed)
            {
                ChoiceHandler.PlayerActionParams playerAction;
                curPlayer.ChoiceHandler.ChoosePlayerAction(curPlayer.Hand.SelectableCards, game, curPlayer, out playerAction);

                if (playerAction.usage == ChoiceHandler.PlayerActionParams.UsageType.Cancelled)
                {
                    continue;
                }

                FileLogger.Instance.Log(playerAction.ToString(curPlayer, game.CurState.StateType));

                bool cardPlayed = false;
                if (playerAction.usage == ChoiceHandler.PlayerActionParams.UsageType.Mulligan
                    && game.CurState.StateType == GameState.GameStateType.TrialInChief
                    && !curPlayer.PerformedMulligan)
                {
                    curPlayer.PerformMulligan();
                    actionPerformed = true;
                }
                else if (playerAction.usage == ChoiceHandler.PlayerActionParams.UsageType.Event)
                {
                    playerAction.card.BeingPlayed = true;

                    bool objected = performObjectionPhase(playerAction.card, game.GetOtherPlayer(curPlayer));
                    actionPerformed = objected;

                    if (!objected)
                    {
                        actionPerformed = playerAction.card.PlayAsEvent(game, curPlayer, (int)playerAction.misc[0], curPlayer.ChoiceHandler);
                    }

                    cardPlayed = true;
                }
                else if (playerAction.usage == ChoiceHandler.PlayerActionParams.UsageType.Action)
                {
                    playerAction.card.BeingPlayed = true;
                    actionPerformed = playerAction.card.PlayAsAction(game, curPlayer, curPlayer.ChoiceHandler);
                    cardPlayed = true;
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
        }

        protected bool performObjectionPhase(Card card, Player curPlayer)
        {
            bool objected = false;

            if (game.CurState.StateType != GameState.GameStateType.TrialInChief && game.CurState.StateType != GameState.GameStateType.Summation)
            {
                return objected;
            }

            List<Card> attorneyCards = curPlayer.Hand.Cards.FindAll(c => c.Template.CanBeUsedToObject(curPlayer));
            if (attorneyCards.Count > 0)
            {
                // TODO: later should also have some way of showing which event was picked by opponent. Should be done on Unity side.
                string desc = "Select attorney card to object " + card.Template.Name + ", or press done to pass";

                BoardChoices boardChoices;
                curPlayer.ChoiceHandler.ChooseCards(
                    attorneyCards,
                    (Dictionary<Card, int> selected) => { return true; },
                    (List<Card> choices, Dictionary<Card, int> selected) => { return choices; },
                    (Dictionary<Card, int> selected, bool isDone) => { return selected.Count == 1 || isDone; },
                    true,
                    game,
                    curPlayer,
                    desc,
                    out boardChoices);

                if (boardChoices.SelectedCards.Count > 0)
                {
                    Card objectCard = boardChoices.SelectedCards.Keys.First();

                    FileLogger.Instance.Log(curPlayer + " objected with " + objectCard.Template.Name);

                    game.Discards.MoveCard(objectCard);
                    objected = true;
                }
            }

            return objected;
        }
    }
}
