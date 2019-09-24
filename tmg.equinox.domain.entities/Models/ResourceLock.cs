using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    
    public partial class ResourceLock 
    {
        public int ResourceLockID { get; set; }
        public int? FolderID { get; set; }
        public bool IsLocked { get; set; }
        public int? LockedBy { get; set; }
        public int TenantID { get; set; }
        public DateTime LockedDate { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual User User { get; set; }
        //public int FolderVersionId { get; set; }
        public int FormInstanceID { get; set; }
        public Folder Folder { get; set; }
        public string LockedUserName { get; set; }
        public List<UserViewModelResourceLock> NotiFyUsers { get; set; }
        public bool NotifyuserFlag { get; set; }
        public string FolderName { get; set; }
        //public string ViewSectionName { get; set; }

        //public List<string> DependantSection { get; set; }


    }

    //public class DependentSection
    //{
    //    public string View { get; set; }
    //    public List<string> Sections  { get; set; }
    //} 




    public class LockStatus
    {
        public bool MarkReadOnly { get; set; }
        public string LockedByUser { get; set; }
    }

    public class ResourceLockState : ResourceLock
    {
        public string LockedByUserName { get; set; }
        public string FormName { get; set; }
        //public int FolderVersionId { get; set; }
        public int FormDesignID { get; set; }
        public string SectionName { get; set; }
        public string DisplayViewName { get; set; }
        public string DisplaySectionName { get; set; }
        public bool IsMasterList { get; set; }
        public List<DependentSection> DependantSection { get; set; }


    }

    public class ResourceLockStateViewModel : ResourceLockState
    {
        public int FormDesignVersionID { get; set; }
        public string FolderName { get; set; }
        public string FolderversionNumber { get; set; }
        public int FolderVersionId { get; set; }
    }

    public class ResourceLockInputModel     {
        public int FormInstanceID { get; set; }
        public string SectionName { get; set; }
    }


    public class DependentSection
    {   
		[Key]
        public int FormInstanceID { get; set; }
        public string SectionName { get; set; }

        public string DisplaySectionName { get; set; }
    }

    public class FormDesignLockSection
    {
        public int FormDesignID { get; set; }
        public string FormName { get; set; }
        public string DisplayText { get; set; }
        public bool IsActive { get; set; }
        public bool IsSectionLock { get; set; }
        public bool IsMasterList { get; set; }
    }

    public class UserViewModelResourceLock
    {
        [Key]
        public string UserName { get; set; }
        public int UserId { get; set; }
    }
}
