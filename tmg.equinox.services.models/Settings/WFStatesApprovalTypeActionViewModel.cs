using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Settings
{
    public class WFStatesApprovalTypeActionViewModel : ViewModelBase
    {
        public int WFStatesApprovalTypeActionID { get; set; }
        public int ActionID { get; set; }
        public string ActionName { get; set; }
        public int WFVersionStatesApprovalTypeID { get; set; }
        public string WFVersionStatesApprovalTypeName { get; set; }
        public string ActionResponse { get; set; }
    }
}
