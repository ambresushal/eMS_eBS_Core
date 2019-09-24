using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class WFVersionStatesApprovalType : Entity
    {
        public int WFVersionStatesApprovalTypeID { get; set; }
        public int WorkFlowStateApprovalTypeID { get; set; }
        public int WorkFlowVersionStateID { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }

        public virtual WorkFlowVersionState WorkFlowVersionStates { get; set; }
        public virtual WorkFlowStateApprovalTypeMaster WorkFlowStateApprovalTypeMaster { get; set; }
        public virtual ICollection<WFStatesApprovalTypeAction> WFStatesApprovalTypeActions { get; set; }
    }
}
