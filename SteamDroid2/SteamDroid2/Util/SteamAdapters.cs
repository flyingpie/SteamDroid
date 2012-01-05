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

using SteamDroid2.Adapters;

namespace SteamDroid2.Util
{
	public class SteamAdapters
	{
		private static FriendsAdapter friendsAdapter;
		
		public static FriendsAdapter GetFriendsAdapter()
		{
			if(friendsAdapter == null)
			{
				Console.WriteLine ("Creating friends adapter");
				friendsAdapter = new FriendsAdapter(SteamAlerts.GetContext());
			}
			
			return friendsAdapter;
		}
	}
}

