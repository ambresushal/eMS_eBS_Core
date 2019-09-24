using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.schema.Base.Model;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.Framework.Caching;
using tmg.equinox.web.RuleEngine;

namespace tmg.equinox.web.Validator
{
    public class DocumentValidatorManager
    {
        private List<FormInstanceViewModel> _formInstances;
        private FolderVersionViewModel _folderVersionModel;
        private JToken _masterListData;
        private bool _isSOTView = false;

        private FormInstanceDataManager _formDataInstanceManager;
        
        private IFormInstanceService _formInstanceService;
        private IFormDesignService _formDesignServices;
        private IFolderVersionServices _folderVersionServices;
        

        public DocumentValidatorManager(List<FormInstanceViewModel> formInstances, FolderVersionViewModel folderVersionService, JToken masterListData, FormInstanceDataManager formDataInstanceManager, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, IFormInstanceService formInstanceService)
        {
            this._formInstances = formInstances;
            this._folderVersionModel = folderVersionService;
            this._masterListData = masterListData;            
            this._formDataInstanceManager = formDataInstanceManager;
            this._folderVersionServices = folderVersionServices;
            this._formDesignServices = formDesignServices;
            this._formInstanceService = formInstanceService;
        }

        public DocumentValidatorManager(List<FormInstanceViewModel> formInstances, FolderVersionViewModel folderVersionService, JToken masterListData, bool isSOTView,int formInstanceId, FormInstanceDataManager formDataInstanceManager, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, IFormInstanceService formInstanceService)
        {
            this._formInstances = formInstances;
            this._folderVersionModel = folderVersionService;
            this._masterListData = masterListData;
            this._isSOTView = isSOTView;           
            this._formDataInstanceManager = formDataInstanceManager;        
            this._folderVersionServices = folderVersionServices;
            this._formDesignServices = formDesignServices;
            this._formInstanceService = formInstanceService;
        }

        public List<dynamic> ExecuteValidation(int tenantId, IFormDesignService _formDesignServices, IFolderVersionServices _folderVersionService, int? userId, bool isCBCMasterList, bool isPortfolio, string sectionName)
        {
            FormDesignDataCacheHandler formDesigncacheHandler = new FormDesignDataCacheHandler();
            IDataCacheHandler cacheHandler = DataCacheHandlerFactory.GetHandler();
            ErrorGridCacheHandler errorCacheHandler = new ErrorGridCacheHandler();
            List<dynamic> validationDataList = new List<dynamic>();

            foreach (var formInstance in this._formInstances)
            {
                bool isAnchor = formInstance.FormInstanceID == formInstance.AnchorDocumentID;
                //Get Form Design from cache.
                FormDesignVersionDetail detail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(tenantId, formInstance.FormDesignVersionID, _formDesignServices);
                //Get form instance data from cache
                var jsonToValidate = formInstance.FormData ?? cacheHandler.Get(1, formInstance.FormInstanceID, false, detail, _folderVersionService, userId);
                //validate an indivvidual form instance.
                DocumentValidator docValidator = new DocumentValidator(detail, JObject.Parse(jsonToValidate), formInstance.FormInstanceID, formInstance.FormDesignName, this._folderVersionModel, _masterListData, _isSOTView, isAnchor, _formDataInstanceManager,  _folderVersionServices,  _formDesignServices,  _formInstanceService);

                List<ErrorSection> errorList = new List<ErrorSection>();
                if (string.IsNullOrEmpty(sectionName))
                {
                    errorList = docValidator.ValidateForm(isCBCMasterList, isPortfolio);
                }
                else
                {
                    errorList = docValidator.ValidateSection(isCBCMasterList, isPortfolio, sectionName);
                }
                //Store formInstance-wise errorlist.
                validationDataList.Add(new
                {
                    FormInstanceID = formInstance.FormInstanceID,
                    ErrorList = errorList
                });

                errorCacheHandler.AddErrorGridData(tenantId, formInstance.FormInstanceID, userId, errorList);
                if (!string.IsNullOrEmpty(formInstance.FormData))
                {
                    cacheHandler.Remove(formInstance.FormInstanceID, userId);
                }
            }
            return validationDataList;
        }
    }

    public class DocumentValidator
    {
        FormDesignVersionDetail formDesign { get; set; }
        JObject formData { get; set; }
        int formInstanceID { get; set; }
        string formName { get; set; }
        FolderVersionViewModel folderDetail { get; set; }
        JToken masterListData { get; set; }
        bool _isSOTView = false;
        bool _isAnchor { get; set; }

        private FormInstanceDataManager _formDataInstanceManager;     
        private IFormInstanceService _formInstanceService;
        private IFormDesignService _formDesignServices;
        private IFolderVersionServices _folderVersionServices;
        
        public DocumentValidator(FormDesignVersionDetail FormDesign, JObject FormData, int FormInstanceID, string FormName, FolderVersionViewModel FolderDetail, JToken masterListData, bool isSOTView, bool isAnchor, FormInstanceDataManager formDataInstanceManager, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, IFormInstanceService formInstanceService)
        {
            this.formDesign = FormDesign;
            this.formData = FormData;
            this.formInstanceID = FormInstanceID;
            this.formName = FormName;
            this.folderDetail = FolderDetail;
            this.masterListData = masterListData;
            this._isSOTView = isSOTView;
            this._isAnchor = isAnchor;            
            this._formDataInstanceManager = formDataInstanceManager;           
            this._folderVersionServices = folderVersionServices;
            this._formDesignServices = formDesignServices;
            this._formInstanceService = formInstanceService;
        }

