using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.MileStoneChecklistReport
{
    public class MilestoneChecklistSearchReportViewModel:ViewModelBase
    {
        public int FolderID { get; set; }
        public int FolderVersionID { get; set; }
        public int FormInstanceID { get; set; }
        public string FolderVersionNumber { get; set; } 
        public string HAXSGroupID { get; set; }     
        public string GroupName { get; set; }       
        public DateTime? EffectiveDate { get; set; }
        public string FolderName { get; set; }       
        public string FormData { get; set; }
        public int AccountID { get; set; }
        public string FormName { get; set; }
        public int FormDesignID { get; set; }
    }
}
