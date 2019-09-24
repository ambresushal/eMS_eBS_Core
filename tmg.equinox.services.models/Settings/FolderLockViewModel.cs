using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Settings
{
    class FolderLockViewModel
    {
        public int FolderLockID { get; set; }
        public int FolderID { get; set; }
        public int TenantID { get; set; }
        public bool IsLocked { get; set; }
        public int LockedBy { get; set; }
    }
}
