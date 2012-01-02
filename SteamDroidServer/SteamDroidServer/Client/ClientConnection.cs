using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SteamDroidServer.Networking;
using SteamDroidServer.Networking.EventArguments;
using SteamKit2;
using SteamDroidServer.Steam;
using SteamDroidServer.Logging;

namespace SteamDroidServer.Client
{
    public class ClientConnection : ICallbackHandler
    {
        private Connection connection;
        private SteamDroidServer server;

        private bool authorized;
        private Steam3 steam;

        public ClientConnection(Connection connection, SteamDroidServer server)
        {
            this.connection = connection;
            connection.MessageReceived += new Connection.MessageReceivedHandler(connection_MessageReceived);
            connection.ConnectionClosed += new Connection.ConnectionClosedHandler(connection_ConnectionClosed);

            this.server = server;

            this.authorized = false;

            connection.StartListener();
        }

        public bool IsAuthorized
        {
            get { return authorized; }
            set { authorized = value; }
        }

        public void HandleCallback(CallbackMsg msg)
        {
            if (msg.IsType<SteamClient.ConnectCallback>())
            {
                Logger.Get().Log("[Callback] Connected");
            }
            else if (msg.IsType<SteamClient.DisconnectCallback>())
            {
                Logger.Get().Log("[Callback] Disconnected");
                IsAuthorized = false;
                Send(Protocol.Server.LoggedOut);
            }
            else if (msg.IsType<SteamUser.LogOnCallback>())
            {
                SteamUser.LogOnCallback callback = (SteamUser.LogOnCallback)msg;

                Logger.Get().Log("[Callback] Logon: " + callback.Result + "/" + callback.ExtendedResult);

                if (callback.Result == EResult.AccountLogonDenied || callback.Result == EResult.InvalidLoginAuthCode)
                {
                    Send(Protocol.Server.AuthRequest);
                    IsAuthorized = true;
                }

                if (callback.Result == EResult.OK)
                {
                    IsAuthorized = true;
                }
            }
            else if (msg.IsType<SteamUser.LoginKeyCallback>())
            {
                Logger.Get().Log("[Callback] Logged in, setting persona state to online");

                steam.Friends.SetPersonaState(SteamKit2.EPersonaState.Online);

                Send(Protocol.Server.LoggedIn);
            }
            else if (msg.IsType<SteamFriends.FriendMsgCallback>())
            {
                SteamFriends.FriendMsgCallback chat = (SteamFriends.FriendMsgCallback)msg;
                if (chat.EntryType == EChatEntryType.ChatMsg)
                {
                    Send(Protocol.Server.ChatReceived, chat.Sender.ToString(), chat.Message);
                }
            }
            else if (msg.IsType<SteamFriends.PersonaStateCallback>())
            {
                SteamFriends.PersonaStateCallback friend = (SteamFriends.PersonaStateCallback)msg;
                Send(Protocol.Server.FriendStateChanged, friend.FriendID.ToString(), friend.State.ToString(), friend.GameName);
            }
        }

        public void Send(params String[] message)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < message.Length; i++)
            {
                if (result.Length > 0) result.Append(Protocol.SplitString);
                result.Append(message[i]);
            }

            connection.Send(result.ToString().Trim());
        }

        private void Authorize(String[] message)
        {
            if (message.Length >= 2 && message[0].Length > 0)
            {
                if (Users.Instance().IsAllowed(message[0]))
                {
                    String authCode = (message.Length == 3) ? message[2] : null;
                    ConnectToSteam(message[0], message[1], authCode);
                    return;
                }

                Logger.Get().Log("User not in allowed list: " + message[0]);
            }
            else
            {
                Logger.Get().Log("Invalid command");
            }

            Send(Protocol.Server.NotAllowed);
            connection.Disconnect();
        }

        private void ConnectToSteam(String username, String password, String authCode)
        {
            Logger.Get().Log("Retrieving Steam instance...");

            Steam3 tmpSteam;

            if (SteamInstances.HasInstance(username))
            {
                Logger.Get().Log("Loaded existing instance");
            }
            else
            {
                Logger.Get().Log("Creating new instance");
            }

            tmpSteam = SteamInstances.GetInstance(username);

            tmpSteam.AddHandler(this);

            if (tmpSteam.LoggedIn)
            {
                Logger.Get().Log("Instance already online, checking credentials...");

                if (tmpSteam.Authorize(username, password))
                {
                    Logger.Get().Log("User logged in");
                    this.steam = tmpSteam;
                    IsAuthorized = true;
                    Send(Protocol.Server.LoggedIn);
                    return;
                }
                else
                {
                    Logger.Get().Log("Invalid credentials");
                    connection.Disconnect();
                    return;
                }
            }

            Logger.Get().Log("Connecting to Steam");
            this.steam = tmpSteam;
            steam.Connect(username, password, authCode);

            Logger.Get().Log("Starting callback thread");
        }

        private void connection_ConnectionClosed(object sender, EventArgs eArgs)
        {
            server.RemoveConnection(this);
            if (steam != null)
            {
                steam.RemoveHandler(this);
            }
        }

        private void connection_MessageReceived(object sender, MessageReceivedEventArgs eArgs)
        {
            String[] split = eArgs.Message.Split(new char[] { ' ' }, 3);

            if (!authorized)
            {
                Authorize(split);
            }
            else
            {
                String[] parameters = new String[split.Length - 1];
                for(int i = 1; i < split.Length; i++)
                {
                    parameters[i - 1] = split[i];
                }

                ClientCommandParser.ParseCommand(new Command(this, steam, split[0], parameters));
            }
        }
    }
}
