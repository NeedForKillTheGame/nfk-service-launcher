using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;

namespace nfkservice
{
    class Server
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

        
        
        
        /// <summary>
        /// start nfk dedicated in new thread
        /// </summary>
        public static void Start()
        {
            var t = new Thread(StartNFK);
            t.Start();
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
                    Arguments = Config.ExeParameters,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                }
            };

            process.Start();

            // wait a second (if no wait then window handle will be 0)
            Thread.Sleep(1000);

            if (isDestroy)
                return;

            process.ProcessorAffinity = (IntPtr)Config.ProcessorAffinity;
            process.PriorityClass = Config.ProcessorPriority; // doesn't work when running as a service
            Process.GetCurrentProcess().PriorityClass = Config.ProcessorPriority; // works when running as a service


            // get control handles
            // FIXME: doesn't work with windows service :(
            mainHandle = process.MainWindowHandle;

            ShowWindow(mainHandle, SW_HIDE); // hide main window
            textHandle = FindWindowEx(mainHandle, new IntPtr(0), "TMemo", null);
            inputHandle = FindWindowEx(mainHandle, new IntPtr(0), "TEdit", null);
            sendHandle = FindWindowEx(mainHandle, new IntPtr(0), "TButton", null);

            Log.Debug(string.Format("Handles: {0}, {1}, {2}, {3}", mainHandle.ToString(), textHandle.ToString(), inputHandle.ToString(), sendHandle.ToString()));


            // write console log to file
            while (!process.HasExited)
            {
                Log.Push(GetOutput(textHandle));

                // a second interval to fetch log from the NFK window
                Thread.Sleep(1000);
            }

            if (isDestroy)
                return;

            Log.Error("Server crashed! Restarting...");
            Thread.Sleep(3000);
            Log.ClearOldText();
            Start();
        }

        /// <summary>
        /// Send command to NFK console
        /// </summary>
        /// <param name="text"></param>
        public static void SendCommand(string text)
        {
            // fill input
            SendMessage(inputHandle, WM_SETTEXT, 0, new StringBuilder(text));
            // click send
            SendMessage(sendHandle, WM_COMMAND, 0, null);

            if (text == "quit")
            {
                Destroy();
                Environment.Exit(0);                
            }
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


        private static bool isDestroy = false;
        public static void Destroy()
        {
            isDestroy = true;

            Log.Info("Shutdown from console");

            // kill nfk on program exit
            if (!process.HasExited)
                process.Kill();
        }
    }
}
