using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.ExpresionBuilder;
using tmg.equinox.web.FormInstanceManager;

namespace tmg.equinox.web.ExpresionBuilder.Handlers
{
    public class IQCommercialMedicalRuleHandler : IExpressionBuilderRuleHandler
    {
        #region private        
        private FormDesignVersionDetail _formDetail;
        private FormInstanceDataManager _formDataInstanceManager;
        private int _formInstanceId;
        private IFormDesignService _formDesignServices;
        private IFolderVersionServices _folderVersionServices;
        private int FolderVersionId;
        #endregion

        #region constructor
        public IQCommercialMedicalRuleHandler(FormInstanceDataManager formDataInstanceManager, FormDesignVersionDetail detail, int formInstanceId, IFormDesignService formDesignServices, IFolderVersionServices folderVersionServices, int folderVersionId)
        {
            this._formDataInstanceManager = formDataInstanceManager;
            this._formDetail = detail;
            this._formInstanceId = formInstanceId;
            this._formDesignServices = formDesignServices;
            this._folderVersionServices = folderVersionServices;
            this.FolderVersionId = folderVersionId;
        }

        #endregion constructor

        public void RunRulesOnSectionLoad(string sectionName, IFormInstanceService formInstanceService)
        {
            switch (sectionName)
            {
                case COMMERCIALMEDICAL.RxSelection:
                    RxSelectionRules prule = new RxSelectionRules(_formDataInstanceManager, _formInstanceId, _formDetail, this._folderVersionServices, this._formDesignServices, this.FolderVersionId);
                    prule.RunOnSectionOnLoad();
                    break;
                case COMMERCIALMEDICAL.AncillarySelection:
                    AncillarySelectionRules dvhRule = new AncillarySelectionRules(_formDataInstanceManager, _formInstanceId, _formDetail, this._folderVersionServices, this._formDesignServices, this.FolderVersionId);
                    dvhRule.RunOnSectionOnLoad();
                    break;
                case COMMERCIALMEDICAL.ProductRules:
                    SectionProductRules pRules = new SectionProductRules(_formInstanceId, _formDetail, this._folderVersionServices);
                    pRules.Run();
                    break;
                case COMMERCIALMEDICAL.CascadingCostShare:
                    CascadingCostShare csRules = new CascadingCostShare(_formDataInstanceManager, _formInstanceId, _formDetail, this._folderVersionServices, this._formDesignServices, this.FolderVersionId);
                    csRules.RunOnSectionOnLoad();
                    break;

            }

        }

        public void RunRulesOnSectionSave(string sectionName, string previousSectionData, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, int folderVersionId)
        {

        }

        public void RunRulesOnDocumentLoad(IFormInstanceService formInstanceService, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, int folderVersionId)
        {

        }

        public bool hasSectionChange(string currentData, string previousData)
        {
            return HasDataChanged.hasDataChanged(currentData, previousData);
        }
    }
}