        public List<ErrorSection> ValidateForm(bool isCBCMasterList, bool isPortfolio)
        {
            List<ErrorSection> errorSectionList = new List<ErrorSection>();
            int maxAllowedErrorCount;
            int.TryParse(System.Web.Configuration.WebConfigurationManager.AppSettings["MaxAllowedErrorListCount"], out maxAllowedErrorCount);
            maxAllowedErrorCount = maxAllowedErrorCount == 0 ? -1 : maxAllowedErrorCount;
            if (formDesign != null)
            {
                List<ValidationDesign> activeValidations = formDesign.Validations.Where(v => v.IsActive).ToList();
                List<RuleDesign> rules = formDesign.Rules.ToList();
                List<DuplicationDesign> duplicationChecks = formDesign.Duplications.Where(s => s.CheckDuplicate).ToList();
                //Where(r => r.TargetPropertyTypeId == (int)RuleProcessor.TargetPropertyTypes.IsRequired ||
                //         r.TargetPropertyTypeId == (int)RuleProcessor.TargetPropertyTypes.RunValidation ||
                //         r.TargetPropertyTypeId == (int)RuleProcessor.TargetPropertyTypes.Error
                //        ).ToList();
                var sections = formDesign.Sections.OrderBy(x => x.Sequence);
                foreach (var section in sections)
                {
                    if (maxAllowedErrorCount == 0) break;

                    ValidationRuleProcessor ruleProcessor = new ValidationRuleProcessor(section.Name, section.FullName, rules.Where(e => e.UIELementID == section.ID).ToList(), formData, null, "",this.formInstanceID,_formDataInstanceManager,_folderVersionServices,_formDesignServices,_formInstanceService);
                    if (!ruleProcessor.EvaluateVisibleEnableRules(section.Visible, section.Enabled, _isAnchor)) { continue; }

                    var errorSection = new ErrorSection()
                    {
                        ID = errorSectionList.Count + 1,
                        SectionID = section.Name,
                        Section = section.Label,
                        Form = this.formName,
                        FormInstanceID = formInstanceID,
                        ErrorRows = new List<ErrorRow>()
                    };

                    JObject sectionData = formData != null ? (JObject)formData[section.GeneratedName] : null;
                    var sectionValidator = new SectionValidator(formData, section, sectionData, activeValidations, null, rules, duplicationChecks, masterListData, _isSOTView, _isAnchor, this.formInstanceID, _formDataInstanceManager,_folderVersionServices,_formDesignServices,_formInstanceService);
                    sectionValidator.Validate(ref errorSection, ref maxAllowedErrorCount, isCBCMasterList);

                    errorSectionList.Add(errorSection);

                }
            }
            return errorSectionList.Where(e => e.ErrorRows != null && e.ErrorRows.Any()).ToList();
        }

        public List<ErrorSection> ValidateSection(bool isCBCMasterList, bool isPortfolio, string sectionName)
        {
            List<ErrorSection> errorSectionList = new List<ErrorSection>();
            int maxAllowedErrorCount;
            int.TryParse(System.Web.Configuration.WebConfigurationManager.AppSettings["MaxAllowedErrorListCount"], out maxAllowedErrorCount);
            maxAllowedErrorCount = maxAllowedErrorCount == 0 ? -1 : maxAllowedErrorCount;
            if (formDesign != null)
            {
                List<ValidationDesign> activeValidations = formDesign.Validations.Where(v => v.IsActive).ToList();
                List<RuleDesign> rules = formDesign.Rules.ToList();
                List<DuplicationDesign> duplicationChecks = formDesign.Duplications.Where(s => s.CheckDuplicate).ToList();

                var section = formDesign.Sections.Where(s => s.FullName == sectionName).FirstOrDefault();
                if (section != null && maxAllowedErrorCount > 0)
                {
                    ValidationRuleProcessor ruleProcessor = new ValidationRuleProcessor(section.Name, section.FullName, rules.Where(e => e.UIELementID == section.ID).ToList(), formData, null, "",this.formInstanceID,_formDataInstanceManager,_folderVersionServices,_formDesignServices,_formInstanceService);

                    var errorSection = new ErrorSection()
                    {
                        ID = errorSectionList.Count + 1,
                        SectionID = section.Name,
                        Section = section.Label,
                        Form = this.formName,
                        FormInstanceID = formInstanceID,
                        ErrorRows = new List<ErrorRow>()
                    };

                    JObject sectionData = formData != null ? (JObject)formData[section.GeneratedName] : null;
                    var sectionValidator = new SectionValidator(formData, section, sectionData, activeValidations, null, rules, duplicationChecks, masterListData, _isSOTView, _isAnchor, this.formInstanceID, _formDataInstanceManager, _folderVersionServices, _formDesignServices, _formInstanceService);
                    sectionValidator.Validate(ref errorSection, ref maxAllowedErrorCount, isCBCMasterList);

                    errorSectionList.Add(errorSection);

                }
            }
            return errorSectionList.Where(e => e.ErrorRows != null && e.ErrorRows.Any()).ToList();
        }

    }
}