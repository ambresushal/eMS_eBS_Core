using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.caching.Adapters;

namespace tmg.equinox.web.Framework.Caching
{
    public class CompiledDcoumentCacheHandler
    {
        private string DocumentRulePrefix = "CDR-{0}";

        public void Add(int documentRuleId, string ruleExpressionCompileJSON)
        {
            string key = string.Format(DocumentRulePrefix,documentRuleId);
            GlavCacheWrapper.Add(key, ruleExpressionCompileJSON);
        }

        public bool Remove(int documentRuleId)
        {
            string key = string.Format(DocumentRulePrefix, documentRuleId);
            GlavCacheWrapper.Remove(key);
            return true;
        }

        public string IsExists(int documentRuleId)
        {
            string key = string.Format(DocumentRulePrefix, documentRuleId);
            string cacheData = GlavCacheWrapper.Get<string>(key);
            return cacheData;
        }
    }
}