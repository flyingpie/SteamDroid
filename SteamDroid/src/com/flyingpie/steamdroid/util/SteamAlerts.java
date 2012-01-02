package com.flyingpie.steamdroid.util;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.ProgressDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.media.MediaPlayer;
import android.os.Vibrator;
import android.preference.PreferenceManager;
import android.util.Log;

import com.flyingpie.steamdroid.R;

/**
 * SteamAlerts handles notifications, sounds and vibrate
 * @author Marco vd Oever
 *
 */
public class SteamAlerts {

	private static MediaPlayer mediaPlayer;
	private static NotificationManager manager;
	private static Vibrator vibrator;
	
	private static Context context;
	
	private static ProgressDialog dialog;
	
	/**
	 * Initializes SteamAlerts, using the specified activity
	 * @param activity
	 */
	public static void initialize(Activity activity)
	{
		SteamAlerts.mediaPlayer = MediaPlayer.create(activity.getApplicationContext(), R.raw.message);
		SteamAlerts.manager = (NotificationManager)activity.getSystemService(Context.NOTIFICATION_SERVICE);
		SteamAlerts.vibrator = (Vibrator)activity.getSystemService(Context.VIBRATOR_SERVICE);
		
		SteamAlerts.context = activity.getApplicationContext();
	}
	
	/**
	 * Returns the context to use for the alerts
	 * @return
	 */
	public static Context getContext()
	{
		return context;
	}
	
	/**
	 * Shows a progress dialog with the specified parameters
	 * @param title
	 * @param message
	 * @param context
	 */
	public static void showProgressDialog(String title, String message, Context context)
	{
		if(dialog == null)
		{
			dialog = new ProgressDialog(context);
		}
		
		dialog.setTitle(title);
		dialog.setMessage(message);
		dialog.show();
	}
	
	/**
	 * Hides the progress dialog
	 */
	public static void hideProgressDialog()
	{
		if(dialog != null)
		{
			dialog.hide();
		}
	}
	
	/**
	 * Shows an alert with the specified parameters
	 * @param title
	 * @param message
	 * @param context
	 */
	public static void showAlert(String title, String message, Context context)
	{
		hideProgressDialog();
		
		AlertDialog.Builder alertUsername = new AlertDialog.Builder(context);
        alertUsername.setTitle(title);
        alertUsername.setMessage(message);
        alertUsername.setPositiveButton("Ok", new DialogInterface.OnClickListener() {
			@Override
			public void onClick(DialogInterface dialog, int which) {
				dialog.cancel();
			}
		});
        alertUsername.show();
	}
	
	/**
	 * Shows a notification with the specified title, message and intent class
	 * @param title
	 * @param message
	 * @param intentClass
	 */
	public static void notification(String title, String message, Class<?> intentClass)
	{
		notification("SteamDroid", title, message, intentClass, null, null);
	}
	
	/**
	 * Shows a notification with the specified parameter
	 * @param title
	 * @param ticker
	 * @param message
	 * @param intentClass
	 * @param key
	 * @param value
	 */
	public static void notification(String title, String ticker, String message, Class<?> intentClass, String key, String value)
	{
		if(manager != null && context != null && PreferenceManager.getDefaultSharedPreferences(SteamAlerts.getContext()).getBoolean("prefEnableNotifications", true))
		{
			Log.v("SteamDroid", "Sending notification...");
			
			int icon = R.drawable.notification_icon;
			CharSequence tickerText = ticker;
			long when = System.currentTimeMillis();
			Notification notification = new Notification(icon, tickerText, when);
			
			CharSequence contentTitle = "SteamDroid | " + title;
			CharSequence contentText = message;
			Intent intent = new Intent(context, intentClass);
			
			if(key != null && value != null)
			{
				intent.putExtra(key, value);
			}
			
			PendingIntent contentIntent = PendingIntent.getActivity(context, 0, intent, 0);
			
			notification.flags = Notification.FLAG_AUTO_CANCEL;
			notification.setLatestEventInfo(context, contentTitle, contentText, contentIntent);
			
			manager.notify(1, notification);
		}
	}
	
	/**
	 * Plays the message received sound
	 */
	public static void playSound()
	{
		if(PreferenceManager.getDefaultSharedPreferences(SteamAlerts.getContext()).getBoolean("prefEnableSounds", true))
		{
			mediaPlayer.start();
		}
	}
	
	/**
	 * Vibrates for the specified duration
	 * @param duration
	 */
	public static void vibrate(int duration)
	{
		if(PreferenceManager.getDefaultSharedPreferences(SteamAlerts.getContext()).getBoolean("prefEnableVibrate", true))
		{
			vibrator.vibrate(duration);
		}
	}
}
