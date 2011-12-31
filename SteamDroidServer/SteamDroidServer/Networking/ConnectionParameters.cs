using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamDroidServer.Networking
{
    /// <summary>
    /// ConnectionParameters contain information for the client used for connecting to a gameserver
    /// </summary>
    public class ConnectionParameters
    {
        private String ip;
        private int port;
        private String connectionKey;

        public ConnectionParameters(String ip, int port, String connectionKey)
        {
            this.ip = ip;
            this.port = port;
            this.connectionKey = connectionKey;
        }

        /// <summary>
        /// The ip of the gameserver
        /// </summary>
        public String Ip
        {
            get { return ip; }
        }

        /// <summary>
        /// The port the gameservers is running at
        /// </summary>
        public int Port
        {
            get { return port; }
        }

        /// <summary>
        /// The connection key to use
        /// </summary>
        public String ConnectionKey
        {
            get { return connectionKey; }
        }
    }
}
