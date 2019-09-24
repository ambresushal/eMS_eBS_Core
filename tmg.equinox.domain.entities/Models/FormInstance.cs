using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormInstance : Entity
    {
        public FormInstance()
        {
            this.FormInstanceDataMaps = new List<FormInstanceDataMap>();
            this.FormInstanceActivityLogs = new List<FormInstanceActivityLog>();
        }

        public int FormInstanceID { get; set; }
        public int TenantID { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int FolderVersionID { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public virtual FolderVersion FolderVersion { get; set; }
        public virtual FormDesign FormDesign { get; set; }
        public virtual FormDesignVersion FormDesignVersion { get; set; }
        public virtual Tenant Tenant { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string ProductJsonHash { get; set; }
        public virtual ICollection<FormInstanceDataMap> FormInstanceDataMaps { get; set; }
        public virtual ICollection<AccountProductMap> AccountProductMaps { get; set; }
        public virtual ICollection<Journal> Journals { get; set; }
        public virtual ICollection<FormInstanceRepeaterDataMap> FormInstanceRepeaterDataMap2 { get; set; }
        public virtual ICollection<FormInstanceActivityLog> FormInstanceActivityLogs { get; set; }
        public virtual ICollection<FormInstanceViewImpactLog> FormInstanceViewImpactLog { get; set; }
        public virtual ICollection<IASFolderMap> IASFolderMaps { get; set; }
        public int DocID { get; set; }
        public Nullable<int> AnchorDocumentID { get; set; }
        //public virtual ICollection<DPFPlanTaskUserMapping> DPFPlanTaskUserMappings { get; set; }
    }
}
