using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
   public class ExportPreQueueLogViewModel:ViewModelBase
    {
        public int ExportPreQueueLog1Up { get; set; }
        public int ExportPreQueueId { get; set; }
        public int PBPDatabase1Up { get; set; }
        public int FromInstanceId { get; set; }
        public string ViewType { get; set; }
        public string Status { get; set; }
        public string ErrorLog { get; set; }
    }
}
