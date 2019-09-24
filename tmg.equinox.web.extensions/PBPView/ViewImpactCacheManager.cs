using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.caching.Adapters;

namespace tmg.equinox.web.PBPView
{
    public class ViewImpactCacheManager
    {
        private string keyPrefix = "DVI-";

        public void Add(int formDesignVersionId, string data)
        {
            string key = keyPrefix + Convert.ToString(formDesignVersionId);
            GlavCacheWrapper.Add(key, data);
        }

        private string Add(int formDesignVersionId, IUIElementService uIElementService)
        {
            string key = keyPrefix + Convert.ToString(formDesignVersionId);
            string data = uIElementService.GetCompiledPBPViewImpacts();
            GlavCacheWrapper.Add(key, data);
            return data;
        }

        public string Get(int formDesignVersionId, IUIElementService uIElementService)
        {
            string key = keyPrefix + Convert.ToString(formDesignVersionId);
            var cacheData = GlavCacheWrapper.Get<string>(key);

            if (string.IsNullOrEmpty(cacheData))
            {
                cacheData = Add(formDesignVersionId, uIElementService);
            }

            return cacheData;
        }

        public bool Remove(int formDesignVersionId)
        {
            string key = keyPrefix + Convert.ToString(formDesignVersionId);
            return GlavCacheWrapper.Remove(key);
        }
    }
}
