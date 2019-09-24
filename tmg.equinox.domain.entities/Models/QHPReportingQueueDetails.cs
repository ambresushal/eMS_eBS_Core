using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class QHPReportingQueueDetails : Entity
    {
        public int QueueDetailID { get; set; }
        public int QueueID { get; set; }
        public int? FolderID { get; set; }
        public int? FolderVersionID { get; set; }
        public int FormInstanceID { get; set; }
        public int FormDesignID { get; set; }
        public QHPReportingQueue Queue { get; set; }
    }
}
