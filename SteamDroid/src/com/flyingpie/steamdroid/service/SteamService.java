package com.flyingpie.steamdroid.service;

import java.util.ArrayList;

import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.os.IBinder;
import android.util.Log;

import com.flyingpie.steamdroid.api.ChatMessage;
import com.flyingpie.steamdroid.api.Friend;
import com.flyingpie.steamdroid.api.MessageListener;
import com.flyingpie.steamdroid.api.SteamClient;
import com.flyingpie.steamdroid.app.Chat;
import com.flyingpie.steamdroid.util.SteamAdapters;
import com.flyingpie.steamdroid.util.SteamAlerts;
import com.flyingpie.steamdroid.util.SteamNotifier;
import com.flyingpie.steamdroid.util.SteamNotifierArguments;

/**
 * SteamService is the persistent service maintaining the SteamClient and handling events coming from the server
 * @author Marco vd Oever
 *
 */
public class SteamService extends Service implements MessageListener {

	private static SteamService steam;
	private static SteamClient client;
	
	private static Context context;
	
	private static Friend activeChat;
	
	private static SteamNotifier notifier;

	/**
	 * Returns the SteamClient
	 * @return
	 */
	public static SteamClient getClient()
	{
		return client;
	}
	
	/**
	 * Returns the application context
	 * @return
	 */
	public static Context getContext()
	{
		return context;
	}
	
	/**
	 * Returns the notifier
	 * @return
	 */
	public static SteamNotifier getNotifier()
	{
		if(notifier == null)
		{
			notifier = new SteamNotifier();
		}
		
		return notifier;
	}
	
	/**
	 * Connects using the specified host, port and credentials
	 * @param host
	 * @param port
	 * @param username
	 * @param password
	 */
	public static boolean connect(String host, int port, String username, String password, String authCode)
	{
		if(!getClient().getLoggedIn())
		{
			Log.v("SteamDroid", "Connecting to SteamDroid server...");

			//client.connect("192.168.0.12", 1337);
			//client.connect("178.21.117.254", 1337);
			client.connect(host, port);
			client.login(username, password, authCode);
			
			getNotifier().notifyObservers(SteamNotifierArguments.createProgress("Connecting", "Logging in..."));
			
			return true;
		}
		
		return false;
	}
	
	/**
	 * Disconnects from the server
	 */
	public static void disconnect()
	{
		Log.v("SteamDroid", "Disconnecting from SteamDroid server...");
		
		client.logout();
	}
	
	/**
	 * Sets the active chat
	 * @param friend
	 */
	public static void setActiveChat(Friend friend)
	{
		SteamService.activeChat = friend;
	}
	
	/**
	 * Clears the active chat
	 */
	public static void clearActiveChat()
	{
		SteamService.activeChat = null;
	}
	
	/**
	 * Called when a connection has been established
	 */
	@Override
	public void connected() {
		Log.v("SteamService", "Connected");
		
		getNotifier().notifyObservers();
	}

	/**
	 * Called when the connection has been closed
	 */
	@Override
	public void disconnected() {
		Log.v("SteamService", "Disconnected");
		
		getNotifier().notifyObservers(SteamNotifierArguments.createMessage("SteamDroid", "Disconnected from server"));
	}

	@Override
	public void notAllowed() {
		Log.v("SteamService", "Not allowed to connect");
		
		SteamAlerts.hideProgressDialog();
		
		getNotifier().notifyObservers(SteamNotifierArguments.createMessage("SteamDroid", "Not allowed to connect to the server"));
	}
	
	/**
	 * Called when logon to steam was successful
	 */
	@Override
	public void loggedIn() {
		Log.v("SteamService", "Logged in");
		
		getNotifier().notifyObservers(SteamNotifierArguments.createProgress("Connecting", "Requesting friends list..."));
		
		getClient().requestFriendsList();
		
		getNotifier().notifyObservers();
	}
	
	/**
	 * Called when logout from steam was successful
	 */
	@Override
	public void loggedOut() {
		Log.v("SteamService", "Logged out");
		
		client.disconnect();
		
		SteamAlerts.hideProgressDialog();
		
		getNotifier().notifyObservers();
	}

	@Override
	public void authRequest() {
		Log.v("SteamService", "Auth key request");
		
		SteamAlerts.hideProgressDialog();
		
		getNotifier().notifyObservers(SteamNotifierArguments.createMessage("Steam Guard", "An auth key was send to your email address, please enter the key in the settings menu"));
	}

	@Override
	public void authSending() {
		Log.v("SteamService", "Sending auth key...");
	}
	
	/**
	 * Called when the friends list is received
	 */
	@Override
	public void listFriends(ArrayList<Friend> friends) {
		Log.v("SteamService", "Updating friends list...");
		
		SteamAdapters.updateFriendsList(friends);
		
		getNotifier().notifyObservers();
		
		SteamAlerts.hideProgressDialog();
	}
	
	/**
	 * Called when a chat message is received
	 */
	@Override
	public void chatReceived(ChatMessage message) {
		Log.v("SteamService", message.getSender() + ": " + message);

		SteamAdapters.handleChatReceived(message);
		
		if(activeChat != message.getSender())
		{
			SteamAlerts.notification(message.getSender().getName(), "Message from " + message.getSender() + " " + message.getMessage(), message.getMessage(), Chat.class, "steamid", message.getSender().getSteamId());
			SteamAlerts.playSound();
			SteamAlerts.vibrate(400);
		}
		
		getNotifier().notifyObservers();
	}

	/**
	 * Called when a friend state is changed
	 */
	@Override
	public void friendStateChanged(Friend friend) {
		Log.v("SteamService", "Friend state changed: " + friend);
		
		SteamAdapters.getFriendsAdapter().notifyDataSetChanged();
		SteamAdapters.getChatAdapter(friend).notifyDataSetChanged();
		
		getNotifier().notifyObservers();
	}
	
	/**
	 * Called when an error has occurred
	 */
	@Override
	public void error(String message) {
		Log.v("SteamService", "Error: " + message);
		
		getNotifier().notifyObservers(SteamNotifierArguments.createMessage("Error", message));
	}
	
	@Override
	public IBinder onBind(Intent arg0) {
		return null;
	}

	@Override
	public void onCreate() {
		super.onCreate();
		
		Log.v("SteamDroid", "Started SteamDroid service");
		
		SteamService.context = getApplicationContext();
		
		steam = new SteamService();
		
		client = new SteamClient();
		client.addListener(steam);
	}
}
