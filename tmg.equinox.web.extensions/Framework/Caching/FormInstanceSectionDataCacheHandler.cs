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
    public class FormInstanceSectionDataCacheHandler
    {
        private string SectionKeyPrefix = "FI-{0}:SECName-{1}:UID-{2}";
        private string SectionCacheListKeyPrefix = "FISECLIST-{0}:UID-{1}";
        private string TargetFormInstanceKeyPrefix = "TARFI-{0}:UID-{1}";

        public void AddSectionData(int formInstanceId, string sectionName, string sectionData, int? userId)
        {
            string key = string.Format(SectionKeyPrefix, formInstanceId, sectionName, userId);
            GlavCacheWrapper.Add(key, sectionData);
            this.AddSectionNameToCache(formInstanceId, sectionName, userId);
        }

        public bool RemoveSectionData(int formInstanceId, string sectionName, int? userId)
        {
            string key = string.Format(SectionKeyPrefix, formInstanceId, sectionName, userId);
            GlavCacheWrapper.Remove(key);
            return true;
        }

        public string IsExists(int formInstanceId, string sectionName, int? userId)
        {
            string key = string.Format(SectionKeyPrefix, formInstanceId, sectionName, userId);
            string cacheData = GlavCacheWrapper.Get<string>(key);
            return cacheData;
        }

        public void UpdateSection(int formInstanceId, string sectionName, string sectionData, int? userId)
        {
            string key = string.Format(SectionKeyPrefix, formInstanceId, sectionName, userId);
            string cacheData = GlavCacheWrapper.Get<string>(key);

            if (cacheData != null)
            {
                GlavCacheWrapper.Add(key, sectionData);
            }
        }

        public List<JToken> GetSectionListFromCache(int formInstanceId, int? userId)
        {
            string key = string.Format(SectionCacheListKeyPrefix, formInstanceId, userId);
            string sectionList = GlavCacheWrapper.Get<string>(key);
            List<JToken> cacheList = new List<JToken>();

            if (sectionList != null)
            {
                var cacheObj = JsonConvert.DeserializeObject(sectionList);
                cacheList = ((JArray)cacheObj).ToList();
            }

            return cacheList;
        }

        private void AddSectionNameToCache(int formInstanceId, string sectionName, int? userId)
        {
            string sectionList = "";
            string key = string.Format(SectionCacheListKeyPrefix, formInstanceId, userId);

            List<JToken> cacheList = this.GetSectionListFromCache(formInstanceId, userId);

            if (cacheList.Count > 0)
            {
                cacheList.Add(sectionName);
                sectionList = JsonConvert.SerializeObject(cacheList);
            }
            else
            {
                List<JToken> newSectionlist = new List<JToken>();
                newSectionlist.Add(sectionName);
                sectionList = JsonConvert.SerializeObject(newSectionlist);
            }

            GlavCacheWrapper.Add(key, sectionList);
        }

        public void RevomeSectionListFromCache(int formInstanceId, int? userId)
        {
            string key = string.Format(SectionCacheListKeyPrefix, formInstanceId, userId);
            GlavCacheWrapper.Remove(key);
        }

        public List<JToken> GetTargetFormInstanceIdsFromCache(int sourceFormInstanceId, int? userId)
        {
            string key = string.Format(TargetFormInstanceKeyPrefix, sourceFormInstanceId, userId);
            string targetInstanceList = GlavCacheWrapper.Get<string>(key);
            List<JToken> cacheList = new List<JToken>();

            if (targetInstanceList != null)
            {
                var cacheObj = JsonConvert.DeserializeObject(targetInstanceList);
                cacheList = ((JArray)cacheObj).ToList();
            }

            return cacheList;
        }

        public void AddTargetFormInstanceIdToCache(int sourceFormInstanceId, int targetFormInstanceId, int? userId)
        {
            string targetFormInstanceList = "";
            string key = string.Format(TargetFormInstanceKeyPrefix, sourceFormInstanceId, userId);

            List<JToken> cacheList = this.GetTargetFormInstanceIdsFromCache(sourceFormInstanceId, userId);

            if (cacheList.Count > 0)
            {
                if (!cacheList.Contains(targetFormInstanceId))
                {
                    cacheList.Add(targetFormInstanceId);
                }
                targetFormInstanceList = JsonConvert.SerializeObject(cacheList);
            }
            else
            {
                List<JToken> newSectionlist = new List<JToken>();
                newSectionlist.Add(targetFormInstanceId);
                targetFormInstanceList = JsonConvert.SerializeObject(newSectionlist);
            }

            GlavCacheWrapper.Add(key, targetFormInstanceList);
        }

        public void RemoveTargetFormInstanceFromCache(int sourceFormInstanceId, int? userId)
        {
            List<JToken> targetFormInstanceIds = this.GetTargetFormInstanceIdsFromCache(sourceFormInstanceId, userId);

            foreach (JToken targetFormInstanceId in targetFormInstanceIds)
            {
                int targetId = Convert.ToInt32(targetFormInstanceId);
                List<JToken> sectionList = this.GetSectionListFromCache(targetId, userId);
                foreach (JToken sectionName in sectionList)
                {
                    this.RemoveSectionData(targetId, sectionName.ToString(), userId);
                }
                this.RevomeSectionListFromCache(targetId, userId);
            }

            string key = string.Format(TargetFormInstanceKeyPrefix, sourceFormInstanceId, userId);
            GlavCacheWrapper.Remove(key);
        }

        public void RemoveAllEntries()
        {
            GlavCacheWrapper.RemoveAll(string.Empty);
        }
    }
}