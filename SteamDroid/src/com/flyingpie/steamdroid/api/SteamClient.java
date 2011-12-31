package com.flyingpie.steamdroid.api;

import java.util.ArrayList;

import android.util.Log;

/**
 * SteamClient, provides an interface to the SteamDroid server.
 * Events can be received by adding a message listener
 * 
 * @author Marco vd Oever
 *
 */
public class SteamClient {
	
	private ArrayList<MessageListener> listeners;
	private boolean loggedIn;
	
	private boolean friendlistPending;
	
	public SteamClient()
	{
		listeners = new ArrayList<MessageListener>();
		loggedIn = false;
		friendlistPending = false;
	}
	
	/**
	 * Adds the specified listener
	 * @param listener
	 * @return Whether the listener was added successfully
	 */
	public boolean addListener(MessageListener listener)
	{
		return listeners.add(listener);
	}
	
	/**
	 * Removes the specified listener
	 * @param listener
	 * @return Whether the listener was removed successfully
	 */
	public boolean removeListener(MessageListener listener)
	{
		return listeners.remove(listener);
	}
	
	/**
	 * Returns whether the client is logged in
	 * @return
	 */
	public boolean getLoggedIn()
	{
		return loggedIn;
	}
	
	/**
	 * Connects to the SteamDroid server, with an auth key
	 * @param ip The ip to connect to
	 * @param port The port to use
	 */
	public void connect(String ip, int port)
	{
		try {
			ServerConnection.connect(ip, port);
			
			Log.v("SteamClient", "Connecting to the SteamDroid server...");
			
			for(int i = 0; i < listeners.size(); i++)
			{
				listeners.get(i).connected();
			}
		} catch (Exception e) {
			e.printStackTrace();
			Log.e("SteamDroidAPI", "Could not connect to server: " + e.getMessage());
			
			for(int i = 0; i < listeners.size(); i++)
			{
				listeners.get(i).error("Could not connect to server: " + e.getMessage());
			}
		}
	}
	
	/**
	 * Disconnects from the SteamDroid server
	 */
	public void disconnect()
	{
		ServerConnection.disconnect();
		
		if(loggedIn)
		{
			logout();
		}
	}
	
	/**
	 * Logs on to the SteamDroid server
	 * @param username The username to use
	 * @param password The password to use
	 * @param authCode The authcode to use
	 */
	public void login(String username, String password, String authCode)
	{
		if(!loggedIn)
		{
			ServerConnection.send(username, password, authCode);
		}
	}
	
	/**
	 * Logs out from the SteamDroid server
	 */
	public void logout()
	{
		if(loggedIn)
		{
			ServerConnection.send(Protocol.Client.LOGOUT);
		}
	}
	
	/**
	 * Requests the friends list from the server
	 */
	public void requestFriendsList()
	{
		if(!friendlistPending)
		{
			Log.v("SteamClient", "Requesting friends list...");
			friendlistPending = true;
			ServerConnection.send(Protocol.Client.LIST_FRIENDS);
		}
	}
	
	/**
	 * Sends a chat message to the specified friend
	 * @param friend The friend to send the message to
	 * @param message The message to send
	 */
	public void chatSend(Friend friend, String message)
	{
		Log.v("SteamClient", "Sending chat message to '" + friend + "': " + message);
		ServerConnection.send(Protocol.Client.CHAT_SEND, friend.getSteamId(), message);
	}
	
	/**
	 * Changes the state of the user
	 * @param state Online, Away, Busy or Offline
	 */
	public void changeState(State state)
	{
		Log.v("SteamClient", "Changing state to " + state);
		ServerConnection.send(Protocol.Client.SET_STATUS, state.getName());
	}
	
	/**
	 * Parses a server command
	 * @param command
	 */
	public void parseCommand(String command)
	{
		String[] split = command.split(Protocol.SPLIT_STRING, 3);
		
		if(split[0].equals(Protocol.Server.LOGGED_IN))
		{
			loggedIn(split);
		}
		else if(split[0].equals(Protocol.Server.LOGGED_OUT))
		{
			loggedOut(split);
		}
		else if(split[0].equals(Protocol.Server.CHAT_RECEIVED))
		{
			chatReceived(split);
		}
		else if(split[0].equals(Protocol.Server.LIST_FRIENDS))
		{
			listFriends(split);
		}
		else if(split[0].equals(Protocol.Server.FRIEND_STATE_CHANGED))
		{
			friendStateChanged(split);
		}
		else if(split[0].equals(Protocol.Server.NOT_ALLOWED))
		{
			notAllowed(split);
		}
		else if(split[0].equals(Protocol.Server.AUTH_REQUEST))
		{
			authRequest(split);
		}
		else if(split[0].equals(Protocol.Server.AUTH_SENDING))
		{
			authSending(split);
		}
	}
	
