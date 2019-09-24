using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class ExportPreQueue : Entity
    {
        public int ExportPreQueue1Up { get; set; }
        public int PBPExportId { get; set; }
        public int PBPDatabase1Up { get; set; }
        public string PreQueueStatus { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public int CurrentUserId { get; set; }
    }
}
