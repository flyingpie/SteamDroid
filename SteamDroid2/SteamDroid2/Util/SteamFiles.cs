using System;
using System.Collections.Generic;
using System.IO;
using Android.OS;
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

        public static void LoadFromWeb(String key, String url, Func<Bitmap, Object> result)
        {
            Initialize();

            // If the image is cached in memory, use it from there
            if (bitmapCache.ContainsKey(key))
            {
                result(bitmapCache[key]);
                return;
            }

            // Image not yet loaded, do so now
            ImageLoader.Load(new ImageLoadArguments(key, url, result));
        }

        private static void CacheImage(String key, Bitmap bitmap)
        {
            try
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 90, File.Open(System.IO.Path.Combine(CacheDir, key), FileMode.OpenOrCreate));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error caching image: " + e.Message);
            }
        }

        class ImageLoader : AsyncTask<ImageLoadArguments, int, Bitmap>
        {
            private const int MAX_THREADS = 8;

            private static Queue<ImageLoadArguments> queue;
            private static int running;

            public static void Load(ImageLoadArguments args)
            {
                if (queue == null)
                {
                    queue = new Queue<ImageLoadArguments>();
                    running = 0;
                }

                Console.WriteLine("Enqueued image (" + queue.Count + ") threads: " + running);
                queue.Enqueue(args);

                Start();
            }

            private static void Start()
            {
                if ((running < MAX_THREADS) && queue.Count > 0)
                {
                    new ImageLoader().Execute(new ImageLoadArguments[] { queue.Dequeue() });
                    running++;
                }
            }

            private ImageLoadArguments arg;

            protected override Bitmap RunInBackground(params ImageLoadArguments[] args)
            {
                Initialize();

                arg = args[0];

                // If the image is cached on disk, use it from there
                String file = System.IO.Path.Combine(CacheDir, arg.Key);
                if (File.Exists(file))
                {
                    Bitmap bmp = BitmapFactory.DecodeFile(file);
                    bitmapCache.Add(arg.Key, bmp);
                    return bmp;
                }

                // Image not yet downloaded, do so now
                try
                {
                    Bitmap bmp = BitmapFactory.DecodeStream(new Java.Net.URL(arg.Url).OpenStream());
                    bitmapCache.Add(arg.Key, bmp);
                    CacheImage(arg.Key, bmp);
                    return bmp;
                }
                catch(Exception)
                {
                    //Console.WriteLine("Error retrieving image '" + arg.Url + "': " + e.Message);
                }

                return null;
            }

            protected override void OnPostExecute(Bitmap result)
            {
                base.OnPostExecute(result);

                if (result != null)
                {
                    arg.Result(result);
                }

                running--;

                Start();
            }
        }

        class ImageLoadArguments : Java.Lang.Object
        {
            public ImageLoadArguments(String key, String url, Func<Bitmap, Object> result)
            {
                Key = key;
                Url = url;
                Result = result;
            }

            public String Key { get; set; }

            public String Url { get; set; }

            public Func<Bitmap, Object> Result { get; set; }
        }
    }
}

