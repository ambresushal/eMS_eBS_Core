using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersion
{
    public class OpenDocumentViewModel
    {
        public int AnchorFormInstanceId { get; set; }
        public int DocId { get; set; }
        public string FormName { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public string ContractCode { get; set; }
        public string PlanNumber { get; set; }
        public List<DocumentViewListViewModel> DocumentViews { get; set; }
        public IEnumerable<FolderVersionViewModel> FolderVersions { get; set; }
        public int LockedFormInstanceId { get; set; }
        public bool IsDocumentLocked { get; set; }
        public bool UnlockDocument { get; set; }
        public string LockedBy { get; set; }
    }

    public class DocumentViewListViewModel
    {
        public int FormInstanceId { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignTypeID { get; set; }
        public string FormDesignTypeName { get; set; }
        public string FormDesignName { get; set; }
        public string FormDesignDisplayName { get; set; }
        public string FormInstanceName { get; set; }
        public int FormDesignVersionID { get; set; }
        public bool IsLocked { get; set; }
        public int? LockedBy { get; set; }
        public string LockedByUser { get; set; }
        public DateTime LockedDate { get; set; }
        public bool IsEditable { get; set; }
        public bool IsSectionLockEnabled { get; set; }
    }

    public class DocumentOverrideSelection
    {
        public int FormInstanceId { get; set; }
        public bool IsOverrideDocument { get; set; }
        public string IsDocumentLocked { get; set; }
        public string SectionLocked { get; set; }
    }

    public class OpenDocumentVM
    {
        public int FormInstanceID { get; set; }
        public string FormInstanceName { get; set; }
        public int DocId { get; set; }
        public int? AnchorDocumentID { get; set; }
        public string FormName { get; set; }
        public string FormDesignDisplayName { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public string ContractCode { get; set; }
        public string PlanNumber { get; set; }
        public int FolderVersionID { get; set; }
        public string FolderVersionNumber { get; set; }
        public bool IsPortfolio { get; set; }
        public int FolderVersionStateID { get; set; }
        public int DocumentDesignTypeID { get; set; }

        public string DocumentDesignName { get; set; }
    }

}
