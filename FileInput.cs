using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace grafs
{
    class FileIntput
    {
        public static string ReadFile(String path)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            if (path == "")
                return null;
            StreamReader sr = new StreamReader(path);
            string inLine = sr.ReadLine();
            string buff = null;
            while (inLine != null)
            {
                buff += inLine + "\n";
                inLine = sr.ReadLine();
            }
            sr.Close();

            return buff;
        }
    }
}
