using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public class FolderLockService : IFolderLockService
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }

        public static List<FolderLock> _folderLock = new List<FolderLock>();
        public FolderLockService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public bool IsFolderLocked(int folderId, int? userId)
        {
            bool islocked = false;

            var objFolderLock = _folderLock.Where(s => s.FolderID == folderId && s.LockedBy == userId).FirstOrDefault();
            if (objFolderLock != null)
            {
                objFolderLock.LockedDate = DateTime.Now;
                islocked = true;
            }

            return islocked;
        }

        public ServiceResult OverrideFolderLock(int folderId, int? userId)
        {
            ServiceResult result = new ServiceResult();

            var objFolderLock = _folderLock.Where(s => s.FolderID == folderId).OrderBy(s => s.FolderID).FirstOrDefault();
            if (objFolderLock != null)
            {
                _folderLock.Remove(objFolderLock);
                result.Result = ServiceResultStatus.Success;
            }
            else
            {
                result.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Folder is already unlocked." } });
            }

            return result;
        }

        public ServiceResult ReleaseFolderLock(int folderId, int? userId)
        {
            ServiceResult result = new ServiceResult();
            FolderLock objFolderLock = null;

            if (userId == null)
            {
                objFolderLock = _folderLock.Where(s => s.FolderID == folderId).OrderBy(s => s.FolderID).FirstOrDefault();
            }
            else
            {
                objFolderLock = _folderLock.Where(s => s.FolderID == folderId && s.LockedBy == userId).OrderBy(s => s.FolderID).FirstOrDefault();
            }

            if (objFolderLock != null)
            {
                _folderLock.Remove(objFolderLock);
                result.Result = ServiceResultStatus.Success;
            }
            else
            {
                result.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Folder is already unlocked." } });
            }
            return result;
        }

        public FolderVersionViewModel GetFolderLockStatus(int? folderId, int? userId)
        {
            FolderVersionViewModel model = null;

            model = (from e in _folderLock
                     join u in this._unitOfWork.RepositoryAsync<User>().Get()
                     on e.LockedBy equals u.UserID
                     where e.FolderID == folderId
                     select new FolderVersionViewModel
                     {
                         LockedBy = e.LockedBy,
                         IsLocked = e.IsLocked,
                         LockedDate = e.LockedDate,
                         LockedByUser=u.UserName
                     }).FirstOrDefault();

            if (model != null && model.LockedBy != userId)
            {
                DateTime DateEnd = DateTime.Now;
                DateTime DateStart = DateEnd - new TimeSpan(0, 0, 3, 0);

                if (model.LockedDate <= DateStart)
                {
                    ServiceResult result = null;
                    result = ReleaseFolderLock(Convert.ToInt32(folderId), 0);

                    if (result.Result == ServiceResultStatus.Success)
                    {
                        model = null;
                    }
                }
            }

            if (model != null)
            {
                if (model.LockedBy == userId)
                {
                    model.IsLocked = false;
                }
            }

            return model;

        }

        public ServiceResult UpdateFolderLockStatus(int? userId, int? folderId)
        {
            ServiceResult result = new ServiceResult();

            var objFolderLock = _folderLock.Where(s => s.FolderID == folderId && s.LockedBy == userId).OrderBy(s => s.FolderID).FirstOrDefault();

            if (objFolderLock != null)
            {
                if (objFolderLock.IsLocked == true)
                {
                    result.Result = ServiceResultStatus.Success;
                }
            }
            else
            {

                FolderLock folderLock = new FolderLock();
                folderLock.IsLocked = true;
                folderLock.LockedBy = userId;
                folderLock.TenantID = 1;
                folderLock.FolderID = folderId;
                folderLock.LockedDate = DateTime.Now;
                _folderLock.Add(folderLock);

                result.Result = ServiceResultStatus.Success;
            }

            return result;
        }
    }
}
