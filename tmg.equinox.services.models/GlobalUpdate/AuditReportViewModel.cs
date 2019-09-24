using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
    public class AuditReportViewModel 
    {

        public string AccountName { get; set; }
        public string FolderName { get; set; }
        public string FolderVersionNumber { get; set; }
        public string EffectiveDate { get; set; }
        public string DocumentName { get; set; }
        public string UpdateStatus { get; set; }

    }
}
