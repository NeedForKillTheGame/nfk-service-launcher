using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;

namespace nfkservice
{
    partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Server.Start();
        }

        protected override void OnStop()
        {
            Server.Destroy();
        }

        protected override void OnShutdown()
        {
            Server.Destroy();
        }

        /// <summary>
        /// Fetch commands from sendcommand.txt and send them to NFK server console
        ///  each command in file starts with new line
        /// </summary>
        /// <param name="command"></param>
        protected override void OnCustomCommand(int command)
        {
            if (command == 255)
            {
                try
                {
                    var commands = File.ReadAllLines("sendcommand.txt");
                    foreach (var cmd in commands)
                    {
                        if (!string.IsNullOrEmpty(cmd.Trim()))
                            Server.SendCommand(cmd.Trim());
                    }
                    Log.Info("External command received (" + commands + ")");

                }
                catch (Exception e)
                {
                    Log.Error("Execute external command failed. " + e.Message);
                }
            }
            
        }
        
    }
}
