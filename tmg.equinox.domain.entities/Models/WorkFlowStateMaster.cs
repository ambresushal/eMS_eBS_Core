using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class WorkFlowStateMaster : Entity
    {
        public int WFStateID { get; set; }
        public int TenantID { get; set; }
        public string WFStateName { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<WorkflowTaskMap> applicableWorkflowTaskMap { get; set; }
        public virtual ICollection<WorkFlowVersionState> WorkFlowVersionStates { get; set; }
        public virtual ICollection<DPFPlanTaskUserMapping> DPFPlanTaskUserMappings { get; set; }
    }
}
