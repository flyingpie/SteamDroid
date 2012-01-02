package com.flyingpie.steamdroid.util;

import java.io.PrintWriter;
import java.io.StringWriter;
import java.io.Writer;
import java.lang.Thread.UncaughtExceptionHandler;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import org.apache.http.NameValuePair;
import org.apache.http.client.entity.UrlEncodedFormEntity;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.message.BasicNameValuePair;
import org.apache.http.protocol.HTTP;

import android.util.Log;

public class CustomExceptionHandler implements UncaughtExceptionHandler {

	public static final String STACKTRACE_URL = "http://dev.flyingpie.nl/stacktrace/upload.php";
	
	private UncaughtExceptionHandler defaultUEH;
	
	public CustomExceptionHandler()
	{
		this.defaultUEH = Thread.getDefaultUncaughtExceptionHandler();
	}
	
	public void uncaughtException(Thread t, Throwable e)
	{
		Date date = new Date();
		String timestamp = date.toString();
		final Writer result = new StringWriter();
		final PrintWriter printWriter = new PrintWriter(result);
		e.printStackTrace(printWriter);
		String stacktrace = result.toString();
		printWriter.close();
		String filename = timestamp + ".stacktrace";
		
		sendToServer(stacktrace, filename);
		
		Log.v("SteamDroid", "Uploading stacktrace");
		this.defaultUEH.uncaughtException(t, e);
	}
	
	private void sendToServer(String stacktrace, String filename)
	{
		DefaultHttpClient httpClient = new DefaultHttpClient();
		HttpPost httpPost = new HttpPost(STACKTRACE_URL);
		List<NameValuePair> nvps = new ArrayList<NameValuePair>();
		nvps.add(new BasicNameValuePair("filename", filename));
		nvps.add(new BasicNameValuePair("stacktrace", stacktrace));
		
		try
		{
			httpPost.setEntity(new UrlEncodedFormEntity(nvps, HTTP.UTF_8));
			httpClient.execute(httpPost);
		}
		catch(Exception e)
		{
			e.printStackTrace();
		}
	}
}
