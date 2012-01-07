using System;
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

