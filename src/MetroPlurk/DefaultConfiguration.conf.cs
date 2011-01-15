using System;
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
            PlurkResources.Username = "";
            PlurkResources.Password = "";
            #endif
            
            // Set API key.
            // Plurto.Config.ApiKey = "";
        }
    }
}
