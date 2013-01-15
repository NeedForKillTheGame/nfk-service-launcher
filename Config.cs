using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace nfkdedic
{
    class Config
    {

        public static string ServerExeFile;

        private static string _logFile;
        public static string LogFile
        {
            get
            { 
                if (_logFile == null)
                    _logFile = Path.GetDirectoryName(ServerExeFile) + @"\basenfk\REALTIMELOG.txt";

                return _logFile; 
            }
        }
    }
}
