package com.flyingpie.steamdroid.util;

import java.util.ArrayList;

public class SteamNotifier {

	private ArrayList<Observer> observers;
	
	public SteamNotifier()
	{
		observers = new ArrayList<Observer>();
	}
	
	public void addObserver(Observer observer)
	{
		observers.add(observer);
	}
	
	public void removeObserver(Observer observer)
	{
		observers.remove(observer);
	}
	
	public void notifyObservers()
	{
		notifyObservers(SteamNotifierArguments.EmptyArguments);
	}
	
	public void notifyObservers(SteamNotifierArguments args)
	{
		for(int i = 0; i < observers.size(); i++)
		{
			observers.get(i).update(this, args);
		}
	}
}
