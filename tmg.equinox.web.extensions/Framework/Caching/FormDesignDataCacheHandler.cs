using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.VersionManagement;
using tmg.equinox.caching;
using tmg.equinox.caching.Adapters;

namespace tmg.equinox.web.Framework.Caching
{
    public class FormDesignDataCacheHandler
    {
        private string keyPrefix = "FD-";

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

            if (string.IsNullOrEmpty(cacheData))
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
    }
}