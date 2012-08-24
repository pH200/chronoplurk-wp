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

        private const int CacheLength = 200;

        private readonly Dictionary<int, double> _cache = new Dictionary<int, double>();

        private readonly Queue<int> _keyOrderCache = new Queue<int>();

        public void Add(string value, double height)
        {
            var key = value.GetHashCode();
            if (_cache.ContainsKey(key))
            {
                _cache[key] = height;
            }
            else
            {
                _cache.Add(key, height);
                _keyOrderCache.Enqueue(key);
                if (_cache.Count > CacheLength)
                {
                    var removeKey = _keyOrderCache.Dequeue();
                    _cache.Remove(removeKey);
                }
            }
        }

        public bool TryGetValue(string key, out double value)
        {
            return _cache.TryGetValue(key.GetHashCode(), out value);
        }
    }
}
