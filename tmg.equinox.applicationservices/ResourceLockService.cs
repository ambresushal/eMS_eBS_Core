using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.notification;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{


    public class ResourceLockService : IResourceLockService
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }

        INotificationService _notificationService;

        public static List<ResourceLock> _resourceLock = new List<ResourceLock>();

        public static List<ResourceLockState> _resourceLockState = new List<ResourceLockState>();

        public static List<FormDesignLockSection> _formDesignLockInfo = new List<FormDesignLockSection>();

        public ResourceLockService(IUnitOfWorkAsync unitOfWork, INotificationService notificationService)
        {
            this._unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public bool IsDocumentLocked(int formInstanceId, int? userId, string sectionName)
        {
            bool islocked = false;
            if (string.IsNullOrEmpty(sectionName))
            {
                var objFolderLock = _resourceLock.Where(s => s.FormInstanceID == formInstanceId && s.LockedBy == userId).FirstOrDefault();
                if (objFolderLock != null)
                {
                    objFolderLock.LockedDate = DateTime.Now;
                    islocked = true;
                }
                else
                {
                    objFolderLock = _resourceLock.Where(s => s.FormInstanceID == formInstanceId).FirstOrDefault();
                    if (objFolderLock == null)
                    {
                        islocked = false;
                    }
                }
            }
            if (islocked == false && !string.IsNullOrEmpty(sectionName))
            {
                var isSectionlocked = IsSectionLocked(formInstanceId, userId, sectionName);
                if (isSectionlocked)
                {
                    islocked = true;
                }
            }
            return islocked;
        }

        public bool IsSectionLocked(int formInstanceId, int? userId, string sectionName)
        {
            bool islocked = false;
            var objSectionLock = _resourceLockState.Where(s => s.FormInstanceID == formInstanceId && s.LockedBy == userId && s.SectionName == sectionName).FirstOrDefault();
            if (objSectionLock != null)
            {
                objSectionLock.LockedDate = DateTime.Now;
                islocked = true;
            }
            else
            {
                objSectionLock = _resourceLockState.Where(s => s.FormInstanceID == formInstanceId && s.SectionName == sectionName && s.LockedBy != userId).FirstOrDefault();
                if (objSectionLock == null)
                {
                    islocked = false;
                }
                else
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
            List<ResourceLockState> sectionLevelLocks = null;

            if (userId == null)
            {
                objResourceLock = _resourceLock.Where(s => s.FolderID == folderId).OrderBy(s => s.FolderID).ToList();
                sectionLevelLocks = _resourceLockState.Where(row => row.FolderID == folderId).ToList();
            }
            else
            {
                objResourceLock = _resourceLock.Where(s => s.FolderID == folderId && s.LockedBy == userId).OrderBy(s => s.FolderID).ToList();
                sectionLevelLocks = _resourceLockState.Where(row => row.FolderID == folderId && row.LockedBy == userId).ToList();
            }

            if ((objResourceLock != null && objResourceLock.Count > 0) || sectionLevelLocks.Count > 0)
            {
                foreach (ResourceLock obj in objResourceLock)
                    _resourceLock.Remove(obj);
                foreach (ResourceLockState obj in sectionLevelLocks)
                    _resourceLockState.Remove(obj);
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
            ReleaseSectionLockonDocumentCloseOrSectionChange(formInstanceID, userId, string.Empty);
            return result;
        }

        public List<FolderVersionViewModel> GetDocumentLockStatus(int? folderId, int? userId, List<int> formInstanceIDs)
        {

            // check docid & section is locked by another user
            // 
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

        /// <summary>
        /// For a given instanceId If Section Name exists in collection of other UserId then mark ReadOnly and send notification "locked by user"
        /// for a given instanceId, if Section Name Does not exists in collection of other UserID then lock the section for a given instanceId
        /// </summary>
        /// <param name="formInstanceId"></param>
        /// <param name="viewName"></param>
        /// <param name="sectionName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>


        public LockStatus ManageSectionLock(int folderId, int formInstanceId, string displayViewName, string displaySectionName, string sectionName, int userId, string userName, int formDesignID, string formName, bool isMasterList)
        {
            
            //check if formInstance is of Collateral Type
            if (!IsSectionLevelLockingEnabledForFormDesign(formDesignID))
            {
                return new LockStatus { MarkReadOnly = false };
            }
            var objFolderLock = _resourceLockState.Where(s => s.FormInstanceID == formInstanceId && s.LockedBy != userId && s.SectionName == sectionName).FirstOrDefault();
            //NJ:TODO: also check in collection 
            if (objFolderLock != null)
            {
                if (objFolderLock.IsLocked == true)
                {
                    var paramaters = new List<Paramters>();
                    paramaters.Add(new Paramters { key = "user", Value = "objFolderLock.LockedByUserName" });

                    var notificationMsg = new NotificationInfo { loggedInUserName = userName, MessageKey = "MessageKey.LOCK", UserID = userId, ParamterValues = paramaters, SentTo = userName };

                    _notificationService.SendNotification(notificationMsg);
                    string message = null;
                    if (objFolderLock.IsMasterList)
                    {
                        message = "The Master List," + objFolderLock.DisplayViewName + ", " + objFolderLock.DisplaySectionName + " is locked for editing by " + objFolderLock.LockedUserName;
                    }
                    else
                        message = objFolderLock.FormName + ", " + objFolderLock.DisplayViewName + ", " + objFolderLock.DisplaySectionName + " is locked for editing by " + objFolderLock.LockedUserName;

                    return new LockStatus { MarkReadOnly = true, LockedByUser = message };
                }
            }
            else
            {
                //lock the section
                var sectionLock = _resourceLockState.Where(s => s.FormInstanceID == formInstanceId &&
                s.LockedBy == userId && s.SectionName == sectionName).FirstOrDefault();
                if (sectionLock == null)
                {
                    ResourceLockState resourceLock = new ResourceLockState();
                    resourceLock.IsLocked = true;
                    resourceLock.LockedBy = userId;
                    resourceLock.TenantID = 1;
                    resourceLock.FolderID = folderId;
                    resourceLock.FormInstanceID = formInstanceId;
                    resourceLock.FormDesignID = formDesignID;
                    resourceLock.LockedDate = DateTime.Now;
                    resourceLock.SectionName = sectionName;
                    resourceLock.DisplaySectionName = displaySectionName;
                    resourceLock.DisplayViewName = displayViewName;
                    resourceLock.FormName = formName;
                    resourceLock.IsMasterList = isMasterList;
                    resourceLock.LockedUserName = userName;
                    _resourceLockState.Add(resourceLock);
                }
                return new LockStatus { MarkReadOnly = false };
            }
            return new LockStatus { MarkReadOnly = true };
        }

        /// <summary>
        /// on close or on section change
        /// </summary>
        /// <param name="formInstanceId"></param>
        /// <param name="userId"></param>
        /// <param name="previousSectionName"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public ServiceResult ReleaseSectionLockonDocumentCloseOrSectionChange(int formInstanceId, int? userId, string previousSectionName)
        {
            ServiceResult result = new ServiceResult();
            var sectionLock = _resourceLockState.Where(s => s.FormInstanceID == formInstanceId && s.LockedBy == userId && s.SectionName == previousSectionName).FirstOrDefault();
            //Todo: also release its dependencies
            if (sectionLock != null)
            {
                if (sectionLock.IsLocked == true)
                {
                    _resourceLockState.Remove(sectionLock);
                    result.Result = ServiceResultStatus.Success;
                    return result;
                }
            }

            if (string.IsNullOrEmpty(previousSectionName))
            {
                var sectionLockList = _resourceLockState.Where(s => s.FormInstanceID == formInstanceId && s.LockedBy == userId).ToList();
                foreach (var secLock in sectionLockList)
                {
                    _resourceLockState.Remove(secLock);
                }
            }
            result.Result = ServiceResultStatus.Success;
            return result;
        }

        public bool IsSectionLevelLockingEnabledForFormDesign(int formDesignId)
        {
            bool isSectionLevelLockingEnabledForFormInstanceDesign = false;
            var sectionlockInfo = GetAllFormDesignSectionListinfo().Where(row => row.FormDesignID == formDesignId).FirstOrDefault();
            if (sectionlockInfo != null && sectionlockInfo.IsSectionLock)
            {
                isSectionLevelLockingEnabledForFormInstanceDesign = true;
            }
            return isSectionLevelLockingEnabledForFormInstanceDesign;
        }

        public List<ResourceLockStateViewModel> GetAllSectionLocks()
        {
            var allLocks = ((from relockState in _resourceLockState
                             select relockState)
                                       .Union(from relock in _resourceLock
                                              select new ResourceLockState()
                                              {
                                                  FormInstanceID = (Int32)relock.FormInstanceID,
                                                  LockedBy = (Int32)relock.LockedBy,
                                                  LockedByUserName = relock.LockedUserName,
                                                  IsLocked = relock.IsLocked,
                                                  FolderID = (Int32)relock.FolderID,
                                                  LockedDate = relock.LockedDate
                                              }
                                              )).ToList();

            List<ResourceLockStateViewModel> allLockslist = (from e in allLocks
                                                             join u in this._unitOfWork.RepositoryAsync<User>().Get()
                                                             on e.LockedBy equals u.UserID
                                                             join formins in _unitOfWork.RepositoryAsync<FormInstance>().Query().Include(c => c.FormDesign).Get()
                                                             on e.FormInstanceID equals formins.FormInstanceID
                                                             join fldrVersion in _unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                                             on formins.FolderVersionID equals fldrVersion.FolderVersionID
                                                             join fldr in _unitOfWork.RepositoryAsync<Folder>().Get()
                                                             on fldrVersion.FolderID equals fldr.FolderID
                                                             select new ResourceLockStateViewModel
                                                             {
                                                                 FolderID = e.FolderID,
                                                                 IsLocked = e.IsLocked,
                                                                 LockedBy = e.LockedBy,
                                                                 LockedDate = e.LockedDate,
                                                                 FormInstanceID = e.FormInstanceID,
                                                                 SectionName = e.SectionName,
                                                                 DisplaySectionName = string.IsNullOrEmpty(e.DisplaySectionName)? "All Sections" : e.DisplaySectionName,
                                                                 DisplayViewName = e.DisplayViewName,
                                                                 LockedByUserName = e.LockedByUserName,
                                                                 LockedUserName = u.UserName,
                                                                 IsMasterList = e.IsMasterList,
                                                                 FormName = String.IsNullOrEmpty(formins.Name) ? formins.FormDesign.FormName : formins.Name,
                                                                 FormDesignID = formins.FormDesignID,
                                                                 FolderName = fldr.Name,
                                                                 FolderversionNumber= fldrVersion.FolderVersionNumber,
                                                                 FolderVersionId=fldrVersion.FolderVersionID,
                                                                 FormDesignVersionID = formins.FormDesignVersionID
                                                             }).ToList();
            

            return allLockslist.ToList();
        }

        public List<FormDesignLockSection> GetAllFormDesignSectionListinfo()
        {
            if (_formDesignLockInfo.Count == 0)
            {
                _formDesignLockInfo = (from formlock in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                       where formlock.IsActive == true
                                       select new FormDesignLockSection
                                       {
                                           FormDesignID = formlock.FormID,
                                           FormName = formlock.FormName,
                                           IsActive = formlock.IsActive,
                                           DisplayText = formlock.DisplayText,
                                           IsSectionLock = formlock.IsSectionLock
                                       }).ToList();

            }
            return _formDesignLockInfo;
        }

        public ServiceResult ReleaseDocumentAndSectionLock(int userId)
        {

            ServiceResult result = new ServiceResult();
            
            var documentLock = _resourceLock.Where(s => s.LockedBy == userId).ToList();

            foreach(var item in documentLock)
            {
                _resourceLock.Remove(item);
            }

            foreach (var item in documentLock)
            {
                _resourceLock.Remove(item);
            }

            var sectionLock = _resourceLockState.Where(s => s.LockedBy == userId).ToList();

            foreach (var item in sectionLock)
            {
                _resourceLockState.Remove(item);
            }


            result.Result = ServiceResultStatus.Success;
            return result;
        }

    }


}
