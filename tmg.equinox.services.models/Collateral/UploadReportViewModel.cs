using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Collateral
{
    public class UploadReportViewModel
    {
        public string ProductID { get; set; }
        public string CollateralName { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string FolderName { get; set; }
        public string FolderVersionNumber { get; set; }
        public string FormInstanceName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public int FormInstanceID { get; set; }
        public int TemplateReportVersionID { get; set; }
        public byte[] WordFile { get; set; }
        public byte[] PrintxFile { get; set; }
        public byte[] File508 { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public int FolderID { get; set; }
        public int FolderVersionId { get; set; }
        public string CreatedBy { get; set; }

        public bool AlreadyConverted508 { get; set; }

    }
}
