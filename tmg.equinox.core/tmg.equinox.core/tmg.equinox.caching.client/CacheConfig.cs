using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tmg.equinox.caching.client.Properties;

namespace tmg.equinox.caching.client
{
    public static class CacheConfig
    {
        #region Public/ Protected / Private Member Variables
        private static NameValueCollection cacheConfigurationSection;
        private static bool enableCacheWorkFlowState;
        private static bool enableCachingEF;
        private static bool enableCache;
        private static string cacheRegion;
        private static TimeSpan timeout;
        private static bool CacheEnabled;

        private static bool enableClaimsCaching;
        private static string cacheToUseForClaims;
        private static bool claimsExpirationPolicy;
        private static string defaultExpirationPolicy;
        private static string cacheName;

        public const string AppSettingsKeyPrefix = "Cache.";
        #endregion Public/ Protected / Private Member Variables

        #region Constructor/Dispose

        #endregion Constructor/Dispose

        #region Private Methods

      
    
        #endregion Private Methods
        #region Public Methods
        public static void RegisterCacheConfiguration()
        {
            cacheConfigurationSection = (NameValueCollection)ConfigurationManager.GetSection("cache");
            enableCachingEF = Convert.ToBoolean(cacheConfigurationSection["EnabledEFCache"]);
            enableCacheWorkFlowState = true;// Convert.ToBoolean(cacheConfigurationSection["EnableCacheWorkFlowState"]);
            enableCache = Convert.ToBoolean(cacheConfigurationSection["CacheEnabled"]);
            cacheRegion = Convert.ToString(cacheConfigurationSection["CacheRegion"]);
         //   TimeSpan.TryParse(Convert.ToString(cacheConfigurationSection["timeout"]), out timeout);
            //OverrideSettingsWithAppSettingsIfPresent();
            CandidatesCachingSettings();
        }
        private static void CandidatesCachingSettings()
        {
            enableClaimsCaching = Caching.Default.ClaimsCacheEnabled;
            cacheToUseForClaims = Caching.Default.CacheForClaims;
            claimsExpirationPolicy = Caching.Default.ClaimsExpirationEnabled;
            defaultExpirationPolicy = Caching.Default.DefaultExpirationPolicy;
            cacheName = Caching.Default.AppfabricCacheName;
            TimeSpan.TryParse(Caching.Default.ClaimsCacheExpirationTimeout, out timeout);
        }
      
        #endregion Public Methods

        #region Properties Variables
        public static bool EnableCaching
        {
            get
            {
                return enableCache;
            }
        }
        public static bool EnableCacheWorkFlowState
        {
            get
            {
                return enableCacheWorkFlowState;
            }
        }
        public static bool EnableEntityFrameworkCaching
        {
            get
            {
                return enableCachingEF;
            }
        }
        
        public static string CacheRegion
        {
            get
            {
                return cacheRegion;
            }
        }

        public static TimeSpan CacheExpirationTimeout
        {
            get
            {
                if (timeout == null)
                    timeout = new TimeSpan(00, 20, 00);
                return timeout;
            }
        }


        public static bool EnableClaimsCaching
        {
            get
            {
                return enableClaimsCaching;
            }
        }
        public static string CacheToUseForClaims
        {
            get
            {
                return cacheToUseForClaims;
            }
        }
        public static bool ClaimsExpirationPolicy
        {
            get
            {
                return claimsExpirationPolicy;
            }
        }
        public static string DefaultExpirationPolicy
        {
            get
            {
                return defaultExpirationPolicy;
            }
        }
        public static string CacheName
        {
            get
            {
                return cacheName;
            }
        }
        #endregion Properties Variables

    }
}




    //private static void OverrideSettingsWithAppSettingsIfPresent()
    //    {
    //        var claimsCacheEnabled = string.Format("{0}EnableClaimsCaching", AppSettingsKeyPrefix);
    //        var cacheToUseForClaims = string.Format("{0}CacheToUseForClaims", AppSettingsKeyPrefix);
    //        var claimsExpirationPolicy = string.Format("{0}ClaimsExpirationPolicy", AppSettingsKeyPrefix);
    //        var defaultExpirationPolicy = string.Format("{0}DefaultExpirationPolicy", AppSettingsKeyPrefix);
    //        var cacheName = string.Format("{0}CacheName", AppSettingsKeyPrefix);
    //        var distributedCacheServersKey = string.Format("{0}DistributedCacheServers", AppSettingsKeyPrefix);

    //        if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[claimsCacheEnabled]))
    //        {
    //            enableClaimsCaching = Convert.ToBoolean(ConfigurationManager.AppSettings[claimsCacheEnabled]);
    //        }
    //        if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[cacheToUseForClaims]))
    //        {
    //            cacheToUseForClaims = ConfigurationManager.AppSettings[cacheToUseForClaims];
    //        }
    //        if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[claimsExpirationPolicy]))
    //        {
    //            claimsExpirationPolicy = ConfigurationManager.AppSettings[claimsExpirationPolicy];
    //        }
    //        if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[defaultExpirationPolicy]))
    //        {
    //            defaultExpirationPolicy = ConfigurationManager.AppSettings[defaultExpirationPolicy];
    //        }
    //        if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[cacheName]))
    //        {
    //            cacheName = ConfigurationManager.AppSettings[cacheName];
    //        }
    //    }
