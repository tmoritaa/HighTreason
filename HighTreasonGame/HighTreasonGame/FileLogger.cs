﻿using System;
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

        public static FileLogger Instance()
        {
            if (instance == null)
            {
                instance = new FileLogger();
            }

            return instance;
        }

        private FileLogger()
        {
            file = new StreamWriter(filePath);
        }

        ~FileLogger()
        {
            file.Close();
        }

        public void SetPath(string str)
        {
            file.Close();

            filePath = str;
            file = new StreamWriter(filePath);
        }

        public void Log(string str)
        {
            file.WriteLine(str);
        }
    }
}
