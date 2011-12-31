using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SteamDroidServer.Networking.EventArguments;

namespace SteamDroidServer.Networking
{
    /// <summary>
    /// Listener provides functionality for hosting a server at a specific port and accepting connections from it
    /// </summary>
    public class Listener
    {
        public delegate void ConnectionOpenedHandler(object sender, ConnectionEventArgs eArgs);

        public event ConnectionOpenedHandler ConnectionOpened;

        private Thread listenThread;

        private Socket socket;
        private IPEndPoint endPoint;

        private List<Connection> connections;

        public Listener()
        {
            connections = new List<Connection>();
        }

        /// <summary>
        /// Start listening at the specified port
        /// </summary>
        /// <param name="port">The port to listen on</param>
        public void Start(String ip, int port)
        {
            // Try to parse the specified ip address, else use the default address, any
            IPAddress ipAddress;
            if (!IPAddress.TryParse(ip, out ipAddress))
            {
                Logging.Logger.Get().Log("Invalid ip address specified, using default address");
                ipAddress = IPAddress.Any;
            }

            endPoint = new IPEndPoint(ipAddress, port);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(endPoint);
            socket.Listen(5);

            listenThread = new Thread(new ThreadStart(ListenThread));
            listenThread.Start();
            Logging.Logger.Get().Log("Started server at " + ipAddress.ToString() + ":" + port);
        }

        /// <summary>
        /// ListenThread method, runs in a seperate thread and listens for new connections
        /// </summary>
        private void ListenThread()
        {
            while (true)
            {
                Socket clientSocket = socket.Accept();
                Connection connection = new Connection(clientSocket);

                if (ConnectionOpened != null)
                {
                    ConnectionOpened(this, new ConnectionEventArgs(connection));
                }
            }
        }
    }
}
