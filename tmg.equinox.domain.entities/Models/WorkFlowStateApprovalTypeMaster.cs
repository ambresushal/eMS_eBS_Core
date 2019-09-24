using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class WorkFlowStateApprovalTypeMaster : Entity
    {
        public int WorkFlowStateApprovalTypeID { get; set; }
        public int TenantID { get; set; }
        public string WorkFlowStateApprovalTypeName { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        //public virtual ICollection<WFVersionStatesApprovalType> WFVersionStatesApprovalType { get; set; }
        public virtual ICollection<FolderVersionWorkFlowState> FolderVersionWorkFlowStates { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}
