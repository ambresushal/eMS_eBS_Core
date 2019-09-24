using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FolderVersionWorkFlowState : Entity
    {
        public int FVWFStateID { get; set; }
        public int TenantID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int FolderVersionID { get; set; }
        public int WFStateID { get; set; }
        public int ApprovalStatusID { get; set; }
        public string Comments { get; set; }
        public Nullable<int> UserID { get; set; }
        public virtual WorkFlowStateApprovalTypeMaster ApprovalStatusType { get; set; }
        public virtual FolderVersion FolderVersion { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual User User { get; set; }
        public virtual WorkFlowVersionState WorkFlowVersionState { get; set; }
    }
}
