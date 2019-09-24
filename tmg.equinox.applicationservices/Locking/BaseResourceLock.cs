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
    public class BaseResourceLock
    {
        protected IUnitOfWorkAsync _unitOfWork;
        protected INotificationService _notificationService;
        protected ResourceLockHolder _resourceLockHolder = new ResourceLockHolder();
        public BaseResourceLock(IUnitOfWorkAsync unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }
        protected List<ResourceLockStateViewModel> GetAllSectionLocks(int currentUserID)
        {

            var allLocks = ((from relockState in _resourceLockHolder.GetSection()
                             select relockState)
                                   .Union(from relock in _resourceLockHolder.GetDocument()
                                          select new ResourceLockState()
                                          {
                                              FormInstanceID = (Int32)relock.FormInstanceID,
                                              LockedBy = (Int32)relock.LockedBy,
                                              LockedByUserName = relock.LockedUserName,
                                              IsLocked = relock.IsLocked,
                                              FolderID = (Int32)relock.FolderID,
                                              LockedDate = relock.LockedDate,
                                              NotiFyUsers = relock.NotiFyUsers
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
                                                             where e.LockedBy != currentUserID
                                                             select new ResourceLockStateViewModel
                                                             {
                                                                 FolderID = e.FolderID,
                                                                 IsLocked = e.IsLocked,
                                                                 LockedBy = e.LockedBy,
                                                                 LockedDate = e.LockedDate,
                                                                 FormInstanceID = e.FormInstanceID,
                                                                 SectionName = e.SectionName,
                                                                 DisplaySectionName = String.IsNullOrEmpty(e.DisplaySectionName) ? "All Sections" : e.DisplaySectionName,
                                                                 DisplayViewName = formins.FormDesign.FormName,
                                                                 LockedByUserName = e.LockedByUserName,
                                                                 LockedUserName = u.UserName,
                                                                 IsMasterList = e.IsMasterList,
                                                                 //FormName = ReplaceAtTheRateSignFromDocumentName(String.IsNullOrEmpty(formins.Name) ? formins.FormDesign.FormName : formins.Name),
                                                                 FormName = GetDocumentName(formins.AnchorDocumentID == 0 ? formins.FormInstanceID : formins.AnchorDocumentID),
                                                                 FormDesignID = formins.FormDesignID,
                                                                 FolderName = fldr.Name,
                                                                 FolderversionNumber = fldrVersion.FolderVersionNumber,
                                                                 FolderVersionId = fldrVersion.FolderVersionID,
                                                                 FormDesignVersionID = formins.FormDesignVersionID,
                                                                 NotifyuserFlag = e.NotiFyUsers != null && e.NotiFyUsers.Where(u => u.UserId == currentUserID).Count() > 0 ? true : false
                                                             }).ToList();
            return allLockslist.ToList();
        }

        private string GetDocumentName(int? anchorDocumentId)
        {
            string documentName = "";
            try
            {
                var formInstance = (from formins in _unitOfWork.RepositoryAsync<FormInstance>().Query().Include(c => c.FormDesign).Include(c => c.FormDesign).Get()
                                        where formins.FormInstanceID == anchorDocumentId
                                        select formins).FirstOrDefault();
                
                if (formInstance != null)
                {
                    //documentName = formInstance.Name;
                    documentName = String.IsNullOrEmpty(formInstance.Name) ? formInstance.FormDesign.FormName : formInstance.Name;
                    documentName = ReplaceAtTheRateSignFromDocumentName(documentName);
                }
            }
            catch (Exception ex)
            {
               
            }
            return documentName;

        }

        protected List<int> GetRelatedFormInstanceids(int formInstanceId)
        {
            var formInstance = this._unitOfWork.RepositoryAsync<FormInstance>().Query().Filter(row => row.FormInstanceID == formInstanceId).Get().FirstOrDefault();
            var formInstanceIds = (from u in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                   join fd in _unitOfWork.RepositoryAsync<FormDesign>().Get()
                                   on u.FormDesignID equals fd.FormID
                                   where u.AnchorDocumentID == formInstance.AnchorDocumentID && u.AnchorDocumentID != 0 && u.IsActive == true && fd.IsSectionLock == false
                                   select u.FormInstanceID
                                   ).ToList();
            return formInstanceIds;
        }

        private string ReplaceAtTheRateSignFromDocumentName(string formName)
        {
            var updatedformName = formName.Substring(0, formName.IndexOf("@@") == -1 ? formName.Length : formName.IndexOf("@@"));
            return updatedformName;
        }
    }
}


