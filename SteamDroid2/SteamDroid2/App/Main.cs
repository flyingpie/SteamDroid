using System;

using Android.App;
using Android.Content;
using Android.Preferences;
using Android.Widget;
using Android.OS;

using SteamKit2;
using SteamDroid2.Api;
using SteamDroid2.Util;

namespace SteamDroid2.App
{
    [Activity(Label = "Steam Droid", MainLauncher = true, Icon = "@drawable/LauncherIcon")]
    public class Main : Activity, ICallbackHandler
    {
        private EditText inputAuthKey;
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            Button buttonConnect = FindViewById<Button>(Resource.Id.ButtonConnect);
            buttonConnect.Click += ClickConnect;
            
            Button buttonDisconnect = FindViewById<Button>(Resource.Id.ButtonDisconnect);
            buttonDisconnect.Click += ClickDisconnect;
            
            Button buttonFriends = FindViewById<Button>(Resource.Id.ButtonFriends);
            buttonFriends.Click += ClickFriends;
            
            Button buttonChangeState = FindViewById<Button>(Resource.Id.ButtonChangeState);
            buttonChangeState.Click += ClickChangeState;
            
            Button buttonSettings = FindViewById<Button>(Resource.Id.ButtonSettings);
            buttonSettings.Click += ClickSettings;

            SteamAlerts.Initialize(this);
            
            Intent steamService = new Intent(this, typeof(SteamService));
            StartService(steamService);
            
            UpdateButtons();
            
            SteamService.GetClient().AddHandler(this);
        }
        
        public void HandleCallback(CallbackMsg msg)
        {
            if(msg.IsType<SteamClient.ConnectCallback>())
            {
                SteamAlerts.ShowProgressDialog("Connecting", "Logging in...", this);
            }
            else if(msg.IsType<SteamClient.DisconnectCallback>())
            {
                SteamAlerts.ShowAlertDialog("Disconnected", "Disconnected from Steam", this);
            }
            else if(msg.IsType<SteamUser.LoginKeyCallback>())
            {
                SteamAlerts.ShowAlertDialog("Connected", "Connected to Steam", this);
            }
            else if(msg.IsType<SteamUser.LogOnCallback>())
            {
                SteamUser.LogOnCallback callback = (SteamUser.LogOnCallback)msg;
                
                if(callback.Result == EResult.AccountLogonDenied)
                {
                    RequestAuthKey();
                }
                else if(callback.Result == EResult.InvalidLoginAuthCode)
                {
                    InvalidAuthKey();
                }
                else if(callback.Result == EResult.InvalidPassword)
                {
                    SteamAlerts.ShowAlertDialog("Invalid credentials", "Invalid username or password", this);
                }
                else if(callback.Result == EResult.AlreadyLoggedInElsewhere)
                {
                    SteamAlerts.ShowAlertDialog("Already logged in", "This Steam account is already logged in elsewhere", this);
                }
            }
            else if(msg.IsType<SteamUser.LoggedOffCallback>())
            {
                SteamUser.LoggedOffCallback callback = (SteamUser.LoggedOffCallback)msg;
                
                if(callback.Result == EResult.InvalidProtocolVer)
                {
                    SteamAlerts.ShowAlertDialog("Error", "Invalid protocol version", this);
                }
            }
            
            UpdateButtons();
        }
        
        private void ClickConnect(object sender, EventArgs e)
        {
            Connect();
        }
        
        private void ClickDisconnect(object sender, EventArgs e)
        {
            SteamService.GetClient().Disconnect();
            SteamAlerts.ShowProgressDialog("Disconnecting", "Disconnecting from the Steam servers...", this);
        }
        
        private void ClickFriends(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(Friends));
            StartActivity(intent);
        }
        
        private void ClickChangeState(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ChangeState));
            StartActivity(intent);
        }
        
        private void ClickSettings(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(Preferences));
            StartActivity(intent);
        }

        private void ClickSendAuthKey(object sender, DialogClickEventArgs e)
        {
            SteamAlerts.HideInputDialog();
            
            if(inputAuthKey.Text.Length > 0)
            {
                Connect(inputAuthKey.Text);
            }
            else
            {
                RequestAuthKey();
                SteamAlerts.ShowAlertDialog("Invalid key", "Please enter a valid auth key", this);
            }
            
            SteamAlerts.EnableAlerts();
        }
        
        private void Connect()
        {
            Connect(null);
        }
        
        private void Connect(String authCode)
        {
            ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(this);
            
            String username = pref.GetString("prefUsername", "");
            String password = pref.GetString("prefPassword", "");
            
            if(username.Length > 0 && password.Length > 0)
            {
                SteamService.GetClient().Connect(username, password, authCode);
                SteamAlerts.ShowProgressDialog("Connecting", "Connecting to the Steam servers...", this);
            }
            else
            {
                SteamAlerts.ShowAlertDialog("Warning", "No valid username or password entered", this);
            }
        }
        
        private void RequestAuthKey()
        {
            RequestAuthKey("Please fill in the auth key that has been send to your email address");
        }
        
        private void RequestAuthKey(String message)
        {
            SteamAlerts.DisableAlerts();

            inputAuthKey = new EditText(this);

            SteamAlerts.ShowInputDialog("Auth key", message, inputAuthKey, ClickSendAuthKey, this);
        }
        
        private void InvalidAuthKey()
        {
            RequestAuthKey("The given auth key is invalid or expired, please try again");
        }
        
        private void UpdateButtons()
        {
            bool connected = SteamService.GetClient() != null && SteamService.GetClient().LoggedIn;
            
            Button buttonConnect = FindViewById<Button>(Resource.Id.ButtonConnect);
            buttonConnect.Enabled = !connected;
            
            Button buttonDisconnect = FindViewById<Button>(Resource.Id.ButtonDisconnect);
            buttonDisconnect.Enabled = connected;
            
            Button buttonFriends = FindViewById<Button>(Resource.Id.ButtonFriends);
            buttonFriends.Enabled = connected;
            
            Button buttonChangeState = FindViewById<Button>(Resource.Id.ButtonChangeState);
            buttonChangeState.Enabled = connected;
        }
    }
}

