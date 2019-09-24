using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesign;

namespace tmg.equinox.rulesprocessor
{
    public class FormDesignGroupMapManager
    {
        public static List<FormDesignGroupMapModel> formDesigns = new List<FormDesignGroupMapModel>();

        public static FormDesignGroupMapModel Get(string ruleAlias, IFormDesignService formDesignService)
        {
            if (formDesigns.Count == 0)
            {
                formDesigns = formDesignService.GetFormDesignGroupMap();
            }
            return formDesigns.Where(s => s.FormName == ruleAlias).FirstOrDefault();
        }
    }
}