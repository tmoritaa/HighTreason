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
            string path = "HighTreasonCardTexts.json";

            if (args.Length > 0)
            {
                path = args[0];
            }

            int numIterations = 1;
            if (args.Length > 1)
            {
                numIterations = Int32.Parse(args[1]);
            }

            ChoiceHandler[] handlers = new ChoiceHandler[] { new RandomAIChoiceHandler(), new RandomAIChoiceHandler() };

            if (args.Length >= 4)
            {
                for (int i = 0; i < 2; ++i)
                {
                    string playerAIStr = args[2 + i];

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

            foreach(var handler in handlers)
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
        }
    }
}
