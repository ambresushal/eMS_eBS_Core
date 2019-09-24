using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersion
{
    public class WorkFlowStateFolderVersionMapViewModel : ViewModelBase
    {
        public int WFStateFolderVersionMapID { get; set; }
        public int ApplicableTeamID { get; set; }
        public string ApplicableTeamName { get; set; }
        public int FolderID { get; set; }
        public int FolderVersionID { get; set; }
        public int[] ApplicableTeamsIDList { get; set; }
    }
}
