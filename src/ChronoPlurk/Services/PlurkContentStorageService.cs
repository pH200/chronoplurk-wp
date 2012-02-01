using System.Collections.Generic;

namespace ChronoPlurk.Services
{
    public interface IPlurkContentStorageService
    {
        void AddOrReplace(long key, string value);
        string GetValueOrDefault(long key);
        bool Remove(long key);
    }

    public class PlurkContentStorageService : IPlurkContentStorageService
    {
        private readonly Dictionary<long, string> _dictionary = new Dictionary<long, string>();

        public void AddOrReplace(long key, string value)
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

        public string GetValueOrDefault(long key)
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

        public bool Remove(long key)
        {
            return _dictionary.Remove(key);
        }
    }
}
