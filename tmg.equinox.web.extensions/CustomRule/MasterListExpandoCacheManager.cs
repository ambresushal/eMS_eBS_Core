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
    public class MasterListExpandoCacheManager
    {
        private IFolderVersionServices _folderVersionServices;
        private int _tenantId;
        private int _formInstanceId;
        private int _formDesignVersionId;
        //private int _folderVersionId;
        private string keyPrefix = "MLE:UID-{0}";

        public MasterListExpandoCacheManager(int tenantId, int formInstanceId, int formDesignVersionId, IFolderVersionServices folderVersionServices)
        {
            this._tenantId = tenantId;
            if (tenantId == 0) 
            {
                this._tenantId = 1; //set to 1 , the default
            }
            this._formInstanceId = formInstanceId;
            this._formDesignVersionId = formDesignVersionId;
            //this._folderVersionId = folderVersionId;
            this._folderVersionServices = folderVersionServices;
        }

        public ExpandoObject GetMasterListData()
        {
            //int sourceFormInstanceID = this.GetGetSourceFormInstanceID();
            //string key = string.Format(keyPrefix, sourceFormInstanceID);
            string key = string.Format(keyPrefix, this._formInstanceId);

            object obj = GlavCacheWrapper.Get<ExpandoObject>(key);
            ExpandoObject masterListObject = null;
            if (obj != null) 
            {
                masterListObject = (ExpandoObject)obj;
            }
            if (masterListObject == null)
            {
                string masterListData = _folderVersionServices.GetFormInstanceData(_tenantId, this._formInstanceId);
                ExpandoObjectConverter converter = new ExpandoObjectConverter();
                masterListObject = JsonConvert.DeserializeObject<ExpandoObject>(masterListData, converter);
                GlavCacheWrapper.Add(key, masterListObject);
            }
            return masterListObject;
        }

        public void RefreshCache(int formInstanceId, ExpandoObject data)
        {
            string key = string.Format(keyPrefix, formInstanceId);
            object obj = GlavCacheWrapper.Get<ExpandoObject>(key);
            ExpandoObject masterListObject = null;
            if (obj != null)
            {
                masterListObject = (ExpandoObject)obj;
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
        //    sourceFormInstanceID = _folderVersionServices.GetSourceFormInstanceID(_formInstanceId, _formDesignVersionId, _folderVersionId, masterListFormDesignID.Value);
        //    return sourceFormInstanceID;
        //}
    }
}