using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SteamKit2;
using SteamDroidServer.Steam;

namespace SteamDroidServer.Client
{
    public class ClientCommandParser
    {
        public static void ParseCommand(Command command)
        {
            String result = Protocol.Server.UnknownCommand;
            
            switch (command.ClientCommand)
            {
                case Protocol.Client.AuthSend:
                    result = AuthSend(command);
                    break;
                case Protocol.Client.ChatSend:
                    result = ChatSend(command);
                    break;
                case Protocol.Client.ListFriends:
                    result = ListFriends(command);
                    break;
                case Protocol.Client.Logout:
                    result = Logout(command);
                    break;
                case Protocol.Client.SetStatus:
                    result = SetStatus(command);
                    break;
                default:
                    break;
            }

            command.Connection.Send(result);
        }

        public static String AuthSend(Command command)
        {
            Logging.Logger.Get().Log("AuthSend(" + command + ")");

            if (command.Parameters.Length == 1 && command.Parameters[0].Length > 0)
            {
                command.Steam.Connect(command.Parameters[0]);
                return Protocol.Server.AuthSending;
            }
            
            return Protocol.Server.InvalidArgument;
        }

        public static String ChatSend(Command command)
        {
            SteamFriends friends = command.Steam.Friends;

            if (command.Parameters.Length == 2)
            {
                int count = friends.GetFriendCount();
                for (int i = 0; i < count; i++)
                {
                    SteamID id = friends.GetFriendByIndex(i);
                    
                    if(id.ToString() == command.Parameters[0])
                    {
                        friends.SendChatMessage(id, EChatEntryType.ChatMsg, command.Parameters[1].Trim('"'));
                        return Protocol.Server.Success;
                    }
                }

                return Protocol.Server.NoSuchFriend;
            }

            return Protocol.Server.InvalidArgument;
        }

        public static String ListFriends(Command command)
        {
            StringBuilder result = new StringBuilder();
            SteamFriends friends = command.Steam.Friends;
            int count = friends.GetFriendCount();
            for (int i = 0; i < count; i++)
            {
                if (result.Length > 0) result.Append(';');
                SteamID steamId = friends.GetFriendByIndex(i);
                String name = friends.GetFriendPersonaName(steamId);
                EPersonaState state = friends.GetFriendPersonaState(steamId);
                String game = friends.GetFriendGamePlayedName(steamId);

                result.Append(steamId + "|" + '"' + name + '"' + "|" + state + "|" + game);
            }

            return Protocol.Server.ListFriends + " " + Protocol.Server.Success + " " + result.ToString();
        }

        public static String Logout(Command command)
        {
            command.Steam.Disconnect();

            command.Connection.IsAuthorized = false;

            return Protocol.Server.LoggedOut;
        }

        public static String SetStatus(Command command)
        {
            String result = Protocol.Server.InvalidArgument;

            if(command.Parameters.Length == 1)
            {
                String status = command.Parameters[0];
                SteamFriends friends = command.Steam.Friends;

                switch (status.ToLower())
                {
                    case "away":
                        friends.SetPersonaState(EPersonaState.Away);
                        result = Protocol.Server.Success;
                        break;
                    case "busy":
                        friends.SetPersonaState(EPersonaState.Busy);
                        result = Protocol.Server.Success;
                        break;
                    case "online":
                        friends.SetPersonaState(EPersonaState.Online);
                        result = Protocol.Server.Success;
                        break;
                    case "offline":
                        friends.SetPersonaState(EPersonaState.Offline);
                        result = Protocol.Server.Success;
                        break;
                    case "snooze":
                        friends.SetPersonaState(EPersonaState.Snooze);
                        result = Protocol.Server.Success;
                        break;
                    default:
                        result = Protocol.Server.InvalidArgument;
                        break;
                }
            }

            return result;
        }
    }
}
