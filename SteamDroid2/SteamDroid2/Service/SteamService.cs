using Android.App;
using Android.Content;
using Android.OS;
using SteamKit2;
using SteamDroid2.Api;
using SteamDroid2.App;
using SteamDroid2.Util;

namespace SteamDroid2
{
    [Service]
    public class SteamService : Service, ICallbackHandler
    {
        private static Steam client;
        
        private static Friend activeChat;
        
        public override IBinder OnBind (Intent intent)
        {
            return null;
        }
        
        public static Steam GetClient()
        {
            if(client == null)
            {
                client = new Steam();	
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
        
        public override void OnStart(Intent intent, int startId)
        {
            base.OnStart (intent, startId);
            
            SteamService.GetClient().AddHandler(this);
            
            SteamAdapters.GetFriendsAdapter();
        }
        
        public void HandleCallback(CallbackMsg msg)
        {
            if(msg.IsType<SteamUser.LoginKeyCallback>())
            {
                GetClient().Friends.SetPersonaState(EPersonaState.Online);
                
                Friend.Me = new Friend(GetClient().User.GetSteamID());
                Friend.Me.Name = "me";
            }
            else if(msg.IsType<SteamFriends.FriendMsgCallback>())
            {
                SteamFriends.FriendMsgCallback callback = (SteamFriends.FriendMsgCallback)msg;

                if (callback.EntryType == EChatEntryType.ChatMsg)
                {
                    Friend friend = Friend.GetFriendBySteamId(callback.Sender.ToString());

                    if(friend != activeChat)
                    {
                        Intent intent = new Intent(SteamAlerts.GetContext(), typeof(Chat));
                        intent.PutExtra("steam_id", friend.SteamId.ToString());

                        SteamAlerts.Notification("Message from " + friend.Name, friend.Name + ": " + callback.Message, callback.Message, intent, "steam_id", friend.SteamId.ToString());
                        SteamAlerts.PlaySound();
                        SteamAlerts.Vibrate(400);
                    }
                }
            }
            else if (msg.IsType<SteamClient.ConnectCallback>())
            {
                SteamAlerts.ShowToast("Connected to Steam");
            }
            else if (msg.IsType<SteamClient.DisconnectCallback>())
            {
                SteamAlerts.ShowToast("Disconnected from Steam");
            }
        }
    }
}

