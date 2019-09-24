using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
    public class PBPTableViewModel
    {
        public string FileName { get; set; }
        public string DataBaseName { get; set; }
        public int PBPImportQueueID { get; set; }
        public List<PBPPlanViewModel> PBPPlanList { get; set; }
        public PBPTableViewModel()
        {
            PBPPlanList = new List<PBPPlanViewModel>();
        }
    }

    public class PBPPlanViewModel {
        public int PBPMatchConfig1Up { get; set; }
        public string QID { get; set; }
        public string PBPPlanName { get; set; }
        public string PBPPlanNumber { get; set; }
        public int UserAction { get; set; }
        public string ebsPlanName { get; set; }
        public string ebsPlanNumber { get; set; }
        public int FolderId { get;set; }
        public int FolderVersionId { get; set; }
        public int FormInstanceId { get; set; }
        public int DocId { get; set; }
        public bool IsTerminateVisiable { get; set; }
        public int Year { get; set; }
        public bool IsEGWPPlan { get; set; }
        public string IsEGWP { get; set; }
    }
}
