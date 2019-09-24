using System;

namespace tmg.equinox.caching.Interfaces
{
    public interface ICacheProvider
    {
        T Add<T>(string cacheKey, T objectToCache);
        bool Exists(string cacheKey);
        T Get<T>(string cacheKey);
        void Remove(string cacheKey);
        bool TryAdd<T>(string cacheKey, System.Linq.Expressions.Expression<Func<T>> cacheItemProvider, out T cachedObject);
        bool TryGet<T>(string cacheKey, out T cachedObject);
    }
}
