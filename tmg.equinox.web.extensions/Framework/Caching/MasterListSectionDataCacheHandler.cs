using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.caching.Adapters;
using tmg.equinox.web.FormInstanceManager;

namespace tmg.equinox.web.Framework.Caching
{
    public class MasterListSectionDataCacheHandler
    {
        private string MLSectionKeyPrefix = "FI-{0}:SECName-{1}";        
        
        public void AddSectionData(int formInstanceId, string sectionName, string sectionData)
        {
            string key = string.Format(MLSectionKeyPrefix, formInstanceId, sectionName);
            GlavCacheWrapper.Add(key, sectionData);                      
        }

        public bool RemoveSectionData(int formInstanceId, string sectionName)
        {
            string key = string.Format(MLSectionKeyPrefix, formInstanceId, sectionName);
            GlavCacheWrapper.Remove(key);
            return true;
        }

        public string IsExists(int formInstanceId, string sectionName)
        {
            string key = string.Format(MLSectionKeyPrefix, formInstanceId, sectionName);
            string cacheData = GlavCacheWrapper.Get<string>(key);
            return cacheData;
        }        
    }
}