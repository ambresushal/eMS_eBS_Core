using System;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class PluginProcessorError
    {
        public int ErrorId { get; set; }
        public Nullable<int> ProcessQueueId { get; set; }
        public string BatchId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public string ErrorDescription { get; set; }
        public Nullable<int> ErrorLine { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
