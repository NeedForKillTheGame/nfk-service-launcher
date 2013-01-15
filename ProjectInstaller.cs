using System;
using System.ComponentModel;
using System.Configuration.Install;

namespace nfkservice
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }


        protected override void OnBeforeInstall(System.Collections.IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);

            // set service name
            this.serviceInstaller1.DisplayName = Config.ServiceName;
            this.serviceInstaller1.ServiceName = Config.ServiceName;
        }

        protected override void OnBeforeUninstall(System.Collections.IDictionary savedState)
        {
            base.OnBeforeUninstall(savedState);

            // set service name
            this.serviceInstaller1.DisplayName = Config.ServiceName;
            this.serviceInstaller1.ServiceName = Config.ServiceName;
        }

    }
}
