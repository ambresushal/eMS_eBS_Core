using System;
using System.Collections.Generic;

namespace tmg.equinox.integration.data.Models
{
    public partial class FormInstance : Entity
    {
        public FormInstance()
        {
            //this.AccountProductMaps = new List<AccountProductMap>();
            this.FormInstanceDataMaps = new List<FormInstanceDataMap>(); 
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
        public string Name { get; set; }
        //public virtual ICollection<AccountProductMap> AccountProductMaps { get; set; }
        public virtual FolderVersion FolderVersion { get; set; }
        //public virtual FormDesign FormDesign { get; set; }
        //public virtual FormDesignVersion FormDesignVersion { get; set; }
        //public virtual Tenant Tenant { get; set; }
        public virtual ICollection<FormInstanceDataMap> FormInstanceDataMaps { get; set; }
    }
}
