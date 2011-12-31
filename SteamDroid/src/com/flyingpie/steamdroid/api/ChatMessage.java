package com.flyingpie.steamdroid.api;

public class ChatMessage {

	private static Friend me = new Friend("", "me", State.getStateOnline());
	
	private Friend sender;
	private String message;
	
	public ChatMessage(Friend sender, String message)
	{
		this.sender = sender;
		this.message = message;
	}
	
	public Friend getSender()
	{
		return sender;
	}
	
	public String getMessage()
	{
		return message;
	}
	
	@Override
	public String toString()
	{
		return sender + ": " + message;
	}
	
	public static ChatMessage create(String message)
	{
		return new ChatMessage(me, message);
	}
}
