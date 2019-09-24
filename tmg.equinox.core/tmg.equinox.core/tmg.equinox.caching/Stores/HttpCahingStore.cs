using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using tmg.equinox.caching.Interfaces;

namespace tmg.equinox.caching.Stores
{
    public class HttpCachingStore : IHttpCahingStore
    {
        #region Public/ Protected / Private Member Variables
        private Cache cache;
        private DateTime absoluteExpirationDateTime;
        private TimeSpan slidingExpirationTimeSpan;
        private bool disposed;
        private ICachingObjectSerializer cachingObjectSerializer = new CachingObjectSerializer();
        #endregion Member Variables

        #region Constructor/Dispose
        public HttpCachingStore()
        {
            cache = HttpContext.Current.Cache;
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

                    disposed = true;
        }
        #endregion Constructor/Dispose

        #region Public Methods
       

        public T TryGetValue<T>(string cacheKey)
        {
            var cachedResult = default(T);
            var cacheddata = this.cache.Get(CachingHelper.GetEncryptedCacheKey(cacheKey)) as byte[];

            if (cacheddata != null)
            {
                using (var memoryStream = new MemoryStream(cacheddata))
                {
                    cachedResult = (T)this.cachingObjectSerializer.Deserialize<T>(memoryStream);
                }
            }
            return cachedResult;

        }

        public void AddOrUpdate<T>(string cacheKey, T objectToCache)
        {
            using (var memoryStream = new MemoryStream())
            {
                this.cachingObjectSerializer.Serialize<T>(objectToCache, memoryStream);
                this.cache.Insert(CachingHelper.GetEncryptedCacheKey(cacheKey), memoryStream.ToArray());
            }
        }

        public void AddOrUpdate<T>(string cacheKey, T objectToCache, TimeSpan timeout)
        {
            using (var memoryStream = new MemoryStream())
            {
                this.cachingObjectSerializer.Serialize<T>(objectToCache, memoryStream);
                this.cache.Insert(CachingHelper.GetEncryptedCacheKey(cacheKey), memoryStream.ToArray(),null ,Cache.NoAbsoluteExpiration,timeout);
            }
        }

        public void SetCache(string key, object cachedata, CacheDependency dependencies)
        {
            using (var memoryStream = new MemoryStream()) 
            {
                this.cachingObjectSerializer.Serialize<object>(cachedata, memoryStream);
                this.cache.Insert(CachingHelper.GetEncryptedCacheKey(key), memoryStream.ToArray(), dependencies);
            }           
        }

       
        public bool Exists(string cacheKey)//This is contractual obligaiton. May have implementation in next web cache version
        {
            throw new NotImplementedException();
        }
       
        public bool TryRemove(string cacheKey)
        {

            bool retVal = false;
            try
            {
                this.cache.Remove(cacheKey);
                retVal = true;
            }
            catch
            {

            }
            return retVal;

        }

        public void Clear()
        {
            var itemsInCache = this.cache.GetEnumerator();
            while (itemsInCache.MoveNext())
            {
                cache.Remove(itemsInCache.Key.ToString());
            }
        }
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods

        #region Helper Methods
        #endregion Helper Methods
    }
}
