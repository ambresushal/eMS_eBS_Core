using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.CustomerCareDocument
{
    public class CustomerCareDocumentSearchViewModel
    {
        public int FolderID { get; set; }
        public int FolderVersionID { get; set; }
        public int FormInstanceID { get; set; }
        public string BenefitMatrix { get; set; }
        public string SBC { get; set; }
        public string SPD { get; set; }
        public string FaxBack { get; set; }
        public string CallGuide { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string Plan { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string FolderName { get; set; }
        public string DocumentType { get; set; }
    }

    public class CustomerCareDocumentViewModel
    {
        public int CustomerCareDocumentID { get; set; }
        public int FolderID { get; set; }
        public int FolderVersionID { get; set; }
        public int FormInstanceID { get; set; }
        public string DocumentName { get; set; }
        public int DocumentTypeID { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string Plan { get; set; }
        public string PlanType { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string FolderVersionNumber { get; set; }
        public bool IsActive { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public int TenantID { get; set; }
        public string AccountName { get; set; }
        public int AccountID { get; set; }
        public string DocumentTypeName { get; set; }
        public string FolderName { get; set; }
        public bool IsUrl { get; set; }
        public bool IsUploaded { get; set; }
        public string DocumentType { get; set; }
    }

    public class DocumentTypeViewModel
    {
        public int documentTypeID { get; set; }
        public string documentTypeName { get; set; }
    }

    public class PlanNameViewModel
    {
        public int FormInstanceId { get; set; }
        public string PlanName { get; set; }
    }

    public class FolderVersionEffectiveDateViewModel
    {
        public int FolderVersionID { get; set; }
        public string EffectiveDate { get; set; }
    }
}
