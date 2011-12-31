using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamDroidServer.Client
{
    public class Protocol
    {
        public const String SplitString = " ";

        public class Client
        {
            public const String ListFriends = "list_friends";
            public const String ChatSend = "chat_send";
            public const String SetStatus = "set_status";
            public const String AuthSend = "auth_send";
            public const String Logout = "logout";
        }

        public class Server
        {
            public const String ListFriends = "list_friends";
            public const String NoSuchFriend = "no_such_friend";
            public const String ChatReceived = "chat_received";
            public const String AuthRequest = "auth_request";
            public const String AuthSending = "auth_sending";
            public const String LoggedIn = "logged_in";
            public const String LoggedOut = "logged_out";
            public const String FriendStateChanged = "friend_state_changed";
            public const String NotAllowed = "not_allowed";

            public const String Success = "success";
            public const String InvalidArgument = "invalid_argument";
            public const String UnknownCommand = "unknown_command";
        }
    }
}
