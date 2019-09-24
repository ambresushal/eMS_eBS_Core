using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class AccountProductMap : Entity
    {
        public int AccountProductMapID { get; set; }
        public int FolderID { get; set; }
        public int FolderVersionID { get; set; }
        public string ProductType { get; set; }
        public string ProductID { get; set; }
        public string PlanCode{ get; set; }
        public int TenantID { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int FormInstanceID { get; set; }
        public bool IsActive { get; set; }
        public virtual Folder Folder { get; set; }
        public virtual FolderVersion FolderVersion { get; set; }
        public virtual FormInstance FormInstance { get; set; }
        public virtual Tenant Tenant { get; set; }
        public string ServiceGroup { get; set; }
        public string ProductName { get; set; }
        public string ANOCChartPlanType { get; set; }
        public string RXBenefit { get; set; }
        public string SNPType { get; set; }
    }
}
