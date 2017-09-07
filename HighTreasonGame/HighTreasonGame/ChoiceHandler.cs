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

        public abstract void ChooseCardAndUsage(List<CardTemplate> cards, Game game, out Player.CardUsageParams outCardUsage);
        public abstract bool ChooseActionUsage(List<Track> choices, int actionPts, Jury deliberationJury, Game game, out Dictionary<Track, int> outTracks);
        public abstract bool ChooseMomentOfInsightUse(Game game, out BoardChoices.MomentOfInsightInfo outMoIInfo);

        public abstract void ChooseBoardObjects(List<BoardObject> choices, 
            Func<Dictionary<BoardObject, int>, bool> validateChoices, 
            Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>> filterChoices,
            Func<Dictionary<BoardObject, int>, bool> choicesComplete,
            Game game,
            out BoardChoices boardChoice);
    }
}
