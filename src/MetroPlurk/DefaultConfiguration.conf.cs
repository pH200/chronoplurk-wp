using System;
using System.IO.IsolatedStorage;
using MetroPlurk.Helpers;

namespace MetroPlurk
{
    public static class DefaultConfiguration
    {
        /**
         * Replace these values with your own connection strings.
         */
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
            IsolatedStorageSettings.ApplicationSettings.Clear();
#endif

            // Set API key.
            // Plurto.Config.ApiKey = "";
        }
    }
}
