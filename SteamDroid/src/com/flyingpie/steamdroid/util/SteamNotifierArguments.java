package com.flyingpie.steamdroid.util;

public class SteamNotifierArguments {

	public static final SteamNotifierArguments EmptyArguments = new SteamNotifierArguments();
	
	public enum NotifierType
	{
		Empty,
		Message,
		Progress
	}
	
	private String title;
	private String message;
	private NotifierType type;
	
	public SteamNotifierArguments()
	{
		type = NotifierType.Empty;
	}
	
	public SteamNotifierArguments(String title, String message)
	{
		this.title = title;
		this.message = message;
		type = NotifierType.Message;
	}
	
	public String getTitle()
	{
		return title;
	}
	
	public String getMessage()
	{
		return message;
	}
	
	public NotifierType getType()
	{
		return type;
	}
	
	private void setType(NotifierType type)
	{
		this.type = type;
	}
	
	public static SteamNotifierArguments createMessage(String title, String message)
	{
		SteamNotifierArguments snt = new SteamNotifierArguments(title, message);
		snt.setType(NotifierType.Message);
		
		return snt;
	}
	
	public static SteamNotifierArguments createProgress(String title, String message)
	{
		SteamNotifierArguments snt = new SteamNotifierArguments(title, message);
		snt.setType(NotifierType.Progress);
		
		return snt;
	}
}
