using Android.App;
using Android.Content;
using Android.OS;
using SteamKit2;
using SteamDroid.Api;
using SteamDroid.App;
using SteamDroid.Util;
using System;
using Android.Preferences;

namespace SteamDroid
{
    [Service]
    public class SteamService : Service, ICallbackHandler
    {
        private static Steam client;
        
        private static Friend activeChat;

        private static bool isRunning;

        private static bool autoReconnect;

        public override IBinder OnBind (Intent intent)
        {
            return null;
        }

        public static bool IsRunning()
        {
            return isRunning;
        }

        public static Steam GetClient()
        {
            if(client == null)
            {
                client = new Steam();
                autoReconnect = true;
            }
            
            return client;
        }

        public static void ClearActiveChat()
        {
            SteamService.activeChat = null;
        }
        
        public static void SetActiveChat(Friend friend)
        {
            SteamService.activeChat = friend;
        }

        public static void EnableAutoReconnect()
        {
            SteamService.autoReconnect = true;
        }

        public static void DisableAutoReconnect()
        {
            SteamService.autoReconnect = false;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            isRunning = true;
        }

        public override void OnStart(Intent intent, int startId)
        {
            base.OnStart (intent, startId);
            
            SteamService.GetClient().AddHandler(this);
            
            SteamAdapters.GetFriendsAdapter();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            isRunning = false;
        }

        public void HandleCallback(CallbackMsg msg)
        {
            if(msg.IsType<SteamUser.LoginKeyCallback>())
            {/*
                GetClient().Friends.SetPersonaState(EPersonaState.Online);
                
                Friend.Me = new Friend(GetClient().User.GetSteamID());
                Friend.Me.Name = "me";

                SteamAlerts.ShowToast("Connected to Steam");*/
            }
            else if (msg.IsType<SteamUser.LogOnCallback>())
            {
                SteamUser.LogOnCallback callback = (SteamUser.LogOnCallback)msg;

                if (callback.Result == EResult.OK)
                {
                    EnableAutoReconnect();
                }
                else
                {
                    DisableAutoReconnect();
                }
            }
            else if (msg.IsType<SteamFriends.FriendMsgCallback>())
            {
                SteamFriends.FriendMsgCallback callback = (SteamFriends.FriendMsgCallback)msg;

                if (callback.EntryType == EChatEntryType.ChatMsg)
                {
                    Friend friend = Friend.GetFriendBySteamId(callback.Sender.ToString());

                    if (friend != activeChat)
                    {
                        Intent intent = new Intent(SteamAlerts.GetContext(), typeof(Chat));
                        intent.SetAction("chat_notification_" + DateTime.Now.Ticks);
                        intent.PutExtra("steam_id", friend.SteamId.ToString());

                        SteamAlerts.Notification("Message from " + friend.Name, friend.Name + ": " + callback.Message, callback.Message, intent, "steam_id", friend.SteamId.ToString());
                        SteamAlerts.PlaySound();
                        SteamAlerts.Vibrate(400);
                    }
                }
            }
            else if (msg.IsType<SteamClient.ConnectCallback>())
            {
                SteamAlerts.Notification("Steam Droid", "Connected to Steam", "Connected to Steam", new Intent(SteamAlerts.GetContext(), typeof(Main)), null, null);
            }
            else if (msg.IsType<SteamClient.DisconnectCallback>())
            {
                SteamAlerts.Notification("Steam Droid", "Disconnected from Steam", "Connected to Steam", new Intent(SteamAlerts.GetContext(), typeof(Main)), null, null);
                SteamAlerts.ShowToast("Disconnected from Steam");

                ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(this);
                bool autoReconnect = SteamService.autoReconnect && pref.GetBoolean("prefEnableReconnect", false);

                if (autoReconnect)
                {
                    String username = pref.GetString("prefUsername", "");
                    String password = pref.GetString("prefPassword", "");

                    if (username.Length > 0 && password.Length > 0)
                    {
                        SteamService.GetClient().Connect(username, password);
                        SteamAlerts.ShowProgressDialog("Connecting", "Connecting to the Steam servers...", this);
                    }
                }
            }
        }
    }
}

