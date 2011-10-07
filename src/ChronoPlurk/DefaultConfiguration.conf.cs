using System;
using System.IO.IsolatedStorage;
using ChronoPlurk.Helpers;
using Plurto.Core;

namespace ChronoPlurk
{
    public static class DefaultConfiguration
    {
        /**
         * Replace these values with your own connection strings.
         */

        public static string ApiKey =
""; // %%omit apikey%%

        /// <summary>
        /// Initialize connection settings.
        /// </summary>
        public static void Initialize()
        {
#if CLEAN_DEBUG
            PlurkResources.Username =
""; // %%omit username%%
            PlurkResources.Password =
""; // %%omit password%%

            // Clear settings for debugging
            try
            {
                IsolatedStorageSettings.ApplicationSettings.Clear();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
#endif
#if DEBUG
            Plurto.Config.LoggingLevel = LoggingLevel.Verbose;
            Plurto.Config.LoggingLinebreak = 80;
#endif
        }
    }
}
