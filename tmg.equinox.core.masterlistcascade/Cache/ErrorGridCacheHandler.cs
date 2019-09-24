using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.caching.Adapters;

namespace tmg.equinox.caching
{
    public class ErrorGridCacheHandler
    {
        private string ErroGridKeyPrefix = "FIEG-{0}:UID-{1}";

        public void AddErrorGridData(int tenantId, int formInstanceId, int? userId, object errorGridData)
        {
            string key = string.Format(ErroGridKeyPrefix, formInstanceId, userId);
            GlavCacheWrapper.Add(key, JsonConvert.SerializeObject(errorGridData));
        }

        public bool ErrorGridDataRemove(int formInstanceId, int? userId)
        {
            string key = string.Format(ErroGridKeyPrefix, formInstanceId, userId);
            GlavCacheWrapper.Remove(key);
            return true;
        }

        public string GetErrorGridData(int tenantId, int formInstanceId, bool reloadData, FormDesignVersionDetail detail, IFolderVersionServices _folderVersionServices, int? userId)
        {
            string key = string.Format(ErroGridKeyPrefix, formInstanceId, userId);
            string cacheData = GlavCacheWrapper.Get<string>(key);

            if (reloadData || cacheData == null)
            {
                cacheData = string.Empty;
            }

            return cacheData;
        }
    }
}