using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamDroidServer.Steam
{
    public class SteamInstances
    {
        public static SortedList<String, Steam3> instances;

        public static void Initialize()
        {
            if (instances == null)
            {
                instances = new SortedList<String, Steam3>();
            }
        }

        public static Steam3 GetInstance(String username)
        {
            Initialize();

            if (!instances.ContainsKey(username))
            {
                instances.Add(username, new Steam3());
            }

            return instances[username];
        }

        public static bool HasInstance(String username)
        {
            Initialize();

            return instances.ContainsKey(username);
        }

        public static void RemoveInstance(String username)
        {
            instances.Remove(username);
        }
    }
}
