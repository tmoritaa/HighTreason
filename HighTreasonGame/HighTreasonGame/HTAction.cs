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

        public Player CurPlayer
        {
            get; private set;
        }

        private ChoiceHandler.ChoiceType choiceType;
        private ChoiceHandler choiceHandler;
        private Func<List<BoardObject>, List<object>> allBOChoiceGenFunc;
        private Func<List<Card>, List<object>> allCardChoiceGenFunc;
        private object[] choiceArgs;

        private bool choiceRequestable = false;

        public HTAction(ChoiceHandler _handler)
        {
            choiceHandler = _handler;
        }

        public HTAction InitForPlayerAction(List<Card> choices, Game game, Player choosingPlayer)
        {
            choiceType = ChoiceHandler.ChoiceType.PlayerAction;
            CurPlayer = choosingPlayer;
            initChoiceArgs(choices, game, choosingPlayer);
            return this;
        }

        public HTAction InitForChooseCards(
            Func<List<Card>, List<object>> _allCardChoiceGenFunc,
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
            CurPlayer = choosingPlayer;
            allCardChoiceGenFunc = _allCardChoiceGenFunc;
            initChoiceArgs(choices, validateChoices, filterChoices, choicesComplete, stoppable, game, choosingPlayer, desc);
            return this;
        }

        public HTAction InitForChooseBOs(
            Func<List<BoardObject>, List<object>> _allBOChoiceGenFunc,
            List<BoardObject> choices,
            Func<Dictionary<BoardObject, int>, bool> validateChoices,
            Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>> filterChoices,
            Func<Dictionary<BoardObject, int>, bool> choicesComplete,
            Game game,
            Player choosingPlayer,
            string desc)
        {
            choiceType = ChoiceHandler.ChoiceType.BoardObjects;
            CurPlayer = choosingPlayer;
            allBOChoiceGenFunc = _allBOChoiceGenFunc;
            initChoiceArgs(choices, validateChoices, filterChoices, choicesComplete, game, choosingPlayer, desc);
            return this;
        }

        //public HTAction InitForCardEffect(
        //    Card cardToPlay,
        //    Game game,
        //    Player choosingPlayer,
        //    string desc)
        //{
        //    choiceType = ChoiceHandler.ChoiceType.CardEffect;
        //    initChoiceArgs(cardToPlay, game, choosingPlayer, desc);
        //    return this;
        //}

        public HTAction InitForMoI(Game game, Player choosingPlayer)
        {
            choiceType = ChoiceHandler.ChoiceType.MoI;
            CurPlayer = choosingPlayer;
            initChoiceArgs(game, choosingPlayer);
            return this;
        }

        public HTAction InitForDoNothing(object result, Player choosingPlayer)
        {
            choiceType = ChoiceHandler.ChoiceType.DoNothing;
            CurPlayer = choosingPlayer;
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
                        choiceHandler.ChoosePlayerAction((List<Card>)choiceArgs[0], (Game)choiceArgs[1], (Player)choiceArgs[2], this, out args);
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
                            this,
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
                            this,
                            out bc);

                        ChoiceResult = bc;
                    } break;
                //case ChoiceHandler.ChoiceType.CardEffect:
                //    {
                //        BoardChoices bc = new BoardChoices();
                //        choiceHandler.ChooseCardEffect(
                //            (Card)choiceArgs[0],
                //            (Game)choiceArgs[1],
                //            (Player)choiceArgs[2],
                //            (string)choiceArgs[3],
                //            this,
                //            out bc.PlayInfo);

                //        ChoiceResult = bc;
                //    } break;
                case ChoiceHandler.ChoiceType.MoI:
                    {
                        BoardChoices bc = new BoardChoices();
                        bool notCancelled = choiceHandler.ChooseMomentOfInsightUse(
                            (Game)choiceArgs[0],
                            (Player)choiceArgs[1],
                            this,
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

        public List<object> generateAllPossibleChoices(int k)
        {
            List<object> results = new List<object>();
            switch (choiceType)
            {
                case ChoiceHandler.ChoiceType.PlayerAction:
                    {
                        List<Card> cards = (List<Card>)choiceArgs[0];
                        Game game = (Game)choiceArgs[1];
                        Player player = (Player)choiceArgs[2];

                        foreach (var card in cards)
                        {
                            var pairs = card.GetEventEffectPairs(game);

                            for (int i = 0; i < pairs.Count; ++i)
                            {
                                CardInfo.EffectInfo effectInfo = card.GetCardInfoEffectInfo(game, i);

                                CardInfo.EffectInfo.EffectType playerSide = (player.Side == Player.PlayerSide.Prosecution) ? CardInfo.EffectInfo.EffectType.Prosecution : CardInfo.EffectInfo.EffectType.Defense;

                                if (pairs[i].Selectable(game, player) && 
                                    (game.CurState.StateType == GameState.GameStateType.JurySelection || effectInfo.Type == CardInfo.EffectInfo.EffectType.Neutral || effectInfo.Type == playerSide))
                                {
                                    results.Add(new PlayerActionParams(PlayerActionParams.UsageType.Event, card, i));
                                }
                            }

                            if (game.CurState.StateType == GameState.GameStateType.TrialInChief || game.CurState.StateType == GameState.GameStateType.Summation)
                            {
                                results.Add(new PlayerActionParams(PlayerActionParams.UsageType.Action, card));
                            }
                        }

                        // TODO: disable for now.
                        //if (game.CurState.StateType == GameState.GameStateType.TrialInChief)
                        //{
                        //    results.Add(new PlayerActionParams(PlayerActionParams.UsageType.Mulligan));
                        //}
                    }
                    break;
                case ChoiceHandler.ChoiceType.Cards:
                    {
                        List<Card> cards = (List<Card>)choiceArgs[0];

                        if (cards.Count > 0)
                        {
                            results.AddRange(allCardChoiceGenFunc(cards));
                        }
                        else
                        {
                            results.Add(new BoardChoices());
                        }
                        
                    }
                    break;
                case ChoiceHandler.ChoiceType.BoardObjects:
                    {
                        List<BoardObject> bos = (List<BoardObject>)choiceArgs[0];

                        if (bos.Count > 0)
                        {
                            results.AddRange(allBOChoiceGenFunc(bos));
                        }
                        else
                        {
                            results.Add(new BoardChoices());
                        }
                        
                    }
                    break;
                case ChoiceHandler.ChoiceType.MoI:
                    {
                        Game game = (Game)choiceArgs[0];
                        Player player = (Player)choiceArgs[1];

                        BoardChoices revBC = new BoardChoices();
                        revBC.MoIInfo.Use = BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Reveal;
                        results.Add(revBC);

                        foreach (Card handCard in player.Hand.SelectableCards)
                        {
                            foreach (Card sumCard in player.SummationDeck.Cards)
                            {
                                BoardChoices bc = new BoardChoices();
                                bc.MoIInfo.Use = BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Swap;
                                bc.MoIInfo.HandCard = handCard;
                                bc.MoIInfo.SummationCard = sumCard;
                                results.Add(bc);
                            }
                        }
                    }
                    break;
                case ChoiceHandler.ChoiceType.DoNothing:
                    {
                        results.Add(choiceArgs[0]);
                    }
                    break;
            }

            List<object> finalResult = new List<object>();
            for (int i = 0; i < k; ++i)
            {
                if (results.Count == 0)
                {
                    break;
                }

                int idx = GlobalRandom.GetRandomNumber(0, results.Count);
                finalResult.Add(results[idx]);
                results.RemoveAt(idx);
            }

            if (finalResult.Count == 0)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debugger.Break();
                }
            }

            return finalResult;
        }

        private void initChoiceArgs(params object[] args)
        {
            choiceArgs = args;
            choiceRequestable = true;
        }
    }
}
