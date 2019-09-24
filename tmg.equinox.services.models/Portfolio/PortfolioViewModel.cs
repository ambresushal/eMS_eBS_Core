using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Portfolio
{
    public class PortfolioViewModel : ViewModelBase
    {
        public string ProductType { get; set; }
        public string ProductName { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string VersionNumber { get; set; }
        public string Status { get; set; }
        public int TenantID { get; set; }
        public string FolderName { get; set; }
        public int? FolderID { get; set; }
        public int? FolderVersionID { get; set; }
        public int? FolderVersionCount { get; set; }
        public int? AccountID { get; set; }
        public int MarketSegmentID{ get; set; }
        public int? PrimaryContactID { get; set; }
        public int? ParentFolderId { get; set; }
        public int? UsesCount { get; set; }
        public int ApprovalStatusID { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string ProductList { get; set; }

        public bool Mode { get; set; }
        public string IsFoundation{ get; set; }
    }
}
