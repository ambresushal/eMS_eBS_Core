using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
   public class PBPPlanConfigViewModel
    {
        public int Index { get; set; }
        public int PBPMatchConfig1Up { get; set; }
        public int PBPImportQueueID { get; set; }
        public string QID { get; set; }
        public string PlanName { get; set; }
        public string PlanNumber { get; set; }
        public string ebsPlanName { get; set; }
        public string eBsPlanNumber { get; set; }
        public int FormInstanceId { get; set; }
        public int DocumentId { get; set; }
        public int Year { get; set; }
        public int FolderId { get; set; }
        public string FolderVersionName{get;set;}
        public int FolderVersionId { get; set; }
        public string FormInstanceName { get; set; }
        public int UserAction { get; set; }
        public string DataBaseFileName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string ProductDetails { get; set; }
        public int PBPDataBase1Up { get; set; }
        public string PBPDataBaseName { get; set; }
        public bool IsIncludeInEbs { get; set; }
        public string FolderVersion { get; set; }
        public string UserActionText { get; set; }
        public bool IsTerminateVisible { get; set; }
        public bool IsProxyUsed { get; set; }
        public bool IsDisableIsIncludeIneBsFlag{ get; set; }
        public bool IsEGWPPlan { get; set; }
    }

    public class PBPBaseLineFolderDetailViewModel
    {
        public int FolderId { get; set; }
        public int BeforeBaseLineFolderVersionId { get; set; }
        public int AfterBaseLineFolderVersionId { get; set; }

    }
}
