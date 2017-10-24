using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public abstract class ChoiceHandler
    {
        public Player.PlayerType PlayerType
        {
            get; protected set;
        }

        public ChoiceHandler(Player.PlayerType playerType)
        {
            PlayerType = playerType;
        }

        public abstract void ChoosePlayerAction(List<Card> cards, Game game, out Player.PlayerActionParams outCardUsage);
        public abstract bool ChooseMomentOfInsightUse(Game game, out BoardChoices.MomentOfInsightInfo outMoIInfo);
        public abstract void ChooseBoardObjects(List<BoardObject> choices, 
            Func<Dictionary<BoardObject, int>, bool> validateChoices, 
            Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>> filterChoices,
            Func<Dictionary<BoardObject, int>, bool> choicesComplete,
            Game game,
            out BoardChoices boardChoice);
    }
}
