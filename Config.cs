using System;
using System.Configuration;
using System.Diagnostics;
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

        private static int? _processorAffinity;
        public static int ProcessorAffinity
        {
            get
            {
                if (_processorAffinity != null)
                    return (int)_processorAffinity;

                _processorAffinity = 0xFF; // default first 8 processors

                int value;
                if (int.TryParse(GetConfigurationValue("ProcessorAffinity"), System.Globalization.NumberStyles.HexNumber, null, out value))
                    if (value > 0)
                        _processorAffinity = value;

                return (int)_processorAffinity;
            }
        }

        private static ProcessPriorityClass? _processorPriority;
        public static ProcessPriorityClass ProcessorPriority
        {
            get
            {
                if (_processorPriority != null)
                    return (ProcessPriorityClass)_processorPriority;

                _processorPriority = ProcessPriorityClass.AboveNormal; // default above normal

                int value;
                if (int.TryParse(GetConfigurationValue("ProcessorPriority"), out value))
                {
                    switch (value)
                    {
                        case 0:
                            _processorPriority = ProcessPriorityClass.Idle;
                            break;
                        case 1:
                            _processorPriority = ProcessPriorityClass.BelowNormal;
                            break;
                        case 2:
                            _processorPriority = ProcessPriorityClass.Normal;
                            break;
                        case 3:
                            _processorPriority = ProcessPriorityClass.AboveNormal;
                            break;
                        case 4:
                            _processorPriority = ProcessPriorityClass.High;
                            break;
                        case 5:
                            _processorPriority = ProcessPriorityClass.RealTime;
                            break;
                    }
                }

                return (ProcessPriorityClass)_processorPriority;
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
