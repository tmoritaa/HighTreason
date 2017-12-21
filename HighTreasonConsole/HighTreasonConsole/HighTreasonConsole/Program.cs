using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HighTreasonGame;

namespace HighTreasonConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            bool cloneTest = false;

            if (args.Length > 0)
            {
                cloneTest = args[0].Equals("test");
            }

            string path = "HighTreasonCardTexts.json";

            if (args.Length > 1)
            {
                path = args[1];
            }

            int numIterations = 1;
            if (args.Length > 2)
            {
                numIterations = Int32.Parse(args[2]);
            }

            ChoiceHandler[] handlers = new ChoiceHandler[] { new RandomAIChoiceHandler(), new RandomAIChoiceHandler() };

            if (args.Length >= 5)
            {
                for (int i = 0; i < 2; ++i)
                {
                    string playerAIStr = args[3 + i];

                    ChoiceHandler handler = new RandomAIChoiceHandler();
                    switch (playerAIStr)
                    {
                        case "random":
                            handler = new RandomAIChoiceHandler();
                            break;
                        case "filter-random":
                            handler = new FilterRandomAIChoiceHandler();
                            break;
                    }

                    handlers[i] = handler;
                }
            }

            string jsonText = System.IO.File.ReadAllText(path);

            foreach (var handler in handlers)
            {
                Console.WriteLine(handler);
            }

            int prosecutionWins = 0;
            int notEnoughGuilt = 0;
            for (int i = 0; i < numIterations; ++i)
            {
                FileLogger.Instance.SetPath("logs" + (i + 1) + ".txt");

                Game game = new Game(new ChoiceHandler[] { handlers[0], handlers[1] }, jsonText);

                game.NotifyGameEnd +=
                    (Player.PlayerSide winningPlayerSide, bool winByNotEnoughGuilt, int finalScore) =>
                    {
                        if (winningPlayerSide == Player.PlayerSide.Prosecution)
                        {
                            prosecutionWins += 1;
                        }

                        if (winByNotEnoughGuilt)
                        {
                            notEnoughGuilt += 1;
                        }
                    };

                HighTreasonGame.Action action = game.Start();
                while (!game.GameEnd)
                {
                    if (cloneTest)
                    {
                        Game cloneGame = new Game(game);
                        bool equal = cloneGame.CheckCloneEquality(game);

                        if (!equal)
                        {
                            if (System.Diagnostics.Debugger.IsAttached)
                            {
                                System.Diagnostics.Debugger.Break();
                            }
                            else
                            {
                                Console.WriteLine("Clone test failed");
                            }
                        }

                        game = cloneGame;
                        game.NotifyGameEnd +=
                            (Player.PlayerSide winningPlayerSide, bool winByNotEnoughGuilt, int finalScore) =>
                            {
                                if (winningPlayerSide == Player.PlayerSide.Prosecution)
                                {
                                    prosecutionWins += 1;
                                }

                                if (winByNotEnoughGuilt)
                                {
                                    notEnoughGuilt += 1;
                                }
                            };
                    }

                    if (action != null)
                    {
                        action.RequestChoice();
                    }
                    action = game.Continue(action);
                }

                Console.WriteLine("Game " + (i + 1) + " has ended");
            }

            FileLogger.Instance.SetPath("results.txt");
            string resultsStr = string.Format("Total Iterations = {0}\nProsecution Wins = {1}\nDefense Wins = {2}\nNot Enough Guilt = {3}",
                numIterations,
                prosecutionWins,
                numIterations - prosecutionWins,
                notEnoughGuilt);
            FileLogger.Instance.Log(resultsStr);

            Console.WriteLine("Press any key to complete");
            Console.ReadLine();
        }
    }
}
