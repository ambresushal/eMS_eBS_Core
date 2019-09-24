using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace tmg.equinox.caching
{
    public class CachingSetting
    {
        #region Public/ Protected / Private Member Variables
        public string CacheName { get; set; }
        public CacheItemPolicy CacheItemPolicy { get; set; }
        public string CacheRegion { get; set; }

        #endregion Member Variables

        #region Constructor/Dispose
        public CachingSetting(CacheItemPolicy policy, string cacheName, string cacheRegion)
        {
            CacheItemPolicy = policy;
            CacheName = cacheName;
            CacheRegion = cacheRegion;
        }        
        #endregion Constructor/Dispose

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods

        #region Helper Methods
        #endregion Helper Methods

    
    }
}
