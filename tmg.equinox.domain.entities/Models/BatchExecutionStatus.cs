using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class BatchExecutionStatus:Entity
    {
        public BatchExecutionStatus() {
            this.BatchExecutions = new List<BatchExecution>();
        }

        #region Instance Properties

        public Int32 BatchExecutionStatusID { get; set; }

        public String StatusName { get; set; }

        public String Description { get; set; }

        public virtual ICollection<BatchExecution> BatchExecutions { get; set; }
        #endregion Instance Properties
    }
}
