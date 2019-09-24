using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class Account:Entity
    {
        public Account()
        {
            this.AccountFolderMaps = new List<AccountFolderMap>();
            this.EmailLogs = new List<EmailLog>();
        }

        public int AccountID { get; set; }
        public int TenantID { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string AccountName { get; set; }
        public bool IsActive { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual Tenant Tenant1 { get; set; }
        public virtual ICollection<AccountFolderMap> AccountFolderMaps { get; set; }
        public virtual ICollection<EmailLog> EmailLogs { get; set; }

    }
}
