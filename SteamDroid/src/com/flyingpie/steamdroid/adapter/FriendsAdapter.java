package com.flyingpie.steamdroid.adapter;

import java.util.ArrayList;

import com.flyingpie.steamdroid.R;
import com.flyingpie.steamdroid.api.Friend;
import com.flyingpie.steamdroid.api.State;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.TextView;

public class FriendsAdapter extends BaseAdapter {

	private ArrayList<Friend> friends;
	private LayoutInflater inflater;
	private Context context;
	
	public FriendsAdapter(Context context, ArrayList<Friend> friends)
	{
		this.friends = friends;
		inflater = LayoutInflater.from(context);
		this.context = context;
	}
	
	public void add(Friend friend)
	{
		friends.add(friend);
	}
	
	public void clear()
	{
		friends.clear();
	}
	
	@Override
	public int getCount() {
		return friends.size();
	}

	@Override
	public Object getItem(int position) {
		return friends.get(position);
	}

	@Override
	public long getItemId(int position) {
		return position;
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent) {
		ViewHolder holder;
		
		if(convertView == null)
		{
			convertView = inflater.inflate(R.layout.adapter_friends, null);
			holder = new ViewHolder();
			holder.textName = (TextView)convertView.findViewById(R.id.name);
			holder.textState = (TextView)convertView.findViewById(R.id.state);
			holder.textGame = (TextView)convertView.findViewById(R.id.game);
			
			convertView.setTag(holder);
		}
		else
		{
			holder = (ViewHolder)convertView.getTag();
		}
		
		Friend friend = friends.get(position);
		
		holder.textName.setText(friend.getName());
		holder.textState.setText(friend.getState().toString());
		holder.textGame.setText(friend.getGame());
		
		int color = R.color.stateOnline;
		
		if(friend.getState().equals(State.getStateOffline()))
		{
			color = R.color.stateOffline;
			
		}
		else if(friend.isPlaying())
		{
			color = R.color.stateInGame;
		}
		
		color = context.getResources().getColor(color);
		
		holder.textName.setTextColor(color);
		holder.textState.setTextColor(color);
		holder.textGame.setTextColor(color);
		
		return convertView;
	}

	static class ViewHolder
	{
		TextView textName;
		TextView textState;
		TextView textGame;
	}
}
