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
            string description,
            out BoardChoices boardChoice);
        public abstract void ChooseCards(List<Card> choices,
            Func<Dictionary<Card, int>, bool> validateChoices,
            Func<List<Card>, Dictionary<Card, int>, List<Card>> filterChoices,
            Func<Dictionary<Card, int>, bool, bool> choicesComplete,
            bool stoppable,
            Game game,
            string description,
            out BoardChoices boardChoice);
        public abstract void ChooseCardEffect(Card cardToPlay, Game game, string description, out BoardChoices.CardPlayInfo cardPlayInfo);
    }
}
