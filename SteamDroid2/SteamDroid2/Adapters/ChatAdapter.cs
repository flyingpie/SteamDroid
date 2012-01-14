using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;

using SteamKit2;
using SteamDroid.Api;

namespace SteamDroid
{
    public class ChatAdapter : BaseAdapter, ICallbackHandler
    {
        private Context context;
        private List<ChatMessage> messages;
        private LayoutInflater inflater;
        
        private Friend friend;
        
        public ChatAdapter(Friend friend, Context context)
        {
            this.context = context;
            messages = new List<ChatMessage>();
            inflater = LayoutInflater.FromContext(context);
            this.friend = friend;
            
            SteamService.GetClient().AddHandler(this);
        }
        
        public override int Count
        {
            get { return messages.Count; }
        }
        
        public void Add(ChatMessage message)
        {
            messages.Add(message);
        }
        
        public ChatMessage GetMessageAt(int position)
        {
            return messages.ElementAt(position);
        }
        
        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }
        
        public override long GetItemId(int position)
        {
            return position;
        }
        
        public override View GetView(int position, View view, ViewGroup parent)
        {
            ViewHolder holder;
            
            if(view == null)
            {
                view = inflater.Inflate(Resource.Layout.AdapterChat, null);
                holder = new ViewHolder();
                holder.TextName = view.FindViewById<TextView>(Resource.Id.ChatName);
                holder.TextMessage = view.FindViewById<TextView>(Resource.Id.ChatMessage);
                
                view.Tag = holder;
            }
            
            holder = (ViewHolder)view.Tag;
            
            ChatMessage message = GetMessageAt(position);
            
            holder.TextName.Text = message.Friend.Name;
            holder.TextMessage.Text = message.Message;
            
            int color = Resource.Color.StateOnline;
            
            if(message.Friend.State == EPersonaState.Offline)
            {
                color = Resource.Color.StateOffline;
            }
            else if(message.Friend.GameName.Length > 0)
            {
                color = Resource.Color.StateInGame;
            }
            
            color = context.Resources.GetColor (color);
            
            holder.TextName.SetTextColor(color);
            
            return view;
        }
        
        public void HandleCallback(CallbackMsg msg)
        {
            if(msg.IsType<SteamFriends.FriendMsgCallback>())
            {
                SteamFriends.FriendMsgCallback callback = (SteamFriends.FriendMsgCallback)msg;
                
                if(callback.EntryType == EChatEntryType.ChatMsg && callback.Sender == friend.SteamId)
                {
                    ChatMessage message = new ChatMessage(friend, callback.Message);
                    Add(message);
                    NotifyDataSetChanged();
                }
            }
        }
        
        class ViewHolder : Java.Lang.Object
        {
            public TextView TextName { get; set; }
            public TextView TextMessage { get; set; }
        }
    }
}

