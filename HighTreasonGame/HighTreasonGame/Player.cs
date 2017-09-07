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

            public CardTemplate card;
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

        public List<CardTemplate> Hand
        {
            get; private set;
        }
        
        public SummationDeck SummationDeck
        {
            get; private set;
        }

        public Player(PlayerSide _side, ChoiceHandler _choiceHandler, Game _game)
        {
            game = _game;
            Side = _side;
            choiceHandler = _choiceHandler;
            SummationDeck = new SummationDeck();
        }

        public void SetupHand(List<CardTemplate> _hand)
        {
            Hand = _hand;
        }

        public void PlayCard()
        {
            bool cardPlayed = false;
            while (!cardPlayed)
            {
                CardUsageParams cardUsage;
                choiceHandler.ChooseCardAndUsage(Hand, game, out cardUsage);

                // Remove card being used from hand.
                Hand.Remove(cardUsage.card);

                if (cardUsage.usage == CardUsageParams.UsageType.Event)
                {
                    cardPlayed = cardUsage.card.PlayAsEvent(game, (int)cardUsage.misc[0], choiceHandler);
                }
                else if (cardUsage.usage == CardUsageParams.UsageType.Action)
                {
                    cardPlayed = cardUsage.card.PlayAsAction(game, choiceHandler);
                }

                if (cardPlayed)
                {
                    if (game.NotifyPlayedCard != null)
                    {
                        game.NotifyPlayedCard(cardUsage);
                    }
                    
                    // Move used card to discard.
                    game.Discards.Add(cardUsage.card);
                }
                else
                {
                    // Readd card to hand since undoing selection.
                    Hand.Add(cardUsage.card);
                }
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
            Hand.ForEach(c => SummationDeck.AddCard(c));
            Hand.Clear();
        }

        public void RevealCardInSummation()
        {
            CardTemplate revealedCard = SummationDeck.RevealRandomCardInSummation();
            Console.WriteLine(revealedCard.Name + " revealed in summation");
        }

        public Jury PerformJuryForDeliberation(List<Jury> juries)
        {
            Jury usedJury;
            while (true)
            {
                usedJury = ChooseJuryChoice();

                int modValue = (this.Side == Player.PlayerSide.Prosecution) ? 1 : -1;
                List<Track> choices = game.GetHTGOFromCondition(
                    (BoardObject htgo) =>
                    {
                        return (htgo.Properties.Contains(Property.Track) &&
                        ((htgo.Properties.Contains(Property.Jury) && htgo.Properties.Contains(Property.Sway))
                        || (htgo.Properties.Contains(Property.Aspect)))
                        && (htgo.Properties.Contains(usedJury.Aspects[0].Aspect) || htgo.Properties.Contains(usedJury.Aspects[1].Aspect) || htgo.Properties.Contains(usedJury.Aspects[2].Aspect)))
                        && ((Track)htgo).CanModifyByAction(modValue);
                    }).Cast<Track>().ToList();

                Dictionary<Track, int> affectedTracks;
                bool choiceMade = choiceHandler.ChooseActionUsage(choices, usedJury.ActionPoints, usedJury, game, out affectedTracks);

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

                    break;
                }
            }

            return usedJury;
        }

        public override string ToString()
        {
            string outStr = Side + " player:\n";

            outStr += "Hand = \n";
            foreach (CardTemplate card in Hand)
            {
                outStr += card.Name + "\n";
            }

            outStr += "Summation = \n";
            foreach (CardTemplate card in SummationDeck.AllCards)
            {
                outStr += card.Name + "\n";
            }

            return outStr;
        }

        private void discardCard(CardTemplate card)
        {
            Hand.Remove(card);
            game.Discards.Add(card);
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
