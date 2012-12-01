using System;
using System.Collections.Generic;

using Android.OS;

using SteamKit2;
using SteamDroid.Util;

namespace SteamDroid.Api
{
    public class Steam
    {
        private SteamClient client;
        private SteamFriends friends;
        private SteamUser user;

        private List<ICallbackHandler> callbackHandlers;

        private String username;
        private String password;
        private String authcode;

        private bool loggedIn;

        private int retry;

        private TimeoutHandler timeout;

        public Steam()
        {
            callbackHandlers = new List<ICallbackHandler>();

            Initialize();

            new SteamCallback().Execute();
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
        /// Connects using the previous username and password
        /// </summary>
        public void Connect()
        {
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
        /// Returns how many retries have been attempted
        /// </summary>
        /// <returns></returns>
        public int GetRetryCount()
        {
            return retry;
        }

        /// <summary>
        /// Processes the callbacks received from the callback thread
        /// </summary>
        /// <param name='msg'>
        /// Message.
        /// </param>
        public void ProcessCallback(CallbackMsg msg)
        {
            if (msg.IsType<SteamClient.ConnectCallback>())
            {
                user.LogOn(new SteamUser.LogOnDetails()
                {
                    Username = username,
                    Password = password,
                    AuthCode = authcode
                });
                /*
                timeout = TimeoutHandler.Start(10000, () =>
                {
                    Disconnect();

                    if (retry < 2)
                    {
                        Connect();

                        retry++;
                    }
                    else
                    {
                        retry = 0;
                    }

                    return null;
                });*/
            }

            if (msg.IsType<SteamClient.DisconnectCallback>() || msg.IsType<SteamUser.LoggedOffCallback>())
            {
                loggedIn = false;
            }

            if (msg.IsType<SteamUser.LogOnCallback>())
            {
                SteamUser.LogOnCallback callback = (SteamUser.LogOnCallback)msg;

                if (callback.Result == EResult.OK)
                {
                    loggedIn = true;
                    authcode = null;
                    retry = 0;
                    /*
                    if (timeout != null)
                    {
                        timeout.Cancel(true);
                    }*/
                    /*
                    Friends.SetPersonaState(EPersonaState.Online);

                    Friend.Me = new Friend(User.GetSteamID());
                    Friend.Me.Name = "me";

                    SteamAlerts.ShowToast("Connected to Steam");
                    SteamAlerts.Notification("SteamDroid", "Connected to Steam", "Connected to Steam", new Android.Content.Intent(SteamAlerts.GetContext(), typeof(App.Main)), null, null);
                     */
                }
            }

            if (msg.IsType<SteamUser.LoginKeyCallback>())
            {
                Friends.SetPersonaState(EPersonaState.Online);

                Friend.Me = new Friend(User.GetSteamID());
                Friend.Me.Name = "me";

                SteamAlerts.ShowToast("Connected to Steam");
                SteamAlerts.Notification("SteamDroid", "Connected to Steam", "Connected to Steam", new Android.Content.Intent(SteamAlerts.GetContext(), typeof(App.Main)), null, null);
            }

            Push(msg);
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
                retry = 0;
            }
        }

        /// <summary>
        /// Pushes a callback to registered handlers
        /// </summary>
        /// <param name="msg">The callback to push</param>
        private void Push(CallbackMsg msg)
        {
            for(int i = 0; i < callbackHandlers.Count; i++)
            {
                callbackHandlers[i].HandleCallback(msg);
            }
        }
        
        class SteamCallback : AsyncTask<Int32, Int32, CallbackMsg>
        {
            protected override void OnPreExecute()
            {
                base.OnPreExecute();
            }
            
            protected override CallbackMsg RunInBackground(params Int32[] parameters)
            {
                CallbackMsg msg = SteamService.GetClient().Client.WaitForCallback(true);

                return msg;
            }
            
            protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] native_parms)
            {
                return base.DoInBackground(native_parms);
            }
            
            protected override void OnPostExecute(CallbackMsg result)
            {
                base.OnPostExecute(result);
                
                SteamClient client = SteamService.GetClient().Client;
                
                if(result != null)
                {
                    client.FreeLastCallback();

                    SteamService.GetClient().ProcessCallback(result);
                }
                
                new SteamCallback().Execute();
            }
        }
    }
}