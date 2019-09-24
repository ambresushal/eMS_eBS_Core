using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Collateral
{
    public class ReportDesignViewModel
    {
        public int ReportId { get; set; }
        public int ReportTemplateVersionID { get; set; }
        public string ReportName { get; set; }
        public string ReportTemplateLocation { get; set; }
        public string VersionNumber { get; set; }
    }

    public class DocumentAccountFolderInfo
    {
        public int AccountID { get; set; }
        public int AccountName { get; set; }
        public int FolderID { get; set; }
        public int FolderName { get; set; }
    }

    public class ComplianceValidationlogModel
    {
        public long logId { get; set; }
        public int FormInstanceID { get; set; }
        public System.DateTime AddedDate { get; set; }

        public int No { get; set; }
        public string Error { get; set; }
        public string AddedBy { get; set; }
        public string ComplianceType { get; set; }
        public string ValidationType { get; set; }

    }
}