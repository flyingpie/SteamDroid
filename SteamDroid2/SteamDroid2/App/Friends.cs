using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SteamDroid2.Adapters;
using SteamDroid2.Api;
using SteamDroid2.Util;

namespace SteamDroid2.App
{
	[Activity (Label = "Friends")]			
	public class Friends : ListActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			ListAdapter = SteamAdapters.GetFriendsAdapter();
			ListView.SetBackgroundColor(Resources.GetColor(Resource.Color.ListBackground));
		}
		
		protected override void OnListItemClick (ListView l, View v, int position, long id)
		{
			base.OnListItemClick (l, v, position, id);
			
			FriendsAdapter adapter = SteamAdapters.GetFriendsAdapter();
			Friend friend = adapter.GetFriendAt(position);

            if (friend.State != SteamKit2.EPersonaState.Offline)
            {
                Intent chatIntent = new Intent(this, typeof(Chat));
                chatIntent.PutExtra("steam_id", friend.SteamId.ToString());
                StartActivity(chatIntent);
            }
            else
            {
                SteamAlerts.ShowToast(friend.Name + " is offline");
            }
		}
	}
}

