using System;

namespace tmg.equinox.domain.entities.Models.CCRTranslator
{
    public class CCRTranslatorQueue : Entity
    {
        public int TranslatorQueue1Up { get; set; }
        public int ProcessGovernance1Up { get; set; }
       
        public int? ConsortiumID { get; set; }
        public string ConsortiumName { get; set; }

        public int AccountID { get; set; }
        public string AccountName { get; set; }

        public int FolderID { get; set; }
        public string FolderName { get; set; }

        public int FolderVersionID { get; set; }
        public string FolderVersionNumber { get; set; }

        public int FormInstanceID { get; set; }
        public string FormInstanceName { get; set; }
        public string ProductName { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public int ProcessStatus1Up { get; set; }
        public bool HasError { get; set; }
        public bool IsActive { get; set; }
        public bool IsProductNew { get; set; }
        public bool GenerateNewProduct { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        
        public string ErrorDescription { get; set; }
       
        public bool IsRetro { get; set; }
       
        public bool IsShell { get; set; }

        public string ProductID { get; set; }
        public string ProductType { get; set; }
    }
}
