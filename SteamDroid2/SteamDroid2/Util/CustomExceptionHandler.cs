using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace SteamDroid2.Util
{
    public class CustomExceptionHandler : Java.Lang.Thread.IUncaughtExceptionHandler
    {
        public const String StacktraceUrl = "http://dev.flyingpie.nl/stacktrace/upload.php";

        private static CustomExceptionHandler handler;

        public static CustomExceptionHandler GetHandler()
        {
            if (handler == null)
            {
                handler = new CustomExceptionHandler();
            }

            return handler;
        }

        public void UncaughtException(Java.Lang.Thread thread, Java.Lang.Throwable ex)
        {
            Console.WriteLine("Uncaught exception: " + ex.Message);
            Java.Util.Date date = new Java.Util.Date();
            String timestamp = date.ToString();
            Java.IO.Writer result = new Java.IO.StringWriter();
            Java.IO.PrintWriter printWriter = new Java.IO.PrintWriter(result);
            ex.PrintStackTrace(printWriter);
            String stacktrace = result.ToString();
            printWriter.Close();
            String filename = timestamp + ".stacktrace";

            SendToServer(stacktrace, filename);
        }

        public IntPtr Handle
        {
            get { return new IntPtr(); }
        }

        private void SendToServer(String stacktrace, String filename)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(StacktraceUrl);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = stacktrace.Length;

            StreamWriter stOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
            stOut.Write(stacktrace);
            stOut.Close();
        }
    }
}
