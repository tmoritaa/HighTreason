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
            Game game = new Game(new ChoiceHandler[] { new ConsolePlayerChoiceHandler(), new ConsolePlayerChoiceHandler() });
            game.NotifyStateStart += () =>
            {
                System.Console.WriteLine("=========================================================================");
                System.Console.WriteLine("Going to state " + game.CurState.GetType());
                System.Console.WriteLine("=========================================================================");
            };
            game.NotifyPlayedCard += (Player.CardUsageParams cardUsage) =>
            {
                System.Console.WriteLine("Player " + game.CurPlayer.Side + " played " + cardUsage.card.Name + " as event at idx " + (int)cardUsage.misc[0]);
            };
            game.NotifyGameEnd += (Player.PlayerSide winningSide) =>
            {
                Console.WriteLine("Player " + winningSide + " won");
            };

            game.StartGame();

            Console.WriteLine("Game has ended");
            Console.ReadLine();
        }
    }
}
