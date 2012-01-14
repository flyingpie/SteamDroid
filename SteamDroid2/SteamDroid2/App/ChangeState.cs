using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

using SteamKit2;

namespace SteamDroid.App
{
	[Activity (Label = "ChangeState")]			
	public class ChangeState : ListActivity
	{
		private ArrayAdapter<EPersonaState> stateAdapter;
		
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			
			stateAdapter = new ArrayAdapter<EPersonaState>(this, Android.Resource.Layout.SimpleListItem1);
			stateAdapter.Add(EPersonaState.Online);
			stateAdapter.Add(EPersonaState.Away);
			stateAdapter.Add(EPersonaState.Busy);
			stateAdapter.Add(EPersonaState.Snooze);
			stateAdapter.Add(EPersonaState.Offline);
			
			ListAdapter = stateAdapter;
		}
		
		protected override void OnListItemClick(ListView l, View v, int position, long id)
		{
			base.OnListItemClick(l, v, position, id);
			
			EPersonaState state = stateAdapter.GetItem(position);
			
			SteamService.GetClient().Friends.SetPersonaState(state);
		}
	}
}

