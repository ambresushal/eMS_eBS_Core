using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class MasterListCascadeBatch : Entity
    {
        public MasterListCascadeBatch()
        {
            MasterListCascadeBatchDetails = new HashSet<MasterListCascadeBatchDetail>();
        }

        public int MasterListCascadeBatchID { get; set; }
        public int FormDesignVersionID { get; set; }
        public DateTime QueuedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
        public virtual MasterListCascadeStatus MasterListCascadeStatus { get; set; }
        public virtual ICollection<MasterListCascadeBatchDetail> MasterListCascadeBatchDetails { get; set; }
        public string QueuedBy { get; set; }
    }
}
