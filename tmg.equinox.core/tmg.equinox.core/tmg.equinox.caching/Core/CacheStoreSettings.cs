using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.caching.Stores
{
    public class CachingStoreSettings
    {
        #region Public/ Protected / Private Member Variables
        // <summary>
		/// Quota i.e. number of bytes that can be used up by the cache.
		/// If 0 then no limit
		/// </summary>
		public long TotalQuota { get; set; }

		/// <summary>
		/// Quota  i.e. number of bytesper hostname (domain) 
		/// </summary>
		public long PerDomainNameQuota { get; set; }

        #endregion Member Variables

        #region Constructor/Dispose
        public CachingStoreSettings()
        {
            TotalQuota = long.MaxValue;
            PerDomainNameQuota = 50 * 1024 * 1024; // 50 MB
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
