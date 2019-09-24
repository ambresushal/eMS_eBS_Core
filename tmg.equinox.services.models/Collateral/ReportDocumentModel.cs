using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Collateral
{
    public class ReportDocumentModel
    {
        // Account Info
        public int? AccountID { get; set; }
        public string AccountName { get; set; }

        // Folder Info
        public int FolderID { get; set; }
        public string FolderName { get; set; }
        public int FolderVersionID { get; set; }
        public string FolderVersionNumber { get; set; }
        public DateTime? FolderVersionEffectiveDate { get; set; }

        // Form Info
        public int FormInstanceID { get; set; }
        public int FormInstanceIdForReportGeneration { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        //public int TemplateReportVersionID { get; set; }
        public string DataSourceName { get; set; }
        public string FormName { get; set; }
        public string DocumentType { get; set; }
        public string AnchorDocumentName { get; set; }
        public string IsSelected { get; set; }
    }
}
