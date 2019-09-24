using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.caching.Interfaces;
using tmg.equinox.caching.Stores;
using System.Runtime.Caching;

namespace tmg.equinox.caching
{
    public class MemoryCachingStoreFactory : ICachingStoreFactory
    {
        #region Public Methods
        public ICachingStore CreateCachingStore(CachingSetting cachingSetting)
        {
            return new MemoryCachingStore(cachingSetting.CacheItemPolicy);
        }
        #endregion Public Methods
    }
}