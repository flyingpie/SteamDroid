using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamDroidServer.Logging
{
    public class Logger
    {
        private static Logger logger;

        public Logger()
        {
            logger = this;
        }

        public static Logger Get()
        {
            if (logger == null) new Logger();
            return logger;
        }

        public void Log(String message)
        {
            Console.WriteLine(DateTime.Now.ToString() + " " + message);
        }
    }
}
