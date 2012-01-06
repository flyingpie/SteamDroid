using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Widget;

namespace SteamDroid2.Util
{
	public class SteamAlerts
	{
		//private static MediaPlayer mediaPlayer;
		private static NotificationManager notificationManager;
		private static Vibrator vibrator;
		
		private static Context context;
		
		private static ProgressDialog dialog;
		private static AlertDialog.Builder alert;
		
		private static bool isAlertsEnabled;
		
		public static void Initialize(Activity activity)
		{
			//SteamAlerts.mediaPlayer = MediaPlayer.Create(context, Resource.Raw);
			SteamAlerts.notificationManager = (NotificationManager)activity.GetSystemService(Context.NotificationService);
			SteamAlerts.vibrator = (Vibrator)activity.GetSystemService(Context.VibratorService);
			
			SteamAlerts.context = activity.ApplicationContext;
			
			SteamAlerts.isAlertsEnabled = true;
		}
		
		public static Context GetContext()
		{
			return context;
		}
		
		public static void ShowAlertDialog(String title, String message, Context context)
		{
			HideProgressDialog();
			
			if(SteamAlerts.isAlertsEnabled)
			{
				AlertDialog.Builder dialog = new AlertDialog.Builder(context);
				dialog.SetTitle(title);
				dialog.SetMessage(message);
				dialog.SetPositiveButton("Ok", delegate (object sender, DialogClickEventArgs e) {
					((Dialog)sender).Cancel();
				});
				dialog.Show();
			}
		}
		
		public static void EnableAlerts()
		{
			SteamAlerts.isAlertsEnabled = true;
		}
		
		public static void DisableAlerts()
		{
			SteamAlerts.isAlertsEnabled = false;
		}
		
		public static void ShowProgressDialog(String title, String message, Context context)
		{
			if(dialog == null)
			{
				dialog = new ProgressDialog(context);
			}
			
			dialog.SetTitle(title);
			dialog.SetMessage(message);
			dialog.Show();
		}
		
		public static void ShowInputDialog(String title, String message, TextView textView, EventHandler<DialogClickEventArgs> handler, Context context)
		{
			alert = new AlertDialog.Builder(context);
			alert.SetTitle(title);
			alert.SetMessage(message);
			
			alert.SetView(textView);
			
			alert.SetPositiveButton("Ok", handler);
			alert.Show();
		}
		
		public static void HideInputDialog()
		{
			if(dialog != null)
			{
				dialog.Cancel();
			}
		}
		
		public static void HideProgressDialog()
		{
			if(dialog != null)
			{
				dialog.Hide();
			}
		}
		
		public static void Notification(String title, String ticker, String message, Intent intent, String key, String value)
		{
			ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(context);
			if(pref.GetBoolean("prefEnableNotifications", true))
			{
				int icon = Resource.Drawable.NotificationIcon;
				Notification notification = new Notification(icon, ticker);
				
				if(key != null && value != null)
				{
					intent.PutExtra(key, value);
				}
				
				PendingIntent contentIntent = PendingIntent.GetActivity(context, 0, intent, 0);
				
				notification.Flags = NotificationFlags.AutoCancel;
				notification.SetLatestEventInfo(context, "SteamDroid | " + title, message, contentIntent);
				
				notificationManager.Notify(1, notification);
			}
		}
		
		public static void Vibrate(int duration)
		{
			ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(context);
			if(pref.GetBoolean("prefEnableVibrate", true))
			{
				vibrator.Vibrate(duration);
			}
		}
	}
}

