using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class Status : Entity
    {
        public Status()
        {
            this.FormDesignVersions = new List<FormDesignVersion>();
        }

        public int StatusID { get; set; }
        public string Status1 { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<FormDesignVersion> FormDesignVersions { get; set; }
    }
}
