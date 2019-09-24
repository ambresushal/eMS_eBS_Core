using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
   public class ODMMigrationQueueViewModel
    {
        public int BatchID { get; set; }
        public string MDBFileName { get; set; }
        public string MDBOriginalFileName { get; set; }
        public string Description { get; set; }
        public string Folder { get; set; }
        public string FolderVersion { get; set; }
        public DateTime MigratedDate { get; set; }
        public string MigratedBy { get; set; }
        public string Status { get; set; }

    }
}
