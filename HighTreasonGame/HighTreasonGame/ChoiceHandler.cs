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

            public string ToString(Player choosingPlayer, GameState.GameStateType curStateType)
            {
                string str = "";

                switch (usage)
                {
                    case UsageType.Event:
                        {
                            str += choosingPlayer.Side + " played " + card.Template.Name + " played as event\n";

                            int idx = (int)misc[0];
                            if (curStateType == GameState.GameStateType.JurySelection)
                            {
                                str += card.Template.CardInfo.JurySelectionInfos[idx].Text;
                            }
                            else if (curStateType == GameState.GameStateType.TrialInChief)
                            {
                                str += card.Template.CardInfo.TrialInChiefInfos[idx].Text;
                            }
                            else if (curStateType == GameState.GameStateType.Summation)
                            {
                                str += card.Template.CardInfo.SummationInfos[idx].Text;
                            }
                        } break;
                    case UsageType.Action:
                        str += choosingPlayer.Side + " played " + card.Template.Name + " played as action for " + card.Template.ActionPts + " points";
                        break;
                    case UsageType.Mulligan:
                        str += choosingPlayer.Side + " mulliganed";
                        break;
                }

                return str;
            }
        }

        public ChoiceHandler(Player.PlayerType playerType)
        {
            PlayerType = playerType;
        }

        public abstract void ChoosePlayerAction(List<Card> cards, Game game, Player choosingPlayer, out ChoiceHandler.PlayerActionParams outCardUsage);
        public abstract bool ChooseMomentOfInsightUse(Game game, Player choosingPlayer, out BoardChoices.MomentOfInsightInfo outMoIInfo);
        public abstract void ChooseBoardObjects(List<BoardObject> choices, 
            Func<Dictionary<BoardObject, int>, bool> validateChoices, 
            Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>> filterChoices,
            Func<Dictionary<BoardObject, int>, bool> choicesComplete,
            Game game,
            Player choosingPlayer,
            string description,
            out BoardChoices boardChoice);
        public abstract void ChooseCards(List<Card> choices,
            Func<Dictionary<Card, int>, bool> validateChoices,
            Func<List<Card>, Dictionary<Card, int>, List<Card>> filterChoices,
            Func<Dictionary<Card, int>, bool, bool> choicesComplete,
            bool stoppable,
            Game game,
            Player choosingPlayer,
            string description,
            out BoardChoices boardChoice);
        public abstract void ChooseCardEffect(Card cardToPlay, Game game, Player choosingPlayer, string description, out BoardChoices.CardPlayInfo cardPlayInfo);
    }
}
