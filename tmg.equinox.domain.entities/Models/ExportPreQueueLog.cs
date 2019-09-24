using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class ExportPreQueueLog:Entity
    {
        public int ExportPreQueueLog1Up { get; set; }
        public int ExportPreQueueId { get; set; }
        public int FromInstanceId { get; set; }
        public string ViewType { get; set; }
        public string Status { get; set; }
        public string ErrorLog { get; set; }
    }
}
