package com.flyingpie.steamdroid.app;

import android.app.ListActivity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.ListAdapter;
import android.widget.ListView;

import com.flyingpie.steamdroid.R;
import com.flyingpie.steamdroid.api.Friend;
import com.flyingpie.steamdroid.util.SteamAdapters;
import com.flyingpie.steamdroid.util.SteamAlerts;

public class Friends extends ListActivity {
	
	private ListAdapter adapter;
	
	/** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        adapter = SteamAdapters.getFriendsAdapter();
        setListAdapter(adapter);
        getListView().setBackgroundColor(getResources().getColor(R.color.listBackground));
    }
    
	@Override
	protected void onListItemClick(ListView l, View v, int position, long id) {
		super.onListItemClick(l, v, position, id);
		
		Friend friend = (Friend)adapter.getItem(position);
		
		if(friend.isAvailable() || SteamAdapters.getChatAdapter(friend).getCount() > 0)
		{			
			Intent intent = new Intent(Friends.this, Chat.class);
			intent.putExtra("steamid", friend.getSteamId());
			startActivity(intent);
		}
		else
		{
			SteamAlerts.showAlert("Warning", friend.getName() + " is offline", this);
		}
	}
}