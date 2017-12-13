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

        public override void ChooseBoardObjects(List<BoardObject> choices, Func<Dictionary<BoardObject, int>, bool> validateChoices, Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>> filterChoices, Func<Dictionary<BoardObject, int>, bool> choicesComplete, Game game, Player choosingPlayer, string description, out BoardChoices boardChoice)
        {
            throw new NotImplementedException();
        }

        public override void ChooseCardEffect(Card cardToPlay, Game game, Player choosingPlayer, string description, out BoardChoices.CardPlayInfo cardPlayInfo)
        {
            throw new NotImplementedException();
        }

        public override void ChooseCards(List<Card> choices, Func<Dictionary<Card, int>, bool> validateChoices, Func<List<Card>, Dictionary<Card, int>, List<Card>> filterChoices, Func<Dictionary<Card, int>, bool, bool> choicesComplete, bool stoppable, Game game, Player choosingPlayer, string description, out BoardChoices boardChoice)
        {
            throw new NotImplementedException();
        }

        public override bool ChooseMomentOfInsightUse(Game game, Player choosingPlayer, out BoardChoices.MomentOfInsightInfo outMoIInfo)
        {
            throw new NotImplementedException();
        }

        public override void ChoosePlayerAction(List<Card> cards, Game game, Player choosingPlayer, out PlayerActionParams outCardUsage)
        {
            throw new NotImplementedException();
        }
    }
}
