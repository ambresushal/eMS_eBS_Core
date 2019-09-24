using System;
namespace tmg.equinox.applicationservices.viewmodels.FolderVersion
{
    public class FormInstanceViewModel : ViewModelBase
    {
        public int FormInstanceID { get; set; }
        public int TenantID { get; set; }
        public int FolderVersionID { get; set; }
        public int FormDesignID { get; set; }
        public string FormDesignName { get; set; }
        public int FormDesignVersionID { get; set; }
        public int ConsortiumID { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        
        public string ConsortiumName { get; set; }
        public bool IsTranslated { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public string FormData { get; set; }
        public int FormInstanceDataMapID { get; set; }
        public int ObjectInstanceID { get; set; }
        public int FolderID { get; set; }
        public int FolderVersionStateID { get; set; }
        public int? FolderVersionBatchID { get; set; }
        public string FormDesignVersionNumber { get; set; }
        public string FormInstanceName { get; set; }
        public int UIElementID { get; set; }
        public string path { get; set; }
        public int? AnchorDocumentID { get; set; }
        public int DocID { get; set; }
        public string FolderVersionNumber { get; set; }
        public bool IsFormInstanceEditable { get; set; }
        public bool UsesMasterListAliasDesign { get; set; }
        public string FolderName { get; set; }
        public DateTime FolderEffectiveDate { get; set; }
        public string ProductName { get; set; }
        public string ProductID { get; set; }
        public string ProductType { get; set; }
    }

    public class FormInstanceAllProductsViewModel : ViewModelBase
    {
        public int FormInstanceID { get; set; }
        public string ProductName { get; set; }
        public string ProductID { get; set; }
        public string ProductType { get; set; }
        public string DocumentName { get; set; }
    }
}
