using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices;

namespace tmg.equinox.web.CustomRule
{
    public static class CustomRuleFactory
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="sectionName"></param>
        /// <param name="sectionData"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        public static ICustomRuleHandler GetHandler(int formDesignId, string formData)
        {
            ICustomRuleHandler handlerInstance = null;
            switch (formDesignId)
            {
                case CustomRuleConstants.ProductFormDesignID:
                    {
                    }
                    break;
            }
            return handlerInstance;
        }

        public static ICustomRuleAssociatedSectionHandler GetAssociateSectionHandler(int formDesignId)
        {
            ICustomRuleAssociatedSectionHandler handlerInstance = null;
            switch (formDesignId)
            {
                case CustomRuleConstants.MasterListFormDesignID:
                    {
                    }
                    break;
            }
            return handlerInstance;
        }

    }
}