using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IFolderLockService
    {
        bool IsFolderLocked(int folderId, int? userId);
        ServiceResult OverrideFolderLock(int folderId, int? userId);
        ServiceResult ReleaseFolderLock(int folderId, int? userId);
        FolderVersionViewModel GetFolderLockStatus(int? folderId, int? userId);
        ServiceResult UpdateFolderLockStatus(int? userId, int? folderId);
    }
}
