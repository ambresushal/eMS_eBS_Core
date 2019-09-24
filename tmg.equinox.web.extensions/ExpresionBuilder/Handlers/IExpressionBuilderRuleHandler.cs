

using tmg.equinox.applicationservices.interfaces;
namespace tmg.equinox.web.ExpresionBuilder
{
    public interface IExpressionBuilderRuleHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="sectionData"></param>
        /// <param name="formData"></param>
        void RunRulesOnSectionLoad(string sectionName, IFormInstanceService formInstanceService);

        void RunRulesOnSectionSave(string sectionName, string previousSectionData, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, int folderVersionId);

        void RunRulesOnDocumentLoad(IFormInstanceService formInstanceService, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, int folderVersionId);

        bool hasSectionChange(string currentData, string previousData);
    }
}
