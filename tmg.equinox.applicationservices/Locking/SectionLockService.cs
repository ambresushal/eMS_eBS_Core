using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.notification;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{



    public class SectionLockService : BaseResourceLock, ISectionLockService
    {
        private static List<FormDesignLockSection> _formDesignLockInfo = new List<FormDesignLockSection>();


        public SectionLockService(IUnitOfWorkAsync unitOfWork, INotificationService notificationService) : base(unitOfWork, notificationService)
        {

        }
        public void RefeshFormDesignLock()
        {
            _formDesignLockInfo = new List<FormDesignLockSection>();
        }

        public ServiceResult ReleaseFolderLock(int folderId, int? userId)
        {
            ServiceResult result = new ServiceResult();
            List<ResourceLockState> sectionLevelLocks = null;
            result.Result = ServiceResultStatus.Success;
            if (userId == null)
            {
                sectionLevelLocks = _resourceLockHolder.GetSection(folderId).ToList();
            }
            else
            {
                sectionLevelLocks = _resourceLockHolder.GetSection(folderId, userId).ToList();
            }

            RemoveSectionWithNotification(sectionLevelLocks);

            result.Result = ServiceResultStatus.Success;

            return result;
        }

        public bool IsSectionLocked(int formInstanceId, int? userId, string sectionName)
        {
            bool islocked = false;
            var objSectionLock = _resourceLockHolder.GetSectionLockedByUser(formInstanceId, sectionName, userId.Value);
            if (objSectionLock != null)
            {
                objSectionLock.LockedDate = DateTime.Now;
                islocked = true;
            }
            else
            {
                //For manual override Should return false if opened by other users as returing true was allowing to save for other users even though it was locked by first user
                islocked = false;

                //objSectionLock = _resourceLockHolder.GetSectionLockedByOtherUser(formInstanceId, sectionName, userId.Value);
                //if (objSectionLock == null)
                //{
                //    islocked = false;
                //}
                //else
                //{
                //    islocked = true;
                //}
            }
            return islocked;
        }

        public LockStatus ManageSectionLock(int folderId, int formInstanceId, string displayViewName, string displaySectionName, string sectionName, int userId, string userName, int formDesignID, string formName, bool isMasterList)
        {

            //check if formInstance is of Collateral Type
            if (!IsSectionLevelLockingEnabledForFormDesign(formDesignID))
            {
                string message = null;
                var objInstanceLock = _resourceLockHolder.CheckIfDocumentLockedByOtherUser(formInstanceId, userId);
                if (objInstanceLock != null)
                {
                    var lockObject = GetAllSectionLocks(userId).Where(row => row.FormInstanceID == objInstanceLock.FormInstanceID && row.SectionName == null).FirstOrDefault();
                    var user = this._unitOfWork.RepositoryAsync<User>().Query().Filter(c => c.UserID == objInstanceLock.LockedBy).Get().ToList().FirstOrDefault();
                    string formObjectName = lockObject.FormName;
                    if (!string.IsNullOrEmpty(formName))
                    {
                        formObjectName = formObjectName.Substring(0, formObjectName.IndexOf("@@") == -1 ? formObjectName.Length : formObjectName.IndexOf("@@"));
                    }
                    message = formObjectName + "=> " + lockObject.DisplayViewName + " is locked for editing by " + user.UserName;
                    return new LockStatus { MarkReadOnly = true, LockedByUser = message };
                }
                else
                {
                    var formInstanceIds = GetRelatedFormInstanceids(formInstanceId);
                    foreach (int formId in formInstanceIds)
                    {
                        var objdocLock = _resourceLockHolder.GetDocument(formId, userId);
                        if (objdocLock == null)
                        {
                            _resourceLockHolder.AddDocument(folderId, formId, userId, userName);
                        }
                    }
                }
                return new LockStatus { MarkReadOnly = false };
            }
            // var objFolderLock = _resourceLockState.Where(s => s.FormInstanceID == formInstanceId && s.LockedBy != userId && s.SectionName == sectionName).FirstOrDefault();
            var objFolderLock = _resourceLockHolder.GetSectionLockedByOtherUser(formInstanceId, sectionName, userId);
            //NJ:TODO: also check in collection 
            if (objFolderLock != null)
            {
                if (objFolderLock.IsLocked == true)
                {
                    var paramaters = new List<Paramters>();
                    paramaters.Add(new Paramters { key = "user", Value = objFolderLock.LockedByUserName });

                    var notificationMsg = new NotificationInfo { loggedInUserName = userName, MessageKey = MessageKey.LOCKED, UserID = userId, ParamterValues = paramaters, SentTo = userName };

                    _notificationService.SendNotification(notificationMsg);
                    string message = null;
                    if (objFolderLock.IsMasterList)
                    {
                        message = "The Master List=> " + objFolderLock.DisplayViewName + "=> " + objFolderLock.DisplaySectionName + " is locked for editing by " + objFolderLock.LockedUserName;
                    }
                    else
                    {
                        string formObjectName = objFolderLock.FormName;
                        if (!string.IsNullOrEmpty(formName))
                        {
                            formObjectName = formObjectName.Substring(0, formObjectName.IndexOf("@@") == -1 ? formObjectName.Length : formObjectName.IndexOf("@@"));
                        }
                        message = formObjectName + "=> " + objFolderLock.DisplayViewName + "=> " + objFolderLock.DisplaySectionName + " is locked for editing by " + objFolderLock.LockedUserName;
                    }

                    return new LockStatus { MarkReadOnly = true, LockedByUser = message };
                }
            }
            else
            {
                //lock the section
                //var sectionLock = _resourceLockState.Where(s => s.FormInstanceID == formInstanceId && s.LockedBy == userId && s.SectionName == sectionName).FirstOrDefault();
                var sectionLock = _resourceLockHolder.GetSectionLockedByUser(formInstanceId, sectionName, userId);
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
                    resourceLock.LockedByUserName = userName;
                    _resourceLockHolder.AddSection(resourceLock);
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

            //var sectionLock = _resourceLockState.Where(s => s.FormInstanceID == formInstanceId && s.LockedBy == userId && s.SectionName == previousSectionName).FirstOrDefault();
            var sectionLock = _resourceLockHolder.GetSectionLockedByUser(formInstanceId, previousSectionName, userId.Value);
            //Todo: also release its dependencies
            if (sectionLock != null)
            {
                if (sectionLock.IsLocked == true)
                {
                    RemoveSectionWithNotification(new List<ResourceLockState>() { sectionLock });
                    result.Result = ServiceResultStatus.Success;
                    return result;
                }
            }

            if (string.IsNullOrEmpty(previousSectionName))
            {
                var sectionLockList = _resourceLockHolder.GetSectionbyInstanceId(formInstanceId, (Int32)userId);
                RemoveSectionWithNotification(sectionLockList);
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

        public bool IsMasterListDesign(int formDesignId)
        {
            bool isMasterListDesign = false;
            var sectionlockInfo = GetAllFormDesignSectionListinfo().Where(row => row.FormDesignID == formDesignId).FirstOrDefault();
            if (sectionlockInfo != null && sectionlockInfo.IsMasterList)
            {
                isMasterListDesign = true;
            }
            return isMasterListDesign;
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
                                           IsSectionLock = formlock.IsSectionLock,
                                           IsMasterList = formlock.IsMasterList
                                       }).ToList();

            }
            return _formDesignLockInfo;
        }


        public ServiceResult ReleaseSectionLock(int userId)
        {

            ServiceResult result = new ServiceResult();

            var sectionsLock = _resourceLockHolder.GetSectionByUser(userId);
            _resourceLockHolder.RemoveSection(sectionsLock);

            result.Result = ServiceResultStatus.Success;
            return result;
        }
        public List<UserViewModelResourceLock> GetNotifyUser()
        {
            return _resourceLockHolder.GetNotifyUser();
        }
        public ServiceResult ReleaseSectionLock(int formInstanceId, string sectionName)
        {

            ServiceResult result = new ServiceResult();


            var sectionsLock = _resourceLockHolder.GetSectionbyInstanceId(formInstanceId, sectionName);
            RemoveSectionWithNotification(sectionsLock);

            result.Result = ServiceResultStatus.Success;
            return result;
        }

        public ServiceResult UpdateNotifyUser(int formInstanceId, string sectionName, int userId, string userName)
        {
            ServiceResult result = new ServiceResult();
            _resourceLockHolder.UpdateNotifyBothResourceAndSection(formInstanceId, sectionName, userId, userName);
            result.Result = ServiceResultStatus.Success;
            return result;
        }
        private void RemoveSectionWithNotification(List<ResourceLockState> resourceLockList)
        {
            foreach (ResourceLockState obj in resourceLockList)
            {
                if (obj.NotiFyUsers != null && obj.NotiFyUsers.Count() > 0)
                {
                    var paramaters = new List<Paramters>();
                    //paramaters.Add(new Paramters { key = "Folder name", Value = "FolderName" });
                    paramaters.Add(new Paramters { key = "Document", Value = obj.FormName });
                    paramaters.Add(new Paramters { key = "View", Value = obj.DisplayViewName });
                    paramaters.Add(new Paramters { key = "Section", Value = obj.DisplaySectionName });
                    paramaters.Add(new Paramters { key = "user", Value = obj.LockedUserName });
                    foreach (UserViewModelResourceLock user in obj.NotiFyUsers)
                    {
                        var notificationMsg = new NotificationInfo { loggedInUserName = obj.LockedUserName, MessageKey = MessageKey.NOTIFY_RELEASE, UserID = user.UserId, ParamterValues = paramaters, SentTo = user.UserName };
                        _notificationService.SendNotification(notificationMsg);
                    }
                }
                _resourceLockHolder.RemoveSection(obj);
            }
        }
    }
}
