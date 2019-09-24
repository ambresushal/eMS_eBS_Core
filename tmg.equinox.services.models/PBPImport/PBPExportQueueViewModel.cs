using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
    public class PBPExportQueueViewModel
    {
        public int PBPExportQueueID { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public int PlanYear { get; set; }
        public DateTime? ExportedDate { get; set; }
        public string ExportedBy { get; set; }
        public int Status { get; set; }
        public int FolderID { get; set; }
        public int FolderVersionID { get; set; }
        public string PBPDatabase { get; set; }
        public string PBPFilePath { get; set; }
        public string PlanAreaFilePath { get; set; }
        public string VBIDFilePath { get; set; }
        public string ZipFilePath { get; set; }
        public string MDBSchemaPath { get; set; }
        public int PBPDatabase1Up { get; set; }
        public string ErrorMessage { get; set; }
        public string ImportStatus { get; set; }
        public int JobId { get; set; }
        public string ErrorMsg { get; set; }
        public string StatusText { get; set; }
       public string UserId { get; set; }


    }
}
