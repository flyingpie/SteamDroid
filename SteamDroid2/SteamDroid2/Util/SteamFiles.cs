using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Java.Net;
using Android.Graphics;

namespace SteamDroid2
{
	class SteamFiles
	{
		public static String CacheDir = Android.OS.Environment.ExternalStorageDirectory + "/Android/data/SteamDroid2/cache/";
		
		private static SortedList<String, String> cache;
        private static SortedList<String, Bitmap> bitmapCache;
		
		public static void Initialize()
		{
			if(cache == null)
			{
				cache = new SortedList<String, String>();
                bitmapCache = new SortedList<String, Bitmap>();

                Directory.CreateDirectory(CacheDir);
			}
		}
		
		public static void LoadFromWeb(ImageView view, String url, String name)
		{
			Console.WriteLine("CacheDir: " + CacheDir);

			try
			{
                new ImageLoader().Execute(new ImageLoadArguments(view, url, name));
			}
			catch(Exception e)
			{
				Console.WriteLine("Error caching: " + e.Message);
			}
		}
		
		private static void Store(String url, String file)
		{
			
		}

        class ImageLoader : AsyncTask
        {
            private ImageLoadArguments arg;

            protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] parameters)
            {
                Initialize();

                arg = (ImageLoadArguments)parameters[0];

                String cacheUrl = System.IO.Path.Combine(SteamFiles.CacheDir, arg.Name);
                Bitmap bmp;

                if (bitmapCache.ContainsKey(arg.Name))
                {
                    Console.WriteLine("Loading bitmap from memory cache");
                    return bitmapCache[arg.Name];
                }

                if (File.Exists(cacheUrl))
                {
                    Console.WriteLine("Loading bitmap from disk cache");
                    bmp = BitmapFactory.DecodeFile(cacheUrl);
                    return bmp;
                }

                try
                {
                    Console.WriteLine("Downloading image '" + arg.Url);
                    bmp = BitmapFactory.DecodeStream(new Java.Net.URL(arg.Url).OpenStream());

                    Console.WriteLine("Caching image");
                    bitmapCache.Add(arg.Name, bmp);

                    try
                    {
                        bmp.Compress(Bitmap.CompressFormat.Png, 90, File.Open(cacheUrl, FileMode.OpenOrCreate));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error caching image to disk: " + e.Message);
                    }

                    return bmp;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error reading image (" + arg.Url + ") " + e.Message);
                }

                return null;
            }

            protected override void OnPostExecute(Java.Lang.Object result)
            {
                base.OnPostExecute(result);

                if (result != null)
                {
                    arg.View.SetImageBitmap((Bitmap)result);
                }
            }
        }

        class ImageLoadArguments : Java.Lang.Object
        {
            public ImageLoadArguments(ImageView view, String url, String name)
            {
                View = view;
                Url = url;
                Name = name;
            }

            public ImageView View { get; set; }

            public String Url { get; set; }

            public String Name { get; set; }
        }
	}
}

