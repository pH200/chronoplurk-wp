using System;
using System.Collections.Generic;

namespace ChronoPlurk.Views.PlurkControls
{
    public sealed class RecommendHeightCache
    {
        public static RecommendHeightCache Instance { get; private set; }

        private RecommendHeightCache()
        {
        }

        static RecommendHeightCache()
        {
            // Singleton evil, right.
            Instance = new RecommendHeightCache();
        }

        private const int CacheLength = 400;

        private readonly Dictionary<int, double> _cache = new Dictionary<int, double>(CacheLength + 1);

        private readonly LinkedList<int> _keyOrderCache = new LinkedList<int>();

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
                _keyOrderCache.AddLast(key);
                RemoveExceededCaches();
            }
        }

        public bool TryGetValue(string str, out double value)
        {
            var key = str.GetHashCode();
            var node = _keyOrderCache.Find(key);
            if (node != null)
            {
                _keyOrderCache.Remove(node);
                _keyOrderCache.AddLast(node);
            }
            else
            {
                _keyOrderCache.AddLast(key);
            }
            RemoveExceededCaches();
            return _cache.TryGetValue(key, out value);
        }

        private void RemoveExceededCaches()
        {
            if (_keyOrderCache.Count > CacheLength)
            {
                var first = _keyOrderCache.First;
                _cache.Remove(first.Value);
                _keyOrderCache.RemoveFirst();
            }
        }
    }
}
