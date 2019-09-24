using System;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class PluginVersionProcessorStatus
    {
        public int PluginVersionStatusId { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
