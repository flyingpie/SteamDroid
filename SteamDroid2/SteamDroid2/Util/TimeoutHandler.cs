using System;
using Android.OS;
using Java.Lang;

namespace SteamDroid.Util
{
    class TimeoutHandler : AsyncTask<TimeoutArguments, Int32, object>
    {
        public static TimeoutHandler Start(int duration, Func<object> handler)
        {
            TimeoutHandler timeout = new TimeoutHandler();
            timeout.Execute(new TimeoutArguments(duration, handler));
            return timeout;
        }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] native_parms)
        {
            return base.DoInBackground(native_parms);
        }

        protected override object RunInBackground(params TimeoutArguments[] parameters)
        {
            TimeoutArguments args = parameters[0];

            Thread.Sleep(args.Sleep);

            return args;
        }

        protected override void OnPostExecute(object result)
        {
            base.OnPostExecute(result);

            TimeoutArguments args = (TimeoutArguments)result;

            args.Handler();
        }
    }

    class TimeoutArguments : Java.Lang.Object
    {
        public TimeoutArguments(int sleep, Func<object> handler)
        {
            Sleep = sleep;
            Handler = handler;
        }

        public int Sleep { get; set; }
        public Func<object> Handler { get; set; }
    }
}