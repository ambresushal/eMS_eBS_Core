using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.ruleprocessor;

namespace tmg.equinox.expressionbuilder
{
    public static class ExpressionBuilderRuleFactory
    {
       
        /// <summary>
       /// 
       /// </summary>
       /// <param name="handler"></param>
       /// <param name="sectionName"></param>
       /// <param name="sectionData"></param>
       /// <param name="formData"></param>
       /// <returns></returns>
        public static IExpressionBuilderRuleHandler GetHandler(FormInstanceDataManager formDataInstanceManager, int formDesignId, FormDesignVersionDetail detail, int formInstanceId, IFormDesignService formDesignServices, IFolderVersionServices folderVersionServices)
        {
            IExpressionBuilderRuleHandler handlerInstance = null;
            switch (formDesignId)
            {
                case ExpressionBuilderConstants.AnchorFormDesignID:
                    {
                        handlerInstance = new IAnchorRuleHandler(detail, formInstanceId);
                    }
                    break;
            }
            return handlerInstance;
        }
    }
}