using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SteamDroidServer.Networking;
using SteamDroidServer.Steam;
using SteamKit2;

namespace SteamDroidServer.Client
{
    public class Command
    {
        private ClientConnection connection;
        private Steam3 steam;

        private String command;
        private String[] parameters;

        public Command(ClientConnection connection, Steam3 steam, String command, String[] parameters)
        {
            this.connection = connection;
            this.steam = steam;

            this.command = command.ToLower();
            this.parameters = parameters;
        }

        public ClientConnection Connection
        {
            get { return connection; }
        }

        public Steam3 Steam
        {
            get { return steam; }
        }

        public String ClientCommand
        {
            get { return command; }
        }

        public String[] Parameters
        {
            get { return parameters; }
        }
    }
}
