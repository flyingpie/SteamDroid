using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SteamDroidServer.Logging;

namespace SteamDroidServer
{
    /// <summary>
    /// Users handles the allowed users on the server, which are read from the allowed file
    /// </summary>
    public class Users
    {
        /// <summary>
        /// The file to read users from
        /// </summary>
        public const String UsersFile = "allowed.txt";

        private static Users instance;

        private List<String> users;

        public Users()
        {
            users = new List<String>();

            LoadUsers();
        }

        /// <summary>
        /// Returns the Users object
        /// </summary>
        /// <returns>Users object</returns>
        public static Users Instance()
        {
            if (instance == null)
            {
                instance = new Users();
            }

            return instance;
        }

        /// <summary>
        /// Returns whether the specified user is allowed to connect to the server
        /// </summary>
        /// <param name="user">The user to check for</param>
        /// <returns></returns>
        public bool IsAllowed(String user)
        {
            return users.Contains(user);
        }

        /// <summary>
        /// Loads the allowed file and imports the users
        /// </summary>
        private void LoadUsers()
        {
            try
            {
                String[] users = File.ReadAllLines(UsersFile);

                for (int i = 0; i < users.Length; i++)
                {
                    String user = users[i].Trim();

                    if (users.Length > 0)
                    {
                        this.users.Add(user);
                    }
                }

                Logger.Get().Log("Read " + this.users.Count + " user(s)");
            }
            catch (Exception ex)
            {
                Logger.Get().Log("Warning; users file could not be opened (" + UsersFile + "): " + ex.Message);
            }
        }
    }
}
