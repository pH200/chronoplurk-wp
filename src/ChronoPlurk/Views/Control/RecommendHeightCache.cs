using System;
using System.Collections.Generic;

namespace ChronoPlurk.Views.PlurkControls
{
    public class RecommendHeightCache
    {
        public static RecommendHeightCache Instance { get; private set; }

        static RecommendHeightCache()
        {
            // Singleton evil, right.
            Instance = new RecommendHeightCache();
        }

        private const int CacheLength = 500;

        private readonly Dictionary<int, double> _cache = new Dictionary<int, double>(CacheLength + 1);

        private readonly List<int> _keyOrderCache = new List<int>(CacheLength + 1);

        public void Add(string str, double height)
        {
            var key = str.GetHashCode();
            if (_cache.ContainsKey(key))
            {
                _cache[key] = height;
            }
            else
            {
                _cache.Add(key, height);
                _keyOrderCache.Add(key);
                if (_keyOrderCache.Count > CacheLength)
                {
                    var removeKey = _keyOrderCache[0];
                    _cache.Remove(removeKey);
                    _keyOrderCache.RemoveAt(0);
                }
            }
        }

        public bool TryGetValue(string str, out double value)
        {
            var key = str.GetHashCode();
            _keyOrderCache.Remove(key);
            _keyOrderCache.Add(key);
            return _cache.TryGetValue(key, out value);
        }
    }
}
