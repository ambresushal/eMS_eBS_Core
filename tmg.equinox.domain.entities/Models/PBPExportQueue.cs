using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class PBPExportQueue : Entity
    {
        public int PBPExportQueueID { get; set; }
        public string Description { get; set; }
        public string ExportName { get; set; }
        public string Location { get; set; }
        public DateTime? ExportedDate { get; set; }
        public string ExportedBy { get; set; }
        public DateTime? ExportStartDate { get; set; }
        public DateTime? ExportEndDate { get; set; }
        public int Status { get; set; }
        public int PBPDatabase1Up { get; set; }
        public string PBPDataBase { get; set; }
        public int PlanYear { get; set; }
        public string PBPFilePath { get; set; }
        public string PlanAreaFilePath { get; set; }
        public string VBIDFilePath { get; set; }
        public string ZipFilePath { get; set; }
        public string MDBSchemaPath { get; set; }
        public int JobId { get; set; }
        public string ErrorMessage { get; set; }
        public string ImportStatus { get; set; }
    }
}
