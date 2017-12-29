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
        //private PlayerActionParams usageContext;
        private object[] choiceArgs;

        private bool choiceRequestable = false;

        public HTAction(ChoiceHandler _handler/*, PlayerActionParams _usageContext*/)
        {
            choiceHandler = _handler;
            //usageContext = _usageContext;
        }

        public HTAction(object choiceResult)
        {
            ChoiceResult = choiceResult;
        }

        public HTAction InitForPlayerAction(List<Card> choices, Game game, Player choosingPlayer)
        {
            choiceType = ChoiceHandler.ChoiceType.PlayerAction;
            initChoiceArgs(choices, game, choosingPlayer);
            return this;
        }

        public HTAction InitForChooseCards(
            List<Card> choices, 
            Func<Dictionary<Card, int>, bool> validateChoices,
            Func<List<Card>, Dictionary<Card, int>, List<Card>> filterChoices,
            Func<Dictionary<Card, int>, bool, bool> choicesComplete,
            bool stoppable,
            Game game,
            Player choosingPlayer,
            string desc)
        {
            choiceType = ChoiceHandler.ChoiceType.Cards;
            initChoiceArgs(choices, validateChoices, filterChoices, choicesComplete, stoppable, game, choosingPlayer, desc);
            return this;
        }

        public HTAction InitForChooseBOs(
            List<BoardObject> choices,
            Func<Dictionary<BoardObject, int>, bool> validateChoices,
            Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>> filterChoices,
            Func<Dictionary<BoardObject, int>, bool> choicesComplete,
            Game game,
            Player choosingPlayer,
            string desc)
        {
            choiceType = ChoiceHandler.ChoiceType.BoardObjects;
            initChoiceArgs(choices, validateChoices, filterChoices, choicesComplete, game, choosingPlayer, desc);
            return this;
        }

        public HTAction InitForCardEffect(
            Card cardToPlay,
            Game game,
            Player choosingPlayer,
            string desc)
        {
            choiceType = ChoiceHandler.ChoiceType.CardEffect;
            initChoiceArgs(cardToPlay, game, choosingPlayer, desc);
            return this;
        }

        public HTAction InitForMoI(Game game, Player choosingPlayer)
        {
            choiceType = ChoiceHandler.ChoiceType.MoI;
            initChoiceArgs(game, choosingPlayer);
            return this;
        }

        public HTAction InitForDoNothing(object result)
        {
            choiceType = ChoiceHandler.ChoiceType.DoNothing;
            initChoiceArgs(result);
            return this;
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
                            (string)choiceArgs[7],
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
                            (string)choiceArgs[3],
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

        private void initChoiceArgs(params object[] args)
        {
            choiceArgs = args;
            choiceRequestable = true;
        }
    }
}
