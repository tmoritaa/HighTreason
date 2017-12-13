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
            string jsonText = System.IO.File.ReadAllText("HighTreasonCardTexts.json");

            Game game = new Game(new ChoiceHandler[] { new RandomAIChoiceHandler(), new RandomAIChoiceHandler() }, jsonText);

            game.StartGame();

            Console.WriteLine("Game has ended");
            Console.ReadLine();
        }
    }
}
