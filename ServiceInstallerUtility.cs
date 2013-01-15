using System.Configuration.Install;
using System.Reflection;

namespace nfkdedic
{
    class ServiceInstallerUtility
    {
        private static readonly string exePath =
          Assembly.GetExecutingAssembly().Location;
        public static bool InstallMe()
        {
            try { ManagedInstallerClass.InstallHelper(new[] { exePath }); }
            catch { return false; }
            return true;
        }
        public static bool UninstallMe()
        {
            try { ManagedInstallerClass.InstallHelper(new[] { "/u", exePath }); }
            catch { return false; }
            return true;
        }
    }
}
