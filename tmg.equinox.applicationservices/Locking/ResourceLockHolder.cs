using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.notification;

namespace tmg.equinox.applicationservices
{
    public class ResourceLockHolder
    {
        private static List<ResourceLockState> _resourceLockState = new List<ResourceLockState>();
        public static List<ResourceLock> _resourceLock = new List<ResourceLock>();

        public ResourceLock GetDocument(int formInstanceId, int userId)
        {
            return _resourceLock.Where(s => s.FormInstanceID == formInstanceId && s.LockedBy == userId).FirstOrDefault();
        }

        public ResourceLock CheckIfDocumentLockedByOtherUser(int formInstanceId, int? userId)
        {
            return _resourceLock.Where(s => s.FormInstanceID == formInstanceId && s.LockedBy != userId).FirstOrDefault();
        }

        public List<ResourceLock> GetDocument()
        {
            return _resourceLock;
        }
        public List<ResourceLock> GetDocumentByUser(int userId)
        {
            List<ResourceLock> documentlist = new List<ResourceLock>();
            documentlist = _resourceLock.Where(s => s.LockedBy == userId).ToList();
            return documentlist;
        }

        public ResourceLock GetDocument(int formInstanceId)
        {
            return _resourceLock.Where(s => s.FormInstanceID == formInstanceId).FirstOrDefault();
        }
        public List<ResourceLock> GetDocumentByFolder(int folderId)
        {
            return _resourceLock.Where(s => s.FolderID == folderId).OrderBy(s => s.FolderID).ToList();
        }
        public List<ResourceLock> GetDocumentByFolder(int folderId, int userId)
        {
            return _resourceLock.Where(s => s.FolderID == folderId && s.LockedBy == userId).OrderBy(s => s.FolderID).ToList();
        }
        public void RemoveDocument(ResourceLock itemToRemove)
        {
            _resourceLock.Remove(itemToRemove);
        }

        public void AddDocument(int? folderId, int formInstanceId, int userId, string userName)
        {
            ResourceLock resourceLock = new ResourceLock();
            resourceLock.FolderID = folderId ?? 0;
            resourceLock.IsLocked = true;
            resourceLock.LockedBy = userId;
            resourceLock.TenantID = 1;
            resourceLock.FormInstanceID = formInstanceId;
            resourceLock.LockedDate = DateTime.Now;
            resourceLock.LockedUserName = userName;
            if (resourceLock.NotiFyUsers == null)
            {
                resourceLock.NotiFyUsers = new List<UserViewModelResourceLock>();
            }
            _resourceLock.Add(resourceLock);
        }
        public void RemoveDocument(List<ResourceLock> itemToRemove)
        {
            foreach (ResourceLock obj in itemToRemove)
                _resourceLock.Remove(obj);
        }

        public List<ResourceLockState> GetSection(int folderId)
        {
            return _resourceLockState.Where(row => row.FolderID == folderId).ToList();
        }

        public void UpdateNotifyBothResourceAndSection(int formInstanceId, string sectionName, int userId, string userName)
        {
            var lockSections = _resourceLockState.Where(row => row.FormInstanceID == formInstanceId && row.SectionName == sectionName).ToList();

            foreach (var lockobj in lockSections)
            {
                if (lockobj.NotiFyUsers == null)
                {
                    lockobj.NotiFyUsers = new List<UserViewModelResourceLock>();
                    lockobj.NotiFyUsers.Add(new UserViewModelResourceLock() { UserName = userName, UserId = userId });
                }
                else
                {
                    if (lockobj.NotiFyUsers.Where(u => u.UserId == userId).Count() == 0)
                    {
                        lockobj.NotiFyUsers.Add(new UserViewModelResourceLock() { UserName = userName, UserId = userId });
                    }
                }


            }
            var lockDoc = _resourceLock.Where(row => row.FormInstanceID == formInstanceId).ToList();

            foreach (var lockobj in lockDoc)
            {
                if (lockobj.NotiFyUsers == null)
                {
                    lockobj.NotiFyUsers = new List<UserViewModelResourceLock>();
                    lockobj.NotiFyUsers.Add(new UserViewModelResourceLock() { UserName = userName, UserId = userId });
                }
                if (lockobj.NotiFyUsers.Where(u => u.UserId == userId).Count() == 0)
                {
                    lockobj.NotiFyUsers.Add(new UserViewModelResourceLock() { UserName = userName, UserId = userId });
                }
            }
        }

