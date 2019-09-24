using System;
using Glav.CacheAdapter.Core;
using Glav.CacheAdapter.Core.DependencyInjection;
using tmg.equinox.core.logging.Logging;

namespace tmg.equinox.caching.Adapters
{
    public class GlavCacheWrapper
    {
        private static readonly ICacheProvider cacheProvider = AppServices.Cache;
        private static readonly ILog _logger = LogProvider.For<GlavCacheWrapper>();


        public static T Get<T>(string key) where T : class
        {
            try
            {
                var jsonData = cacheProvider.InnerCache.Get<T>(key);
                return jsonData;
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("CachingGet-{0}", key), ex, key);
                return null;
            }
        }
        public static void Add<T>(string key, T data)
        {
            try
            {
                cacheProvider.InvalidateCacheItem(key);
                cacheProvider.Add(key, DateTime.Now.AddDays(1), data);
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("CachingAdd-{0}", key), ex, key);
            }
        }

        public static void Add<T>(string key, DateTime Expiration, T data)
        {
            try
            {
                cacheProvider.InvalidateCacheItem(key);
                cacheProvider.Add(key, Expiration, data);
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("CachingAdd-{0}", key), ex, key);
            }
        }

        public static bool Remove(string key)
        {
            cacheProvider.InvalidateCacheItem(key);
            return true;
        }

        public static void RemoveAll(string key)
        {
            try
            {
                cacheProvider.ClearAll();
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("CachingRemove-{0}", key), ex, key);
            }
        }
    }
}