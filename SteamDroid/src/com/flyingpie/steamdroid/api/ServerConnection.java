package com.flyingpie.steamdroid.api;

import java.io.*;
import java.net.*;

import com.flyingpie.steamdroid.service.SteamService;

import android.os.AsyncTask;
import android.util.Log;

/**
 * ServerConnection class, maintains a connection with the server and sends and receives messages
 * Received messages are passed to the SteamClient's parseCommand method
 * 
 * @author Marco vd Oever
 *
 */
public class ServerConnection extends AsyncTask<Integer, Integer, String> {

	private static Socket socket;
	private static BufferedReader reader;
	private static BufferedWriter writer;
	
	private static ServerConnection worker;
	
	public ServerConnection()
	{
		
	}
	
	/**
	 * Connects to the specified ip and port, throws an exception if unsuccessful
	 * @param ip The ip to connect to
	 * @param port The port to use
	 * @return Whether the connection was successful
	 * @throws UnknownHostException
	 * @throws IOException
	 */
	public static void connect(String ip, int port) throws UnknownHostException, IOException
	{
		socket = new Socket(ip, port);

		reader = new BufferedReader(new InputStreamReader(socket.getInputStream()));
		writer = new BufferedWriter(new OutputStreamWriter(socket.getOutputStream()));
		
		start();
	}
	
	/**
	 * Disconnects from the server
	 */
	public static void disconnect()
	{
		Log.v("SteamDroidAPI", "Disconnecting from server");
		stop();
		
		try
		{
			if(socket != null)
			{
				socket.close();
				socket = null;
			}
			
			if(reader != null)
			{
				reader.close();
				reader = null;
			}
			
			if(writer != null)
			{
				writer.close();
				writer = null;
			}
		}
		catch(Exception e)
		{
			System.out.println("Error closing socket: " + e.getMessage());
		}
	}
	
	/**
	 * Starts the message listener, reading one line at a time
	 */
	public static void start()
	{
		worker = new ServerConnection();
		worker.execute();
	}
	
	/**
	 * Stops the listener
	 */
	public static void stop()
	{
		worker.cancel(true);
	}

	/**
	 * Sends the specified string(s) to the server
	 * @param message An array of one or more strings to send
	 */
	public static void send(String... message)
	{
		StringBuilder result = new StringBuilder();
		
		for(int i = 0; i < message.length; i++)
		{
			if(result.length() > 0) result.append(' ');
			result.append(message[i]);
		}
		
		String data = result.toString();
		
		if(writer != null && data.length() > 0)
		{
			try {
				data = Encryption.encrypt(data);
				
				writer.write(data);
				writer.flush();
			} catch (Exception e) {
				e.printStackTrace();
				System.out.println("ERROR Failed to send data: " + e.getMessage());
			}
		}
	}

	@Override
	protected String doInBackground(Integer... params) {
		String result = readLine();
		
		if(result == null)
		{
			Log.v("ServerConnection", "Error reading line");
		}
		
		return result;
	}

	@Override
	protected void onPostExecute(String result) {
		if(result != null && result.length() > 0)
		{
			Log.v("SteamDroidAPI", "Received line: " + result);

			SteamService.getClient().parseCommand(result);
			start();
		}
		else
		{
			SteamService.getClient().errorReading();
		}
	}
	
	private String readLine()
	{
		String result = null;
		StringBuilder builder = new StringBuilder();
		int c;
		
		try
		{
			while(true)
			{
				c = socket.getInputStream().read();
				
				if(c == '\r') continue;
				if(c == -1 || c == '\n') break;
				
				builder.append((char)c);
			}
			
			if(builder.length() > 0)
			{
				result = Encryption.decrypt(builder.toString());
			}
		}
		catch(Exception e)
		{
			Log.e("ServerConnection", "Error reading byte: " + e.getMessage());
			return null;
		}
		
		return result;
	}
}
