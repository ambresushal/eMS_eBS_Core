using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.CustomRule;
using tmg.equinox.web.DataSource;
using tmg.equinox.web.DataSource.SyncManager;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.Framework;
using tmg.equinox.web.Framework.Caching;
using tmg.equinox.web.Framework.MasterList;
using tmg.equinox.web.RuleEngine;
using tmg.equinox.web.VersionManagement;

namespace tmg.equinox.web.Handler
{
    public class FormInstanceDataHandler
    {
        private int _tenantId = 1;
        private int _formInstanceId;
        private int _folderVersionId;
        private int _formDesignVersionId;
        private bool _isFolderReleased;
        private JObject _oldFormData;
        private string _newFormData;
        private string _formDesign;
        private IFolderVersionServices _folderVersionServices;
        private IFormDesignService _formDesignService;
        private IMasterListService _masterListService;
        private int? _currentUserId;
        public bool IsNewFormInstance = false;        

        IDataCacheHandler _cacheHandler { get; set; }

        public FormInstanceDataHandler(IFolderVersionServices folderVersionServices, IFormDesignService formDesignService, int formInstanceId, int folderVersionId, int formDesignVersionId, bool isFolderReleased, int? userId, IMasterListService masterListService)
        {
            this._formInstanceId = formInstanceId;
            this._folderVersionId = folderVersionId;
            this._formDesignVersionId = formDesignVersionId;
            this._isFolderReleased = isFolderReleased;
            this._folderVersionServices = folderVersionServices;
            this._formDesignService = formDesignService;
            this._currentUserId = userId;
            this._masterListService = masterListService;
        }

        public FormDesignVersionDetail ExecuteDocumentHandler(bool reloadData)
        {
            FormDesignVersionDetail _detail = this.GetFormDesignVersion();

            bool isRefreshCache = _detail.FormDesignId == CustomRuleConstants.MasterListFormDesignID ? true : reloadData;
            
            string jsonData = this.GetFormInstanceData(_detail, isRefreshCache);

            _detail.errorGridData = this.GeFormInstanceErrorGridData(isRefreshCache);

            _detail = this.preProcessFormData(_detail, jsonData);

            if (!this._isFolderReleased)
            {
                if (_detail.FormDesignId == CustomRuleConstants.MasterListFormDesignID)
                {
                    _newFormData = _detail.JSONData;
                }
                else
                {
                    //Execute Data Source Synchronizer           
                    this.SyncDataSources(_detail.JSONData, _detail, true, "", "");
                }

                //Execute Configured Rules            
                this.ExecuteConfiguredRules(_detail);

                //Execute Custom Rules
                //this.ExecuteCustomRulesForForm(_detail);

                //Update Cache
                _cacheHandler.Add(_tenantId, _formInstanceId, _currentUserId, _newFormData);
                _detail.JSONData = _newFormData;
            }
            else
            {
                _detail.JSONData = jsonData;
            }

            return _detail;
        }

        public CustomRuleResult ExecuteSectionDataHandler(string sectionName, string sectionData)
        {
            FormDesignVersionDetail _detail = this.GetFormDesignVersion();
            string jsonData = this.GetFormInstanceData(_detail, false);

            //Execute Data Source Synchronizer
            var syncUpdatedSections = this.SyncDataSources(jsonData, _detail, false, sectionName, sectionData);

            //Execute Configured Rules
            this.ExecuteConfiguredRules(_detail);

            //Execute Custom Rules
            var returnResult = this.ExecuteCustomRulesForSection(_detail, sectionName, sectionData);

            //Update Cache
            _cacheHandler.Add(_tenantId, _formInstanceId, _currentUserId, _newFormData);
            _detail.JSONData = _newFormData;

            returnResult.updatedSections = GetFinalListOfUpdatedSections(syncUpdatedSections, returnResult.updatedSections);

            return returnResult;
        }

        private FormDesignVersionDetail preProcessFormData(FormDesignVersionDetail detail, string jsonData)
        {
            detail.IsNewFormInstance = this.IsNewFormInstance;
            string defaultJSONData = detail.GetDefaultJSONDataObject();

            //sync data to latest design
            if (!String.IsNullOrEmpty(jsonData) && this.IsNewFormInstance == false)
            {
                VersionDataSynchronizer syncMgr = new VersionDataSynchronizer(jsonData, defaultJSONData);
                if (syncMgr.isSyncRequired() == true)
                {
                    detail.JSONData = syncMgr.Synchronize();
                }
                else 
                {
                    detail.JSONData = jsonData; 
                }
            }
            else
            {
                detail.JSONData = defaultJSONData;
            }

            //Run Data Mapper - Sync with other form's data
            if (detail.DataSources != null && detail.DataSources.Count > 0)
            {
                //DataSourceMapper dm = new DataSourceMapper(_tenantId, _formInstanceId, _folderVersionId, detail.FormDesignId, _formDesignVersionId, _isFolderReleased, _folderVersionServices, detail.JSONData, detail);
                //dm.AddDataSourceRange(detail.DataSources);
                //detail.JSONData = dm.MapDataSources();
            }

            return detail;
        }

