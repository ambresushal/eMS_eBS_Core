using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
    public class BatchViewModel : ViewModelBase
    {
        #region Instance Properties
        public Guid BatchID { get; set; }
        public String BatchName { get; set; }
        public String ExecutionType { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public TimeSpan? ScheduledTime { get; set; }
        public Boolean IsApproved { get; set; }
        public string IsApprovedString { get; set; }
        public String ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string BatchExecutionStatus { get; set; }
        #endregion Instance Properties
    }
}
