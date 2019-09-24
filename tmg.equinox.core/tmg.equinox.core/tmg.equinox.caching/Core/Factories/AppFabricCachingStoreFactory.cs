using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.caching.Interfaces;
using tmg.equinox.caching.Stores;

namespace tmg.equinox.caching
{
    public class AppFabricCachingStoreFactory : ICachingStoreFactory
    {
              
        #region Public Methods
        public ICachingStore CreateCachingStore(CachingSetting cachingSetting)
        {
            return new AppFabricCachingStore(cachingSetting.CacheName, cachingSetting.CacheRegion);
        }
        #endregion Public Methods
    }
}
