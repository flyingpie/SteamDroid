package com.flyingpie.steamdroid.app;

import android.app.Activity;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.AbsListView;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;

import com.flyingpie.steamdroid.R;
import com.flyingpie.steamdroid.adapter.ChatAdapter;
import com.flyingpie.steamdroid.api.ChatMessage;
import com.flyingpie.steamdroid.api.Friend;
import com.flyingpie.steamdroid.service.SteamService;
import com.flyingpie.steamdroid.util.SteamAdapters;

public class Chat extends Activity implements OnClickListener {

	private ChatAdapter adapter;
	private Friend friend;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.chat);
		
		Button buttonSend = (Button)findViewById(R.id.buttonSend);
		buttonSend.setOnClickListener(this);
		
		ListView listOutput = (ListView)findViewById(R.id.listOutput);
		listOutput.setTranscriptMode(AbsListView.TRANSCRIPT_MODE_ALWAYS_SCROLL);

		Log.v("SteamDroid", "Opened chat window with " + getIntent().getStringExtra("steamid"));
		
		String steamId = getIntent().getStringExtra("steamid");
		friend = Friend.getFriendWithSteamId(steamId);
		adapter = SteamAdapters.getChatAdapter(friend);
		
		if(adapter != null)
		{
			listOutput.setAdapter(adapter);
		}
		
		SteamService.setActiveChat(friend);
	}

	@Override
	public void onClick(View v) {
		TextView textInput = (TextView) findViewById(R.id.textInput);
		
		String message = textInput.getText().toString();
		textInput.setText("");

		adapter.add(ChatMessage.create(message));
		adapter.notifyDataSetChanged();
		
		friend.chatSend(message);
	}

	@Override
	protected void onPause() {
		super.onPause();
		
		SteamService.clearActiveChat();
	}
}
