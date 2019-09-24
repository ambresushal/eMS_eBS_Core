using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class WFStatesApprovalTypeAction : Entity
    {
        public int WFStatesApprovalTypeActionID { get; set; }
        public int ActionID { get; set; }
        public int WFVersionStatesApprovalTypeID { get; set; }
        public string ActionResponse { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public virtual WFVersionStatesApprovalType WFVersionStatesApprovalType { get; set; }
        public virtual WorkFlowAction WorkFlowAction { get; set; }
    }
}
