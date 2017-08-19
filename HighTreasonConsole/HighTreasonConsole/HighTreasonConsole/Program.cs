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
            Game game = new Game(new ConsoleEventHandler(), new IChoiceHandler[] { new ConsolePlayerChoiceHandler(), new ConsolePlayerChoiceHandler() });
            game.StartGame();

            Console.WriteLine("Game has ended");
            Console.ReadLine();
        }
    }
}
