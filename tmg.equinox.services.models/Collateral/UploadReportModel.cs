using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Collateral
{
    public class UploadReportModel
    {
        public int ID { get; set; }
        public string ProductID { get; set; }
        public string CollateralName { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string FolderName { get; set; }
        public string FolderVersionNumber { get; set; }
        public string FormInstanceName { get; set; }
        public DateTime? CreatedDate { get; set; }
        //public DateTime? ProcessedDate { get; set; }
        public int FormInstanceID { get; set; }
        //public int TemplateReportVersionID { get; set; }

    }

    public class DownloadFileModel
    {
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
}
