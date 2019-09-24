using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class PBPImportQueue : Entity
    {
        public int PBPImportQueueID { get; set; }
        public string Description { get; set; }
        public string PBPFileName { get; set; }
        public string PBPPlanAreaFileName { get; set; }
        public string Location { get; set; }
        public int PBPDatabase1Up { get; set; }
        public DateTime ImportStartDate { get; set; }
        public DateTime ImportEndDate { get; set; }
        public int Status { get; set; }
        public int Year { get; set; }
        public string PBPDataBase { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsFullMigration { get; set; }
        public int JobId { get; set; }
        public string JobLocation { get; set; }
        public string ErrorMessage { get; set; }
        public string ImportStatus { get; set; }
        public string PBPFileDisplayName { get; set; }
        public string PBPPlanAreaFileDisplayName { get; set; }
    }
}
