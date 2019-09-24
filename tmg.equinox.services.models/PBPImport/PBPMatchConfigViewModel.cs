using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
    public class PBPMatchConfigViewModel:ViewModelBase
    {
        public int PBPMatchConfig1Up { get; set; }
        public int PBPImportQueueID { get; set; }
        public string QID { get; set; }
        public string PlanName { get; set; }
        public string PlanNumber { get; set; }
        public string ebsPlanName { get; set; }
        public string eBsPlanNumber { get; set; }
        public int FormInstanceId { get; set; }
        public int DocumentId { get; set; }
        public int FolderId { get; set; }
        public int FolderVersionId { get; set; }
        public int UserAction { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsIncludeInEbs { get;set;}
        public bool IsProxyUsed { get; set; }
        public bool IsTerminateVisiable { get; set; }
        public int Year { get; set; }
        public bool IsEGWPPlan { get; set; }
    }
}
