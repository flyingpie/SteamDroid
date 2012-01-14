using System;
using SteamDroid.Adapters;

namespace SteamDroid.Util
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