        public List<UserViewModelResourceLock> GetNotifyUser()
        {
            var lockSections = _resourceLockState.ToList();

            List<UserViewModelResourceLock> users = new List<UserViewModelResourceLock>();
            foreach (var lockobj in lockSections)
            {
                users.AddRange(lockobj.NotiFyUsers);
            }
            var lockDoc = _resourceLock.ToList();

            foreach (var lockobj in lockDoc)
            {
                users.AddRange(lockobj.NotiFyUsers);
            }
            return users;
        }
        public List<ResourceLockState> GetSection()
        {
            return _resourceLockState;
        }
        public List<ResourceLockState> GetSection(int folderId, int? userId)
        {
            return _resourceLockState.Where(row => row.FolderID == folderId && row.LockedBy == userId).ToList();
        }
        public List<ResourceLockState> GetSectionByUser(int userId)
        {
            return _resourceLockState.Where(row => row.LockedBy == userId).ToList();
        }
        public List<ResourceLockState> GetSectionbyInstanceId(int formInstanceId, int userId)
        {
            return _resourceLockState.Where(s => s.FormInstanceID == formInstanceId && s.LockedBy == userId).ToList();
        }
        public List<ResourceLockState> GetSectionbyInstanceId(int formInstanceId, string sectionName)
        {
            return _resourceLockState.Where(s => s.FormInstanceID == formInstanceId && s.SectionName == sectionName).ToList();
        }
        public void RemoveSection(List<ResourceLockState> itemsToRemove)
        {
            foreach (ResourceLockState obj in itemsToRemove)
                _resourceLockState.Remove(obj);
        }
        public void RemoveSection(ResourceLockState itemToRemove)
        {
            _resourceLockState.Remove(itemToRemove);
        }
        public ResourceLockState GetSectionLockedByOtherUser(int formInstanceId, string sectionName, int loggedInuserId)
        {
            return _resourceLockState.Where(s => s.FormInstanceID == formInstanceId && s.SectionName == sectionName && s.LockedBy != loggedInuserId).FirstOrDefault();
        }
        public ResourceLockState GetSectionLockedByUser(int formInstanceId, string sectionName, int loggedInuserId)
        {
            return _resourceLockState.Where(s => s.FormInstanceID == formInstanceId && s.SectionName == sectionName && s.LockedBy == loggedInuserId).FirstOrDefault();
        }
        public void AddSection(ResourceLockState sectionLock)
        {
            _resourceLockState.Add(sectionLock);
        }

        public void RemoveUserNotification(int formInstanceId, string sectionName, int userId)
        {
            if (String.IsNullOrEmpty(sectionName))
            {
                var lockObject = _resourceLock.Where(row => row.FormInstanceID == formInstanceId).FirstOrDefault();
                if (lockObject != null)
                {
                    var user = lockObject.NotiFyUsers.Where(u => u.UserId == userId).FirstOrDefault();
                    if (user != null)
                        lockObject.NotiFyUsers.Remove(user);
                }
            }
            else
            {
                var lockObject = _resourceLockState.Where(row => row.FormInstanceID == formInstanceId && row.SectionName == sectionName).FirstOrDefault();
                if (lockObject != null)
                {
                    var user = lockObject.NotiFyUsers.Where(u => u.UserId == userId).FirstOrDefault();
                    if (user != null)
                        lockObject.NotiFyUsers.Remove(user);
                }
            }
        }
    }
}
