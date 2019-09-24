using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Report
{
    public class SBCReportHeaderViewModel:ViewModelBase
    {
        public string AccountName { get; set; }
        public int TenantID { get; set; }
        public int FormInstanceId { get; set; }
        public int AccountID { get; set; }
    }
}