        private FormDesignVersionDetail GetFormDesignVersion()
        {
            FormDesignVersionDetail detail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(_tenantId, _formDesignVersionId, _formDesignService);
            return detail;
        }

        private string GetFormInstanceData(FormDesignVersionDetail detail, bool reloadData)
        {
            _cacheHandler = DataCacheHandlerFactory.GetHandler();
            string jsonData = _cacheHandler.Get(_tenantId, _formInstanceId, reloadData, detail, _folderVersionServices, _currentUserId);
            this.IsNewFormInstance = _cacheHandler.IsNewFormInstance;

            _oldFormData = JObject.Parse(jsonData);
            return jsonData;
        }

        private string GeFormInstanceErrorGridData(bool reloadData)
        {
            ErrorGridCacheHandler cacheHandler = new ErrorGridCacheHandler();
            string cacheErrorGridData = cacheHandler.GetErrorGridData(_tenantId, _formInstanceId, reloadData, null, _folderVersionServices, _currentUserId);
           
           return cacheErrorGridData;
            
        }

        private List<SectionResult> SyncDataSources(string jsonData, FormDesignVersionDetail detail, bool SyncForm, string sectionName, string sectionData)
        {
            DataSourceSynchManager syncManager = new DataSourceSynchManager(_oldFormData, jsonData, detail);
            if (SyncForm)
            {
                _newFormData = syncManager.SyncFormDataSources();
            }
            else
            {
                _newFormData = syncManager.SyncSectionDataSources(sectionName, sectionData);
            }
            return syncManager.GetUpdatedSections();
        }

        private void ExecuteConfiguredRules(FormDesignVersionDetail detail)
        {
            //RuleManager ruleMgr = new RuleManager(detail.Rules, _newFormData);
            //_newFormData = ruleMgr.ExecuteRules();
        }

        private void ExecuteCustomRulesForForm(FormDesignVersionDetail detail)
        {
            ICustomRuleHandler handler = CustomRuleFactory.GetHandler(detail.FormDesignId, _newFormData);
            if (handler != null)
            {
                //get Master List Data
                //int sourceFormInstanceID = _folderVersionServices.GetSourceFormInstanceID(_formInstanceId, _formDesignVersionId, _folderVersionId, CustomRuleConstants.MasterListFormDesignID);
                //string masterListData = _folderVersionServices.GetFormInstanceData(_tenantId, sourceFormInstanceID);
                //handler.SetMasterListData(masterListData);

                //_newFormData = handler.RunRulesForDocument(_oldFormData);
            }
        }

        private CustomRuleResult ExecuteCustomRulesForSection(FormDesignVersionDetail detail, string sectionName, string sectionData)
        {
            JObject data = JObject.Parse(_newFormData);
            CustomRuleResult returnResult = new CustomRuleResult();
            ICustomRuleHandler handler = CustomRuleFactory.GetHandler(detail.FormDesignId, _newFormData);

            if (handler != null && handler.HasCustomRules(sectionName, JObject.Parse(sectionData)))
            {
                MasterListReader ms = new MasterListReader(_tenantId, _masterListService);
                JObject masterListData = ms.GetCompleteData(_folderVersionId);
                handler.SetMasterListData(masterListData);
                returnResult = handler.RunRulesForSection(sectionName, JObject.Parse(sectionData));
                if (returnResult.updatedSections != null && returnResult.updatedSections.Count > 0)
                {
                    foreach (var section in returnResult.updatedSections)
                    {
                        data[section.SectionName] = JObject.Parse(section.SectionData);
                    }
                }
                else
                {
                    data[sectionName] = JObject.Parse(sectionData);
                }
            }
            else
            {
                data[sectionName] = JObject.Parse(sectionData);
            }
            _newFormData = JsonConvert.SerializeObject(data);

            return returnResult;
        }

        private List<SectionResult> GetFinalListOfUpdatedSections(List<SectionResult> dataSyncUpdatedSections, List<SectionResult> customRuleUpdatedSections)
        {
            if (customRuleUpdatedSections == null)
            {
                return dataSyncUpdatedSections;
            }
            foreach (var updatedSection in dataSyncUpdatedSections)
            {
                if (!customRuleUpdatedSections.Where(s => s.SectionName == updatedSection.SectionName).Any())
                {
                    customRuleUpdatedSections.Add(updatedSection);
                }
            }
            return customRuleUpdatedSections;
        }
    }
}