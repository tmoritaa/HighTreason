using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class HTAction
    {
        public object ChoiceResult
        {
            get; private set;
        }

        private ChoiceHandler.ChoiceType choiceType;
        private ChoiceHandler choiceHandler;
        private object[] choiceArgs;

        private bool choiceRequestable = false;

        public HTAction(ChoiceHandler.ChoiceType _choiceType, ChoiceHandler _handler, params object[] _choiceArgs)
        {
            choiceType = _choiceType;
            choiceHandler = _handler;
            choiceArgs = _choiceArgs;
            choiceRequestable = true;
        }

        public HTAction(object choiceResult)
        {
            ChoiceResult = choiceResult;
        }

        public void RequestChoice()
        {
            System.Diagnostics.Debug.Assert(choiceRequestable, "RequestChoice called with ChoiceRequestable being false. Should never happen.");

            switch (choiceType)
            {
                case ChoiceHandler.ChoiceType.PlayerAction:
                    {
                        PlayerActionParams args;
                        choiceHandler.ChoosePlayerAction((List<Card>)choiceArgs[0], (Game)choiceArgs[1], (Player)choiceArgs[2], out args);
                        ChoiceResult = args;
                    } break;
                case ChoiceHandler.ChoiceType.Cards:
                    {
                        BoardChoices bc;
                        choiceHandler.ChooseCards(
                            (List<Card>)choiceArgs[0],
                            (Func<Dictionary<Card, int>, bool>)choiceArgs[1],
                            (Func<List<Card>, Dictionary<Card, int>, List<Card>>)choiceArgs[2],
                            (Func<Dictionary<Card, int>, bool, bool>)choiceArgs[3],
                            (bool)choiceArgs[4],
                            (Game)choiceArgs[5],
                            (Player)choiceArgs[6],
                            (PlayerActionParams)choiceArgs[7],
                            out bc);
                        ChoiceResult = bc;
                    } break;
                case ChoiceHandler.ChoiceType.BoardObjects:
                    {
                        BoardChoices bc;
                        choiceHandler.ChooseBoardObjects(
                            (List<BoardObject>)choiceArgs[0],
                            (Func<Dictionary<BoardObject, int>, bool>)choiceArgs[1],
                            (Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>>)choiceArgs[2],
                            (Func<Dictionary<BoardObject, int>, bool>)choiceArgs[3],
                            (Game)choiceArgs[4],
                            (Player)choiceArgs[5],
                            (string)choiceArgs[6],
                            out bc);

                        ChoiceResult = bc;
                    } break;
                case ChoiceHandler.ChoiceType.CardEffect:
                    {
                        BoardChoices bc = new BoardChoices();
                        choiceHandler.ChooseCardEffect(
                            (Card)choiceArgs[0],
                            (Game)choiceArgs[1],
                            (Player)choiceArgs[2],
                            (PlayerActionParams)choiceArgs[3],
                            out bc.PlayInfo);

                        ChoiceResult = bc;
                    } break;
                case ChoiceHandler.ChoiceType.MoI:
                    {
                        BoardChoices bc = new BoardChoices();
                        bool notCancelled = choiceHandler.ChooseMomentOfInsightUse(
                            (Game)choiceArgs[0],
                            (Player)choiceArgs[1],
                            out bc.MoIInfo);

                        bc.NotCancelled = notCancelled;
                        ChoiceResult = bc;
                    } break;
                case ChoiceHandler.ChoiceType.DoNothing:
                    {
                        ChoiceResult = choiceArgs[0];
                    } break;
            }
        }
    }
}
