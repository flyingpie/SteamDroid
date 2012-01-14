using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Android.Graphics;
using Android.OS;
using Android.Util;

namespace SteamDroid
{
    /// <summary>
    /// SteamFiles handles loading and caching of images
    /// </summary>
    class SteamFiles
    {
        public static String CacheDir = Android.OS.Environment.ExternalStorageDirectory + "/Android/data/SteamDroid2/cache/";
        
        private static SortedList<String, Bitmap> bitmapCache;
        
        public static void Initialize()
        {
            if(bitmapCache == null)
            {
                bitmapCache = new SortedList<String, Bitmap>();

                Directory.CreateDirectory(CacheDir);
            }
        }

        /// <summary>
        /// Loads an image from the web, or cache if available
        /// </summary>
        /// <param name="key">A unique image key, used for caching</param>
        /// <param name="url">The url of the image, if downloading is required</param>
        /// <param name="result">The callback handler that is called when the image is loaded</param>
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

        /// <summary>
        /// Cache the image to memory and disk
        /// </summary>
        /// <param name="key"></param>
        /// <param name="bitmap"></param>
        private static void CacheImage(String key, Bitmap bitmap)
        {
            try
            {
                // Cache the image to memory
                bitmapCache.Add(key, bitmap);

                // Combine and encode the path
                String path = EncodePath(System.IO.Path.Combine(CacheDir, key));

                // Open a stream and write the bitmap
                FileStream stream = File.Open(path, FileMode.OpenOrCreate);
                bitmap.Compress(Bitmap.CompressFormat.Png, 90, stream);
                stream.Close();
            }
            catch (Exception e)
            {
                Log.Error("SteamFiles", "Error caching image: " + e.Message);
            }
        }

        /// <summary>
        /// Loads the image from cache, if it exists
        /// </summary>
        /// <param name="key"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private static bool LoadImageFromCache(String key, out Bitmap bitmap)
        {
            try
            {
                // Try the memory cache first
                if (bitmapCache.ContainsKey(key))
                {
                    bitmap = bitmapCache[key];
                    return true;
                }
                
                // Combine and encode the cache path
                String path = EncodePath(System.IO.Path.Combine(CacheDir, key));

                // Check if the path exists
                if (File.Exists(path))
                {
                    // Check if the cached image is not empty
                    FileInfo info = new FileInfo(path);

                    // If the image is not empty, load it
                    if (info.Length > 0)
                    {
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
                Log.Error("SteamFiles", "Error loading image from cache: " + e.Message);
            }

            bitmap = null;
            return false;
        }

        /// <summary>
        /// Removes any invalid characters from the cache path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static String EncodePath(String path)
        {
            return path.Replace(":", "_");
        }

        /// <summary>
        /// The image loader provides an asynchronous interface for retrieving images from the web
        /// </summary>
        class ImageLoader : AsyncTask<ImageLoadArguments, int, Bitmap>
        {
            private const int MAX_THREADS = 8;

            private static Queue<ImageLoadArguments> queue;
            private static int running;

            /// <summary>
            /// Loads an image based on the specified load arguments
            /// </summary>
            /// <param name="args"></param>
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
                        return;
                    }
                }

                queue.Enqueue(args);

                Start();
            }

            /// <summary>
            /// If the maximum thread count hasn't been reached, start another thread
            /// </summary>
            private static void Start()
            {
                if ((running < MAX_THREADS) && queue.Count > 0)
                {
                    new ImageLoader().Execute(new ImageLoadArguments[] { queue.Dequeue() });
                    running++;
                }
            }

            private ImageLoadArguments arg;

            /// <summary>
            /// Downloads the image
            /// </summary>
            /// <param name="args"></param>
            /// <returns></returns>
            protected override Bitmap RunInBackground(params ImageLoadArguments[] args)
            {
                Initialize();

                arg = args[0];

                try
                {
                    // Retrieve the image
                    WebClient web = new WebClient();
                    byte[] data = web.DownloadData(arg.Url);
                    Bitmap bitmap = BitmapFactory.DecodeByteArray(data, 0, data.Length);

                    // Cache the downloaded image
                    CacheImage(arg.Key, bitmap);

                    return bitmap;
                }
                catch (Exception e)
                {
                    Log.Error("SteamFiles", "Error loading image: " + e.Message);
                }

                return null;
            }

            /// <summary>
            /// Calls the callback handler if the image is successfully downloaded
            /// </summary>
            /// <param name="result"></param>
            protected override void OnPostExecute(Bitmap result)
            {
                base.OnPostExecute(result);

                if (result != null)
                {
                    arg.Result(result);
                }

                running--;

                // Start a new downloader if neccessary
                Start();
            }
        }

        /// <summary>
        /// Image load arguments, contains parameters for loading an image, either from cache or from the web
        /// </summary>
        class ImageLoadArguments : Java.Lang.Object
        {
            public ImageLoadArguments(String key, String url, Func<Bitmap, Object> result)
            {
                Key = key;
                Url = url;
                Result = result;
            }

            /// <summary>
            /// The key, used for caching
            /// </summary>
            public String Key { get; set; }

            /// <summary>
            /// The url, if the image needs to be downloaded from the web
            /// </summary>
            public String Url { get; set; }

            /// <summary>
            /// The function that is called when the image is loaded
            /// </summary>
            public Func<Bitmap, Object> Result { get; set; }
        }
    }
}

