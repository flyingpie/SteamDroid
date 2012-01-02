package com.flyingpie.steamdroid.api;

import java.security.MessageDigest;

import javax.crypto.Cipher;
import javax.crypto.SecretKey;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;

import android.util.Base64;

/**
 * Encryption encrypts and decrypts data from- and to base64 encoded AES
 * @author Marco vd Oever
 *
 */
public class Encryption {

	public static final int KEY_SIZE = 16;
	
	private static byte[] key;
	
	/**
	 * Sets the key to use for encryption
	 * @param key
	 * @throws Exception
	 */
	public static void setKey(String key) throws Exception
	{
		String hash = sha1(key);
		Encryption.key = new byte[KEY_SIZE];
		
		for(int i = 0; i < KEY_SIZE; i++)
		{
			Encryption.key[i] = (byte)hash.charAt(i);
		}
	}
	
	/**
	 * Encrypts the specified data
	 * @param cleartext
	 * @return
	 * @throws Exception
	 */
	public static String encrypt(String cleartext) throws Exception {
		byte[] rawKey = getRawKey(key);
		byte[] result = encrypt(rawKey, cleartext.getBytes());
		
		return toBase64(result);
	}

	/**
	 * Decrypts the specified data
	 * @param encrypted
	 * @return
	 * @throws Exception
	 */
	public static String decrypt(String encrypted) throws Exception {
		byte[] rawKey = getRawKey(key);
		byte[] enc = fromBase64(encrypted);
		byte[] result = decrypt(rawKey, enc);
		
		return new String(result);
	}

	private static byte[] getRawKey(byte[] key) throws Exception {
		SecretKey skey = new SecretKeySpec(key, "AES");

		byte[] raw = skey.getEncoded();

		return raw;
	}

	private static byte[] encrypt(byte[] raw, byte[] clear) throws Exception {
		SecretKeySpec skeySpec = new SecretKeySpec(raw, "AES");

		Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5Padding");
		IvParameterSpec ivParameterSpec = new IvParameterSpec(raw);

		cipher.init(Cipher.ENCRYPT_MODE, skeySpec, ivParameterSpec);
		byte[] encrypted = cipher.doFinal(clear);
		return encrypted;
	}

	private static byte[] decrypt(byte[] raw, byte[] encrypted) throws Exception {
		SecretKeySpec skeySpec = new SecretKeySpec(raw, "AES");
		Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5Padding");
		IvParameterSpec ivParameterSpec = new IvParameterSpec(raw);

		cipher.init(Cipher.DECRYPT_MODE, skeySpec, ivParameterSpec);
		byte[] decrypted = cipher.doFinal(encrypted);
		
		return decrypted;
	}

	public static String toBase64(byte[] buf) {
		return Base64.encodeToString(buf, 0);
	}

	public static byte[] fromBase64(String str) throws Exception {
		return Base64.decode(str, 0);
	}
	
	public static String sha1(String text) throws Exception { 
		MessageDigest md;
		md = MessageDigest.getInstance("SHA-1");
		byte[] sha1hash = new byte[40];
		md.update(text.getBytes("UTF-8"), 0, text.length());
		sha1hash = md.digest();
		return toBase64(sha1hash);
	}

	/*
	private static Encryption instance;
	
	private Cipher cipher;
	private SecretKeySpec key;
	private IvParameterSpec paramSpec;
	
	public Encryption(byte[] keyBytes)
	{
		try
		{
			setKey(keyBytes);
			
			cipher = Cipher.getInstance("AES");
		}
		catch(Exception e)
		{
			Log.e("Encryption", "Error initializing encryption: " + e.getMessage());
		}
	}

	public static Encryption instance()
	{
		if(instance == null)
		{
			instance = new Encryption(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x10 });
		}
		
		return instance;
	}
	
	public void setKey(byte[] keyBytes)
	{
		key = new SecretKeySpec(keyBytes, "AES");
		//paramSpec = new IvParameterSpec(keyBytes);
	}
	
	public byte[] encrypt(String input)
	{
		try
		{
			byte[] inputBytes = input.getBytes();
			
			cipher.init(Cipher.ENCRYPT_MODE, key);
			byte[] cipherText = new byte[cipher.getOutputSize(input.length())];
			int ctLength = cipher.update(inputBytes, 0, inputBytes.length, cipherText, 0);
			ctLength += cipher.doFinal(cipherText, ctLength);
			
			return cipherText;
		}
		catch (Exception e)
		{
			e.printStackTrace();
		}
		
		return null;
	}
	
	public String decrypt(byte[] input)
	{
		try
		{			
			cipher.init(Cipher.DECRYPT_MODE, key);
			byte[] plainText = new byte[cipher.getOutputSize(input.length)];
			int ptLength = cipher.update(input, 0, input.length, plainText, 0);
			ptLength += cipher.doFinal(plainText, ptLength);
			
			return new String(plainText);
		}
		catch(Exception e)
		{
			e.printStackTrace();
		}
		
		return null;
	}*/
}
