using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.ReportingCenter
{
    public class ReportingCenterDataSyncViewModel
    {
        public int FormInstanceId { get; set; }
        public int FormDesignVersionId { get; set; }
        public string Name { get; set; }

        public DateTime EffectiveDate { get; set; }

        public string Status { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Version { get; set; }

        public string FolderName { get; set; }

        public string ProcessType { get; set; }

        public string ViewType { get; set; }


    }
}
