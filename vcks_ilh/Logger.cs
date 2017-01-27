using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace vcks_ilh
{
    static class Logger
    {
        static string path = "log.txt";

        public static void Log(string message)
        {            
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine("{0}{1}{2}", new String('~', 50),Environment.NewLine, message);
            }
        }
    }
}
