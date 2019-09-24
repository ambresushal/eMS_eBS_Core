using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Report
{
    public class SBCReportServiceViewModel:ViewModelBase
    {
        public int SBCReportServiceMasterID { get; set; }
        public string ReportPlaceHolderID { get; set; }
        public string Category1Name { get; set; }
        public string Category2Name { get; set; }
        public string Category3Name { get; set; }
        public string POSName { get; set; }
        public string NetworkName { get; set; }
        public int TenantID { get; set; }
    }
}
