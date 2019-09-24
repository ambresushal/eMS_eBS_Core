using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.caching.Adapters;
using tmg.equinox.web.FormInstance;
using tmg.equinox.web.Framework.Caching;

namespace tmg.equinox.web.CustomRule
{
    public class MasterListCacheManager
    {
        //private IFolderVersionServices _folderVersionServices;
        private IMasterListService _masterListService;
        private int _tenantId;
        private int _formInstanceId;
        //private int _formDesignVersionId;
        //private int _folderVersionId;
        private string keyPrefix = "ML:UID-{0}";

        public MasterListCacheManager(int tenantId, int formInstanceId, IMasterListService masterListService)
        {
            this._tenantId = tenantId;
            if (tenantId == 0) 
            {
                this._tenantId = 1; //set to 1 , the default
            }
            this._formInstanceId = formInstanceId;
            //this._formDesignVersionId = formDesignVersionId;
            //this._folderVersionId = folderVersionId;
            this._masterListService = masterListService;
        }
        public JObject GetMasterListData()
        {
            string key = string.Format(keyPrefix, _formInstanceId);
            object obj = GlavCacheWrapper.Get<JObject>(key);
            JObject masterListObject = null;
            if (obj != null)
            {
                masterListObject = (JObject)obj;
            }
            if (masterListObject == null)
            {
                string masterListData = _masterListService.GetFormInstanceData(_tenantId, _formInstanceId);
                if (!string.IsNullOrEmpty(masterListData))
                {
                    masterListObject = JObject.Parse(masterListData);
                    GlavCacheWrapper.Add(key, masterListObject);
                }
            }
            return masterListObject;
        }
        public JObject GetMasterListDataOld()
        {
            //int sourceFormInstanceID = this.GetGetSourceFormInstanceID();
            JObject masterListObjectAll = new JObject();
            DateTime folderEffectiveDate = _masterListService.GetEffectiveDate(1);//_folderVersionId
            List<int> sourceFormInstanceIds = _masterListService.GetFormInstanceIds(folderEffectiveDate);
            foreach (var sourceFormInstanceId in sourceFormInstanceIds)
            {
                //string key = string.Format(keyPrefix, sourceFormInstanceID);
                string sectionName = _masterListService.GetSectionNameFromFormInstanceID(_tenantId, sourceFormInstanceId);
                string key = string.Format(keyPrefix, sourceFormInstanceId);
                object obj = GlavCacheWrapper.Get<JObject>(key);
                JObject masterListObject = (JObject)obj;
                if (obj != null)
                {
                    masterListObjectAll[sectionName] = (JObject)obj;
                }
                if (masterListObject == null)
                {
                    //string masterListData = _folderVersionServices.GetFormInstanceData(_tenantId, sourceFormInstanceID);
                    string masterListData = _masterListService.GetFormInstanceData(_tenantId, _formInstanceId);
                    
                    if (!string.IsNullOrEmpty(masterListData))
                    {
                        masterListObject = JObject.Parse(masterListData);
                        GlavCacheWrapper.Add(key, masterListObject);
                        masterListObjectAll[sectionName] = masterListObject;
                    }
                }
            }
            return masterListObjectAll;
        }

        public void RefreshCache(int formInstanceId, JObject data)
        {
            string key = string.Format(keyPrefix, formInstanceId);
            object obj = GlavCacheWrapper.Get<JObject>(key);
            JObject masterListObject = null;
            if (obj != null)
            {
                masterListObject = (JObject)obj;
            }
            if (masterListObject != null)
            {
                GlavCacheWrapper.Remove(key);
            }
            GlavCacheWrapper.Add(key, data);
        }

        //private int GetGetSourceFormInstanceID()
        //{
        //    int sourceFormInstanceID = 0;
        //    int? masterListFormDesignID = 0;
        //    masterListFormDesignID = _folderVersionServices.GetMasterListFormDesignID(_folderVersionId);
        //    sourceFormInstanceID = _folderVersionServices.GetSourceFormInstanceID(_formInstanceId, _formDesignVersionId, _folderVersionId,   masterListFormDesignID.Value);
        //    return sourceFormInstanceID;
        //}
    }
}