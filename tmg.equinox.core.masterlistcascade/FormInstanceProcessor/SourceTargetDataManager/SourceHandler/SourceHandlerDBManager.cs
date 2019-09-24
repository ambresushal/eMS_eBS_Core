using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.forminstanceprocessor.expressionbuilder;
using tmg.equinox.ruleprocessor;
using tmg.equinox.ruleprocessor.formdesignmanager;


namespace tmg.equinox.expressionbuilder
{
    public class SourceHandlerDBManager
    {
        IFolderVersionServices _folderVersionServices;
        FormInstanceDataManager _formDataInstanceManager;
        IFormDesignService _formDesignServices;
        IMasterListService _masterListService;
        int _tenantId;


        public SourceHandlerDBManager(int tenantId, IFolderVersionServices folderVersionServices, FormInstanceDataManager formDataInstanceManager, IFormDesignService formDesignServices, IMasterListService masterListService)
        {
            _folderVersionServices = folderVersionServices;
            _formDataInstanceManager = formDataInstanceManager;
            _formDesignServices = formDesignServices;
            _tenantId = tenantId;
            _masterListService = masterListService;
        }

        public string GetSectionData(string sectionName, int formInstanceId)
        {
            FormInstanceViewModel formInstanceDetail = _folderVersionServices.GetFormInstance(_tenantId, formInstanceId);
            FormDesignVersionDetail designDetails = GetFormDesignVersionDetails(formInstanceDetail.FormDesignVersionID);
            return _formDataInstanceManager.GetSectionData(formInstanceId, sectionName, false, designDetails, false, false);
        }

        public string GetSectionData(string sectionName, int formInstanceId, CurrentRequestContext requestContext)
        {
            int formDesignVersionId = 0;
            if (requestContext.FormInstanceDesignVersionMap.ContainsKey(formInstanceId) == false)
            {
                FormInstanceViewModel formInstanceDetail = _folderVersionServices.GetFormInstance(_tenantId, formInstanceId);
                formDesignVersionId = formInstanceDetail.FormDesignVersionID;
                requestContext.FormInstanceDesignVersionMap.Add(formInstanceId, formDesignVersionId);
            }
            else
            {
                formDesignVersionId = requestContext.FormInstanceDesignVersionMap[formInstanceId];
            }

            FormDesignVersionDetail designDetails = null;
            if (requestContext.FormDesignVersionMaps.ContainsKey(formDesignVersionId) == false)
            {
                designDetails = GetFormDesignVersionDetails(formDesignVersionId);
                requestContext.FormDesignVersionMaps.Add(formDesignVersionId, designDetails);
            }
            else
            {
                designDetails = requestContext.FormDesignVersionMaps[formDesignVersionId];
            }
            return _formDataInstanceManager.GetSectionData(formInstanceId, sectionName, false, designDetails, false, false);
        }

        public JObject GetMasterListData(int folderVersionId, List<string> sections, string ruleAlias, FormDesignVersionDetail sourceDesign)
        {
            MasterListReader masterListReader = new MasterListReader(_tenantId, _masterListService);
            JObject masterSections = masterListReader.GetData(folderVersionId, sections, _formDataInstanceManager, _folderVersionServices, _formDesignServices, ruleAlias, sourceDesign);
            return masterSections;
        }

        public void UpdateProcessedRuleSection(int formInstanceId, string sectionName, string sectionData)
        {
            _formDataInstanceManager.SetCacheData(formInstanceId, sectionName, sectionData);
        }

        public bool IsMasterList(string formName)
        {
            return _formDesignServices.isMasterList(formName);
        }

        private int GetMasterListFormInstanceID(FormInstanceViewModel formInstanceViewModel)
        {
            int sourceFormInstanceId = _folderVersionServices.GetSourceFormInstanceID(formInstanceViewModel.FormInstanceID, formInstanceViewModel.FormDesignVersionID, formInstanceViewModel.FolderVersionID, formInstanceViewModel.FormDesignID);
            return sourceFormInstanceId;
        }

        private FormDesignVersionDetail GetFormDesignVersionDetails(int formDesignVersionId)
        {
            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(_tenantId, formDesignVersionId, _formDesignServices);
            FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(true);
            return detail;
        }

    }
}
