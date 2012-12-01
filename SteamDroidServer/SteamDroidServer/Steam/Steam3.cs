using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SteamKit2;
using System.Threading;

namespace SteamDroidServer.Steam
{
    /// <summary>
    /// Steam client, handles the connection with the Steam network
    /// </summary>
    public class Steam3
    {
        private SteamClient client;
        private SteamFriends friends;
        private SteamUser user;

        private List<ICallbackHandler> callbackHandlers;

        private String username;
        private String password;
        private String authcode;

        private bool loggedIn;

        public Steam3()
        {
            callbackHandlers = new List<ICallbackHandler>();

            Initialize();

            new Thread(new ThreadStart(SteamThread)).Start();
        }

        /// <summary>
        /// SteamClient, contains client related methods
        /// </summary>
        public SteamClient Client
        {
            get { return client; }
        }

        /// <summary>
        /// SteamFriends, contains friends related methods
        /// </summary>
        public SteamFriends Friends
        {
            get { return friends; }
        }

        /// <summary>
        /// SteamUser, contains user related methods
        /// </summary>
        public SteamUser User
        {
            get { return user; }
        }

        /// <summary>
        /// Returns whether the client is logged in
        /// </summary>
        public bool LoggedIn
        {
            get { return loggedIn; }
        }

        /// <summary>
        /// Authorizes against an open connection using the specified username and password
        /// </summary>
        /// <param name="username">The username to use</param>
        /// <param name="password">The password to use</param>
        /// <returns></returns>
        public bool Authorize(String username, String password)
        {
            return this.username == username && this.password == password;
        }

        /// <summary>
        /// Connects using the specified authcode
        /// </summary>
        /// <param name="authcode">The authcode to use</param>
        public void Connect(String authcode)
        {
            this.authcode = authcode;

            Connect(username, password);
        }

        /// <summary>
        /// Connects using the specified username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void Connect(String username, String password)
        {
            Connect(username, password, null);
        }

        /// <summary>
        /// Connects using the specified username, password and authcode
        /// </summary>
        /// <param name="username">The username to use</param>
        /// <param name="password">The password to use</param>
        /// <param name="authcode">The authcode to use</param>
        public void Connect(String username, String password, String authcode)
        {
            if (!loggedIn)
            {
                this.username = username;
                this.password = password;
                this.authcode = authcode;

                client.Connect();
            }
        }

        /// <summary>
        /// Disconnects from the Steam network
        /// </summary>
        public void Disconnect()
        {
            user.LogOff();
            client.Disconnect();

            SteamInstances.RemoveInstance(username);
        }

        /// <summary>
        /// Adds a handler
        /// </summary>
        /// <param name="handler">The handler to add</param>
        public void AddHandler(ICallbackHandler handler)
        {
            callbackHandlers.Add(handler);
        }

        /// <summary>
        /// Removes a handler
        /// </summary>
        /// <param name="handler">The handler to remove</param>
        public void RemoveHandler(ICallbackHandler handler)
        {
            callbackHandlers.Remove(handler);
        }

        /// <summary>
        /// Updates the Steam client and passes any pending callbacks
        /// </summary>
        public void Update()
        {
            while (true)
            {
                CallbackMsg msg = client.WaitForCallback(true);

                if (msg == null)
                {
                    return;
                }

                client.FreeLastCallback();

                if (msg.IsType<SteamClient.ConnectCallback>())
                {
                    user.LogOn(new SteamUser.LogOnDetails()
                    {
                        Username = username,
                        Password = password,
                        AuthCode = authcode
                    });
                }

                if (msg.IsType<SteamClient.DisconnectCallback>())
                {
                    loggedIn = false;
                }

                if (msg.IsType<SteamUser.LogOnCallback>())
                {
                    SteamUser.LogOnCallback callback = (SteamUser.LogOnCallback)msg;

                    if (callback.Result == EResult.OK)
                    {
                        this.loggedIn = true;
                        this.authcode = null;
                    }

                    Friends.SetPersonaState(EPersonaState.Online);
                }

                Push(msg);
            }
        }

        /// <summary>
        /// Initializes the Steam client
        /// </summary>
        private void Initialize()
        {
            if (client == null)
            {
                client = new SteamClient();
                user = client.GetHandler<SteamUser>();
                friends = client.GetHandler<SteamFriends>();
                loggedIn = false;
            }
        }

        /// <summary>
        /// Pushes a callback to registered handlers
        /// </summary>
        /// <param name="msg">The callback to push</param>
        private void Push(CallbackMsg msg)
        {
            foreach (ICallbackHandler handler in callbackHandlers)
            {
                handler.HandleCallback(msg);
            }
        }

        /// <summary>
        /// Updates the Steam client while connected
        /// </summary>
        private void SteamThread()
        {
            while (true)
            {
                Update();
            }
        }
    }
}
