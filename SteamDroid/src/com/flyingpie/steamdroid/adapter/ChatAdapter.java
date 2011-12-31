package com.flyingpie.steamdroid.adapter;

import java.util.ArrayList;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.TextView;

import com.flyingpie.steamdroid.R;
import com.flyingpie.steamdroid.api.ChatMessage;
import com.flyingpie.steamdroid.api.State;

public class ChatAdapter extends BaseAdapter {

	private ArrayList<ChatMessage> messages;
	private LayoutInflater inflater;
	private Context context;
	
	public ChatAdapter(Context context, ArrayList<ChatMessage> messages)
	{
		this.messages = messages;
		this.inflater = LayoutInflater.from(context);
		this.context = context;
	}
	
	public void add(ChatMessage message)
	{
		messages.add(message);
	}
	
	public void remove(ChatMessage message)
	{
		messages.remove(message);
	}
	
	public void clear()
	{
		messages.clear();
	}
	
	@Override
	public int getCount() {
		return messages.size();
	}

	@Override
	public Object getItem(int position) {
		return messages.get(position);
	}

	@Override
	public long getItemId(int position) {
		return position;
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent) {
		ChatViewHolder holder;
		
		if(convertView == null)
		{
			convertView = inflater.inflate(R.layout.adapter_chat, null);
			holder = new ChatViewHolder();
			holder.textName = (TextView)convertView.findViewById(R.id.chatName);
			holder.textMessage = (TextView)convertView.findViewById(R.id.chatMessage);
			
			convertView.setTag(holder);
		}
		else
		{
			holder = (ChatViewHolder)convertView.getTag();
		}
		
		ChatMessage message = messages.get(position);
		
		holder.textName.setText(message.getSender().getName());
		holder.textMessage.setText(message.getMessage());
		
		int color = R.color.stateOnline;
		
		if(message.getSender().getState().equals(State.getStateOffline()))
		{
			color = R.color.stateOffline;
		}
		
		if(message.getSender().isPlaying())
		{
			color = R.color.stateInGame;
		}
		
		color = context.getResources().getColor(color);
		
		holder.textName.setTextColor(color);
		
		return convertView;
	}

	static class ChatViewHolder
	{
		TextView textName;
		TextView textMessage;
	}
}
