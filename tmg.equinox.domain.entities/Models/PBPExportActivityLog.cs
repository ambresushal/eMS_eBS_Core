using System;

namespace tmg.equinox.domain.entities.Models
{
    public class PBPExportActivityLog : Entity
    {
        public int PBPExportActivityLogID { get; set; }
        public int PBPExportQueueID { get; set; }
        public string TableName { get; set; }
        public string Message { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
