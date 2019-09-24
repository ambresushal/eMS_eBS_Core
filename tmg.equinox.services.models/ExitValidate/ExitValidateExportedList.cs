using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.exitvalidate
{
    public class ExitValidateExportedList
    {
        public int ExitValidateQueueID { get; set; }        
        public int FormInstanceID { get; set; }
        public int FolderVersionID { get; set; }
        public int FolderID { get; set; }
        public int FormDesignVersionID { get; set; }
        public string FolderName { get; set; }
        public string ProductID { get; set; }
        public string FolderVersion { get; set; }
        public DateTime QueuedDateTime { get; set; }
        public Nullable<DateTime> CompletedDateTime { get; set; }
        public string Status { get; set; }
        public string User { get; set; }
        public string ErrorMessage { get; set; }

    }
}
