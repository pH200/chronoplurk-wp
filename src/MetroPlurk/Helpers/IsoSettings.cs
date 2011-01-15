using System;
using System.IO.IsolatedStorage;
using System.Linq;

namespace MetroPlurk.Helpers
{
    public static class IsoSettings
    {
        private static readonly object Padlock = new object();

        public static IsolatedStorageSettings Settings { get; private set; }

        static IsoSettings()
        {
            Settings = IsolatedStorageSettings.ApplicationSettings;
        }

        public static void AddOrChange(string key, object value, bool save=false)
        {
            if (Settings.Contains(key))
            {
                Settings[key] = value;
            }
            else
            {
                Settings.Add(key, value);
            }
            if (save)
            {
                SaveAsync();
            }
        }
        
        public static bool TryGetValue<T>(string key, out T value)
        {
            return Settings.TryGetValue(key, out value);
        }

        public static void ClearAll()
        {
            Settings.Clear();
            Save();
        }

        public static void SaveAsync()
        {
            Observable.ToAsync(Save)().Subscribe();
        }

        private static void Save()
        {
            lock (Padlock)
            {
                Settings.Save();
            }
        }
    }
}
