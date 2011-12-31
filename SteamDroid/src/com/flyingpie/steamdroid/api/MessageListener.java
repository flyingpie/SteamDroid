package com.flyingpie.steamdroid.api;

import java.util.*;

/**
 * MessageListener interface, receives events from the steamclient
 * 
 * @author Marco vd Oever
 *
 */
public interface MessageListener {

	void connected();
	void disconnected();
	void notAllowed();
	
	void loggedIn();
	void loggedOut();
	
	void authRequest();
	void authSending();
	
	void listFriends(ArrayList<Friend> friends);
	void chatReceived(ChatMessage message);
	void friendStateChanged(Friend friend);
	
	void error(String message);
}
