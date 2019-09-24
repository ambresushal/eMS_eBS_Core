using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
    public class PBPDataMapViewModel
    {
        public int PBPDataMapId { get; set; }
        public string QID { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public string JsonData { get; set; }
        public int PBPImportQueueID { get; set; }
    }
}
