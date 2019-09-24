using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Settings
{
    public class WorkFlowVersionStatesViewModel
    {
        public int TenantID { get; set; }
        public int WorkFlowVersionStateID { get; set; }
        public int WorkFlowVersionID { get; set; }
        public int WFStateID { get; set; }
        public string WFStateName { get; set; }
        public int Sequence { get; set; }
        public int? WFStateGroupID { get; set; }
    }
}
