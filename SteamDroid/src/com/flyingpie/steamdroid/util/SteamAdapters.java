package com.flyingpie.steamdroid.util;

import java.util.ArrayList;
import java.util.HashMap;

import android.widget.ArrayAdapter;

import com.flyingpie.steamdroid.adapter.ChatAdapter;
import com.flyingpie.steamdroid.adapter.FriendsAdapter;
import com.flyingpie.steamdroid.api.ChatMessage;
import com.flyingpie.steamdroid.api.Friend;
import com.flyingpie.steamdroid.api.State;
import com.flyingpie.steamdroid.service.SteamService;

/**
 * SteamAdapters manages all adapters used in the app
 * @author Marco vd Oever
 *
 */
public class SteamAdapters {

	//private static ArrayAdapter<Friend> friendsAdapter;
	private static FriendsAdapter friendsAdapter;
	//private static HashMap<Friend, ArrayAdapter<ChatMessage>> chatAdapters;
	private static HashMap<Friend, ChatAdapter> chatAdapters;
	private static ArrayAdapter<State> stateAdapter;
	
	/**
	 * Returns the friends adapter
	 * @return
	 */
	//public static ArrayAdapter<Friend> getFriendsAdapter()
	public static FriendsAdapter getFriendsAdapter()
	{
		initialize();
		
		return friendsAdapter;
	}
	
	/**
	 * Returns the chat adapter from the specified friend
	 * @param friend The friend to get the adapter from
	 * @return
	 */
	public static ChatAdapter getChatAdapter(Friend friend)
	{
		initialize();
		
		if(!chatAdapters.containsKey(friend))
		{
			ChatAdapter adapter = new ChatAdapter(SteamService.getContext(), new ArrayList<ChatMessage>());
			chatAdapters.put(friend, adapter);
		}
		
		return chatAdapters.get(friend);
	}
	
	/**
	 * Returns the states adapter
	 * @return
	 */
	public static ArrayAdapter<State> getStateAdapter()
	{
		initialize();
		
		if(stateAdapter.getCount() == 0)
		{
			ArrayList<State> states = State.getStates();
			
			for(int i = 0; i < states.size(); i++)
			{
				stateAdapter.add(states.get(i));
			}
		}
		
		return stateAdapter;
	}
	
	/**
	 * Updates the friends adapter using the specified list of friends
	 * @param friends
	 */
	public static void updateFriendsList(ArrayList<Friend> friends)
	{
		initialize();
		
		friendsAdapter.clear();
		
		for(int i = 0; i < friends.size(); i++)
		{
			friendsAdapter.add(friends.get(i));
		}
		
		friendsAdapter.notifyDataSetChanged();
	}
	
	/**
	 * Handles the specified chat message by processing it in the proper chat adapter
	 * @param message
	 */
	public static void handleChatReceived(ChatMessage message)
	{
		initialize();
		
		Friend friend = message.getSender();
		/*
		if(!chatAdapters.containsKey(friend))
		{
			chatAdapters.put(message.getSender(), new ChatAdapter(SteamAlerts.getContext(), new ArrayList<ChatMessage>()));
		}
*/
		
		
		ChatAdapter adapter = getChatAdapter(friend);
		
		adapter.add(message);
		
		while(adapter.getCount() > 50)
		{
			adapter.remove((ChatMessage)adapter.getItem(0));
		}
		
		adapter.notifyDataSetChanged();
	}
	
	/**
	 * Initializes the adapters
	 */
	private static void initialize()
	{
		if(friendsAdapter == null)
		{
			//friendsAdapter = new ArrayAdapter<Friend>(SteamService.getContext(), android.R.layout.simple_list_item_1);
			friendsAdapter = new FriendsAdapter(SteamService.getContext(), new ArrayList<Friend>());
		}
		
		if(chatAdapters == null)
		{
			chatAdapters = new HashMap<Friend, ChatAdapter>();
		}
		
		if(stateAdapter == null)
		{
			stateAdapter = new ArrayAdapter<State>(SteamService.getContext(), android.R.layout.simple_list_item_1);
		}
	}
}
