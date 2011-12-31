using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SteamDroidServer.Logging;
using SteamDroidServer.Networking;
using SteamDroidServer.Networking.EventArguments;
using SteamDroidServer.Client;
using SteamDroidServer.Steam;
using SteamKit2;

namespace SteamDroidServer
{
    /// <summary>
    /// SteamDroidServer handles incomming connections
    /// </summary>
    public class SteamDroidServer
    {
        private Listener listener;
        private List<ClientConnection> clients;
        
        public SteamDroidServer()
        {
            listener = new Listener();
            clients = new List<ClientConnection>();

            Users.Instance();
        }

        /// <summary>
        /// Starts the server listening on the specified ip and port
        /// </summary>
        /// <param name="ip">The ip to listen on</param>
        /// <param name="port">The port to listen on</param>
        public void Start(String ip, int port)
        {
            listener.ConnectionOpened += new Listener.ConnectionOpenedHandler(listener_ConnectionOpened);
            listener.Start(ip, port);
        }

        /// <summary>
        /// Removes a connection from the clients list
        /// </summary>
        /// <param name="connection">The connection to remove</param>
        public void RemoveConnection(ClientConnection connection)
        {
            clients.Remove(connection);
        }

        /// <summary>
        /// Handles new connections
        /// </summary>
        /// <param name="sender">The listener accepting the connection</param>
        /// <param name="eArgs">Arguments, containing the opened connection</param>
        private void listener_ConnectionOpened(object sender, ConnectionEventArgs eArgs)
        {
            Logger.Get().Log("Client connected: " + eArgs.Connection.RemoteEndPoint.Address + ":" + eArgs.Connection.RemoteEndPoint.Port);
            clients.Add(new ClientConnection(eArgs.Connection, this));
        }
    }
}
