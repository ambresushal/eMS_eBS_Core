using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
    public class ExportPreQueueViewModel : ViewModelBase
    {
        public int ExportPreQueue1Up { get; set; }
        public int PBPDatabase1Up { get; set; }
        public string PreQueueStatus { get; set; }
        public int ExportId { get; set; }
        public int? UserId{get;set;}
    }
}
