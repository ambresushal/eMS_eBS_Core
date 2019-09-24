using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.caching.Interfaces;
using tmg.equinox.caching.Stores;

namespace tmg.equinox.caching
{
    public class HttpCachingStoreFactory : ICachingStoreFactory
    {
        #region Public Methods
        public ICachingStore CreateCachingStore(CachingSetting cachingSetting)
        {
            return new HttpCachingStore();
        }
        #endregion Public Methods
    }
}
