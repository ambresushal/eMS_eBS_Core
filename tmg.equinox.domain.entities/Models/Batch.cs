using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class Batch : Entity
    {
        public Batch()
        {
            this.AuditReports = new List<AuditReport>();
            this.BatchExecutions = new List<BatchExecution>();
            this.BatchIASMaps = new List<BatchIASMap>();
        }

        #region Instance Properties

        public Guid BatchID { get; set; }

        public String BatchName { get; set; }

        public String ExecutionType { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public TimeSpan? ScheduledTime { get; set; }

        public Boolean IsApproved { get; set; }

        public String ApprovedBy { get; set; }

        public DateTime AddedDate { get; set; }

        public String AddedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public String UpdatedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public virtual ICollection<BatchExecution> BatchExecutions { get; set; }
        public virtual ICollection<AuditReport> AuditReports { get; set; }       
        public virtual ICollection<BatchIASMap> BatchIASMaps { get; set; }

        #endregion Instance Properties
    }
}
