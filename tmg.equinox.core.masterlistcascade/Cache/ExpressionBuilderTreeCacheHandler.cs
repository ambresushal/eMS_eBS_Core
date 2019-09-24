using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.caching.Adapters;

namespace tmg.equinox.caching
{
    public class ExpressionBuilderTreeCacheHandler
    {
        private string TreeKeyPrefix = "FDVTJ-{0}";

        public void Add(int formDesignVersionId,string ruleExecutionTreeJSON)
        {
            string key = string.Format(TreeKeyPrefix, formDesignVersionId);
            GlavCacheWrapper.Add(key, ruleExecutionTreeJSON);
        }

        public bool Remove(int formDesignVersionId)
        {
            string key = string.Format(TreeKeyPrefix, formDesignVersionId);
            GlavCacheWrapper.Remove(key);
            return true;
        }

        public string IsExists(int formDesignVersionId)
        {
            string key = string.Format(TreeKeyPrefix, formDesignVersionId);
            string cacheData = GlavCacheWrapper.Get<string>(key);
            return cacheData;
        }
    }
}