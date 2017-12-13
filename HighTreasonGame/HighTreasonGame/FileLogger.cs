using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace HighTreasonGame
{
    public class FileLogger
    {
        private string filePath = "logs.txt";

        private static FileLogger instance = null;

        private StreamWriter file;

        public static FileLogger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FileLogger();
                }

                return instance;
            }
        }

        private FileLogger()
        {
            File.WriteAllText(filePath, String.Empty);
            file = new StreamWriter(filePath);
        }

        public void SetPath(string str)
        {
            file.Close();

            filePath = str;
            File.WriteAllText(filePath, String.Empty);

            file = new StreamWriter(filePath, true);
        }

        public void Log(string str)
        {
            file.WriteLine(str);
            file.Flush();
        }

        public void Close()
        {
            file.Close();
        }
    }
}
