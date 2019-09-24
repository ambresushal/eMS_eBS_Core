using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.caching;

namespace tmg.equinox.ruleprocessor.formdesignmanager
{
    public class FormDesignVersionDetailReadOnlyObjectCache
    {
        private static Dictionary<int, FormDesignVersionDetail> FormDesignVersionDetailInstances = new Dictionary<int, FormDesignVersionDetail>();

        private static object lockObject = new object();
        private FormDesignVersionDetailReadOnlyObjectCache()
        {

        }

        public static FormDesignVersionDetail GetFormDesignVersionDetail(int tenantID, int formDesignVersionID, IFormDesignService formDesignService)
        {
            FormDesignVersionDetail detail = null;
            lock (lockObject)
            {
                if (FormDesignVersionDetailInstances.ContainsKey(formDesignVersionID) == false)
                {
                    FormDesignDataCacheHandler formDesignCacheHandler = new FormDesignDataCacheHandler();
                    string formDesign = formDesignCacheHandler.Get(tenantID, formDesignVersionID, formDesignService);
                    detail = JsonConvert.DeserializeObject<FormDesignVersionDetail>(formDesign);
                    FormDesignVersionDetailInstances.Add(formDesignVersionID, detail);
                }
                else
                {
                    detail = FormDesignVersionDetailInstances[formDesignVersionID];
                }
            }
            return detail;
        }

        public static void SetFormDesignVersionDetail(int formDesignVersionID,FormDesignVersionDetail detail)
        {
            
            lock (lockObject)
            {
                if (FormDesignVersionDetailInstances.ContainsKey(formDesignVersionID) == true)
                {
                    FormDesignVersionDetailInstances[formDesignVersionID] = detail;
                }
                else
                {
                    FormDesignVersionDetailInstances.Add(formDesignVersionID,detail);
                }
            }
        }
    }
}
