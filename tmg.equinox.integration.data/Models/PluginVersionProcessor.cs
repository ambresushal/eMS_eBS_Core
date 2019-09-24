using System;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class PluginVersionProcessor
    {
        public int PluginVersionProcessorId { get; set; }
        public string Name { get; set; }
        public string OutPutFormat { get; set; }
        public Nullable<int> PluginVersionId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public virtual PluginVersion PluginVersion { get; set; }
    }
}
