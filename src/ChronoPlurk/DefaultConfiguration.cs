using System;
using System.IO.IsolatedStorage;
using ChronoPlurk.Helpers;

namespace ChronoPlurk
{
    public static partial class DefaultConfiguration
    {
        /**
         * Replace these values with your own connection strings.
         */
        public static string OAuthConsumerKey;

        public static string OAuthConsumerSecret;

        // Network.http.keep-alive
        public static TimeSpan TimeoutTimeline = TimeSpan.FromSeconds(115);

        public static TimeSpan TimeoutCompose = TimeSpan.FromSeconds(115);

        public static TimeSpan TimeoutUpload = TimeSpan.FromSeconds(115);

        public static int RetryCount = 3;

        public static int ResizeWidth = 800; // This is the maximum value for uploaded photos on Plurk.
        
        public static int ResizeHeight = 800;

        public static int CachedItemsCount = 10;

        public const string VersionText = "1.7.4";

        /// <summary>
        /// Initialize connection settings.
        /// </summary>
        public static void Initialize()
        {
#if CLEAN_DEBUG
            //PlurkResources.Username = DebugUsername;
            //PlurkResources.Password = DebugPassword;
            // Clear settings for debugging
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    store.Remove();
                }
                IsolatedStorageSettings.ApplicationSettings.Clear();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
#endif
#if DEBUG
            //Plurto.Config.LoggingLevel = LoggingLevel.Verbose;
            Plurto.Config.LoggingLinebreak = 80;
#endif
        }
    }
}
