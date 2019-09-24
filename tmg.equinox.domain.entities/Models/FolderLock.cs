using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FolderLock : Entity
    {

        public int FolderLockID { get; set; }
        public int? FolderID { get; set; }
        public bool IsLocked { get; set; }
        public int? LockedBy { get; set; }
        public int TenantID { get; set; }
        public System.DateTime LockedDate { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual User User { get; set; }

        public Folder Folder { get; set; }
    }
}
