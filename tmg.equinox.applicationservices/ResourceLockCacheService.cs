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
    public class ResourceLockCacheService : IResourceLockService
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }

        public static List<ResourceLock> _resourceLock = new List<ResourceLock>();
        public ResourceLockCacheService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public bool IsDocumentLocked(int formInstanceId, int? userId)
        {
            bool islocked = false;
            var objFolderLock = _resourceLock.Where(s => s.FormInstanceID == formInstanceId && s.LockedBy == userId).FirstOrDefault();
            if (objFolderLock != null)
            {
                objFolderLock.LockedDate = DateTime.Now;
                islocked = true;
            }
            else
            {
                objFolderLock = _resourceLock.Where(s => s.FormInstanceID == formInstanceId).FirstOrDefault();
                if(objFolderLock == null)
                {
                    islocked = true;
                }
            }
            return islocked;
        }

        public ServiceResult OverrideDocumentLock(int folderId, int? userId, List<int> formInstanceIDs)
        {
            ServiceResult result = new ServiceResult();

            foreach (int formInstanceId in formInstanceIDs)
            {
                var objResourceLock = _resourceLock.Where(s => s.FormInstanceID == formInstanceId).FirstOrDefault();
                if (objResourceLock != null)
                {
                    _resourceLock.Remove(objResourceLock);
                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                    ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Document is already unlocked." } });
                }
            }
            return result;
        }

        public ServiceResult ReleaseFolderLock(int folderId, int? userId)
        {
            ServiceResult result = new ServiceResult();
            List<ResourceLock> objResourceLock = null;

            if (userId == null)
                objResourceLock = _resourceLock.Where(s => s.FolderID == folderId).OrderBy(s => s.FolderID).ToList();
            else
                objResourceLock = _resourceLock.Where(s => s.FolderID == folderId && s.LockedBy == userId).OrderBy(s => s.FolderID).ToList();

            if (objResourceLock != null && objResourceLock.Count > 0)
            {
                foreach (ResourceLock obj in objResourceLock)
                    _resourceLock.Remove(obj);
                result.Result = ServiceResultStatus.Success;
            }
            else
            {
                result.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Folder is already unlocked." } });
            }
            return result;
        }

        public ServiceResult ReleaseDocumentLockOnTabClose(List<int> formInstanceIds, int? userId)
        {
            ServiceResult result = new ServiceResult();
            foreach (int formInstanceId in formInstanceIds)
                result = this.ReleaseDocumentLock(formInstanceId, userId);
            return result;
        }

        public ServiceResult ReleaseDocumentLock(int formInstanceID, int? userId)
        {
            ServiceResult result = new ServiceResult();
            ResourceLock objDocumentLock = null;

            if (userId == null)
                objDocumentLock = _resourceLock.Where(s => s.FormInstanceID == formInstanceID).OrderBy(s => s.FormInstanceID).FirstOrDefault();

            else
                objDocumentLock = _resourceLock.Where(s => s.FormInstanceID == formInstanceID && s.LockedBy == userId).OrderBy(s => s.FormInstanceID).FirstOrDefault();


            if (objDocumentLock != null)
            {
                _resourceLock.Remove(objDocumentLock);
                result.Result = ServiceResultStatus.Success;
            }
            else
            {
                result.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Document is already unlocked." } });
            }
            return result;
        }

        public List<FolderVersionViewModel> GetDocumentLockStatus(int? folderId, int? userId, List<int> formInstanceIDs)
        {
            List<FolderVersionViewModel> models = new List<FolderVersionViewModel>();

            foreach (int formInstanceId in formInstanceIDs)
            {
                FolderVersionViewModel model = (from e in _resourceLock
                                                join u in this._unitOfWork.RepositoryAsync<User>().Get()
                                                on e.LockedBy equals u.UserID
                                                //where e.FolderID == folderId
                                                where e.FormInstanceID == formInstanceId
                                                select new FolderVersionViewModel
                                                {
                                                    LockedBy = e.LockedBy,
                                                    IsLocked = e.IsLocked,
                                                    LockedDate = e.LockedDate,
                                                    LockedByUser = u.UserName,
                                                    FormInstanceID = (int)e.FormInstanceID
                                                }).FirstOrDefault();

                if (model != null && model.LockedBy != userId)
                {
                    DateTime DateEnd = DateTime.Now;
                    DateTime DateStart = DateEnd - new TimeSpan(0, 0, 3, 0);

                    if (model.LockedDate <= DateStart)
                    {
                        ServiceResult result = null;
                        result = ReleaseDocumentLock(Convert.ToInt32(formInstanceId), 0);

                        if (result.Result == ServiceResultStatus.Success)
                            model = null;
                    }
                }

                if (model != null)
                {
                    if (model.LockedBy == userId)
                        model.IsLocked = false;
                    model.FormInstanceID = formInstanceId;
                    models.Add(model);
                }
            }
            return models;
        }

        public ServiceResult UpdateDocumentLockStatus(int? folderId, int? userId, List<int> formInstanceIDs)
        {
            ServiceResult result = new ServiceResult();
            foreach (int formInstanceId in formInstanceIDs)
            {
                var objFolderLock = _resourceLock.Where(s => s.FormInstanceID == formInstanceId && s.LockedBy == userId).OrderBy(s => s.FormInstanceID).FirstOrDefault();

                if (objFolderLock != null)
                {
                    if (objFolderLock.IsLocked == true)
                        result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    ResourceLock resourceLock = new ResourceLock();
                    resourceLock.FolderID = folderId ?? 0;
                    resourceLock.IsLocked = true;
                    resourceLock.LockedBy = userId;
                    resourceLock.TenantID = 1;
                    resourceLock.FormInstanceID = formInstanceId;
                    resourceLock.LockedDate = DateTime.Now;
                    _resourceLock.Add(resourceLock);
                    result.Result = ServiceResultStatus.Success;
                }
            }
            return result;
        }

        public List<ResourceLock> GetLockedDocuments(int? userId)
        {
            List<ResourceLock> list = (from e in _resourceLock
                                       join u in this._unitOfWork.RepositoryAsync<User>().Get()
                                       on e.LockedBy equals u.UserID
                                       select new ResourceLock
                                       {
                                           FolderID = e.FolderID,
                                           IsLocked = e.IsLocked,
                                           LockedBy = e.LockedBy,
                                           LockedDate = e.LockedDate,
                                           FormInstanceID = e.FormInstanceID,
                                           LockedUserName = u.UserName
                                       }).ToList();

            //List<ResourceLock> list = _resourceLock.Where(c => c.LockedBy != userId).ToList();
            return list.Where(c => c.LockedBy != userId).ToList();
        }
    }
}
