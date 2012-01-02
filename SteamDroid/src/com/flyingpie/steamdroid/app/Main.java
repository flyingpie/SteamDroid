package com.flyingpie.steamdroid.app;

import android.app.Activity;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;

import com.flyingpie.steamdroid.R;
import com.flyingpie.steamdroid.api.Encryption;
import com.flyingpie.steamdroid.api.SteamClient;
import com.flyingpie.steamdroid.service.SteamService;
import com.flyingpie.steamdroid.util.Observer;
import com.flyingpie.steamdroid.util.SteamAlerts;
import com.flyingpie.steamdroid.util.SteamNotifier;
import com.flyingpie.steamdroid.util.SteamNotifierArguments;

public class Main extends Activity implements Observer, OnClickListener {

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.main);
		
		SteamAlerts.initialize(this);
		
		Intent steamService = new Intent(this, SteamService.class);
        startService(steamService);
        
		Button buttonFriends = (Button)findViewById(R.id.buttonFriends);
		buttonFriends.setOnClickListener(this);
		
		Button buttonSettings = (Button)findViewById(R.id.buttonSettings);
		buttonSettings.setOnClickListener(this);
		
		Button buttonConnect = (Button)findViewById(R.id.buttonConnect);
		buttonConnect.setOnClickListener(this);
		
		Button buttonDisconnect = (Button)findViewById(R.id.buttonDisconnect);
		buttonDisconnect.setOnClickListener(this);
		
		Button buttonChangeState = (Button)findViewById(R.id.buttonChangeState);
		buttonChangeState.setOnClickListener(this);
		
		SteamService.getNotifier().addObserver(this);
		
		updateButtons();
	}

	@Override
	protected void onDestroy() {
		super.onDestroy();
		
		SteamService.getNotifier().removeObserver(this);
	}

	@Override
	protected void onResume() {
		super.onResume();
		
		//updateButtons();
	}

	@Override
	public void onClick(View v) {
		Class<?> target = null;
		
		switch(v.getId())
		{
		case R.id.buttonConnect:
			connect();
			break;
		case R.id.buttonDisconnect:
			disconnect();
			break;
		case R.id.buttonFriends:
			target = Friends.class;
			break;
		case R.id.buttonChangeState:
			target = ChangeState.class;
			break;
		case R.id.buttonSettings:
			target = Preferences.class;
			break;
		}
		
		if(target != null)
		{
			Intent intent = new Intent(this, target);
			startActivity(intent);
		}
	}
	
	private void connect()
	{
		if(!SteamService.getClient().getLoggedIn())
		{
			SharedPreferences pref = PreferenceManager.getDefaultSharedPreferences(this);
			
			String username = pref.getString("prefUsername", "");
			String password = pref.getString("prefPassword", "");
			String authCode = pref.getString("prefAuthCode", "");
			String host = pref.getString("prefServerAddress", "");
			String portString = pref.getString("prefServerPort", "");
			int port = (portString.length() > 0) ? Integer.parseInt(portString) : -1;
			String securityKey = pref.getString("prefSecurityKey", "");
			
			if(username.length() == 0 || password.length() == 0)
			{
				SteamAlerts.showAlert("Warning", "No valid username and / or password given", this);
			}
			else if(host.length() == 0 || port <= 0)
			{
				SteamAlerts.showAlert("Warning", "No valid server address and / or port given", this);
			}
			else if(securityKey.length() == 0)
			{
				SteamAlerts.showAlert("Warning", "No valid security key given", this);
			}
			else
			{
				SteamAlerts.showProgressDialog("Connecting", "Connecting to the SteamDroid server...", this);
				
				try {
					Encryption.setKey(securityKey);
				} catch (Exception e) {
					e.printStackTrace();
					SteamAlerts.showAlert("Error", "Could not set security key: " + e.getMessage(), this);
					return;
				}
				
				boolean connecting = SteamService.connect(host, port, username, password, authCode);
				
				if(!connecting)
				{
					SteamAlerts.showAlert("Error", "Could not connect to server", this);
				}
			}
		}
		else
		{
			SteamAlerts.showAlert("Warning", "Already connected", this);
		}
	}
	
	private void disconnect()
	{
		if(SteamService.getClient().getLoggedIn())
		{
			SteamAlerts.showProgressDialog("Disconnecting", "Logging out and disconnecting from the SteamDroid server...", this);
			SteamService.disconnect();
		}
		else
		{
			SteamAlerts.showAlert("Warning", "Not connected", this);
		}
	}
	
	private void updateButtons()
	{
		SteamClient client = SteamService.getClient();
        boolean connected = client != null && client.getLoggedIn();
        
		Button buttonConnect = (Button)findViewById(R.id.buttonConnect);
		buttonConnect.setEnabled(!connected);
		
		Button buttonDisconnect = (Button)findViewById(R.id.buttonDisconnect);
		buttonDisconnect.setEnabled(connected);
		
		Button buttonFriends = (Button)findViewById(R.id.buttonFriends);
		buttonFriends.setEnabled(connected);
		
		Button buttonChangeState = (Button)findViewById(R.id.buttonChangeState);
		buttonChangeState.setEnabled(connected);
	}

	@Override
	public void update(SteamNotifier observable, SteamNotifierArguments args) {
		updateButtons();
		
		switch(args.getType())
		{
		case Message:
			SteamAlerts.showAlert("SteamDroid", args.getMessage(), this);
			break;
		case Progress:
			SteamAlerts.showProgressDialog(args.getTitle(), args.getMessage(), this);
		}
	}
}
