using System;

using HighTreasonGame.EventHandlers;

namespace HighTreasonGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(new ConsoleEventHandler());
            game.StartGame();

            Console.WriteLine("Game has ended");
            Console.ReadLine();
        }
    }
}
