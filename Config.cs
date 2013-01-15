using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace nfkservice
{
    class Config
    {


        private static string _serverExeFile;
        public static string ServerExeFile
        {
            get
            {
                if (_serverExeFile != null)
                    return _serverExeFile;

                var value = GetConfigurationValue("ServerExeFile");
                _serverExeFile = value ?? "server.dat";

                return _serverExeFile;
            }
        }

        private static string _exeParameters;
        public static string ExeParameters
        {
            get
            {
                if (_exeParameters != null)
                    return _exeParameters;

                var value = GetConfigurationValue("ExeParameters");
                _exeParameters = value ?? "+gowindow +nosound +nfkplanet +game server +exec server +dontsavecfg software";

                return _exeParameters;
            }
        }

        private static string _logFile;
        public static string LogFile
        {
            get
            {
                if (_logFile != null)
                    return _logFile;

                var value = GetConfigurationValue("LogFile");
                _logFile = value ?? @"basenfk\realtimelog.txt";

                return _logFile; 
            }
        }

        private static string _serviceName;
        public static string ServiceName
        {
            get
            {
                if (_serviceName != null)
                    return _serviceName;

                var value = GetConfigurationValue("ServiceName");
                _serviceName = value ?? "NFK";

                return _serviceName;
            }
        }

        private static int _processorAffinity;
        public static int ProcessorAffinity
        {
            get
            {
                if (_processorAffinity > 0)
                    return _processorAffinity;

                int value;
                int.TryParse(GetConfigurationValue("ProcessorAffinity"), System.Globalization.NumberStyles.HexNumber, null, out value);
                _processorAffinity = (value > 0) ? value : 0xFF;

                return _processorAffinity;
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
