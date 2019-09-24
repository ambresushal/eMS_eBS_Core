using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;

namespace tmg.equinox.expressionbuilder
{
    public class IAnchorRuleHandler : IExpressionBuilderRuleHandler
    {
        private FormDesignVersionDetail _detail;
        private int _formInstanceId;


        public IAnchorRuleHandler(FormDesignVersionDetail detail, int formInstanceId)
        {
            this._formInstanceId = formInstanceId;
            this._detail = detail;
        }

        public bool hasSectionChange(string currentData, string previousData)
        {
            return true;
        }

        public void RunRulesOnDocumentLoad(IFormInstanceService formInstanceService, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, int folderVersionId)
        {
            SectionARules pRules = new SectionARules(_detail, formInstanceService, _formInstanceId);
            pRules.Run();
        }

        public void RunRulesOnSectionLoad(string sectionName, IFormInstanceService formInstanceService)
        {
            switch (sectionName)
            {
                case ExpressionBuilderConstants.SectionA:
                    SectionARules pRules = new SectionARules(_detail, formInstanceService, _formInstanceId);
                    pRules.Run();
                    break;
            }
        }

        public void RunRulesOnSectionSave(string sectionName, string previousSectionData, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, int folderVersionId)
        {
        }
    }
}
