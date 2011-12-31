package com.flyingpie.steamdroid.api;

import java.util.ArrayList;
import java.util.Collections;

import com.flyingpie.steamdroid.service.SteamService;

public class Friend implements Comparable<Friend> {

	private String steamId;
	private String name;
	private State state;
	private String game;
	
	private static ArrayList<Friend> friends;
	
	public Friend(String steamId, String name, State state)
	{
		this.steamId = steamId;
		this.name = name;
		this.state = state;
		this.game = "";
		
		friends.add(this);
	}
	
	public String getSteamId()
	{
		return steamId;
	}
	
	public String getName()
	{
		return name;
	}
	
	public State getState()
	{
		return state;
	}
	
	public String getGame()
	{
		return game;
	}
	
	public boolean isAvailable()
	{
		return !state.equals(State.getStateOffline());
	}
	
	public boolean isPlaying()
	{
		return game.length() > 0;
	}
	
	public void setName(String name)
	{
		this.name = name;
	}
	
	public void setState(State state)
	{
		this.state = state;
	}
	
	public void setGame(String game)
	{
		this.game = game;
	}
	
	public void chatSend(String message)
	{
		if(message != null && message.length() > 0)
		{
			//SteamClient.instance().send("chat_send " + steamId + ' ' + '"' + message + '"');
			SteamService.getClient().chatSend(this, message);
		}
	}
	
	public static Friend createFriend(String steamId, String name, State state)
	{
		initialize();
		
		Friend withSteamId = getFriendWithSteamId(steamId);
		
		if(withSteamId != null)
		{
			withSteamId.name = name;
			withSteamId.state = state;
			return withSteamId;
		}
		
		return new Friend(steamId, name, state);
	}
	
	public static Friend getFriendWithSteamId(String steamId)
	{
		initialize();
		
		for(int i = 0; i < friends.size(); i++)
		{
			if(friends.get(i).getSteamId().equals(steamId))
			{
				return friends.get(i);
			}
		}
		
		return null;
	}
	
	public static Friend getFriendWithName(String name)
	{
		initialize();
		
		for(int i = 0; i < friends.size(); i++)
		{
			if(friends.get(i).getName().equals(name))
			{
				return friends.get(i);
			}
		}
		
		return null;
	}

	public static Friend parse(String data)
	{
		String[] currentFriend = data.split("\\|");
		
		String steamId = currentFriend[0];
		String name = currentFriend[1].substring(1, currentFriend[1].length() - 1);
		String state = currentFriend[2];
		
		Friend friend = Friend.createFriend(steamId, name, State.getState(state));
		
		if(currentFriend.length >= 4)
		{
			String game = currentFriend[3];
			friend.setGame(game);
		}
		
		return friend;
	}
	
	public static ArrayList<Friend> getFriends()
	{
		return friends;
	}
	
	public static void sortFriends()
	{
		Collections.sort(friends);
	}
	
	@Override
	public boolean equals(Object obj) {
		if(obj.getClass() == getClass())
		{
			Friend friend = (Friend)obj;
			
			return friend.getSteamId().equals(getSteamId());
		}
		
		return false;
	}

	@Override
	public String toString()
	{		
		return name;
	}
	
	private static void initialize()
	{
		if(friends == null)
		{
			friends = new ArrayList<Friend>();
		}
	}

	@Override
	public int compareTo(Friend another) {
		if(isPlaying() && !another.isPlaying())
		{
			return -1;
		}
		
		int stateCompare = getState().compareTo(another.getState());
		if(stateCompare == 0)
		{
			return getName().compareTo(another.getName());
		}
		
		return stateCompare;
	}
}
