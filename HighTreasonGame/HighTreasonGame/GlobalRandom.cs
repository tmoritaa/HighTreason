using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class GlobalRandom
    {
        [ThreadStatic]
        private static Random random = new Random();

        private static object syncLock = new object();

        public static int GetRandomNumber(int min, int max)
        {
            int num = 0;
            lock (syncLock)
            {
                num = random.Next(min, max);
            }
            return num;
        }
    }
}
