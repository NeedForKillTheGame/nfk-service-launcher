using System;
using System.IO;
using System.Net;
using System.Xml;

namespace nfkservice
{
    class AutoUpdate
    {
        private string workingDirectory;
        private IniFile iniFile;

        public AutoUpdate()
        {
            workingDirectory = Path.GetDirectoryName(Config.ServerExeFile);
            iniFile = new IniFile(workingDirectory + @"\basenfk\nfksetup.ini");

        }

        /* Example XML:
         * <?xml version="1.0" encoding="UTF-8"?><update><files ver="3"><file dir="\" url="/">Server.dat</file></files><files ver="12"><file dir="\SERVER\" url="/">bot.dll</file></files><lastver>12</lastver></update>
         */
        public void Run()
        {
            try
            {
                var xmlString = downloadString(Config.AutoUpdateUrl);

                Log.Debug("AutoUpdate XML downloaded: " + xmlString);

                var xmlDoc= new XmlDocument(); 
                xmlDoc.LoadXml(xmlString);

                // assign "lastver" value to RemoteVersion
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                    if (node.Name == "lastver")
                        RemoteVersion = int.Parse(node.InnerText);
                
                
                if (RemoteVersion > LocalVersion)
                {
                    Log.Info(string.Format("Remote version({0}) > Local version({1}). Starting update...", RemoteVersion, LocalVersion));

                    // download updated files
                    foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                        if (node.Name == "files" && int.Parse(node.Attributes["ver"].Value) > LocalVersion) // if files pack version > local version then download each file
                            foreach (XmlNode file in node.ChildNodes)
                            {
                                var remoteFile = file.Attributes["url"].Value + file.InnerText; // relative path
                                if (file.Attributes["url"].Value == "/")
                                    remoteFile = Config.AutoUpdateUrl.Remove(Config.AutoUpdateUrl.Length - Path.GetFileName(Config.AutoUpdateUrl).Length - 1) + remoteFile; // full path

                                var localFile = file.Attributes["dir"].Value + file.InnerText; // relative path
                                localFile = workingDirectory + localFile; // full path

                                downloadFile(remoteFile, localFile);
                            }

                    // update local version into ini
                    LocalVersion = RemoteVersion;
                }
            }
            catch (Exception e)
            {
                Log.Error("AutoUpdate failed. " + e.Message + e.InnerException);
            }
        }

        private void downloadFile(string remoteFile, string localFile)
        {
            Log.Info(string.Format("Download file from '{0}' to '{1}'", remoteFile, localFile));
            new MyWebClient().DownloadFile(remoteFile, localFile);
        }

        private string downloadString(string url)
        {
            var str = new MyWebClient().DownloadString(url);
            return str;
        }

        /// <summary>
        /// Local version of the server
        /// </summary>
        public int LocalVersion
        {
            get
            {
                if (_localVersion == 0)
                {
                    string value = iniFile.IniReadValue("SERVER_VERSION", "Update");
                    int.TryParse(value, out _localVersion);
                }
                return _localVersion;
            }
            set
            {
                _localVersion = value;
                iniFile.IniWriteValue("SERVER_VERSION", "Update", _localVersion.ToString());
            }
        }
        private int _localVersion;

        /// <summary>
        /// Remove version of the server
        /// </summary>
        public int RemoteVersion;


        /// <summary>
        /// Webclient with timeout
        /// </summary>
        private class MyWebClient : WebClient
        {
            private readonly int _timeout;

            public MyWebClient(int timeout = 2000)
            {
                _timeout = timeout;
            }

            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                w.Timeout = _timeout;
                return w;
            }
        }
    }


}
