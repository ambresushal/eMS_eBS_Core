using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.caching.Adapters;
using tmg.equinox.infrastructure.util;
using tmg.equinox.schema.Base.Model;
using tmg.equinox.web.VersionManagement;

namespace tmg.equinox.web.Framework.Caching
{
    public class FormInstanceCompressedDataCacheHandler : IDataCacheHandler
    {
        private ICompressionBase handler = null;
        private FormInstanceSectionDataCacheHandler _sectionDataHandler;

        public FormInstanceCompressedDataCacheHandler()
        {
            handler = CompressionFactory.GetCompressionFactory(CompressionType.JSON, null, "", "", "");
            this._sectionDataHandler = new FormInstanceSectionDataCacheHandler();
        }

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
            GlavCacheWrapper.Add(key, handler.Compress(formData));
        }

        public string Add(int tenantId, int formInstanceId, FormDesignVersionDetail detail, IFolderVersionServices _folderVersionServices, int? userId)
        {
            string defaultJSONData = detail.GetDefaultJSONDataObject();

            //if a document is not cached, get it from the database
            string jsonData = _folderVersionServices.GetFormInstanceDataCompressed(tenantId, formInstanceId);

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
            string compressedJson = handler.Compress(jsonData).ToString();
            GlavCacheWrapper.Add(key, compressedJson);

            return compressedJson;
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
            return handler.Decompress(cacheData).ToString();
        }

        public string IsExists(int tenantId, int formInstanceId, int? userId)
        {
            string key = string.Format(keyPrefix, formInstanceId, userId);
            string cacheData = GlavCacheWrapper.Get<string>(key);
            if (cacheData != null)
            {
                return handler.Decompress(cacheData).ToString();
            }
            else
            {
                return null;
            }

        }

        public void AddMultiple(int tenantId, IFolderVersionServices _folderVersionServices, IFormDesignService _formDesignServices, List<FormInstanceViewModel> formInstancesData, int? userId)
        {

            FormDesignDataCacheHandler formDesigncacheHandler = new FormDesignDataCacheHandler();
            foreach (var formInstance in formInstancesData)
            {
                if (!(String.IsNullOrEmpty(formInstance.FormData)))
                {
                    this.Add(tenantId, formInstance.FormInstanceID, userId, formInstance.FormData);
                }
            }

        }

        public string GetSection(int formInstanceId, string sectionName, int? userId)
        {
            string key = string.Format(keyPrefix, formInstanceId, userId);
            string sectionData = null;
            string cacheData = GlavCacheWrapper.Get<string>(key);
            if (cacheData != null)
            {
                cacheData = handler.Decompress(cacheData).ToString();
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
                cacheData = handler.Decompress(cacheData).ToString();
                JObject formData = JObject.Parse(cacheData);
                formData[sectionName] = JObject.Parse(sectionData);
                string jsonData = JsonConvert.SerializeObject(formData);
                GlavCacheWrapper.Add(key, handler.Compress(jsonData));
                return jsonData;
            }
            return null;
        }

        public ServiceResult Save(IFolderVersionServices _folderVersionServices, int tenantId, int folderVersionId, int formInstanceId, string formInstanceData, string userName, int? userId, IReportingDataService _reportingDataService, int formDesignId, int formDesignVersionId)
        {
            string key = string.Format(keyPrefix, formInstanceId, userId);
            ServiceResult result = new ServiceResult();

            bool IsSaved = _folderVersionServices.SaveFormInstanceDataCompressed(formInstanceId, formInstanceData, folderVersionId, userName);
            result.Result = ServiceResultStatus.Success;

            return result;
        }
    }
}