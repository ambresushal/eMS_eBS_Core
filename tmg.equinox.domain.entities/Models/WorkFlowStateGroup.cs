using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class WorkFlowStateGroup : Entity
    {
        public WorkFlowStateGroup()
        {
            //this.WorkFlowStates = new List<WorkFlowVersionState>();
        }

        public int WFStateGroupID { get; set; }
        public int TenantID { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string WFStateGroupName { get; set; }
        //public virtual ICollection<WorkFlowVersionState> WorkFlowStates { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}
