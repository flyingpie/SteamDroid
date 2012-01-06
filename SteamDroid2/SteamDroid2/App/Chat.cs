using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SteamKit2;
using SteamDroid2.Api;

namespace SteamDroid2.App
{
	[Activity (Label = "Chat")]			
	public class Chat : Activity
	{
		private Friend friend;
		
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView (Resource.Layout.Chat);
			
			String steamId = Intent.GetStringExtra("steam_id");
			friend = Friend.GetFriendBySteamId(steamId);
			
			ListView listOutput = FindViewById<ListView>(Resource.Id.ListOutput);
			listOutput.TranscriptMode = TranscriptMode.AlwaysScroll;
			listOutput.Adapter = friend.Adapter;
			
			Button buttonSend = FindViewById<Button>(Resource.Id.ButtonSend);
			buttonSend.Click += ClickSend;

            UpdateFriend();

			SteamService.SetActiveChat(friend);
		}
		
		protected override void OnStop()
		{
			base.OnStop();
			
			SteamService.ClearActiveChat();
		}

        protected void UpdateFriend()
        {
            ImageView imageAvatar = FindViewById<ImageView>(Resource.Id.ImageAvatar);
            
            int color = Resource.Color.StateOnline;

            if (friend.State == EPersonaState.Offline)
            {
                color = Resource.Color.StateOffline;
            }
            else if (friend.GameName.Length > 0)
            {
                color = Resource.Color.StateInGame;
            }

            color = Resources.GetColor(color);

            imageAvatar.SetBackgroundColor(color);

            if (friend.AvatarBitmap != null)
            {
                imageAvatar.SetImageBitmap(friend.AvatarBitmap);
            }
            else
            {
                imageAvatar.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.DefaultAvatar));
            }

            TextView textName = FindViewById<TextView>(Resource.Id.FriendName);
            textName.Text = friend.Name;
            textName.SetTextColor(color);

            TextView textState = FindViewById<TextView>(Resource.Id.FriendState);
            textState.Text = friend.State.ToString();
            textState.SetTextColor(color);

            TextView textGame = FindViewById<TextView>(Resource.Id.FriendGame);
            textGame.Text = friend.GameName;
            textGame.SetTextColor(color);
        }

		private void ClickSend(object sender, EventArgs e)
		{
			EditText textInput = FindViewById<EditText>(Resource.Id.TextInput);
			
			String message = textInput.Text;
			
			if(message.Length > 0)
			{
				friend.SendMessage(message);
			}
			
			textInput.Text = String.Empty;
		}
	}
}

