using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SteamKit2;

namespace SteamDroid2.Api
{
    public interface ICallbackHandler
    {
        void HandleCallback(CallbackMsg msg);
    }
}