	/**
	 * Handles read errors, occurs when the connection is remotely closed
	 */
	public void errorReading()
	{
		Log.v("SteamClient", "Error reading, disconnecting");
		
		loggedIn = false;
		
		for(int i = 0; i < listeners.size(); i++)
		{
			listeners.get(i).disconnected();
		}
	}
	
	/**
	 * Handles a logged in message
	 * @param command
	 */
	private void loggedIn(String[] command)
	{
		Log.v("SteamClient", "Logged in");
		
		loggedIn = true;
		
		for(int i = 0; i < listeners.size(); i++)
		{
			listeners.get(i).loggedIn();
		}
	}
	
	/**
	 * Handles a logged out message
	 * @param command
	 */
	private void loggedOut(String[] command)
	{
		Log.v("SteamClient", "Logged out");
		
		loggedIn = false;
		
		for(int i = 0; i < listeners.size(); i++)
		{
			listeners.get(i).loggedOut();
		}
	}
	
	/**
	 * Handles auth key request
	 * @param command
	 */
	private void authRequest(String[] command)
	{
		Log.v("SteamClient", "Received auth request");
		
		for(int i = 0; i < listeners.size(); i++)
		{
			listeners.get(i).authRequest();
		}
	}
	
	/**
	 * Handles auth key sending
	 * @param command
	 */
	private void authSending(String[] command)
	{
		Log.v("SteamClient", "Sending auth key");
		
		for(int i = 0; i < listeners.size(); i++)
		{
			listeners.get(i).authSending();
		}
	}
	
	/**
	 * Handles a not allowed message
	 * @param command
	 */
	private void notAllowed(String[] command)
	{
		Log.v("SteamClient", "Not allowed to connect");
		
		for(int i = 0; i < listeners.size(); i++)
		{
			listeners.get(i).notAllowed();
		}
	}
	
	/**
	 * Handles a received chat message
	 * @param command
	 */
	private void chatReceived(String[] command)
	{
		ChatMessage message = new ChatMessage(Friend.getFriendWithSteamId(command[1]), command[2]);
		
		Log.v("SteamClient", "Chat received: " + message);
		
		for(int i = 0; i < listeners.size(); i++)
		{
			listeners.get(i).chatReceived(message);
		}
	}
	
	/**
	 * Handles a received friends list
	 * @param command
	 */
	private void listFriends(String[] command)
	{
		Log.v("SteamClient", "Friends list received");
		
		String[] friendSplit = command[2].split(";(?=([^\"]*\"[^\"]*\")*[^\"]*$)");
		
		for(int i = 0; i < friendSplit.length; i++)
		{
			Friend.parse(friendSplit[i]);
		}
		
		Friend.sortFriends();
		
		for(int i = 0; i < listeners.size(); i++)
		{
			listeners.get(i).listFriends(Friend.getFriends());
		}
		
		friendlistPending = false;
	}
	
	/**
	 * Handles a friend state changed event
	 * @param command
	 */
	private void friendStateChanged(String[] command)
	{
		if(command.length == 3)
		{
			Friend friend = Friend.getFriendWithSteamId(command[1]);
			
			if(friend == null)
			{
				if(getLoggedIn())
				{
					requestFriendsList();
				}
			}
			else
			{
				String[] state = command[2].split(Protocol.SPLIT_STRING);
				
				friend.setState(State.getState(state[0]));
				
				if(state.length == 2)
				{
					friend.setGame(state[1]);
				}
				else
				{
					friend.setGame("");
				}
				
				for(int i = 0; i < listeners.size(); i++)
				{
					listeners.get(i).friendStateChanged(friend);
				}
			}
		}
		else
		{
			Log.e("SteamClient", "Invalid friend state changed event");
		}
	}
}
