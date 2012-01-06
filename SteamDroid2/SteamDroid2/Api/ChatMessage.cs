using System;

namespace SteamDroid2.Api
{
	/// <summary>
	/// Chat message represents a single message send or received from or to the chat
	/// </summary>
	public class ChatMessage
	{
		private Friend friend;
		private String message;
		
		public ChatMessage(Friend friend, String message)
		{
			this.friend = friend;
			this.message = message;
		}
		
		/// <summary>
		/// Gets the friend.
		/// </summary>
		/// <value>
		/// The friend.
		/// </value>
		public Friend Friend
		{
			get { return friend; }
		}
		
		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		public String Message
		{
			get { return message; }
		}
	}
}

