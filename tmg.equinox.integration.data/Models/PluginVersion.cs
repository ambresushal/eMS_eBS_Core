using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class PluginVersion : Entity
    {
        public int PluginVersionId { get; set; }
        public string Description { get; set; }
        public Nullable<int> PluginId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public virtual Plugin Plugin { get; set; }
    }
}
