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
        public static string ApiKey = "";

        public static string DebugUsername = "";

        public static string DebugPassword = "";

        /// <summary>
        /// Initialize connection settings.
        /// </summary>
        public static void Initialize()
        {
#if CLEAN_DEBUG
            PlurkResources.Username = DebugUsername;
            PlurkResources.Password = DebugPassword;
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
