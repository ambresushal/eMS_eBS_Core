using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.exitvalidate
{
    public class ExitValidateViewModel : ViewModelBase
    {
        public int FormInstanceID { get; set; }
        public int FolderVersionID { get; set; }
        public int FormDesignVersionID { get; set; }
        public int FolderID { get; set; }
        public string ProductID { get; set; }
        public int UserID { get; set; }
        public int QueueID { get; set; }
        public string[] Sections { get; set; }
        public bool SetAsDefault { get; set; }
        public int JobId { get; set; }
        public string Status { get; set; }
        public string ExitValidateFilePath { get; set; }
        public string VBIDFilePath { get; set; }
        public string PlanAreaFilePath { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime Year { get; set; }
        public string Name { get; set; }
        public string FormName { get; set; }
        public int? AnchorDocumentID { get; set; }
        public string IsExitValidate { get; set; }
        public bool IsLatest { get; set; }
        public Nullable<DateTime> CompletedDate { get; set; }
        public string Section { get; set; }
        public string QID { get; set; }
        public bool IsQueuedForWFStateUpdate { get; set; }
        public string UsersInterestedInStatus { get; set; }
    }
    
}
