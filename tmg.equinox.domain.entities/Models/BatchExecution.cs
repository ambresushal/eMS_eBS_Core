using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class BatchExecution: Entity
    {


        #region Instance Properties

        public Int32 BatchExecutionID { get; set; }

        public Guid BatchID { get; set; }
        public Int32 BatchExecutionStatusID{get; set;}
        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public DateTime AddedDate { get; set; }

        public String AddedBy { get; set; }

        //public string StatusId { get;set; }

        public DateTime? UpdatedDate { get; set; }

        public String UpdatedBy { get; set; }
        public string RollBackComments { get; set; }
        public virtual Batch Batch { get; set; }
        public virtual BatchExecutionStatus BatchExecutionStatus { get; set; }
        #endregion Instance Properties
    }
}
