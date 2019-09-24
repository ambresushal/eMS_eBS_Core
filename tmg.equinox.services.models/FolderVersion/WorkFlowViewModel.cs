using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersion
{
    public class WorkFlowViewModel : ViewModelBase
    {
        public int WFStateID { get; set; }
        public int TenantID { get; set; }
        public string WFStateName { get; set; }
        public int Sequence { get; set; }
        public bool IsActive { get; set; }
        public int? WFStateGroupID { get; set; }
    }
}
