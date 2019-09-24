﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.caching.Adapters;
using tmg.equinox.synchronizer;

namespace tmg.equinox.caching
{
    public class FormInstanceDataCacheHandler : IDataCacheHandler
    {
        private string keyPrefix = "FI-{0}:UID-{1}";
        public bool HasNewFormInstance = false;

        public bool IsNewFormInstance
        {
            get
            {
                return HasNewFormInstance;
            }
            set
            {
                HasNewFormInstance = value;
            }
        }
        public void Add(int tenantId, int formInstanceId, int? userId, string formData)
        {
            string key = string.Format(keyPrefix, formInstanceId, userId);
            GlavCacheWrapper.Add(key, formData);
        }

        public string Add(int tenantId, int formInstanceId, FormDesignVersionDetail detail, IFolderVersionServices _folderVersionServices, int? userId)
        {


            string defaultJSONData = detail.GetDefaultJSONDataObject();


            //if a document is not cached, get it from the database
            string jsonData = _folderVersionServices.GetFormInstanceData(tenantId, formInstanceId);


            //if json is blank then add default empty object
            if (string.IsNullOrEmpty(jsonData))
            {
                IsNewFormInstance = true;
                jsonData = defaultJSONData;
            }

            if (!String.IsNullOrEmpty(defaultJSONData))
            {

                //sync data to latest design
                VersionDataSynchronizer syncMgr = new VersionDataSynchronizer(jsonData, defaultJSONData);
                if (syncMgr.isSyncRequired() == true)
                {
                    jsonData = syncMgr.Synchronize();
                }
            }

            string key = string.Format(keyPrefix, formInstanceId, userId);
            GlavCacheWrapper.Add(key, jsonData);

            return jsonData;
        }

        public bool Remove(int formInstanceId, int? userId)
        {
            string key = string.Format(keyPrefix, formInstanceId, userId);
            GlavCacheWrapper.Remove(key);
            return true;
        }

        public string Get(int tenantId, int formInstanceId, bool reloadData, FormDesignVersionDetail detail, IFolderVersionServices _folderVersionServices, int? userId)
        {

            string key = string.Format(keyPrefix, formInstanceId, userId);
            string cacheData = GlavCacheWrapper.Get<string>(key);

            if (reloadData || cacheData == null)
            {
                cacheData = this.Add(tenantId, formInstanceId, detail, _folderVersionServices, userId);
            }

            return cacheData;
        }

        public string IsExists(int tenantId, int formInstanceId, int? userId)
        {
            string key = string.Format(keyPrefix, formInstanceId, userId);
            string cacheData = GlavCacheWrapper.Get<string>(key);
            return cacheData;
        }

        public void AddMultiple(int tenantId, IFolderVersionServices _folderVersionServices, IFormDesignService _formDesignServices, List<FormInstanceViewModel> formInstancesData, int? userId)
        {

            FormDesignDataCacheHandler formDesigncacheHandler = new FormDesignDataCacheHandler();
            foreach (var formInstance in formInstancesData)
            {
                this.Add(tenantId, formInstance.FormInstanceID, userId, formInstance.FormData);
            }

        }

        public string GetSection(int formInstanceId, string sectionName, int? userId)
        {
            string key = string.Format(keyPrefix, formInstanceId, userId);
            string sectionData = null;
            string cacheData = GlavCacheWrapper.Get<string>(key);
            if (cacheData != null)
            {
                Dictionary<string, object> dataDict = new Dictionary<string, object>();
                dataDict.Add(sectionName, JObject.Parse(cacheData)[sectionName]);
                sectionData = JsonConvert.SerializeObject(dataDict);
            }
            return sectionData;
        }

        public string UpdateSection(int formInstanceId, string sectionName, string sectionData, int? userId)
        {
            string key = string.Format(keyPrefix, formInstanceId, userId);
            string cacheData = GlavCacheWrapper.Get<string>(key);

            if (cacheData != null)
            {
                JObject formData = JObject.Parse(cacheData);
                formData[sectionName] = JObject.Parse(sectionData);
                GlavCacheWrapper.Add(key, JsonConvert.SerializeObject(formData));
                return JsonConvert.SerializeObject(cacheData);
            }
            return null;
        }

        public ServiceResult Save(IFolderVersionServices _folderVersionServices, int tenantId, int folderVersionId, int formInstanceId, string formInstanceData, string userName)
        {
            return _folderVersionServices.SaveFormInstanceData(tenantId, folderVersionId, formInstanceId, formInstanceData, userName);
        }
    }
}