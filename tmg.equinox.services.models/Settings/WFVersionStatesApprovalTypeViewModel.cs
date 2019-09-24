using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Settings
{
    public class WFVersionStatesApprovalTypeViewModel : ViewModelBase
    {
        public int WFVersionStatesApprovalTypeID { get; set; }
        public int WorkFlowStateApprovalTypeID { get; set; }
        public string WorkFlowStateApprovalTypeName { get; set; }
        public int WorkFlowVersionStateID { get; set; }
    }
}
