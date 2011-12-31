using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SteamKit2;

namespace SteamDroidServer.Steam
{
    /// <summary>
    /// Interface for accepting callbacks from Steam
    /// </summary>
    public interface ICallbackHandler
    {
        void HandleCallback(CallbackMsg msg);
    }
}
