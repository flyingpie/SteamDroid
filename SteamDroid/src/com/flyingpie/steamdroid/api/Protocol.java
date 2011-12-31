package com.flyingpie.steamdroid.api;

public class Protocol {

	public static final String SPLIT_STRING = " ";
	
	public class Client
	{
		public static final String LIST_FRIENDS = "list_friends";
		public static final String CHAT_SEND = "chat_send";
		public static final String SET_STATUS = "set_status";
		public static final String AUTH_SEND = "auth_send";
		public static final String LOGOUT = "logout";
	}
	
	public class Server
	{
		public static final String LIST_FRIENDS = "list_friends";
		public static final String NO_SUCH_FRIEND = "no_such_friend";
		public static final String CHAT_RECEIVED = "chat_received";
		public static final String AUTH_REQUEST = "auth_request";
		public static final String AUTH_SENDING = "auth_sending";
		public static final String LOGGED_IN = "logged_in";
		public static final String LOGGED_OUT = "logged_out";
		public static final String NOT_ALLOWED = "not_allowed";
		public static final String FRIEND_STATE_CHANGED = "friend_state_changed";
		public static final String SUCCESS = "success";
		public static final String INVALID_ARGUMENT = "invalid_argument";
		public static final String UNKNOWN_COMMAND = "unknown_command";
	}
}
