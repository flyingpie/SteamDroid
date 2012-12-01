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
using SteamDroidServer.Networking;

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
            try
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
            catch (Exception e)
            {
                Logger.Get().Log("Exception: " + e.Message);
                Console.ReadLine();
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

        /// <summary>
        /// Loads the security key used for AES encryption
        /// </summary>
        /// <returns>Whether the key was successfully loaded</returns>
        static bool LoadKey()
        {
            if (File.Exists(Encryption.KeyFile))
            {
                String data = File.ReadAllText(Encryption.KeyFile).Trim();

                if (data.Length == 0)
                {
                    Logger.Get().Log("Warning; no security key entered");
                }
                else
                {
                    Encryption.SetKey(data);
                    Logger.Get().Log("Security key loaded");
                    return true;
                }
            }
            else
            {
                Logger.Get().Log("Warning; security key could not be found (" + Encryption.KeyFile + ")");
            }

            return false;
        }
    }
}
