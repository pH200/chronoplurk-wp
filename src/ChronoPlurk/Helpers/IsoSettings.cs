using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Polenter.Serialization;
using Polenter.Serialization.Core;

namespace ChronoPlurk.Helpers
{
    public static class IsoSettings
    {
        private static readonly object Padlock = new object();

        public static IsolatedStorageSettings Settings { get; private set; }

        static IsoSettings()
        {
            Settings = IsolatedStorageSettings.ApplicationSettings;
        }

        public static void SerializeStore(object data, string filename)
        {
            lock (Padlock)
            {
                using (var appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (var file = appStorage.FileExists(filename) 
                        ? appStorage.OpenFile(filename, FileMode.Truncate)
                        : appStorage.CreateFile(filename))
                    {
                        var sharpSerializer = new SharpSerializer(true);
                        sharpSerializer.Serialize(data, file);
                    }
                }
            }
        }

        public static object DeserializeLoad(string filename)
        {
            lock (Padlock)
            {
                using (var appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (appStorage.FileExists(filename))
                    {
                        using (var file = appStorage.OpenFile(filename, FileMode.Open, FileAccess.Read))
                        {
                            var sharpSerializer = new SharpSerializer(true);
                            try
                            {
                                return sharpSerializer.Deserialize(file);
                            }
#if DEBUG
                            catch (DeserializingException e)
                            {
                                ThreadEx.OnUIThread(() => MessageBox.Show(e.ToString(), "DEBUG MODE", MessageBoxButton.OK));
                                return null;
                            }
#else
                            catch (DeserializingException)
                            {
                                return null;
                            }
#endif
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
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
            lock (Padlock)
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    store.Remove();
                }
            }
            Settings.Clear();
            Save();
        }

        public static void SaveAsync()
        {
            Observable.ToAsync(Save)().IgnoreAllExceptions().Subscribe();
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
