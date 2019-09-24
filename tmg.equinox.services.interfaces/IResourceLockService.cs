using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IResourceLockService
    {
        bool IsDocumentLocked(int formInstanceId, int? userId, string sectionName, int formDesignId, bool checkifLockedByOtherUser= false);
        ServiceResult OverrideDocumentLock(int folderId, int? userId, List<int> formInstanceIDs);
        ServiceResult ReleaseFolderLock(int folderId, int? userId);
        ServiceResult ReleaseDocumentLock(int folderId, int? userId);
        List<FolderVersionViewModel> GetDocumentLockStatus(int? folderId, int? userId, List<int> formInstanceID);
        ServiceResult UpdateDocumentLockStatus(int? folderId, int? userId, List<int> formInstanceId, string userName);
        List<ResourceLock> GetLockedDocuments(int? userId);
        ServiceResult ReleaseDocumentLockOnTabClose(List<int> formInstanceIds, int? userId);
		List<ResourceLock> GetLockedDocuments();
        ServiceResult ReleaseDocumentAndSectionLock(int userId);
        ServiceResult ReleaseDocumentAndSectionLock(int formInstanceID, string sectionName);
        List<ResourceLockStateViewModel> GetAllLockIntances(int currentUserID);
        void UpdateNotifyUserList(List<ResourceLockInputModel> resourceLockInputModel, int userId, string userName);
        bool CheckIfFormInstanceasLockingEnabled(int formInstanceId);
    }


    public interface ISectionLockService
    {
        ServiceResult ReleaseFolderLock(int folderId, int? userId);
        ServiceResult ReleaseSectionLock(int formInstanceId, string sectionName);
        bool IsSectionLocked(int formInstanceId, int? userId, string sectionName);

        ServiceResult ReleaseSectionLockonDocumentCloseOrSectionChange(int formInstanceId, int? userId, string previousSectionName);

        ServiceResult ReleaseSectionLock(int userId);

        LockStatus ManageSectionLock(int folderId, int formInstanceId, string displayViewName, string displaySectionName, string sectionName, int userId, string userName, int formDesignID, string formName, bool isMasterList);

        bool IsSectionLevelLockingEnabledForFormDesign(int formDesignId);

        ServiceResult UpdateNotifyUser(int formInstanceId, string sectionName, int userId, string userName);

        void RefeshFormDesignLock();

        bool IsMasterListDesign(int formDesignId);
    }


}
