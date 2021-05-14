using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Config
    {
        public static string host { get; set; } = "127.0.0.1";
        public static int port { get; set; } = 2222;
        public static string logFile { get; set; } = "Log.txt";

    }
}
