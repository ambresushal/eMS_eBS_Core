using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class PluginTransmissionProcessQueue : Entity
    {
        public int ProcessQueueId { get; set; }
        public Nullable<int> PluginVersionProcessorId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public int PluginVersionStatusId { get; set; }
        public Nullable<bool> HasError { get; set; }
        public string BatchId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public virtual PluginVersionProcessor PluginVersionProcessor { get; set; }
        public virtual PluginVersionProcessorStatus PluginVersionProcessorStatus { get; set; }
        public string FolderVersionNumber { get; set; }
        public string FormInstanceName { get; set; }
        public string FolderName { get; set; }
        public string TrasmittedFilePath { get; set; }
    }
}
