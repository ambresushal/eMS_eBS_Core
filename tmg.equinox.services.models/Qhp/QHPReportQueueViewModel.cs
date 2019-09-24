using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Qhp
{
    public class QHPReportQueueViewModel : ViewModelBase
    {
        public int QueueID { get; set; }
        public DateTime QueuedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string DocumentLocation { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
