using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.CollateralWindowService
{
    public class CollateralProcessQueueViewModel
    {
        public int CollateralProcessQueue1Up { get; set; }
        public int ProcessGovernance1Up { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public int? AccountID { get; set; }
        public string AccountName { get; set; }
        public int? FolderID { get; set; }
        public string FolderName { get; set; }
        public int? FolderVersionID { get; set; }
        public string FolderVersionNumber { get; set; }
        public int? FormInstanceID { get; set; }
        public string FormInstanceName { get; set; }
        public int? FormDesignID { get; set; }
        public int? FormDesignVersionID { get; set; }
        public DateTime? EffectiveDate { get; set; }
        //public string ProductJsonHash { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? ProcessStatus1Up { get; set; }
        public int? TemplateReportID { get; set; }
        public int? TemplateReportVersionID { get; set; }
        public DateTime? TemplateReportVersionEffectiveDate { get; set; }
        public string CollateralStorageLocation { get; set; }
        public bool? HasError { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ErrorDescription { get; set; }
        public string FilePath { get; set; }
    }
}
