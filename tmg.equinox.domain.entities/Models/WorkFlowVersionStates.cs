using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class WorkFlowVersionState : Entity
    {
        public int WorkFlowVersionStateID { get; set; }
        public int WorkFlowVersionID { get; set; }
        public int WFStateID { get; set; }
        public int Sequence { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public int? WFStateGroupID { get; set; }

        public virtual ICollection<WFVersionStatesApprovalType> WFVersionStatesApprovalType { get; set; }
        public virtual ICollection<WorkFlowVersionStatesAccess> WorkFlowVersionStatesAccess { get; set; }
        public virtual ICollection<FolderVersionWorkFlowState> FolderVersionWorkFlowStates { get; set; }
        public virtual ICollection<WorkFlowStateUserMap> WorkFlowStateUserMaps { get; set; }
        public virtual ICollection<WorkFlowStateUserMap> WorkFlowStateUserMaps1 { get; set; }
        public virtual ICollection<FolderVersion> FolderVersions { get; set; }
        public virtual ICollection<Journal> Journals { get; set; }
        public virtual ICollection<Journal> Journals1 { get; set; }
        public virtual ICollection<EmailLog> EmailLogs { get; set; }
       
        public virtual WorkFlowCategoryMapping WorkFlowCategoryMapping { get; set; }
        public virtual WorkFlowStateMaster WorkFlowState { get; set; }
    }
}
