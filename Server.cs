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
        const int SW_SHOW = 5;

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
            // FIXME: run without window on start
            process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Config.ServerExeFile,
                    WorkingDirectory = Config.WorkingDirectory,
                    Arguments = Config.ExeParameters,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                }
            };

            process.Start();
            
            if (Environment.UserInteractive)
            {
                // wait a second (if no wait then window handle will be 0)
                Thread.Sleep(1000);

                if (isDestroy)
                    return;


                // get control handles
                // it doesn't work with a windows service but it isn't needed there
                mainHandle = process.MainWindowHandle;

                ShowWindow(mainHandle, SW_HIDE); // hide main window
                textHandle = FindWindowEx(mainHandle, new IntPtr(0), "TMemo", null);
                inputHandle = FindWindowEx(mainHandle, new IntPtr(0), "TEdit", null);
                sendHandle = FindWindowEx(mainHandle, new IntPtr(0), "TButton", null);

                Log.Debug(string.Format("Handles: {0}, {1}, {2}, {3}", mainHandle.ToString(), textHandle.ToString(), inputHandle.ToString(), sendHandle.ToString()));

                process.PriorityClass = Config.ProcessorPriority; // process priority - doesn't work when running as a service
            }
            else
                Process.GetCurrentProcess().PriorityClass = Config.ProcessorPriority; // process priority - works when running as a service
            
            process.ProcessorAffinity = (IntPtr)Config.ProcessorAffinity;


            // wait for process end
            while (!process.HasExited)
            {
                if (Environment.UserInteractive)
                    Log.Push(GetOutput(textHandle)); // fetch log lines from the NFK window
         
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
        /// Return log output from nfk console
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
        /// <summary>
        /// Kill nfk server process
        /// </summary>
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
