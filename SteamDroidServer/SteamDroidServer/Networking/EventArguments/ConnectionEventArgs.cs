using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamDroidServer.Networking.EventArguments
{
    public class ConnectionEventArgs : EventArgs
    {
        private Connection connection;

        public ConnectionEventArgs(Connection connection)
        {
            this.connection = connection;
        }

        public Connection Connection
        {
            get { return connection; }
        }
    }
}
