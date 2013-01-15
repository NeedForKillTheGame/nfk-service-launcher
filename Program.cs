using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace nfkdedic
{
    class Program
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, int param, System.Text.StringBuilder text);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        const int SW_HIDE = 0;

        const int WM_GETTEXT = 0x0D;
        const int WM_GETTEXTLENGTH = 0x0E;
        const int WM_SETTEXT = 0x000C;
        const int WM_COMMAND = 0xf5;

        private static Process process;

        private static IntPtr mainHandle;
        private static IntPtr textHandle;
        private static IntPtr inputHandle;
        private static IntPtr sendHandle;



        static void Main(string[] args)
        {
            Config.ServerExeFile = @"m:\SERVERS\NFK\Server_1-All\Server_1-All\Server.dat";


            // TODO: register console close event
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            // start nfk dedicated in new thread
            var t = new Thread(StartNFK);
            t.Start();



            while (true)
            {
                SendCommand(Console.ReadLine());
            }
        }

        [STAThread]
        private static void StartNFK()
        {
            // TODO: run without window on start
            process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Config.ServerExeFile,
                    WorkingDirectory = Path.GetDirectoryName(Config.ServerExeFile),
                    Arguments = "+gowindow +nosound +nfkplanet +game server +exec server +dontsavecfg software",
                    RedirectStandardOutput = false,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Minimized
                }
            };

            process.Start();

            // wait a second
            Thread.Sleep(1000);


            // get control handles
            mainHandle = process.MainWindowHandle;

            ShowWindow(mainHandle, SW_HIDE); // hide main window
            textHandle = FindWindowEx(mainHandle, new IntPtr(0), "TMemo", null);
            inputHandle = FindWindowEx(mainHandle, new IntPtr(0), "TEdit", null);
            sendHandle = FindWindowEx(mainHandle, new IntPtr(0), "TButton", null);


            // write console log to file
            while (!process.HasExited)
            {
                Log.Push(GetOutput(textHandle));

                // second interfal to fetch log from the NFK window
                Thread.Sleep(1000);
            }

        }

        /// <summary>
        /// Send command to NFK console
        /// </summary>
        /// <param name="text"></param>
        private static void SendCommand(string text)
        {
            // fill input
            SendMessage(inputHandle, WM_SETTEXT, 0, new StringBuilder(text));
            // click send
            SendMessage(sendHandle, WM_COMMAND, 0, null);
        }

        /// <summary>
        /// Return output from NFK console
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        private static string GetOutput(IntPtr hWnd)
        {
            int Len = SendMessage(hWnd, WM_GETTEXTLENGTH, 0, null) + 1;

            var text = new System.Text.StringBuilder(Len);  // or length from call with GETTEXTLENGTH
            int RetVal = SendMessage(hWnd, WM_GETTEXT, text.Capacity, text);

            return text.ToString();
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            // kill nfk on program exit
            if (!process.HasExited)
                process.Kill();

            Environment.Exit(0);
        }


    }
}
