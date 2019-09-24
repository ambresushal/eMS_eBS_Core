using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;

namespace tmg.equinox.caching
{
    public interface IDataCacheHandler
    {
        bool IsNewFormInstance
        {
            get;
            set;
        }
        void Add(int tenantId, int formInstanceId, int? userId, string formData);

        string Add(int tenantId, int formInstanceId, FormDesignVersionDetail detail, IFolderVersionServices _folderVersionServices, int? userId);

        bool Remove(int formInstanceId, int? userId);

        string Get(int tenantId, int formInstanceId, bool reloadData, FormDesignVersionDetail detail, IFolderVersionServices _folderVersionServices, int? userId);

        string IsExists(int tenantId, int formInstanceId, int? userId);

        void AddMultiple(int tenantId, IFolderVersionServices _folderVersionServices, IFormDesignService _formDesignServices, List<FormInstanceViewModel> formInstancesData, int? userId);

        string GetSection(int formInstanceId, string sectionName, int? userId);

        string UpdateSection(int formInstanceId, string sectionName, string sectionData, int? userId);

        ServiceResult Save(IFolderVersionServices _folderVersionServices, int tenantId, int folderVersionId, int formInstanceId, string formInstanceData, string userName);
    }
}
