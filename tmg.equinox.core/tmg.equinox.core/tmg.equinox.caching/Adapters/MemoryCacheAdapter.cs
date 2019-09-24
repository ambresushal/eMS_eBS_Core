using System;
using System.Linq.Expressions;
using System.Runtime.Caching;
using tmg.equinox.caching.Interfaces;

namespace tmg.equinox.caching.Adapters
{
    internal class MemoryCacheAdapter : ICacheProvider
    {
        #region Private Memebers
        private ObjectCache cache;
        private CacheItemPolicy cacheItemPolicy;
        #endregion Private Members

        #region Public Properties
        #endregion Public Properties

        #region Constructor
        public MemoryCacheAdapter(CacheItemPolicy policy)
        {
            cache = MemoryCache.Default;
            cacheItemPolicy = policy;
        }

        public MemoryCacheAdapter(ObjectCache objectCache, CacheItemPolicy policy)
        {
            cache = objectCache;
            cacheItemPolicy = policy;
        }
        #endregion Constructor

        #region Public Methods   
            
        public bool Exists(string cacheKey) {

            return cache.Contains(cacheKey);
        }

        public T Get<T>(string cacheKey) {

            return Exists(cacheKey) ? (T)cache[cacheKey] : default(T);
        }

        public T Add<T>(string cacheKey, T objectToCache) {
            cache.Set(cacheKey,
                            objectToCache,
                            cacheItemPolicy);

            return objectToCache;
        }

        public void Remove(string cacheKey) {
            cache.Remove(cacheKey);
        }

        public bool TryGet<T>(string cacheKey, out T cachedObject) {
            bool result = Exists(cacheKey);
            cachedObject = result ? (T)cache[cacheKey] : default(T);

            return result;
        }

        public bool TryAdd<T>(string cacheKey, Expression<Func<T>> cacheItemProvider, out T cachedObject) {
            bool result = Exists(cacheKey);
            cachedObject = result ? (T)cache[cacheKey] : Add<T>(cacheKey, cacheItemProvider.Compile()());

            return result;
        }
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
