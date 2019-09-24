using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
   public class PBPImportActivityLogViewModel:ViewModelBase
    {
        public int PBPImportActivityLog1Up { get; set; }
        public int PBPImportQueueID { get; set; }
        public int PBPImportBatchID { get; set; }
        public string FileName { get; set; }
        public string TableName { get; set; }
        public string Message { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserErrorMessage { get; set; }
    }
}
