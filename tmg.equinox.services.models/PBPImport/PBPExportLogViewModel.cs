using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
   public class PBPExportLogViewModel
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public int ExportQueueID { get; set; }
        public string UserName { get; set; }

        public DateTime LogTime { get; set; }
    }
}
