package com.flyingpie.steamdroid.api;

import java.util.ArrayList;

import android.util.Log;

import com.flyingpie.steamdroid.service.SteamService;

public class State implements Comparable<State> {

	public static final String STATE_ONLINE = "online";
	public static final String STATE_AWAY = "away";
	public static final String STATE_BUSY = "busy";
	public static final String STATE_SNOOZE = "snooze";
	public static final String STATE_OFFLINE = "offline";
	
	private static ArrayList<State> states;
	
	private String name;
	
	public State(String name)
	{
		this.name = name;
	}
	
	/**
	 * Returns the name of the state
	 * @return
	 */
	public String getName()
	{
		return name;
	}
	
	/**
	 * Sets the state to this one
	 */
	public void setActive()
	{
		SteamService.getClient().changeState(this);
	}
	
	@Override
	public String toString() {
		return name.substring(0, 1).toUpperCase() + name.substring(1);
	}

	public static ArrayList<State> getStates()
	{
		if(states == null)
		{
			states = new ArrayList<State>();
			
			states.add(new State(STATE_ONLINE));
			states.add(new State(STATE_AWAY));
			states.add(new State(STATE_BUSY));
			states.add(new State(STATE_SNOOZE));
			states.add(new State(STATE_OFFLINE));
		}
		
		return states;
	}
	
	public static State getState(String state)
	{
		getStates();
		
		state = state.toLowerCase();
		
		if(state.equals(STATE_ONLINE))
		{
			return getStateOnline();
		}
		
		if(state.equals(STATE_AWAY))
		{
			return getStateAway();
		}
		
		if(state.equals(STATE_BUSY))
		{
			return getStateBusy();
		}
		
		if(state.equals(STATE_SNOOZE))
		{
			return getStateSnooze();
		}
		
		if(state.equals(STATE_OFFLINE))
		{
			return getStateOffline();
		}

		Log.e("SteamState", "Requesting unknown state: " + state);
		
		return null;
	}

	public static State getStateOnline()
	{
		return states.get(0);
	}
	
	public static State getStateAway()
	{
		return states.get(1);
	}
	
	public static State getStateBusy()
	{
		return states.get(2);
	}
	
	public static State getStateSnooze()
	{
		return states.get(3);
	}
	
	public static State getStateOffline()
	{
		return states.get(4);
	}
	
	@Override
	public int compareTo(State another) {
		if(another.getName().equals(STATE_OFFLINE) && !getName().equals(STATE_OFFLINE))
		{
			return -1;
		}
		else if(!another.getName().equals(STATE_OFFLINE) && getName().equals(STATE_OFFLINE))
		{
			return 1;
		}
		
		return 0;
	}
}
