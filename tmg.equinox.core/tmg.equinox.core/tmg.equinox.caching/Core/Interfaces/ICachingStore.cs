using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.caching.Interfaces
{
    public interface ICachingStore : IDisposable
    {
        T TryGetValue<T>(string cacheKey);       
        void AddOrUpdate<T>(string cacheKey, T objectToCache);
        void AddOrUpdate<T>(string cacheKey, T objectToCache, TimeSpan timeout);        
        bool TryRemove(string cacheKey);
        bool Exists(string cacheKey);
        void Clear();        
    }
}
  