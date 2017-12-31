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
            public int NumVisits
            {
                get; private set;
            }

            public int NumWins
            {
                get; private set;
            }

            public Node Parent
            {
                get; private set;
            }

            private Game gameState = null;
            private object incomingResult = null;
            private HTAction outGoingAction = null;
            private List<object> possibleChoices = new List<object>();
            private List<Node> children = new List<Node>();
            private Player searchPlayer;

            public Node(Game game, Player player, HTAction _outGoingAction, Node parent = null, object actionResult = null)
            {
                gameState = game;
                searchPlayer = player;
                Parent = parent;

                if (Parent != null)
                {
                    Parent.AddChild(this);
                }

                NumVisits = 0;
                NumWins = 0;

                incomingResult = actionResult;
                outGoingAction = _outGoingAction;

                if (!gameState.GameEnd)
                {
                    possibleChoices = outGoingAction.generateAllPossibleChoices(10);
                }
            }

            public void AddChild(Node child)
            {
                children.Add(child);
            }

            public Node Expand()
            {
                int idx = GlobalRandom.GetRandomNumber(0, possibleChoices.Count);
                object choice = possibleChoices[idx];

                if (choice.GetType() != typeof(PlayerActionParams) && choice.GetType() != typeof(BoardChoices))
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                }

                possibleChoices.RemoveAt(idx);

                Game newGame = new Game(gameState, new ChoiceHandler[] { new FilterRandomAIChoiceHandler(), new FilterRandomAIChoiceHandler() });

                HTAction action = newGame.Continue(choice);
                while ((action == null || action.CurPlayer.Side != searchPlayer.Side) && !newGame.GameEnd)
                {
                    action?.RequestChoice();

                    action = newGame.Continue(action?.ChoiceResult);
                }

                return new Node(newGame, searchPlayer, action, this, choice);
            }

            public bool CanExpand()
            {
                return possibleChoices.Count > 0;
            }

            public bool IsTerminal()
            {
                return gameState.GameEnd;
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

                if (bestNode == null)
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                }

                return bestNode;
            }

            public bool PlayoutGame()
            {
                Player.PlayerSide winningSide = Player.PlayerSide.Prosecution;

                gameState.NotifyGameEnd =
                    (Player.PlayerSide winningPlayerSide, bool winByNotEnoughGuilt, int finalScore) =>
                    {
                        winningSide = winningPlayerSide;
                    };

                Game game = new Game(gameState, new ChoiceHandler[] { new FilterRandomAIChoiceHandler(), new FilterRandomAIChoiceHandler() });
                HTAction action = outGoingAction;
                while (!game.GameEnd)
                {
                    if (action != null)
                    {
                        action.RequestChoice();
                    }
                    action = game.Continue(action?.ChoiceResult);
                }

                return searchPlayer.Side == winningSide;
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

            public object PickActionForBestChild()
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

                return bestNode.incomingResult;
            }
        }

        public CheatingMCTSChoiceHandler() : base(Player.PlayerType.AI)
        {}

        private object PerformMCTS(Game game, Player searchPlayer, HTAction action)
        {
            FileLogger.Instance.SetLogOn(false);

            Node root = new Node(
                new Game(game, new ChoiceHandler[] { new FilterRandomAIChoiceHandler(), new FilterRandomAIChoiceHandler() }), 
                searchPlayer,
                action);

            for (int i = 0; i < 1000; ++i){
                Node node = findNodeToExpand(root);

                bool gameWon = node.PlayoutGame();
                node.Backpropagate(gameWon);
            }

            FileLogger.Instance.SetLogOn(true);

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

        public override void ChoosePlayerAction(List<Card> cards, Game game, Player choosingPlayer, HTAction action, out PlayerActionParams outCardUsage)
        {
            object choiceResult = PerformMCTS(game, choosingPlayer, action);
            outCardUsage = (PlayerActionParams)choiceResult;
        }

        public override void ChooseBoardObjects(List<BoardObject> choices, Func<Dictionary<BoardObject, int>, bool> validateChoices, Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>> filterChoices, Func<Dictionary<BoardObject, int>, bool> choicesComplete, Game game, Player choosingPlayer, string description, HTAction action, out BoardChoices boardChoice)
        {
            object choiceResult = PerformMCTS(game, choosingPlayer, action);
            boardChoice = (BoardChoices)choiceResult;
        }

        public override void ChooseCardEffect(Card cardToPlay, Game game, Player choosingPlayer, string description, HTAction action, out BoardChoices.CardPlayInfo cardPlayInfo)
        {
            throw new NotImplementedException();
        }

        public override void ChooseCards(List<Card> choices, Func<Dictionary<Card, int>, bool> validateChoices, Func<List<Card>, Dictionary<Card, int>, List<Card>> filterChoices, Func<Dictionary<Card, int>, bool, bool> choicesComplete, bool stoppable, Game game, Player choosingPlayer, string description, HTAction action, out BoardChoices boardChoice)
        {
            object choiceResult = PerformMCTS(game, choosingPlayer, action);
            boardChoice = (BoardChoices)choiceResult;
        }

        public override bool ChooseMomentOfInsightUse(Game game, Player choosingPlayer, HTAction action, out BoardChoices.MomentOfInsightInfo outMoIInfo)
        {
            object choiceResult = PerformMCTS(game, choosingPlayer, action);
            outMoIInfo = ((BoardChoices)choiceResult).MoIInfo;
            return true;
        }
    }
}
