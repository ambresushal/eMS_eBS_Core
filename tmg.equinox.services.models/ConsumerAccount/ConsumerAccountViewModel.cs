using System;
using System.Collections.Generic;

namespace tmg.equinox.applicationservices.viewmodels.ConsumerAccount
{
    public class ConsumerAccountViewModel : ViewModelBase
    {
        public string RowID { get; set; }
        public int? AccountID { get; set; }
        public string AccountName { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string VersionNumber { get; set; }
        public string Status { get; set; }
        public bool? Portfolio { get; set; }
        public int TenantID { get; set; }
        public string FolderName { get; set; }
        public int? FolderID { get; set; }
        public int? FolderVersionID { get; set; }
        public int FolderVersionCount { get; set; }
        public bool IsExpanded { get; set; }
        public string ConsortiumName { get; set; }
        public int ApprovalStatusID { get; set; }
        public int FolderVersionStateID { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string HAXSGroupID { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }

    }

    public class AccountDetailViewModel : ViewModelBase
    {
        public int TenantID { get; set; }
        public int AccountID { get; set; }
        public int FolderID { get; set; }
        public string FolderName { get; set; }
        public string ConsortiumName { get; set; }
        public int FolderVersionID { get; set; }
        public string VersionNumber { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public int FolderVersionStateID { get; set; }
    }

    public class DocumentViewModel : ViewModelBase
    {
        public string RowID { get; set; }
        public int TenantID { get; set; }
        public int? AccountID { get; set; }
        public string AccountName { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string VersionNumber { get; set; }
        public string FolderName { get; set; }
        public int? FolderID { get; set; }
        public int? FolderVersionID { get; set; }
        public string FolderVersionStateName { get; set; }
        public string ConsortiumName { get; set; }
        public string FormInstanceName { get; set; }
        public string CompareFilter { get; set; }
        public int FormInstanceID { get; set; }
        public int DocumentId { get; set; }
        public int FormDesignID { get; set; }
        public string DesignType { get; set; }
        public string MarketType { get; set; }
        public string State { get; set; }
        public string CSRVariationType { get; set; }
        public string CompareType { get; set; }

    }
    public class PortfolioFoldersDocumentViewModel : ViewModelBase
    {
        public string RowID { get; set; }
        public int TenantID { get; set; }
        public int? AccountID { get; set; }
        public string AccountName { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string VersionNumber { get; set; }
        public string FolderName { get; set; }
        public int? FolderID { get; set; }
        public int? FolderVersionID { get; set; }
        public string FolderVersionStateName { get; set; }
        public string ConsortiumName { get; set; }
        public string FormInstanceName { get; set; }
        public string CompareFilter { get; set; }
        public int? FormInstanceID { get; set; }
        public int? DocumentId { get; set; }
        public string eBsPlanName { get; set; }
        public string eBsPlanNumber { get; set; }
    }
}
