using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.viewmodels
{
    public class MasterListCascadeBatchDetailViewModel
    {
        public int MasterListCascadeDetailID { get; set; }

        public int MasterListCascadeBatchID { get; set; }

        public int MasterListCascadeID { get; set; }

        public int TargetFolderID { get; set; }

        public int PreviousFolderVersionID { get; set; }

        public int PreviousFormInstanceID { get; set; }

        public int NewFolderVersionID { get; set; }

        public int NewFormInstanceID { get; set; }

        public bool IsTargetMasterList { get; set; }

        public int Status { get; set; }

        public DateTime ProcessedDate { get; set; }

        public string Message { get; set; }

        public string FolderName { get; set; }

        public string FolderVersionNumber { get; set; }

        public string FormInstanceName { get; set; }

        public string FormDesignName { get; set; }

        public string StatusName { get; set; }

        public string PlanCode { get; set; }
    }
}
