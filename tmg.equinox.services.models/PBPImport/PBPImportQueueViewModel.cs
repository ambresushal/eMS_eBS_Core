using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
    public class PBPImportQueueViewModel:ViewModelBase
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
        public int StatusCode { get; set; }
        public int Year { get; set; }
        public string PBPDataBase { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string DataBaseName { get; set; }
        public bool IsFullMigration { get; set; }
        public int JobId { get; set; }
        public string ImportStatus { get; set; }
        public string ErrorMessage { get; set; }
        public string StatusText { get; set; }
        public string PBPFileDisplayName { get; set; }
        public string PBPPlanAreaFileDisplayName { get; set; }
        public int UpdateHours { get; set; }
        public DateTime UdateDateTime { get; set; }
    }

    public class FormInstanceViewModel
    {
        public int FormInstanceID { get; set; }
        public int DocId { get; set; }
        public string Name { get; set; }
        public int FolderVersionId { get; set; }
        public int FolderId { get; set; }
        public int FormDesignVersionID { get; set; }
    }
}

