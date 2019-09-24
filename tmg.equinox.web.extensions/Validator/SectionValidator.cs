using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.FormInstanceManager;

namespace tmg.equinox.web.Validator
{
    public class SectionValidator : Validator
    {
        SectionDesign sectionDesign { get; set; }
        JObject sectionData { get; set; }
        JToken _masterListData { get; set; }
        bool _isSOTView = false;

        private FormInstanceDataManager _formDataInstanceManager;
        private IFormInstanceService _formInstanceService;
        private IFormDesignService _formDesignServices;
        private IFolderVersionServices _folderVersionServices;
        private int _formInstanceId;
        public SectionValidator(JObject formData, SectionDesign sectionDesign, JObject sectionData, List<ValidationDesign> validations, string sectionPath, List<RuleDesign> validationRules, List<DuplicationDesign> duplicationChecks, JToken masterListData, bool isSOTView, bool isAnchor, int formInstanceId, FormInstanceDataManager formDataInstanceManager, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, IFormInstanceService formInstanceService)
            : base(formData, sectionPath, validations, validationRules, duplicationChecks, isAnchor)
        {
            this._formData = formData;
            this.sectionDesign = sectionDesign;
            this.sectionData = sectionData;
            this.validations = validations;
            this.sectionPath = string.IsNullOrEmpty(sectionPath) ? sectionDesign.Label : (sectionPath + " => " + sectionDesign.Label);
            this.validationRules = validationRules;
            this.duplicationChecks = duplicationChecks;
            this._masterListData = masterListData;
            this._isSOTView = isSOTView;
            this._formInstanceId = formInstanceId;
            this._formDataInstanceManager = formDataInstanceManager;
            this._folderVersionServices = folderVersionServices;
            this._formDesignServices = formDesignServices;
            this._formInstanceService = formInstanceService;
        }

        public override void Validate(ref ErrorSection errorSection, ref int maxAllowedErrorCount, bool isCBCMasterList)
        {
            foreach (var element in sectionDesign.Elements)
            {
                if (IsElementApplicableForValidations(element))
                {
                    if (maxAllowedErrorCount == 0) break;
                    var elementValidator = new ElementValidator(_formData, element, sectionData, validations, 0, sectionPath, "", validationRules, null, duplicationChecks, _masterListData, _isSOTView, _isAnchor, _formInstanceId, _formDataInstanceManager, _folderVersionServices, _formDesignServices, _formInstanceService);
                    elementValidator.Validate(ref errorSection, ref maxAllowedErrorCount, isCBCMasterList);
                }
            }
        }

        private bool IsElementApplicableForValidations(ElementDesign element)
        {
            bool isApplicable = false;
            if (this._isSOTView)
            {
                if (string.Equals(element.Layout, "SOT", StringComparison.OrdinalIgnoreCase) || string.Equals(element.Layout, "Both", StringComparison.OrdinalIgnoreCase))
                {
                    isApplicable = true;
                }
            }

            if (!this._isSOTView)
            {
                if (string.Equals(element.Layout, "Default", StringComparison.OrdinalIgnoreCase) || string.Equals(element.Layout, "Both", StringComparison.OrdinalIgnoreCase))
                {
                    isApplicable = true;
                }
            }

            return isApplicable;
        }
    }
}
