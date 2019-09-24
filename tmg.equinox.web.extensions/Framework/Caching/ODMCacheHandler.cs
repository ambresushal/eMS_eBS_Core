using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.caching.Adapters;

namespace tmg.equinox.web.Framework.Caching
{
    public class ODMCacheHandler
    {
        private string FolderVersionKey = "ODMFV-{0}";
        private string KeyForOpenFolderVersion = "ODMFVOPN-{0}";
        public void Add(int folderVersionId)
        {
            string key = string.Format(FolderVersionKey, Convert.ToString(folderVersionId));
            GlavCacheWrapper.Add(key, Convert.ToString(folderVersionId));

            string OpenFolderVersion = string.Format(KeyForOpenFolderVersion, Convert.ToString(folderVersionId));
            GlavCacheWrapper.Add(OpenFolderVersion, Convert.ToString(folderVersionId));
        }

        public bool Remove(int folderVersionId)
        {
            string key = string.Format(FolderVersionKey, folderVersionId);
            GlavCacheWrapper.Remove(key);

            string OpenFolderVersion = string.Format(KeyForOpenFolderVersion, folderVersionId);
            GlavCacheWrapper.Remove(OpenFolderVersion);
            return true;
        }

        public string IsExists(int folderVersionId)
        {
            string key = string.Format(FolderVersionKey, Convert.ToString(folderVersionId));
            string cacheData = GlavCacheWrapper.Get<string>(key);
            return cacheData;
        }

        public bool RemoveOpenFolderVersion(int folderVersionId)
        {
            string key = string.Format(KeyForOpenFolderVersion, folderVersionId);
            GlavCacheWrapper.Remove(key);
            return true;
        }

        public string IsExistsOpenFolderVersion(int folderVersionId)
        {
            string key = string.Format(KeyForOpenFolderVersion, Convert.ToString(folderVersionId));
            string cacheData = GlavCacheWrapper.Get<string>(key);
            return cacheData;
        }
    }
}