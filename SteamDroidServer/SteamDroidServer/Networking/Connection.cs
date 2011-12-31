using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SteamDroidServer.Logging;
using SteamDroidServer.Networking.EventArguments;

namespace SteamDroidServer.Networking
{
    /// <summary>
    /// Connection connects to a remote host and handles the sending and receiving of data
    /// </summary>
    public class Connection
    {
        public delegate void ConnectionOpenedHandler(object sender, EventArgs eArgs);
        public delegate void ConnectionClosedHandler(object sender, EventArgs eArgs);
        public delegate void MessageReceivedHandler(object sender, MessageReceivedEventArgs eArgs);

        public event ConnectionOpenedHandler ConnectionOpened;
        public event ConnectionClosedHandler ConnectionClosed;
        public event MessageReceivedHandler MessageReceived;

        private TcpClient client;
        private NetworkStream stream;
        private StreamReader reader;
        private StreamWriter writer;

        private Socket socket;

        private Thread connectionThread;

        private bool isConnected;
        private bool listenAfterConnect;

        public Connection(Socket socket) : this()
        {
            this.socket = socket;
            isConnected = true;

            stream = new NetworkStream(socket);
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
        }

        public Connection()
        {
            connectionThread = new Thread(new ThreadStart(Listen));
            listenAfterConnect = true;
        }

        /// <summary>
        /// Specifies whether the listener should start after a connection was made
        /// </summary>
        public bool ListenAfterConnect
        {
            get { return listenAfterConnect; }
            set { listenAfterConnect = value; }
        }

        /// <summary>
        /// Local end point of this connection
        /// </summary>
        public IPEndPoint LocalEndPoint
        {
            get { return (IPEndPoint)socket.LocalEndPoint; }
        }

        /// <summary>
        /// Remote end point of this connection
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get { return (IPEndPoint)socket.RemoteEndPoint; }
        }

        /// <summary>
        /// Connects to the specified ip and port
        /// </summary>
        /// <param name="ip">The ip to connect to</param>
        /// <param name="port">The port to use</param>
        /// <returns>Whether the connection was successful</returns>
        public bool Connect(String ip, int port)
        {
            if (isConnected) return false;

            try
            {
                client = new TcpClient(ip, port);
                stream = client.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);
                socket = client.Client;

                if(listenAfterConnect)
                    connectionThread.Start();

                if (ConnectionOpened != null)
                {
                    ConnectionOpened(this, EventArgs.Empty);
                }

                isConnected = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting: " + ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Disconnects from the host
        /// </summary>
        public void Disconnect()
        {
            if (reader != null) reader.Close();
            if (writer != null) writer.Close();
            if (stream != null) stream.Close();
            if (client != null) client.Close();
            if (socket != null) socket.Close();
        }

        /// <summary>
        /// Sends the specified message to the host
        /// </summary>
        /// <param name="data">The message to send</param>
        /// <returns>Whether the send was successful</returns>
        public bool Send(String data)
        {
            if (writer != null)
            {
                writer.WriteLine(data);
                writer.Flush();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sends the specified message to the host, parameterized
        /// </summary>
        /// <param name="data">The data to send</param>
        /// <returns>Whether the send was successful</returns>
        public bool Send(params String[] data)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                if (result.Length > 0) result.Append(" ");
                result.Append(data[i]);
            }

            return Send(result.ToString());
        }

        /// <summary>
        /// Starts the connection thread
        /// </summary>
        public void StartListener()
        {
            if (!connectionThread.IsAlive) connectionThread.Start();
        }

        /// <summary>
        /// Sends the specified message to the host and waits for a response
        /// </summary>
        /// <param name="data">The data to send</param>
        /// <returns>The received data</returns>
        public String SendSynchronous(String data)
        {
            Send(data);

            return reader.ReadLine();
        }

        /// <summary>
        /// Listens for incoming messages and fires and event on data
        /// </summary>
        private void Listen()
        {
            try
            {
                String data;
                while ((data = reader.ReadLine()) != null)
                {
                    if (MessageReceived != null)
                    {
                        MessageReceived(this, new MessageReceivedEventArgs(data));
                    }
                }
            }
            catch (Exception)
            {
                
            }
            finally
            {
                Disconnect();

                Logger.Get().Log("Connection closed");

                if (ConnectionClosed != null)
                {
                    ConnectionClosed(this, EventArgs.Empty);
                }
            }
        }
    }
}
