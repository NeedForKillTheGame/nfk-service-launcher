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
    }
}
