using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Settings
{
    public class WorkFlowVersionStatesAccessViewModel : ViewModelBase
    {
        public int WorkFlowVersionStatesAccessID { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public int WorkFlowVersionStateID { get; set; }
    }
}
