using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace nfkdedic
{
    class Config
    {


        private static string _serverExeFile = "server.dat";
        public static string ServerExeFile
        {
            get
            {
                var value = GetConfigurationValue("ServerExeFile");
                if (value != null)
                    _serverExeFile = value;

                return _serverExeFile;
            }
        }

        private static string _exeParameters = "+gowindow +nosound +nfkplanet +game server +exec server +dontsavecfg software";
        public static string ExeParameters
        {
            get
            {
                var value = GetConfigurationValue("ExeParameters");
                if (value != null)
                    _exeParameters = value;

                return _exeParameters;
            }
        }

        private static string _logFile = @"basenfk\realtimelog.txt";
        public static string LogFile
        {
            get
            {
                var value = GetConfigurationValue("LogFile");
                if (value != null)
                    _logFile = value;

                return _logFile; 
            }
        }

        private static int _processorAffinity = 0xFF;
        public static int ProcessorAffinity
        {
            get
            {
                int value = 0;
                int.TryParse(GetConfigurationValue("ProcessorAffinity"), System.Globalization.NumberStyles.HexNumber, null, out value);
                if (value != 0)
                    _processorAffinity = value;

                return _processorAffinity;
            }
        }

        private static string _serviceName = "NFK";
        public static string ServiceName
        {
            get
            {
                var value = GetConfigurationValue("ServiceName");
                if (value != null)
                    _serviceName = value;

                return _serviceName;
            }
        }


        private static string GetConfigurationValue(string key)
        {
            Assembly service = Assembly.GetAssembly(typeof(Program));
            Configuration config = ConfigurationManager.OpenExeConfiguration(service.Location);
            if (config.AppSettings.Settings[key] != null)
            {
                return config.AppSettings.Settings[key].Value;
            }
            else
            {
                return null;
                //throw new IndexOutOfRangeException("Settings collection does not contain the requested key: " + key);
            }
        }
    }
}
