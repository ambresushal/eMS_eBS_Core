using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Collateral
{
    public class QueuedReportModel
    {
        public int CollateralProcessQueue1Up { get; set; }
        public string ProductID { get; set; }
        public string CollateralName { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string FolderName { get; set; }
        public string FolderVersionNumber { get; set; }
        public string ConsortiumName { get; set; }
        public string FormInstanceName { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public int FormInstanceID { get; set; }
        public int TemplateReportVersionID { get; set; }
        public bool PrintXManallyUploaded { get; set; }

    }
}
