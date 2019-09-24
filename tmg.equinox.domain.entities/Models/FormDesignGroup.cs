using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormDesignGroup : Entity
    {
        public FormDesignGroup()
        {
            this.Folders = new List<Folder>();
            this.FormDesignGroupMappings = new List<FormDesignGroupMapping>();
        }

        public int FormDesignGroupID { get; set; }
        public string GroupName { get; set; }
        public int TenantID { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsMasterList { get;set; }
        public virtual ICollection<Folder> Folders { get; set; }
        public virtual ICollection<FormDesignGroupMapping> FormDesignGroupMappings { get; set; }
    }
}
