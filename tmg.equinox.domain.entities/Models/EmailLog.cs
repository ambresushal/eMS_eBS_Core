using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class EmailLog : Entity
    {
        public int EmailLogID { get; set; }
        public int UserID { get; set; }
        public int FolderID { get; set; }
        public int FolderVersionID { get; set; }
        public int FolderVersionStateID { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string EmailContent { get; set; }
        public int? AccountID { get; set; }
        public int TenantID { get; set; }
        public Nullable<System.DateTime> FolderEffectiveDate { get; set; }
        public int ApprovedWorkFlowStateID { get; set; }
        public int CurrentWorkFlowStateID { get; set; }
        public string Comments { get; set; }
        public System.DateTime EmailSentDateTime { get; set; }
        public string AddedBy { get; set; }
        public virtual Account Account { get; set; }
        public virtual Folder Folder { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual FolderVersion FolderVersion { get; set; }
        public virtual FolderVersionState FolderVersionState { get; set; }
        public virtual WorkFlowVersionState WorkFlowState { get; set; }
        public virtual User User { get; set; }
    }
}
