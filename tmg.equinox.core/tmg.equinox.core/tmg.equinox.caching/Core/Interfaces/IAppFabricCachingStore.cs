using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.caching.Interfaces
{
    /// <summary>
    /// Wrapper interface providing  more specific contracts for appfabric caching
    /// </summary>
    internal interface IAppFabricCachingStore : ICachingStoreAsync
    {
        void CreateRegion(string regionName);
        void InvalidateCacheItem(string key);
        void InvalidateSets(IEnumerable<string> entitySets);//needed for using with EF entities        
    }
}
