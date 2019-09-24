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


    public class ResourceLockService : BaseResourceLock, IResourceLockService
    {
        private ISectionLockService _sectionLockService;
        private IFormInstanceService _formInstanceService;
        public ResourceLockService(ISectionLockService sectionLockService, IUnitOfWorkAsync unitOfWork, INotificationService notificationService, IFormInstanceService formInstanceService) : base(unitOfWork, notificationService)
        {
            _sectionLockService = sectionLockService;
            _formInstanceService = formInstanceService;
        }

        public bool IsDocumentLocked(int formInstanceId, int? userId, string sectionName, int formDesignId, bool checkifLockedByOtherUser= false)
        {
            bool islocked = false;


            //if (string.IsNullOrEmpty(sectionName)`)
            //{
            //  var objFolderLock = _resourceLock.Where(s => s.FormInstanceID == formInstanceId && s.LockedBy == userId).FirstOrDefault();
            var objFolderLock = _resourceLockHolder.GetDocument(formInstanceId, userId.Value);

            if (objFolderLock != null)
            {
                objFolderLock.LockedDate = DateTime.Now;
                islocked = true;
            }
            else
            {
                // objFolderLock = _resourceLock.Where(s => s.FormInstanceID == formInstanceId).FirstOrDefault();
                objFolderLock = _resourceLockHolder.GetDocument(formInstanceId);
                if (objFolderLock == null)
                {
                    islocked = true;
                }
            }
            //}
            if (_sectionLockService.IsSectionLevelLockingEnabledForFormDesign(formDesignId))
            {
                if (islocked == false && !string.IsNullOrEmpty(sectionName))
                {
                    var isSectionlockedbyCurrentUser = _sectionLockService.IsSectionLocked(formInstanceId, userId, sectionName);
                    if (isSectionlockedbyCurrentUser)
                    {
                        islocked = true;
                    }else if(checkifLockedByOtherUser)
                    {
                        var isSectionlockedbyOtherUser = _resourceLockHolder.GetSectionLockedByOtherUser(formInstanceId, sectionName, userId.Value);
                        if (isSectionlockedbyOtherUser != null)
                        {
                            islocked = false;
                        }
                        else
                        {
                            islocked = true;
                        }
                    }
                }
            }
            else if(_sectionLockService.IsMasterListDesign(formDesignId))
            {
                islocked = true;
            }
            return islocked;
        }

        public ServiceResult OverrideDocumentLock(int folderId, int? userId, List<int> formInstanceIDs)
        {
            ServiceResult result = new ServiceResult();

            foreach (int formInstanceId in formInstanceIDs)
            {
                //var objResourceLock = _resourceLock.Where(s => s.FormInstanceID == formInstanceId).FirstOrDefault();
                var objResourceLock = _resourceLockHolder.GetDocument(formInstanceId);
                if (objResourceLock != null)
                {
                    RemoveDocumentWithNotification(new List<ResourceLock>() { objResourceLock });

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
            {
                //objResourceLock = _resourceLock.Where(s => s.FolderID == folderId).OrderBy(s => s.FolderID).ToList();
                objResourceLock = _resourceLockHolder.GetDocumentByFolder(folderId);
            }
            else
            {
                //objResourceLock = _resourceLock.Where(s => s.FolderID == folderId && s.LockedBy == userId).OrderBy(s => s.FolderID).ToList();
                objResourceLock = _resourceLockHolder.GetDocumentByFolder(folderId, userId.Value);
            }

            if ((objResourceLock != null && objResourceLock.Count > 0))
            {
                RemoveDocumentWithNotification(objResourceLock);
                result.Result = ServiceResultStatus.Success;
            }
            else
            {
                result.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Folder is already unlocked." } });
            }

            result = _sectionLockService.ReleaseFolderLock(folderId, userId);

            return result;
        }
        public ServiceResult ReleaseLock(int userId)
        {

            ServiceResult result = new ServiceResult();

            //var documentLock = _resourceLock.Where(s => s.LockedBy == userId).ToList();
            var documentLock = _resourceLockHolder.GetDocumentByUser(userId);

            RemoveDocumentWithNotification(documentLock);
            _sectionLockService.ReleaseSectionLock(userId);

            result.Result = ServiceResultStatus.Success;
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
                //objDocumentLock = _resourceLock.Where(s => s.FormInstanceID == formInstanceID).OrderBy(s => s.FormInstanceID).FirstOrDefault();
                objDocumentLock = _resourceLockHolder.GetDocument(formInstanceID);

            else
                //objDocumentLock = _resourceLock.Where(s => s.FormInstanceID == formInstanceID && s.LockedBy == userId).OrderBy(s => s.FormInstanceID).FirstOrDefault();
                objDocumentLock = _resourceLockHolder.GetDocument(formInstanceID, userId.Value);


            if (objDocumentLock != null)
            {
                RemoveDocumentWithNotification(new List<ResourceLock>() { objDocumentLock });
                result.Result = ServiceResultStatus.Success;
            }
            else
            {
                result.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Document is already unlocked." } });
            }
            _sectionLockService.ReleaseSectionLockonDocumentCloseOrSectionChange(formInstanceID, userId, string.Empty);
            return result;
        }

        public List<FolderVersionViewModel> GetDocumentLockStatus(int? folderId, int? userId, List<int> formInstanceIDs)
        {

            // check docid & section is locked by another user
            // 
            List<FolderVersionViewModel> models = new List<FolderVersionViewModel>();

            foreach (int formInstanceId in formInstanceIDs)
            {
                FolderVersionViewModel model = (from e in _resourceLockHolder.GetDocument()
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

        public ServiceResult UpdateDocumentLockStatus(int? folderId, int? userId, List<int> formInstanceIDs, string userName)
        {
            ServiceResult result = new ServiceResult();
            foreach (int formInstanceId in formInstanceIDs)
            {
                //var objFolderLock = _resourceLock.Where(s => s.FormInstanceID == formInstanceId && s.LockedBy == userId).OrderBy(s => s.FormInstanceID).FirstOrDefault();
                var objFolderLock = _resourceLockHolder.GetDocument(formInstanceId, userId.Value);
                if (objFolderLock != null)
                {
                    if (objFolderLock.IsLocked == true)
                        result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    _resourceLockHolder.AddDocument(folderId, formInstanceId, userId.Value, userName);
                    result.Result = ServiceResultStatus.Success;
                }
            }
            return result;
        }

        public List<ResourceLock> GetLockedDocuments(int? userId)
        {
            List<ResourceLock> list = (from e in _resourceLockHolder.GetDocument()
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

        public ServiceResult ReleaseDocumentAndSectionLock(int userId)
        {

            ServiceResult result = new ServiceResult();
            try
            {
                //var documentLock = _resourceLock.Where(s => s.LockedBy == userId).ToList();
                var documentLock = _resourceLockHolder.GetDocumentByUser(userId);
                RemoveDocumentWithNotification(documentLock);
                _sectionLockService.ReleaseSectionLock(userId);

                result.Result = ServiceResultStatus.Success;
            }
            catch(Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
            }
            return result;
        }

        public ServiceResult ReleaseDocumentAndSectionLock(int formInstanceId, string sectionName)
        {
            ServiceResult result = new ServiceResult();
            var documentLock = _resourceLockHolder.GetDocument(formInstanceId);
            if (documentLock != null)
            {
                var relatedFormInstanceIds = GetRelatedFormInstanceids(formInstanceId);
                RemoveDocumentWithNotification(new List<ResourceLock>() { documentLock });
                foreach (int formId in relatedFormInstanceIds)
                {
                    var lockeddoc = _resourceLockHolder.GetDocument(formId);
                    if (lockeddoc != null)
                    {
                        RemoveDocumentWithNotification(new List<ResourceLock>() { lockeddoc });
                    }
                }
            }
            _sectionLockService.ReleaseSectionLock(formInstanceId, sectionName);
            result.Result = ServiceResultStatus.Success;
            return result;
        }

        public List<ResourceLockStateViewModel> GetAllLockIntances(int currentUserID)
        {
            return base.GetAllSectionLocks(currentUserID);
        }

        /// <summary>
        /// clear all forminstanceid for that user
        /// get all checked form instance id from UI
        /// Add in Notification User List against each instanceId
        /// </summary>
        /// <param name="resourceLockInputModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public void UpdateNotifyUserList(List<ResourceLockInputModel> resourceLockInputModel, int userId, string userName)
        {

            var allNotificationsByUser = GetAllSectionLocks(userId).Where(row => row.NotifyuserFlag == true).ToList();
            foreach (var removeNotify in allNotificationsByUser)
            {
                _resourceLockHolder.RemoveUserNotification(removeNotify.FormInstanceID, removeNotify.SectionName, userId);
            }
            if (resourceLockInputModel != null)
            {
                foreach (var item in resourceLockInputModel)
                {
                    _sectionLockService.UpdateNotifyUser(item.FormInstanceID, item.SectionName, userId, userName);
                }
            }
        }

        public bool CheckIfFormInstanceasLockingEnabled(int formInstanceId)
        {
            bool isLockingEnabled = false;
            var formInstance = this._unitOfWork.RepositoryAsync<FormInstance>().Query().Filter(row => row.FormInstanceID == formInstanceId).Get().FirstOrDefault();
            if (formInstance != null)
            {
                isLockingEnabled = _sectionLockService.IsSectionLevelLockingEnabledForFormDesign(formInstance.FormDesignID);
            }
            return isLockingEnabled;
        }

        private void RemoveDocumentWithNotification(List<ResourceLock> resourceLockList)
        {
            foreach (ResourceLock obj in resourceLockList)
            {
                var formDetails = _formInstanceService.GetFormInstanceDetails(obj.FormInstanceID);
                var formName = "";
                if(formDetails != null)
                {
                    formName = _formInstanceService.GetFormInstanceDetails(obj.FormInstanceID).FormName;
                    if (obj.NotiFyUsers != null && obj.NotiFyUsers.Count() > 0)
                    {
                        var paramaters = new List<Paramters>();
                        paramaters.Add(new Paramters { key = "Document", Value = formName });
                        paramaters.Add(new Paramters { key = "user", Value = obj.LockedUserName });
                        foreach (UserViewModelResourceLock user in obj.NotiFyUsers)
                        {
                            var notificationMsg = new NotificationInfo { loggedInUserName = obj.LockedUserName, MessageKey = MessageKey.NOTIFY_RELEASE_DOC, UserID = user.UserId, ParamterValues = paramaters, SentTo = user.UserName };
                            _notificationService.SendNotification(notificationMsg);
                        }
                    }
                    _resourceLockHolder.RemoveDocument(obj);
                }
            }
        }

        public List<ResourceLock> GetLockedDocuments()
        {
            List<ResourceLock> list = (from e in _resourceLockHolder.GetDocument()
                                       join u in this._unitOfWork.RepositoryAsync<User>().Get()
                                       on e.LockedBy equals u.UserID
                                       join fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                       on e.FolderID equals fld.FolderID
                                       select new ResourceLock
                                       {
                                           FolderID = e.FolderID,
                                           IsLocked = e.IsLocked,
                                           LockedBy = e.LockedBy,
                                           LockedDate = e.LockedDate,
                                           FormInstanceID = e.FormInstanceID,
                                           LockedUserName = u.UserName,
                                           FolderName = fld.Name
                                       }).ToList();
            //List<ResourceLock> list = (from e in _resourceLock
            //                           join u in this._unitOfWork.RepositoryAsync<User>().Get()
            //                           on e.LockedBy equals u.UserID
            //                           join fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
            //                           on e.FolderID equals fld.FolderID
            //                           select new ResourceLock
            //                           {
            //                               FolderID = e.FolderID,
            //                               IsLocked = e.IsLocked,
            //                               LockedBy = e.LockedBy,
            //                               LockedDate = e.LockedDate,
            //                               FormInstanceID = e.FormInstanceID,
            //                               LockedUserName = u.UserName,
            //                               FolderName = fld.Name
            //                           }).ToList();

            return list;
        }
    }


}
