using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Preferences;
using Android.Views;
using Android.Widget;

using SteamKit2;
using SteamDroid2.Api;

namespace SteamDroid2.Adapters
{
    public class FriendsAdapter : BaseAdapter, ICallbackHandler
    {
        private Context context;
        private List<Friend> friends;
        private LayoutInflater inflater;
        
        public FriendsAdapter(Context context)
        {
            this.context = context;
            friends = new List<Friend>();
            inflater = LayoutInflater.FromContext(context);
            
            SteamService.GetClient().AddHandler(this);
        }
        
        public override int Count
        {
            get { return friends.Count; }
        }
        
        public void Add(Friend friend)
        {
            friends.Add (friend);
        }
        
        public void Clear()
        {
            friends.Clear();
        }
        
        public Friend GetFriendAt(int position)
        {
            return friends.ElementAt(position);
        }
        
        public Friend GetFriendBySteamId(SteamID steamId)
        {
            for(int i = 0; i < friends.Count; i++)
            {
                if(friends.ElementAt(i).SteamId == steamId)
                {
                    return friends.ElementAt(i);
                }
            }
            
            return null;
        }
        
        public override Java.Lang.Object GetItem (int position)
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
                view = inflater.Inflate(Resource.Layout.AdapterFriends, null);
                holder = new ViewHolder();
                holder.TextName = view.FindViewById<TextView>(Resource.Id.FriendName);
                holder.TextState = view.FindViewById<TextView>(Resource.Id.FriendState);
                holder.TextGame = view.FindViewById<TextView>(Resource.Id.FriendGame);
                holder.ImageAvatar = view.FindViewById<ImageView>(Resource.Id.ImageAvatar);
                view.Tag = holder;
            }
            else
            {
                holder = (ViewHolder)view.Tag;
            }
            
            Friend friend = friends.ElementAt(position);
            
            String name = friend.Name;
            String state = friend.State.ToString();
            
            holder.TextName.Text = friend.Name;
            holder.TextState.Text = friend.State.ToString();
            holder.TextGame.Text = friend.GameName;
            
            int color = Resource.Color.StateOnline;
            
            if(friend.State == EPersonaState.Offline)
            {
                color = Resource.Color.StateOffline;
            }
            else if(friend.GameName.Length > 0)
            {
                color = Resource.Color.StateInGame;
            }
            
            color = context.Resources.GetColor (color);
            
            holder.TextName.SetTextColor(color);
            holder.TextState.SetTextColor(color);
            holder.TextGame.SetTextColor(color);

            ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(context);

            if (pref.GetBoolean("prefEnableAvatars", false))
            {
                String url = friend.Avatar;
                
                holder.ImageAvatar.SetBackgroundColor(color);

                SetAvatarImage(holder.ImageAvatar, friend);

                friend.DataChangedHandler = delegate(object sender, EventArgs e)
                {
                    SetAvatarImage(holder.ImageAvatar, friend);
                };
            }
            
            return view;
        }

        private void SetAvatarImage(ImageView view, Friend friend)
        {
            if (friend.AvatarBitmap != null)
            {
                view.SetImageBitmap(friend.AvatarBitmap);
            }
            else
            {
                view.SetImageDrawable(context.Resources.GetDrawable(Resource.Drawable.DefaultAvatar));
                friend.DownloadAvatar();
            }

            view.Invalidate();
        }
        
        public void HandleCallback(CallbackMsg msg)
        {
            if(msg.IsType<SteamFriends.PersonaStateCallback>())
            {
                SteamFriends.PersonaStateCallback callback = (SteamFriends.PersonaStateCallback)msg;
                LoadFriendsList();
            }
        }
        
        private void LoadFriendsList()
        {
            SteamFriends steamFriends = SteamService.GetClient().Friends;
            
            int count = steamFriends.GetFriendCount();
            
            for(int i = 0; i < count; i++)
            {
                SteamID steamId = steamFriends.GetFriendByIndex(i);
                Friend friend = GetFriendBySteamId(steamId);

                if (friend == null)
                {
                    friend = new Friend(steamId);
                    Add(friend);
                }

                friend.Update();
            }

            friends.Sort();
            
            NotifyDataSetChanged();
        }
        
        class ViewHolder : Java.Lang.Object
        {
            public TextView TextName { get; set; }
            public TextView TextState { get; set; }
            public TextView TextGame { get; set; }
            public ImageView ImageAvatar { get; set; }
        }
    }
}

