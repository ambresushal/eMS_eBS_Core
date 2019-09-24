using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.ExpresionBuilder.Handlers;
using tmg.equinox.web.FormInstanceManager;


namespace tmg.equinox.web.ExpresionBuilder
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
        public static IExpressionBuilderRuleHandler GetHandler(FormInstanceDataManager formDataInstanceManager, int formDesignId, FormDesignVersionDetail detail, int formInstanceId, IFormDesignService formDesignServices, IFolderVersionServices folderVersionServices,int folderversionId)
        {
            IExpressionBuilderRuleHandler handlerInstance = null;
            switch (formDesignId)
            {
                case ExpressionBuilderConstants.AnchorFormDesignID:
                    {
                        handlerInstance = new IAnchorRuleHandler(detail, formInstanceId);
                    }
                    break;
                case ExpressionBuilderConstants.COMMERCIALMEDICAL:
                    {
                        handlerInstance = new IQCommercialMedicalRuleHandler(formDataInstanceManager, detail, formInstanceId,formDesignServices,folderVersionServices, folderversionId);
                    }
                    break;
            }
            return handlerInstance;
        }

        public static bool IsAnchor(int formDesignId)
        {
            bool result = false;
            switch (formDesignId)
            {
                case ExpressionBuilderConstants.AnchorFormDesignID:
                    { result = true; }
                    break;
                case ExpressionBuilderConstants.COMMERCIALMEDICAL:
                    { result = true; }
                    break;
            }
            return result;
        }
    }
}