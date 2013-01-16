using System;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace nfkservice
{
    class Program
    {
        static void Main(string[] args)
        {
            // self service installer/uninstaller
            if (args != null && args.Length == 1
                && (args[0][0] == '-' || args[0][0] == '/'))
            {
                switch (args[0].Substring(1).ToLower())
                {
                    case "install":
                    case "i":
                        if (!ServiceInstallerUtility.InstallMe())
                            Console.WriteLine("Failed to install service");
                        break;
                    case "uninstall":
                    case "u":
                        if (!ServiceInstallerUtility.UninstallMe())
                            Console.WriteLine("Failed to uninstall service");
                        break;
                    default:
                        Console.WriteLine("Unrecognized parameters.");
                        break;
                }
                Environment.Exit(0);
            }

            // set directory where the program placed
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            if (!File.Exists(Config.ServerExeFile))
            {
                var msg = string.Format("'{0}' doesn't exists!", Config.ServerExeFile);
                Console.WriteLine(msg);
                Log.Error(msg);
                Environment.Exit(0);
            }

            // console mode
            if (Environment.UserInteractive)
            {
                // register console close event
                _consoleHandler = new ConsoleCtrlHandlerDelegate(ConsoleEventHandler);
                SetConsoleCtrlHandler(_consoleHandler, true);

                Server.Start();
                // handle for console input command
                while (true)
                {
                    Server.SendCommand(Console.ReadLine());
                }
            }
            // service mode
            else
            {
                var service = new Service1();
                var servicesToRun = new ServiceBase[] { service };

                ServiceBase.Run(servicesToRun);
            }
        }




        #region Page Event Setup
        enum ConsoleCtrlHandlerCode : uint
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
        delegate bool ConsoleCtrlHandlerDelegate(ConsoleCtrlHandlerCode eventCode);
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ConsoleCtrlHandlerDelegate handlerProc, bool add);
        static ConsoleCtrlHandlerDelegate _consoleHandler;
        #endregion

        #region Page Events
        static bool ConsoleEventHandler(ConsoleCtrlHandlerCode eventCode)
        {
            // Handle close event here...
            switch (eventCode)
            {
                case ConsoleCtrlHandlerCode.CTRL_C_EVENT:
                case ConsoleCtrlHandlerCode.CTRL_CLOSE_EVENT:
                case ConsoleCtrlHandlerCode.CTRL_BREAK_EVENT:
                case ConsoleCtrlHandlerCode.CTRL_LOGOFF_EVENT:
                case ConsoleCtrlHandlerCode.CTRL_SHUTDOWN_EVENT:

                    Server.Destroy();

                    Environment.Exit(0);
                    break;
            }

            return (false);
        }
        #endregion


    }
}
