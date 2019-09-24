using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.caching.Interfaces
{
    public interface ICachingStoreAsync:ICachingStore
    {
        Task<T> TryGetValueAysnc<T>(string cacheKey);
        Task AddOrUpdateAysnc<T>(string cacheKey, T objectToCache, TimeSpan timeout);
    }
}
