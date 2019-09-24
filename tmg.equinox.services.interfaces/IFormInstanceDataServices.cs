using System.Collections.Generic;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IFormInstanceDataServices
    {
        string GetSectionData(int tenantId, int formInstanceId, string sectionName, FormDesignVersionDetail detail, string currentUserName);

        void UpdateSectionData(int tenantId, int formInstanceId, string sectionName, string sectionData);

        ServiceResult SaveFormInstanceSectionsData(int tenantId, string jsonString, string currentUserName);

        string AddNodeToSectionData(string sectionName, string sectionData);

        dynamic IsMasterListFolderVersionRelease(int folderVersionId);

        void SaveDefaultJSONData(int tenantId, int formInstanceId, string defaultJSONData, string currentUserName);

        ServiceResult SaveFormInstanceCommentLog(int formInstanceId, int formDesignId, int formDesignVersionId, string userName, string commentData);
        string GetFormInstanceCommentLog(int formInstanceId);
        //List<FormInstanceAllProductsViewModel> GetFormInstanceProductsList(int formInstanceId);

    }
}
