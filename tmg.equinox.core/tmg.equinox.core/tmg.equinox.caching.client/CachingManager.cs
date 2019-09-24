using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.caching.Interfaces;
using tmg.equinox.caching.Stores;
using System.Runtime.Caching;
using System.Security.Claims;
using tmg.equinox.caching.client.Properties;

namespace tmg.equinox.caching.client
{
    /// <summary>
    /// provides a facade approach for exposing all caching managament functions to outside world
    /// </summary>
    public class CachingManager
    {
        #region Public/ Protected / Private Member Variables
        static ICachingStoreFactory iCachingStoreFactory = null;
        public static ICachingStore appFabricCachingStore = null;
        public static ICachingStore httpCachingStore = null;
        public static ICachingStore memoryCachingStore = null;
        public static CacheItemPolicy policy;
        public static CachingSetting cachingSetting;
        private static ICachingStore _cache = null;

        #endregion Member Variables

        #region Constructor/Dispose
        static CachingManager()
        {
            CacheConfig.RegisterCacheConfiguration();
            policy = new CacheItemPolicy();
            cachingSetting = new CachingSetting(policy, CacheConfig.CacheName, CacheConfig.CacheRegion);
        }
        #endregion Constructor/Dispose



        #region Public Methods
        public static void InitializeCache()
        {
            appFabricCachingStore = GetAppFabricCachingStore(cachingSetting);
            httpCachingStore = GetHttpCachingStore(cachingSetting);
            memoryCachingStore = GetMemoryCachingStore(cachingSetting);
        }
        public static void InitializeMemoryCache()
        {
            memoryCachingStore = GetMemoryCachingStore(cachingSetting);
        }

        /// <summary>
        /// Add claims of user in cache
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objectToCache"></param>
        /// <returns></returns>
        public static bool SetClaimsCache(string key, List<Claim> objectToCache)
        {
            bool ret = false;
            if (CacheConfig.EnableClaimsCaching)
            {

                switch (CacheConfig.CacheToUseForClaims)
                {
                    case CachingStoreType.AppFabricCache:
                        appFabricCachingStore.AddOrUpdate(key, objectToCache, CacheConfig.CacheExpirationTimeout);
                        break;
                    case CachingStoreType.HttpCache:
                        httpCachingStore.AddOrUpdate(key, objectToCache, CacheConfig.CacheExpirationTimeout);
                        break;
                    case CachingStoreType.MemoryCache:
                        memoryCachingStore.AddOrUpdate(key, objectToCache, CacheConfig.CacheExpirationTimeout);
                        break;
                }
                ret = true;
            }
            return ret;
        }
        /// <summary>
        /// Get Claims from cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="claims"></param>
        /// <returns></returns>

        public static bool GetCachedClaims(string key, out List<Claim> claims)
        {

            bool result = false;
            claims = null;
            if (CacheConfig.EnableClaimsCaching)
            {
                switch (CacheConfig.CacheToUseForClaims)
                {
                    case CachingStoreType.AppFabricCache:
                        claims = appFabricCachingStore.TryGetValue<List<Claim>>(key);
                        break;
                    case CachingStoreType.HttpCache:
                        claims = httpCachingStore.TryGetValue<List<Claim>>(key);
                        break;
                    case CachingStoreType.MemoryCache:
                        claims = memoryCachingStore.TryGetValue<List<Claim>>(key);
                        break;
                }
                result = true;
            }
            return result;
        }
        /// <summary>
        /// Remove Entries from cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool RemoveClaimsFromCache(string key)
        {
            bool returnValue = false;
            if (CacheConfig.EnableClaimsCaching)
            {
                switch (CacheConfig.CacheToUseForClaims)
                {
                    case CachingStoreType.AppFabricCache:
                        returnValue = appFabricCachingStore.TryRemove(key);
                        break;
                    case CachingStoreType.HttpCache:
                        returnValue = httpCachingStore.TryRemove(key);
                        break;
                    case CachingStoreType.MemoryCache:
                        returnValue = memoryCachingStore.TryRemove(key);
                        break;
                }
                returnValue = true;
            }
            return returnValue;
        }

        #endregion Public Methods

        #region Private Methods
        private static AppFabricCachingStore GetAppFabricCachingStore(CachingSetting cachingSetting)
        {
            iCachingStoreFactory = new AppFabricCachingStoreFactory();

            return (AppFabricCachingStore)iCachingStoreFactory.CreateCachingStore(cachingSetting);

        }

        private static HttpCachingStore GetHttpCachingStore(CachingSetting cachingSetting)
        {
            iCachingStoreFactory = new HttpCachingStoreFactory();

            return (HttpCachingStore)iCachingStoreFactory.CreateCachingStore(cachingSetting);
        }
        private static MemoryCachingStore GetMemoryCachingStore(CachingSetting cachingSetting)
        {
            iCachingStoreFactory = new MemoryCachingStoreFactory();

            return (MemoryCachingStore)iCachingStoreFactory.CreateCachingStore(cachingSetting);
        }

        #endregion Private Methods

        #region Helper Methods
        #endregion Helper Methods

    }
}

