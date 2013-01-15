using System;
using System.IO;
using System.Text;

namespace nfkdedic
{
    static class Log
    {
        private static StringBuilder oldtext = new StringBuilder();

        /// <summary>
        /// Push only modified lines to log
        /// </summary>
        /// <param name="text"></param>
        public static void Push(string text)
        {
            var oldtextlength = oldtext.Length;

            if (text.Length == oldtextlength)
                return;

            // iterate text starting from the end
            StringBuilder appendtext = new StringBuilder();
            StringBuilder line = new StringBuilder();
            for (int i = oldtextlength; i < text.Length; i++)
            {
                line.Append(text[i]);

                if (text[i] == '\n')
                {
                    appendtext.AppendFormat("[{0}] {1}", DateTime.Now, line);
                    line = new StringBuilder();
                }
            }
                
            Write(appendtext.ToString());

            oldtext.Append(text.Substring(oldtextlength, text.Length - oldtextlength));
        }


        public static void Append(string text)
        {
            Write(text);
        }


        static void Write(string text)
        {
            using (StreamWriter w = File.AppendText(Config.LogFile))
            {
                w.Write(text);
                Console.Write(text);
            }
        }
    }
}
