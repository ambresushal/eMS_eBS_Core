using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
    public class ODMPlanConfigViewModel
    {
        public int Index { get; set; }
        public int PBPMatchConfig1Up { get; set; }
        public string QID { get; set; }

        public int FormInstanceId { get; set; }

        public int AnchorDocumentID { get; set; }
        public int SOTFormInstanceId { get; set; }

        public int PBPFormInstanceId { get; set; }

        public int FormDesignVersionId { get; set; }
        public int FolderId { get; set; }
        public int FolderVersionId { get; set; }
        public int Year { get; set; }
        public string Folder { get; set; }
        public string FolderVersion { get; set; }
        public bool IsIncludeInEbs { get; set; }

        public string View { get; set; }
        public string SelectPlan { get; set; }

        public bool IsActive { get; set; }

        public int BatchID { get; set; }

        public System.DateTime EffectiveDate { get; set; }
        public string FolderVersionNumber { get; set; }

        public int DocId { get; set; }
    }
}
