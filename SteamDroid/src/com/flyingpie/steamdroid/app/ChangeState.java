package com.flyingpie.steamdroid.app;

import com.flyingpie.steamdroid.R;
import com.flyingpie.steamdroid.api.State;
import com.flyingpie.steamdroid.util.SteamAdapters;

import android.app.ListActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.ListView;

public class ChangeState extends ListActivity {

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		
		setListAdapter(SteamAdapters.getStateAdapter());
		getListView().setBackgroundColor(getResources().getColor(R.color.listBackground));
	}

	@Override
	protected void onListItemClick(ListView l, View v, int position, long id) {
		super.onListItemClick(l, v, position, id);
		
		State state = SteamAdapters.getStateAdapter().getItem(position);
		state.setActive();
	}
}
