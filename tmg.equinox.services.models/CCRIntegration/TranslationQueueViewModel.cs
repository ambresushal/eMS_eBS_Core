using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.CCRIntegration
{
    public class TranslationQueueViewModel
    {
        public string ProductID { get; set; }
        public string Status { get; set; }
        public string FolderVersionNumber { get; set; }
        public string ProcessTime { get; set; }

        public string AccountName { get; set; }
        public string FolderName { get; set; }
        public DateTime? EffectiveDate { get; set; }

        public int ProcessGovernance1Up { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? ProcessStatus1Up { get; set; }

        public string ProductName { get; set; }

        public string ProductType { get; set; }
    }
}
