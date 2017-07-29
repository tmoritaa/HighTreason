using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class Game
    {
        public Board board;

        public Game()
        {
            board = new Board();
        }

        public override string ToString()
        {
            return board.ToString();
        }

    }
}
