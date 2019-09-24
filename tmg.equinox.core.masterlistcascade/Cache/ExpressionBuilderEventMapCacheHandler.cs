using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.caching.Adapters;

namespace tmg.equinox.caching
{
    public class ExpressionBuilderEventMapCacheHandler
    {
        private string EventMapKeyPrefix = "FDVMJ-{0}";

        public void Add(int formDesignVersionId, string eventMapJSON)
        {
            string key = string.Format(EventMapKeyPrefix, formDesignVersionId);
            GlavCacheWrapper.Add(key, eventMapJSON);
        }

        public bool Remove(int formDesignVersionId)
        {
            string key = string.Format(EventMapKeyPrefix, formDesignVersionId);
            GlavCacheWrapper.Remove(key);
            return true;
        }

        public string IsExists(int formDesignVersionId)
        {
            string key = string.Format(EventMapKeyPrefix, formDesignVersionId);
            string cacheData = GlavCacheWrapper.Get<string>(key);
            return cacheData;
        }
    }
}