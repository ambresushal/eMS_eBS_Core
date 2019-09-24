using System;

namespace tmg.equinox.applicationservices.viewmodels.DocumentCollateral
{
    public class DocumentCollateralViewModel : ViewModelBase
    {
        public string RowID { get; set; }
        public int? AccountID { get; set; }
        public string AccountName { get; set; }
        public string ProductName { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string VersionNumber { get; set; }
        public string Status { get; set; }
        public int TenantID { get; set; }
        public string FolderName { get; set; }
        public int? FolderID { get; set; }
        public int? FolderVersionID { get; set; }
        public int FormInstanceID { get; set; }
    }
}
