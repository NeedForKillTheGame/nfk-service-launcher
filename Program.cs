using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace nfkdedic
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("usage: nfkdedic.exe [server.dat]");
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("'{0}' doesn't exists!", args[0]);
                return;
            }

            Config.ServerExeFile = args[0];


            // register console close event
            _consoleHandler = new ConsoleCtrlHandlerDelegate(ConsoleEventHandler);
            SetConsoleCtrlHandler(_consoleHandler, true);


            // run process in new thread
            Server.Start();

            // handle for console input command
            while (true)
            {
                Server.SendCommand(Console.ReadLine());
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
