using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.caching.Interfaces;

namespace tmg.equinox.caching.Stores
{
    public class MemoryCachingStore : ICachingStore
    {
        #region Public/ Protected / Private Member Variables
        private ObjectCache cache;
        private CacheItemPolicy cacheItemPolicy;
        private bool disposed;
        private ICachingObjectSerializer cachingObjectSerializer = new CachingObjectSerializer();
        private string cacheRegion = "CacheClient";//"MemoryCache";
        #endregion Member Variables

        #region Constructor/Dispose

        public MemoryCachingStore(CacheItemPolicy policy)
        {
            cache = MemoryCache.Default;
            cacheItemPolicy = policy;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
                if (disposing)
                    cachingObjectSerializer = null;
            disposed = true;
        }
        #endregion Constructor/Dispose


        #region Public Methods

        public T TryGetValue<T>(string cacheKey)
        {
            return Exists(cacheKey) ? (T)cache[cacheKey] : default(T);
        }

        public bool TryGet<T>(string cacheKey, out T cachedObject)
        {
            bool result = Exists(cacheKey);
            cachedObject = result ? (T)cache[cacheKey] : default(T);

            return result;
        }

        //Get Object from Region
        public bool TryGet<T>(string cacheKey, out T cachedObject, string region)
        {
            bool result = Exists(cacheKey, region);
            // cachedObject = result ? (T)cache.Get(cacheKey, region) : default(T);
            cachedObject = result ? (T)cache.Get(CreateKeyWithRegion(cacheKey, region)) : default(T);

            return result;
        }

        public void AddOrUpdate<T>(string cacheKey, T cachedObject)
        {
            bool result = Exists(cacheKey);
            cachedObject = result ? (T)cache[cacheKey] : Add<T>(cacheKey, cachedObject);
        }

        public void AddOrUpdate<T>(string cacheKey, T objectToCache, TimeSpan timeout)
        {
            DateTimeOffset dateTimeOffset = new DateTimeOffset(DateTime.UtcNow, timeout);

            using (var memoryStream = new MemoryStream())
            {
                this.cachingObjectSerializer.Serialize<T>(objectToCache, memoryStream);
                this.cache.Set(CachingHelper.GetEncryptedCacheKey(cacheKey), memoryStream.ToArray(), dateTimeOffset);
            }
        }

        //Add Or Update Object from Region 
        public void AddOrUpdate<T>(string cacheKey, T objectToCache, TimeSpan timeout, string cacheRegion)
        {
            DateTime dt = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, timeout.Hours, timeout.Minutes, timeout.Seconds);
            DateTimeOffset dateTimeOffset = new DateTimeOffset(dt, TimeZoneInfo.Local.GetUtcOffset(dt));

            // DateTimeOffset dateTimeOffset = new DateTimeOffset(DateTime.UtcNow, timeout);
            using (var memoryStream = new MemoryStream())
            {
                this.cachingObjectSerializer.Serialize<T>(objectToCache, memoryStream);
                var key = CreateKeyWithRegion(cacheKey, cacheRegion);
                this.cache.Set(key, memoryStream.ToArray(), cacheItemPolicy);
            }
        }

        public bool TryRemove(string cacheKey)
        {
            bool retVal = false;

            bool result = Exists(cacheKey);

            if (result)
            {
                this.Remove(cacheKey);
                retVal = true;
            }
            return retVal;
        }

        //Remove Object from Region 
        public bool TryRemove(string cacheKey, string RegionName)
        {
            bool retVal = false;

            bool result = Exists(cacheKey, RegionName);

            if (result)
            {
                this.cache.Remove(cacheKey, RegionName);
                retVal = true;
            }
            return retVal;
        }

        public bool Exists(string cacheKey)
        {
            return this.cache.Contains(cacheKey);
        }
        public bool Exists(string cacheKey, string RegionName)
        {
            var result = false;
            var key = CreateKeyWithRegion(cacheKey, RegionName);
            result = this.cache.Contains(key);
            return result;
        }

        public void Clear()
        {
            // this.cacheRegion.Remove();
        }
        #endregion Public Methods

        #region Private Methods
        private string CreateKeyWithRegion(string cachekey, string region)
        {
            var key = CachingHelper.GetEncryptedCacheKey(cachekey);
            return "region:" + (region == null ? "null_region" : region) + ";key=" + key;
        }
        private T Add<T>(string cacheKey, T objectToCache)
        {
            cache.Set(cacheKey, objectToCache, cacheItemPolicy);
            return objectToCache;
        }

        private void Remove(string cacheKey)
        {
            cache.Remove(cacheKey);
        }
        #endregion Private Methods

        #region Helper Methods
        #endregion Helper Methods
    }
}