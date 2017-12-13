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
        public RandomAIChoiceHandler() : base(Player.PlayerType.AI)
        {
        }

        public override void ChoosePlayerAction(List<Card> cards, Game game, Player choosingPlayer, out PlayerActionParams outCardUsage)
        {
            Random rand = new Random();

            outCardUsage = new PlayerActionParams();

            while (true)
            {
                // Pick card
                Card card = cards[rand.Next() % cards.Count];
                outCardUsage.card = card;

                List<CardTemplate.CardEffectPair> pairs = card.Template.SelectionEvents;
                int modVal = 0;
                if (game.CurState.StateType == GameState.GameStateType.JurySelection)
                {
                    pairs = card.Template.SelectionEvents.Where(p => p.Selectable(game, choosingPlayer)).ToList();
                    modVal = pairs.Count;
                }
                else if (game.CurState.StateType == GameState.GameStateType.TrialInChief)
                {
                    pairs = card.Template.TrialEvents.Where(p => p.Selectable(game, choosingPlayer)).ToList();
                    modVal = pairs.Count + 1;
                }
                else if (game.CurState.StateType == GameState.GameStateType.Summation)
                {
                    pairs = card.Template.SummationEvents.Where(p => p.Selectable(game, choosingPlayer)).ToList();
                    modVal = pairs.Count + 1;
                }

                if (pairs.Count == 0)
                {
                    if (game.CurState.StateType != GameState.GameStateType.JurySelection)
                    {
                        modVal = 1;
                    }
                    else
                    {
                        continue;
                    }
                }

                int idx = rand.Next() % modVal;
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

            Random random = new Random();
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

            Random random = new Random();
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
            Random rand = new Random();
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

            int idx = rand.Next() % pairs.Count;
            cardPlayInfo.eventIdx = idx;
        }

        public override bool ChooseMomentOfInsightUse(Game game, Player choosingPlayer, out BoardChoices.MomentOfInsightInfo outMoIInfo)
        {
            outMoIInfo = new BoardChoices.MomentOfInsightInfo();

            Random rand = new Random();
            int choice = rand.Next() % 2;

            // reveal
            if (choice == 0)
            {
                outMoIInfo.Use = BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Reveal;
            }
            // swap
            else
            {
                List<Card> cards = choosingPlayer.Hand.SelectableCards;

                int handCardIdx = rand.Next() % cards.Count;
                int sumCardIdx = 0;

                while (true)
                {
                    sumCardIdx = rand.Next() % cards.Count;

                    if (sumCardIdx != handCardIdx)
                    {
                        break;
                    }
                }

                outMoIInfo.Use = BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Swap;
                outMoIInfo.HandCard = cards[handCardIdx];
                outMoIInfo.SummationCard = cards[sumCardIdx];
            }

            return true;
        }
    }
}
