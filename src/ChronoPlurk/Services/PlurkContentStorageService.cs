using System.Collections.Generic;

namespace ChronoPlurk.Services
{
    public interface IPlurkContentStorageService
    {
        void AddOrReplace(int key, string value);
        string GetValueOrDefault(int key);
        bool Remove(int key);
    }

    public class PlurkContentStorageService : IPlurkContentStorageService
    {
        private readonly Dictionary<int, string> _dictionary = new Dictionary<int, string>();

        public void AddOrReplace(int key, string value)
        {
            if (_dictionary.ContainsKey(key))
            {
                _dictionary[key] = value;
            }
            else
            {
                _dictionary.Add(key, value);
            }
        }

        public string GetValueOrDefault(int key)
        {
            string value;
            if (_dictionary.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        public bool Remove(int key)
        {
            return _dictionary.Remove(key);
        }
    }
}
