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


            // TODO: register console close event
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);



            Server.Start();

            while (true)
            {
                Server.SendCommand(Console.ReadLine());
            }
        }


        

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {


            Environment.Exit(0);
        }


    }
}
