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

            // If the image is cached in memory or file, use it from there
            Bitmap bitmap;
            if (LoadImageFromCache(key, out bitmap))
            {
                result(bitmap);
                return;
            }

            // Image not yet loaded, do so now
            ImageLoader.Load(new ImageLoadArguments(key, url, result));
        }

        private static void CacheImage(String key, Bitmap bitmap)
        {
            try
            {
                String path = EncodePath(System.IO.Path.Combine(CacheDir, key));
                Console.WriteLine("Caching image " + key + " to " + path);
                FileStream stream = File.Open(path, FileMode.OpenOrCreate);
                bitmap.Compress(Bitmap.CompressFormat.Png, 90, stream);
                stream.Close();

                bitmapCache.Add(key, bitmap);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error caching image: " + e.Message);
            }
        }

        private static bool LoadImageFromCache(String key, out Bitmap bitmap)
        {
            try
            {
                if (bitmapCache.ContainsKey(key))
                {
                    Console.WriteLine("Loading image from memory cache");
                    bitmap = bitmapCache[key];
                    return true;
                }
                
                String path = EncodePath(System.IO.Path.Combine(CacheDir, key));

                if (File.Exists(path))
                {
                    FileInfo info = new FileInfo(path);

                    if (info.Length > 0)
                    {
                        Console.WriteLine("Loading image from file cache");
                        bitmap = BitmapFactory.DecodeFile(path);
                        return true;
                    }
                    else
                    {
                        File.Delete(path);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading image from cache: " + e.Message);
            }

            bitmap = null;
            return false;
        }

        private static String EncodePath(String path)
        {
            return path.Replace(":", "_");
        }

        class ImageLoader : AsyncTask<ImageLoadArguments, int, Bitmap>
        {
            private const int MAX_THREADS = 1;

            private static Queue<ImageLoadArguments> queue;
            private static int running;

            public static void Load(ImageLoadArguments args)
            {
                if (queue == null)
                {
                    queue = new Queue<ImageLoadArguments>();
                    running = 0;
                }

                foreach (ImageLoadArguments currentArg in queue)
                {
                    if (currentArg.Key == args.Key)
                    {
                        Console.WriteLine("Image already in download queue");
                        return;
                    }
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

                try
                {
                    Bitmap bitmap = BitmapFactory.DecodeStream(new Java.Net.URL(arg.Url).OpenStream());

                    CacheImage(arg.Key, bitmap);

                    return bitmap;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error loading image: " + e.Message);
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

