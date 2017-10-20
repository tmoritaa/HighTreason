using System;
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

        public class CardUsageParams
        {
            public enum UsageType
            {
                Event,
                Action,
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

        private Game game;
        private ChoiceHandler choiceHandler;

        public HandHolder Hand
        {
            get; private set;
        }
        
        public SummationDeckHolder SummationDeck
        {
            get; private set;
        }

        public Player(PlayerSide _side, ChoiceHandler _choiceHandler, Game _game)
        {
            game = _game;
            Side = _side;
            choiceHandler = _choiceHandler;
            SummationDeck = new SummationDeckHolder();
            Hand = new HandHolder();
        }

        public void PlayCard()
        {
            bool cardPlayed = false;
            while (!cardPlayed)
            {
                CardUsageParams cardUsage;
                choiceHandler.ChooseCardAndUsage(Hand.SelectableCards, game, out cardUsage);

                // Remove card being used from hand.
                cardUsage.card.BeingPlayed = true;

                if (cardUsage.usage == CardUsageParams.UsageType.Event)
                {
                    cardPlayed = cardUsage.card.Template.PlayAsEvent(game, (int)cardUsage.misc[0], choiceHandler);
                }
                else if (cardUsage.usage == CardUsageParams.UsageType.Action)
                {
                    cardPlayed = cardUsage.card.Template.PlayAsAction(game, choiceHandler);
                }

                if (cardPlayed)
                {                    
                    if (game.NotifyPlayedCard != null)
                    {
                        game.NotifyPlayedCard(cardUsage);
                    }
                    
                    // Move used card to discard.
                    game.Discards.MoveCard(cardUsage.card);
                }
                
                cardUsage.card.BeingPlayed = false;
            }
        }

        public void DismissJury()
        {
            Jury jury = ChooseJuryChoice();

            Console.WriteLine("Dismissed Jury\n" + jury);

            game.RemoveJury(jury);
        }

        public void AddHandToSummation()
        {
            Hand.MoveAllCardsToHolder(SummationDeck);
        }

        public void RevealCardInSummation()
        {
            Card revealedCard = SummationDeck.RevealRandomCardInSummation();
            Console.WriteLine(revealedCard.Template.Name + " revealed in summation");
        }

        public Jury PerformJuryForDeliberation(List<Jury> juries)
        {
            Jury usedJury;
            while (true)
            {
                usedJury = ChooseJuryChoice();

                int modValue = (this.Side == Player.PlayerSide.Prosecution) ? 1 : -1;
                List<BoardObject> choices = game.FindBO(
                    (Type t) =>
                    {
                        return (t != typeof(Card));
                    },
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

        private void discardCard(Card card)
        {
            game.Discards.MoveCard(card);
        }

        private Jury ChooseJuryChoice()
        {
            Jury jury = null;
            while (jury == null)
            {
                BoardChoices boardChoices;
                choiceHandler.ChooseBoardObjects(
                    game.Board.Juries.Cast<BoardObject>().ToList(),
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
