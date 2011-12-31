using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using SteamKit2;
using SteamDroidServer.Logging;
using SteamDroidServer.Steam;

namespace SteamDroidServer
{
    class Program
    {
        /// <summary>
        /// The filename of the SteamKit2 library
        /// </summary>
        public const String SteamKitDll = "SteamKit2.dll";

        public const String DefaultIp = "";
        public const int DefaultPort = 1337;

        static void Main(string[] args)
        {
            String ip = DefaultIp;
            int port = DefaultPort;

            if (CheckSteamKit())
            {
                // Use the specified ip address, if any
                if (args.Length >= 1)
                {
                    String useIp = args[0];
                    ip = useIp;
                }
                
                // Use the specified port number, if any
                if (args.Length >= 2)
                {
                    int usePort;
                    if (int.TryParse(args[1], out usePort))
                    {
                        port = usePort;
                    }
                }

                SteamDroidServer server = new SteamDroidServer();
                server.Start(ip, port);
            }
        }

        /// <summary>
        /// Checks for existance of the SteamKit library
        /// </summary>
        /// <returns>Whether the library exists</returns>
        static bool CheckSteamKit()
        {
            bool result = File.Exists(SteamKitDll);

            if (!result)
            {
                Logger.Get().Log("Warning; missing SteamKit2.dll, please download it from https://bitbucket.org/VoiDeD/steamre/downloads");
                Console.ReadLine();
            }

            return result;
        }
    }
}
