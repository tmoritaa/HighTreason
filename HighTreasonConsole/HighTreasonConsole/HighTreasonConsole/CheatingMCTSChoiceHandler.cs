using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HighTreasonGame;

namespace HighTreasonConsole
{
    public class CheatingMCTSChoiceHandler : ChoiceHandler
    {
        private class Node
        {
            public Game GameState = null;
            public HTAction IncomingAction = null;

            private List<object> possibleChoices = new List<object>();

            public int NumVisits = 0;
            public int NumWins = 0;

            public Node Parent;
            public List<Node> children = new List<Node>();

            public Player SearchPlayer;

            public Node(Game game, Player player/*, HTAction outGoingAction*/, Node parent = null, HTAction action = null)
            {
                GameState = game;
                SearchPlayer = player;
                Parent = parent;
                IncomingAction = action;

                // TODO: node needs way to generate all possible choices.
                //possibleChoices = outGoingAction.GetPossibleChoices();
            }

            public Node Expand()
            {
                int idx = GlobalRandom.GetRandomNumber(0, possibleChoices.Count);
                object choice = possibleChoices[idx];
                possibleChoices.RemoveAt(idx);

                var action = new HTAction(choice);

                Game newGame = new Game(GameState, new ChoiceHandler[] { new FilterRandomAIChoiceHandler(), new FilterRandomAIChoiceHandler() });
                HTAction outgoingAction = newGame.Continue(action); // TODO: not sure if we have to save the action generated from this continue.

                // TODO: node needs way to generate all possible choices.
                return new Node(newGame, SearchPlayer, /*outgoingAction,*/ this, action);
            }

            public bool CanExpand()
            {
                return possibleChoices.Count > 0;
            }

            public bool IsTerminal()
            {
                return GameState.GameEnd;
            }

            public Node BestUCTChild()
            {
                float bestVal = -1;
                Node bestNode = null;

                float C = 1.0f / (float)Math.Sqrt(2.0);
                foreach (Node n in children)
                {
                    if (n.NumVisits > 0)
                    {
                        float uctVal = ((float)n.NumWins / n.NumVisits) + (2.0f * C * ((float)Math.Log(NumVisits) / n.NumVisits));

                        if (uctVal > bestVal)
                        {
                            bestVal = uctVal;
                            bestNode = n;
                        }
                    }
                    else
                    {
                        bestNode = n;
                        break;
                    }
                }

                return bestNode;
            }

            public bool PlayoutGame()
            {
                Player.PlayerSide winningSide = Player.PlayerSide.Prosecution;

                GameState.NotifyGameEnd =
                    (Player.PlayerSide winningPlayerSide, bool winByNotEnoughGuilt, int finalScore) =>
                    {
                        winningSide = winningPlayerSide;
                    };

                var action = GameState.Continue(this.IncomingAction);
                while (!GameState.GameEnd)
                {
                    if (action != null)
                    {
                        action.RequestChoice();
                    }
                    action = GameState.Continue(action);
                }

                return SearchPlayer.Side == winningSide;
            }

            public void Backpropagate(bool won)
            {
                Node node = this;

                while(node != null)
                {
                    node.NumVisits += 1;
                    node.NumWins += (won ? 1 : 0);
                    node = node.Parent;
                }
            }

            public HTAction PickActionForBestChild()
            {
                int bestVal = -1;
                Node bestNode = null;

                foreach (Node n in children)
                {
                    if (n.NumWins > bestVal)
                    {
                        bestNode = n;
                        bestVal = n.NumWins;
                    }
                }

                return bestNode.IncomingAction;
            }
        }

        public CheatingMCTSChoiceHandler() : base(Player.PlayerType.AI)
        {}

        private HTAction StartMCTS(Game game, Player searchPlayer)
        {
            Node root = new Node(new Game(game, new ChoiceHandler[] { new FilterRandomAIChoiceHandler(), new FilterRandomAIChoiceHandler() }), searchPlayer);

            for (int i = 0; i < 10000; ++i){
                Node node = findNodeToExpand(root);

                bool gameWon = node.PlayoutGame();
                node.Backpropagate(gameWon);
            }

            return root.PickActionForBestChild();
        }

        private Node findNodeToExpand(Node root)
        {
            Node expandNode = root;
            while (!expandNode.IsTerminal())
            {
                if (expandNode.CanExpand())
                {
                    return expandNode.Expand();
                }
                else
                {
                    expandNode = expandNode.BestUCTChild();
                }
            }

            return expandNode;
        }

        public override void ChoosePlayerAction(List<Card> cards, Game game, Player choosingPlayer, out PlayerActionParams outCardUsage)
        {
            HTAction action = StartMCTS(game, choosingPlayer);

            outCardUsage = (PlayerActionParams)action.ChoiceResult;
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
    }
}
