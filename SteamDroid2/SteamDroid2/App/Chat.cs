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
			
			SteamService.SetActiveChat(friend);
		}
		
		protected override void OnStop()
		{
			base.OnStop();
			
			SteamService.ClearActiveChat();
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

