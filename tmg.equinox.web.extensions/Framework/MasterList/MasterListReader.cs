using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.CustomRule;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.FormInstanceManager;

namespace tmg.equinox.web.Framework.MasterList
{
    public class MasterListReader
    {
        private IMasterListService _masterListService;
        private int _tenantId;

        public MasterListReader(int tenantId, IMasterListService masterListService)
        {
            this._tenantId = tenantId;
            if (tenantId == 0)
            {
                this._tenantId = 1; //set to 1 , the default
            }
            
            this._masterListService = masterListService;
        }

        public JObject GetData(int targetFolderVersionID, List<string> sectionNames, FormInstanceDataManager formDataInstanceManager, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, string ruleAlias, bool isMasterList,string mlDocumentFilter)
        {
            JObject completeML = new JObject();
            DateTime folderEffectiveDate = _masterListService.GetEffectiveDate(targetFolderVersionID);
            List<int> formInstanceIds = _masterListService.GetFormInstanceIds(folderEffectiveDate, ruleAlias,mlDocumentFilter);
                        
            foreach (int formInstanceId in formInstanceIds)
            {
                FormInstanceViewModel formInstanceDetail = folderVersionServices.GetFormInstance(_tenantId, formInstanceId);
                FormDesignVersionDetail designDetails = GetFormDesignVersionDetails(formInstanceDetail.FormDesignVersionID, formDesignServices);

                foreach (string secName in sectionNames)
                {
                    JObject sectionML = null;
                    bool isMasterListAsSource = true;
                    if (isMasterList == designDetails.IsMasterList)
                    {
                        isMasterListAsSource = false;
                    }

                    string secdata = formDataInstanceManager.GetSectionData(formInstanceId, secName, false, designDetails, false, isMasterListAsSource);                    
                    if (secdata != null && secdata != "")
                    {
                        sectionML = JObject.Parse(secdata);
                        completeML.Add(secName, sectionML.SelectToken(secName));                        
                    }                    
                }
            }
            return completeML;
        }
        public JObject GetCompleteData(int targetFolderVersionID)
        {
            JObject completeML = new JObject();
            try
            {
                DateTime folderEffectiveDate = _masterListService.GetEffectiveDate(targetFolderVersionID); //DateTime.Parse("1/1/2020 12:00:00 AM"); //
                List<int> formInstanceIds = _masterListService.GetFormInstanceIds(folderEffectiveDate);
                foreach (int formInstanceId in formInstanceIds)
                {
                    string sectionName = string.Empty;
                    sectionName = _masterListService.GetSectionNameFromFormInstanceID(_tenantId, formInstanceId);
                    MasterListCacheManager cache = new MasterListCacheManager(_tenantId, formInstanceId, _masterListService);
                    var sectionML = cache.GetMasterListData();
                    if(sectionML != null)
                        completeML.Add(sectionName, sectionML.SelectToken(sectionName));
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return completeML;
        }
        public void RefreshCache(int formInstanceId, JObject data)
        {
            MasterListCacheManager cache = new MasterListCacheManager(_tenantId, formInstanceId, _masterListService);
            cache.RefreshCache(formInstanceId, data);
        }

        private FormDesignVersionDetail GetFormDesignVersionDetails(int formDesignVersionId, IFormDesignService formDesignServices)
        {
            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(_tenantId, formDesignVersionId, formDesignServices);
            FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(true);
            return detail;
        }
    }
}