using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;

namespace SteamDroid2.Util
{
    public class CustomExceptionHandler
    {
        public const String StacktraceUrl = "http://dev.flyingpie.nl/stacktrace/upload.php";

        public static void UncaughtException(Exception ex)
        {
            Console.WriteLine("Uncaught exception: " + ex.Message);
            Java.Util.Date date = new Java.Util.Date();
            String timestamp = date.ToString();
            String stacktrace = ex.StackTrace;
            String filename = timestamp + ".stacktrace";

            SendToServer(stacktrace, filename);
        }

        private static void SendToServer(String stacktrace, String filename)
        {
            Console.WriteLine("Sending to server: " + filename);
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
