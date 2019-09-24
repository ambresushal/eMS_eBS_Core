using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.ApplicationServer.Caching;
using tmg.equinox.caching.Interfaces;
using System.Security.Cryptography;

namespace tmg.equinox.caching.Stores
{
	public class AppFabricCachingStore:IAppFabricCachingStore
	{
		
		#region Public/ Protected / Private Member Variables
        private bool disposed;
		private readonly DataCache cache;
		private  string cacheRegion = "CacheClient";
        private ICachingObjectSerializer cachingObjectSerializer = new CachingObjectSerializer();
        private DataCacheFactory dataCacheFactory = new DataCacheFactory();
		#endregion

		#region Constructor /Dispose
        public AppFabricCachingStore()           
		{
            cache = dataCacheFactory.GetDefaultCache();
            cache.CreateRegion(cacheRegion);
		}        
        
        public AppFabricCachingStore(string cacheName,string cacheRegionName)
        {
            cacheRegion = cacheRegionName;
            cache = dataCacheFactory.GetCache(cacheName);
            cache.CreateRegion(cacheRegion);            
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
                    dataCacheFactory.Dispose();
            disposed = true;
        }
        #endregion Constructor/Dispose

        #region Public Methods
        public async Task<T> TryGetValueAysnc<T>(string cacheKey)
        {                         
            var cachedResult = default(T);
            var cacheddata = this.cache.Get(CachingHelper.GetEncryptedCacheKey(cacheKey), cacheRegion) as byte[];

            if (cacheddata != null)
            {
                using (var memoryStream = new MemoryStream(cacheddata))
                {                   
                    cachedResult = await this.cachingObjectSerializer.DeserializeAsync<T>(memoryStream);
                }
            }
            return cachedResult;
        }
        public T TryGetValue<T>(string cacheKey)
        {
            var cachedResult = default(T);
            var cacheddata = this.cache.Get(CachingHelper.GetEncryptedCacheKey(cacheKey), cacheRegion) as byte[];

            if (cacheddata != null)
            {
                using (var memoryStream = new MemoryStream(cacheddata))
                {
                    cachedResult = (T)this.cachingObjectSerializer.Deserialize<T>(memoryStream);
                }
            }
            return cachedResult;

        }
        /// <summary>
        /// Commented Add method and Used Put 
        /// Put method overwrite/update content if Key exist in cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="objectToCache"></param>
        public void AddOrUpdate<T>(string cacheKey, T objectToCache)
        {
            using (var memoryStream = new MemoryStream())
            {           
                 this.cachingObjectSerializer.Serialize<T>(objectToCache, memoryStream);
                 this.cache.Put(CachingHelper.GetEncryptedCacheKey(cacheKey), memoryStream.ToArray(), cacheRegion);
                //this.cache.Add(cacheKey, memoryStream.ToArray(), CacheRegion);
            }
        }
        /// <summary>
        /// Overloaded method with extra param - timeout
        /// By SK
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="objectToCache"></param>
        /// <param name="timeout"></param>
        public async Task AddOrUpdateAysnc<T>(string cacheKey, T objectToCache, TimeSpan timeout)
        {
            using (var memoryStream = new MemoryStream())
            {
                await this.cachingObjectSerializer.SerializeAsync<T>(objectToCache, memoryStream);
                this.cache.Put(CachingHelper.GetEncryptedCacheKey(cacheKey), memoryStream.ToArray(), timeout, cacheRegion);               
            }
        }

        public void AddOrUpdate<T>(string cacheKey, T objectToCache, TimeSpan timeout)
        {
            using (var memoryStream = new MemoryStream())
            {
                this.cachingObjectSerializer.Serialize<T>(objectToCache, memoryStream);
                this.cache.Put(CachingHelper.GetEncryptedCacheKey(cacheKey), memoryStream.ToArray(), timeout, cacheRegion);
            }
        }
        public bool TryRemove(string cacheKey)
        {
            bool retVal = false;

            retVal = this.cache.Remove(CachingHelper.GetEncryptedCacheKey(cacheKey), cacheRegion);

            return retVal;
        }

        public void Clear()
        {
            this.cache.ClearRegion(cacheRegion);
        }
        public bool Exists(string cacheKey)//This is contractual obligaiton. May have implementation in next appfabric version
        {
            throw new NotImplementedException();
        }
        public void InvalidateCacheItem(string key)
        {
            key = CachingHelper.GetEncryptedCacheKey(key);

            DataCacheItem item = this.cache.GetCacheItem(key);
            this.cache.Remove(key);

            foreach (var tag in item.Tags)
            {
                this.cache.Remove(key, tag.ToString());
            }
        }
        public void InvalidateSets(IEnumerable<string> entitySets)
        {
            // Go through the list of objects in each of the set. 
            foreach (var set in entitySets)
            {
                foreach (var val in this.cache.GetObjectsInRegion(set))
                {
                    this.cache.Remove(val.Key);
                }
            }
        }

        public void CreateRegion(string regionName)
        {
            try
            {
                this.cache.CreateRegion(regionName);
            }
            catch (DataCacheException de)
            {
                if (de.ErrorCode != DataCacheErrorCode.RegionAlreadyExists)
                {
                    throw;
                }
            }
        }      

		#endregion Public Methods

		#region Private Methods
       
		#endregion Private Methods

		#region Helper Methods
		#endregion

	}
}
