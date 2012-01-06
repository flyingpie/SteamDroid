using Android.App;
using Android.OS;
using Android.Preferences;

namespace SteamDroid2.App
{
	[Activity (Label = "Preferences")]			
	public class Preferences : PreferenceActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			AddPreferencesFromResource(Resource.Layout.Preferences);
		}
	}
}

