using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using Android.Util;

namespace SteamDroid.Util
{
    public class SteamAlerts
    {
        private static MediaPlayer mediaPlayer;

        private static AudioManager audioManager;
        private static NotificationManager notificationManager;
        private static Vibrator vibrator;
        
        private static Context context;
        
        private static ProgressDialog dialog;
        private static AlertDialog.Builder alert;
        
        private static bool isAlertsEnabled;
        private static bool isProgressEnabled;
        
        public static void Initialize(Activity activity)
        {
            audioManager = (AudioManager)activity.GetSystemService(Context.AudioService);
            
            mediaPlayer = MediaPlayer.Create(activity.ApplicationContext, Resource.Raw.Message);
            
            notificationManager = (NotificationManager)activity.GetSystemService(Context.NotificationService);
            vibrator = (Vibrator)activity.GetSystemService(Context.VibratorService);
            
            //context = activity.ApplicationContext;
            context = activity;

            isAlertsEnabled = true;
            isProgressEnabled = true;
        }
        
        public static Context GetContext()
        {
            return context;
        }
        
        public static void ShowAlertDialog(String title, String message, Context context)
        {
            HideProgressDialog();
            
            if(isAlertsEnabled)
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
            isAlertsEnabled = true;
        }
        
        public static void DisableAlerts()
        {
            isAlertsEnabled = false;
        }

        public static void EnableProgress()
        {
            isProgressEnabled = true;
        }

        public static void DisableProgress()
        {
            isProgressEnabled = false;
            HideProgressDialog();
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
            if (dialog != null)
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

                bool permNotification = pref.GetBoolean("prefEnablePermNotification", false);

                if (!permNotification)
                {
                    notification.Flags = NotificationFlags.AutoCancel;
                }

                notification.SetLatestEventInfo(context, "SteamDroid | " + title, message, contentIntent);
                
                notificationManager.Notify(1, notification);
            }
        }


        public static void ShowToast(String message)
        {
            HideProgressDialog();

            Toast toast = Toast.MakeText(context, message, ToastLength.Short);
            toast.Show();
        }

        public static void Vibrate(int duration)
        {
            ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(context);
            if(pref.GetBoolean("prefEnableVibrate", true))
            {
                vibrator.Vibrate(duration);
            }
        }

        public static void PlaySound()
        {
            if (PreferenceManager.GetDefaultSharedPreferences(GetContext()).GetBoolean("prefEnableSound", true))
            {
                int streamVolume = audioManager.GetStreamVolume(Stream.Notification);
                int streamVolumeMax = audioManager.GetStreamMaxVolume(Stream.Notification);
                float volume = (float)streamVolume / (float)streamVolumeMax;
                mediaPlayer.SetVolume(volume, volume);

                mediaPlayer.Start();
            }
        }
    }
}

