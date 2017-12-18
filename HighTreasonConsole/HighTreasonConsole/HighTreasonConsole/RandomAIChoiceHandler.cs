using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HighTreasonGame;

namespace HighTreasonConsole
{
    public class RandomAIChoiceHandler : ChoiceHandler
    {
        protected Random random = new Random();

        public RandomAIChoiceHandler() : base(Player.PlayerType.AI)
        {
        }

        public override void ChoosePlayerAction(List<Card> cards, Game game, Player choosingPlayer, out PlayerActionParams outCardUsage)
        {
            outCardUsage = new PlayerActionParams();

            while (true)
            {
                // Pick card
                Card card = cards[random.Next() % cards.Count];
                outCardUsage.card = card;

                List<CardTemplate.CardEffectPair> pairs = card.Template.SelectionEvents;

                if (game.CurState.StateType == GameState.GameStateType.JurySelection)
                {
                    pairs = card.Template.SelectionEvents;
                }
                else if (game.CurState.StateType == GameState.GameStateType.TrialInChief)
                {
                    pairs = card.Template.TrialEvents;
                }
                else if (game.CurState.StateType == GameState.GameStateType.Summation)
                {
                    pairs = card.Template.SummationEvents;
                }

                List<int> validIndices = new List<int>();
                for (int i = 0; i < pairs.Count; ++i)
                {
                    if (pairs[i].Selectable(game, choosingPlayer))
                    {
                        validIndices.Add(i);
                    }
                }

                if (game.CurState.StateType != GameState.GameStateType.JurySelection)
                {
                    validIndices.Add(pairs.Count);
                }

                if (validIndices.Count == 0)
                {
                    continue;
                }

                int idx = validIndices[random.Next() % validIndices.Count];
                if (idx == pairs.Count)
                {
                    outCardUsage.usage = PlayerActionParams.UsageType.Action;
                }
                else
                {
                    outCardUsage.usage = PlayerActionParams.UsageType.Event;
                    outCardUsage.misc.Add(idx);
                }

                break;
            }
        }

        public override void ChooseBoardObjects(List<BoardObject> choices, Func<Dictionary<BoardObject, int>, bool> validateChoices, Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>> filterChoices, Func<Dictionary<BoardObject, int>, bool> choicesComplete, Game game, Player choosingPlayer, string description, out BoardChoices boardChoice)
        {
            boardChoice = new BoardChoices();

            if (choices.Count == 0)
            {
                return;
            }

            Dictionary<BoardObject, int> selected = new Dictionary<BoardObject, int>();
            List<BoardObject> remainingChoices = new List<BoardObject>(choices);

            while (true)
            {
                if (remainingChoices.Count <= 0)
                {
                    break;
                }

                int idx = random.Next() % remainingChoices.Count;

                BoardObject obj = remainingChoices[idx];
                if (!selected.ContainsKey(obj))
                {
                    selected[obj] = 0;
                }
                selected[obj] += 1;

                bool valid = validateChoices(selected);

                if (!valid)
                {
                    selected[obj] -= 1;
                    if (selected[obj] < 0)
                    {
                        selected.Remove(obj);
                    }
                    continue;
                }

                bool complete = choicesComplete(selected);

                if (complete)
                {
                    break;
                }

                remainingChoices = filterChoices(remainingChoices, selected);
            }

            boardChoice.SelectedObjs = selected;
        }

        public override void ChooseCards(List<Card> choices, Func<Dictionary<Card, int>, bool> validateChoices, Func<List<Card>, Dictionary<Card, int>, List<Card>> filterChoices, Func<Dictionary<Card, int>, bool, bool> choicesComplete, bool stoppable, Game game, Player choosingPlayer, string description, out BoardChoices boardChoice)
        {
            boardChoice = new BoardChoices();

            if (choices.Count == 0)
            {
                return;
            }

            Dictionary<Card, int> selected = new Dictionary<Card, int>();
            List<Card> remainingChoices = new List<Card>(choices);

            while (true)
            {
                if (remainingChoices.Count <= 0)
                {
                    break;
                }

                int idx = random.Next() % remainingChoices.Count;

                Card obj = remainingChoices[idx];
                if (!selected.ContainsKey(obj))
                {
                    selected[obj] = 0;
                }
                selected[obj] += 1;

                bool valid = validateChoices(selected);

                if (!valid)
                {
                    selected[obj] -= 1;
                    if (selected[obj] < 0)
                    {
                        selected.Remove(obj);
                    }
                    continue;
                }

                bool complete = choicesComplete(selected, false);

                if (complete)
                {
                    break;
                }

                remainingChoices = filterChoices(remainingChoices, selected);
            }

            boardChoice.SelectedCards = selected;
        }

        public override void ChooseCardEffect(Card card, Game game, Player choosingPlayer, string description, out BoardChoices.CardPlayInfo cardPlayInfo)
        {
            cardPlayInfo = new BoardChoices.CardPlayInfo();

            List<CardTemplate.CardEffectPair> pairs = card.Template.SelectionEvents;
            if (game.CurState.StateType == GameState.GameStateType.JurySelection)
            {
                pairs = card.Template.SelectionEvents;
            }
            else if (game.CurState.StateType == GameState.GameStateType.TrialInChief)
            {
                pairs = card.Template.TrialEvents;
            }
            else if (game.CurState.StateType == GameState.GameStateType.Summation)
            {
                pairs = card.Template.SummationEvents;
            }

            int idx = random.Next() % pairs.Count;
            cardPlayInfo.eventIdx = idx;
        }

        public override bool ChooseMomentOfInsightUse(Game game, Player choosingPlayer, out BoardChoices.MomentOfInsightInfo outMoIInfo)
        {
            outMoIInfo = new BoardChoices.MomentOfInsightInfo();

            int choice = random.Next() % 2;

            // reveal
            if (choice == 0)
            {
                outMoIInfo.Use = BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Reveal;
            }
            // swap
            else
            {
                List<Card> handCards = choosingPlayer.Hand.SelectableCards;
                List<Card> sumCards = choosingPlayer.SummationDeck.Cards;

                int handCardIdx = random.Next() % handCards.Count;
                int sumCardIdx = random.Next() % sumCards.Count;
                
                outMoIInfo.Use = BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Swap;
                outMoIInfo.HandCard = handCards[handCardIdx];
                outMoIInfo.SummationCard = sumCards[sumCardIdx];
            }

            return true;
        }
    }
}
