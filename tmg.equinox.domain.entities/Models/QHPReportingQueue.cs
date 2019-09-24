using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class QHPReportingQueue : Entity
    {
        public int QueueID { get; set; }
        public DateTime QueuedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string DocumentLocation { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public bool OffExchangeOnly { get; set; }
       public ICollection<QHPReportingQueueDetails> QueueDetails { get; set; }
    }
}
