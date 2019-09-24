using tmg.equinox.caching.Adapters;


namespace tmg.equinox.caching
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