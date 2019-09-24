using System;
using System.Collections.Generic;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesign;

namespace tmg.equinox.ruleprocessor.formdesignmanager
{
    public class FormDesignsManager
    {
        static Dictionary<string, bool> formDesignList = new Dictionary<string, bool>();

        public static Dictionary<string, bool> GetFormDesignList(int tenantId, IFormDesignService formDesignService)
        {
            if (formDesignList.Count == 0)
            {
                IEnumerable<FormDesignRowModel> designlist = formDesignService.GetFormDesignList(tenantId);

                foreach (FormDesignRowModel des in designlist)
                {
                    if (!formDesignList.ContainsKey(des.FormDesignName))
                    {
                        formDesignList.Add(des.FormDesignName, des.IsMasterList);
                    }
                }
            }
            return formDesignList;
        }
    }
}