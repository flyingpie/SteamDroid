using System;
using Android.Graphics;
using Android.OS;
using SteamKit2;
using SteamDroid2.Adapters;
using SteamDroid2.Util;

namespace SteamDroid2.Api
{
    /// <summary>
    /// Friend class contains information on a single friend and is used for list adapters and sending messages
    /// </summary>
    public class Friend : ICallbackHandler, IComparable<Friend>
    {
        public static Friend Me;
        public static String Unknown = "[Unknown]";

        private SteamID steamId;
        private ChatAdapter adapter;

        private String name;
        private String avatar;

        private Bitmap avatarBitmap;

        private EventHandler dataChangedHandler;
        
        public Friend(SteamID steamId)
        {
            this.steamId = steamId;
            this.name = Unknown;
            this.avatar = Unknown;
            
            adapter = new ChatAdapter(this, SteamAlerts.GetContext());
        
            SteamService.GetClient().AddHandler(this);
        }

        /// <summary>
        /// Gets the avatar.
        /// </summary>
        /// <value>
        /// The avatar.
        /// </value>
        public String Avatar
        {
            get { return avatar; }
        }

        /// <summary>
        /// Gets the avatar as bitmap
        /// </summary>
        public Bitmap AvatarBitmap
        {
            get { return avatarBitmap; }
            set { avatarBitmap = value; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public virtual String Name
        {
            get { return name; }
            set { name = value; }
        }
        
        /// <summary>
        /// Gets the steam identifier.
        /// </summary>
        /// <value>
        /// The steam identifier.
        /// </value>
        public virtual SteamID SteamId
        {
            get { return steamId; }	
        }
        
        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public virtual EPersonaState State
        {
            get { return SteamService.GetClient().Friends.GetFriendPersonaState(steamId); }
        }
        
        /// <summary>
        /// Gets the name of the game.
        /// </summary>
        /// <value>
        /// The name of the game.
        /// </value>
        public virtual String GameName
        {
            get { return SteamService.GetClient().Friends.GetFriendGamePlayedName(steamId); }
        }
        
        /// <summary>
        /// Gets the chat adapter.
        /// </summary>
        /// <value>
        /// The adapter.
        /// </value>
        public virtual ChatAdapter Adapter
        {
            get { return adapter; }
        }

        /// <summary>
        /// Gets or sets the data changed handler of this friend
        /// </summary>
        public virtual EventHandler DataChangedHandler
        {
            get { return dataChangedHandler; }
            set { dataChangedHandler = value; }
        }

        /// <summary>
        /// Downloads the avatar
        /// </summary>
        public void DownloadAvatar()
        {
            SteamFiles.LoadFromWeb("avatar_" + SteamId.ToString(), Avatar, (x) => AvatarBitmap = x);
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name='message'>
        /// Message.
        /// </param>
        public virtual void SendMessage(String message)
        {
            ChatMessage chatMessage = new ChatMessage(Friend.Me, message);
            adapter.Add(chatMessage);
            
            SteamService.GetClient().Friends.SendChatMessage(steamId, EChatEntryType.ChatMsg, message);
            
            adapter.NotifyDataSetChanged();
        }
        
        /// <summary>
        /// Handles callbacks from steam.
        /// </summary>
        /// <param name='msg'>
        /// Message.
        /// </param>
        public void HandleCallback(SteamKit2.CallbackMsg msg)
        {
            if(msg.IsType<SteamFriends.ChatMsgCallback>())
            {
                SteamFriends.ChatMsgCallback callback = (SteamFriends.ChatMsgCallback)msg;
                
                if(callback.ChatMsgType == EChatEntryType.ChatMsg)
                {
                    ChatMessage message = new ChatMessage(this, callback.Message);
                    adapter.Add(message);
                }
            }
        }

        /// <summary>
        /// Updates friend attributes
        /// </summary>
        public void Update()
        {
            String name = SteamService.GetClient().Friends.GetFriendPersonaName(steamId);

            if (name.Length > 0)
            {
                this.name = name;
            }
            
            byte[] hash = SteamService.GetClient().Friends.GetFriendAvatar(steamId);
            
            if(hash != null)
            {
                String avatar = BitConverter.ToString(hash).Replace("-", "").ToLower();
                if (!avatar.StartsWith("000000"))
                {
                    avatar = avatar.Substring(0, 2) + "/" + avatar;
                    avatar = "http://media.steampowered.com/steamcommunity/public/images/avatars/" + avatar + "_medium.jpg";
                    this.avatar = avatar;
                }
            }
        }
        
        public static Friend GetFriendBySteamId(String steamId)
        {
            FriendsAdapter adapter = SteamAdapters.GetFriendsAdapter();
            
            for(int i = 0; i < adapter.Count; i++)
            {
                if(adapter.GetFriendAt(i).SteamId.ToString() == steamId)
                {
                    return adapter.GetFriendAt(i);
                }
            }
            
            throw new Exception("Error retrieving friend by Steam ID: " + steamId);
        }

        public int CompareTo(Friend other)
        {
            bool meInGame = GameName != null && GameName.Length > 0;
            bool otherInGame = other.GameName != null && other.GameName.Length > 0;

            if (meInGame && !otherInGame)
            {
                return -1;
            }
            else if (!meInGame && otherInGame)
            {
                return 1;
            }
            else if (State == EPersonaState.Offline && other.State != EPersonaState.Offline)
            {
                return 1;
            }
            else if (State != EPersonaState.Offline && other.State == EPersonaState.Offline)
            {
                return -1;
            }
            else
            {
                return Name.CompareTo(other.Name);
            }
        }

        class AvatarDownloader : AsyncTask<Friend, int, int>
        {
            protected override int RunInBackground(params Friend[] friend)
            {
                try
                {
                    Bitmap bmp = BitmapFactory.DecodeStream(new Java.Net.URL(friend[0].Avatar).OpenStream());

                    friend[0].AvatarBitmap = bmp;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error downloading avatar: " + e.Message);
                }
                return 0;
            }
        }
    }
}

