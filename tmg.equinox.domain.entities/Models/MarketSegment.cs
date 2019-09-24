using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class MarketSegment : Entity
    {
        public MarketSegment()
        {
            this.Folders = new List<Folder>();
        }

        public int MarketSegmentID { get; set; }
        public int TenantID { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string MarketSegmentName { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Folder> Folders { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}
