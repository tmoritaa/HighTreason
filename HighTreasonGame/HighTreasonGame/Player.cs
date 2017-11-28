﻿using System;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame.GameStates;

namespace HighTreasonGame
{
    public class Player
    {
        public enum PlayerType
        {
            Human,
            AI,
        }

        public enum PlayerSide
        {
            Prosecution,
            Defense
        }

        public class PlayerActionParams
        {
            public enum UsageType
            {
                Event,
                Action,
                Mulligan,
                Cancelled,
            }

            public Card card;
            public UsageType usage;
            public List<object> misc = new List<object>();
        }

        public PlayerSide Side 
        {
            get; private set;
        }

        public PlayerType ChoiceType
        {
            get
            {
                return choiceHandler.PlayerType;
            }
        }

        public HandHolder Hand
        {
            get; private set;
        }
        
        public SummationDeckHolder SummationDeck
        {
            get; private set;
        }

        public bool PerformedMulligan
        {
            get; private set;
        }

        private Game game;
        private ChoiceHandler choiceHandler;

        public Player(PlayerSide _side, ChoiceHandler _choiceHandler, Game _game)
        {
            game = _game;
            Side = _side;
            choiceHandler = _choiceHandler;
            SummationDeck = new SummationDeckHolder();
            Hand = new HandHolder();
            PerformedMulligan = false;
        }

        public void PerformPlayerAction()
        {
            bool actionPerformed = false;
            while (!actionPerformed)
            {
                PlayerActionParams playerAction;
                choiceHandler.ChoosePlayerAction(Hand.SelectableCards, game, out playerAction);

                if (playerAction.usage == PlayerActionParams.UsageType.Cancelled)
                {
                    continue;
                }

                bool cardPlayed = false;
                if (playerAction.usage == PlayerActionParams.UsageType.Mulligan 
                    && game.CurState.StateType == GameState.GameStateType.TrialInChief 
                    && !PerformedMulligan)
                {
                    performMulligan();
                    actionPerformed = true;
                }
                else if (playerAction.usage == PlayerActionParams.UsageType.Event)
                {
                    playerAction.card.BeingPlayed = true;
                    actionPerformed = playerAction.card.Template.PlayAsEvent(game, (int)playerAction.misc[0], choiceHandler);
                    cardPlayed = true;
                }
                else if (playerAction.usage == PlayerActionParams.UsageType.Action)
                {
                    playerAction.card.BeingPlayed = true;
                    actionPerformed = playerAction.card.Template.PlayAsAction(game, choiceHandler);
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

        public void DismissJury()
        {
            Jury jury = chooseJuryChoice(game.Board.Juries);

            Console.WriteLine("Dismissed Jury\n" + jury);

            game.RemoveJury(jury);
        }

        public void AddHandToSummation()
        {
            Hand.MoveAllCardsToHolder(SummationDeck);
        }

        public void RevealCardInSummation()
        {
            SummationDeck.RevealRandomCardInSummation();
        }

        public Jury PerformJuryForDeliberation(List<Jury> juries)
        {
            Jury usedJury;
            while (true)
            {
                usedJury = chooseJuryChoice(juries);

                int modValue = (this.Side == Player.PlayerSide.Prosecution) ? 1 : -1;
                List<BoardObject> choices = game.FindBO(
                    (BoardObject htgo) =>
                    {
                        return (htgo.Properties.Contains(Property.Track) &&
                        ((htgo.Properties.Contains(Property.Jury) && htgo.Properties.Contains(Property.Sway))
                        || (htgo.Properties.Contains(Property.Aspect)))
                        && (htgo.Properties.Contains(usedJury.Aspects[0].Aspect) || htgo.Properties.Contains(usedJury.Aspects[1].Aspect) || htgo.Properties.Contains(usedJury.Aspects[2].Aspect)))
                        && ((Track)htgo).CanModifyByAction(modValue);
                    });

                BoardChoices boardChoices;
                choiceHandler.ChooseBoardObjects(choices,
                    HTUtility.GenActionValidateChoicesFunc(usedJury.ActionPoints, usedJury),
                    HTUtility.GenActionFilterChoicesFunc(usedJury.ActionPoints, usedJury),
                    HTUtility.GenActionChoicesCompleteFunc(usedJury.ActionPoints, usedJury),
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

                    break;
                }
            }

            return usedJury;
        }

        public override string ToString()
        {
            string outStr = Side + " player:\n";

            outStr += "Hand = \n";
            foreach (Card card in Hand.Cards)
            {
                outStr += card.Template.Name + "\n";
            }

            outStr += "Summation = \n";
            foreach (Card card in SummationDeck.Cards)
            {
                outStr += card.Template.Name + "\n";
            }

            return outStr;
        }

        private void performMulligan()
        {
            int prevHandSize = Hand.Cards.Count;

            Hand.MoveAllCardsToHolder(game.Discards);
            Hand.SetupHand(game.Deck.DealCards(prevHandSize - 1));

            PerformedMulligan = true;
        }

        private void discardCard(Card card)
        {
            game.Discards.MoveCard(card);
        }

        private Jury chooseJuryChoice(List<Jury> juries)
        {
            Jury jury = null;
            while (jury == null)
            {
                BoardChoices boardChoices;
                choiceHandler.ChooseBoardObjects(
                    juries.Cast<BoardObject>().ToList(),
                    (Dictionary<BoardObject, int> selected) => { return true; },
                    (List<BoardObject> remainingChoices, Dictionary<BoardObject, int> selected) =>
                    {
                        return remainingChoices.Where(obj => !selected.ContainsKey(obj)).ToList();
                    },
                    (Dictionary<BoardObject, int> selected) => { return selected.Keys.Count == 1; },
                    game,
                    out boardChoices);

                if (boardChoices.NotCancelled)
                {
                    jury = (Jury)boardChoices.SelectedObjs.Keys.First();
                }
            }

            return jury;
        }
    }
}
