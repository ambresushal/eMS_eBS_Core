using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.caching.Adapters;

namespace tmg.equinox.caching
{
    public class FormDesignDataCacheHandler
    {
        private string keyPrefix = "FD-";
        private string keySectionPrefix = "Section-";
        private string Add(int tenantId, int formDesignId, IFormDesignService _formDesignServices)
        {
            string key = keyPrefix + Convert.ToString(formDesignId);

            //if a document is not cached, get it from the database
            string jsonData = _formDesignServices.GetCompiledFormDesignVersion(tenantId, formDesignId);

            GlavCacheWrapper.Add(key, jsonData);
            return jsonData;
        }

        public void Add(int tenantId, int formDesignId, string jsonData)
        {
            string key = keyPrefix + Convert.ToString(formDesignId);
            GlavCacheWrapper.Add(key, jsonData);
        }

        public JObject Get(int formDesignId)
        {
            string key = keyPrefix + Convert.ToString(formDesignId);
            var cacheData = GlavCacheWrapper.Get<string>(key);

            return JObject.Parse(cacheData);
        }

        public string Get(int tenantId, int formDesignId, IFormDesignService _formDesignServices)
        {
            string key = keyPrefix + Convert.ToString(formDesignId);
            string cacheData = GlavCacheWrapper.Get<string>(key);

            if (cacheData == null)
            {
                cacheData = this.Add(tenantId, formDesignId, _formDesignServices);
            }

            return cacheData;
        }

        public List<RuleDesign> GetRuleDesigns(int formDesignId)
        {
            List<RuleDesign> rules = new List<RuleDesign>();
            JObject formDesign = this.Get(formDesignId);
            string rulesDesign = null;
            if (formDesign != null)
            {
                rulesDesign = JsonConvert.SerializeObject(formDesign["Rules"]);
            }
            rules = JsonConvert.DeserializeObject<List<RuleDesign>>(rulesDesign);
            return rules;
        }

        public List<SectionDesign> GetSectionNames(int tenantId, string formDesignVersionId, IFormDesignService _formDesignService)
        {
            string formDesign = String.Empty;
            FormDesignVersionDetail detail = null;

           var sectionsList = getSectionFromCache(formDesignVersionId);

            if (sectionsList == null)
            {
                formDesign = Get(tenantId, Convert.ToInt32(formDesignVersionId), _formDesignService);
                detail = JsonConvert.DeserializeObject<FormDesignVersionDetail>(formDesign);
                foreach (SectionDesign design in detail.Sections)
                {
                    design.Label = detail.FormName + " - " + design.Label;
                }
                sectionsList = detail.Sections;

                addSectionToCache(formDesignVersionId, sectionsList);
            }
            return sectionsList;
        }

        private List<SectionDesign> getSectionFromCache(string formDesignVersionId)
        {
            string key = keySectionPrefix + Convert.ToString(formDesignVersionId);
            return GlavCacheWrapper.Get<List<SectionDesign>>(key);
        }
        private void addSectionToCache(string formDesignVersionId, List<SectionDesign> sectionList)
        {
            string key = keySectionPrefix + formDesignVersionId;
            GlavCacheWrapper.Add(key, sectionList);
        }

    }
}