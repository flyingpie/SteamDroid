using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamDroidServer.Networking.EventArguments
{
    public class MessageReceivedEventArgs
    {
        private String message;

        public MessageReceivedEventArgs(String message)
        {
            this.message = message;
        }

        public String Message
        {
            get { return message; }
        }
    }
}
