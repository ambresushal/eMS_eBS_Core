using System;
using System.Runtime.Caching;
using System.Web.Caching;

namespace tmg.equinox.caching.Interfaces
{
    interface ICacheProviderFactory : IDisposable
    {
        ICacheProvider GetDataCacheService(TimeSpan timeOut);
        ICacheProvider GetHttpCacheService(Cache cache, DateTime absoluteExpiration, TimeSpan slidingExpiration);
        ICacheProvider GetMemoryCacheService(CacheItemPolicy policy);
    }
}
