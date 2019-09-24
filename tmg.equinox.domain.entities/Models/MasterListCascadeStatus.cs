using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class MasterListCascadeStatus : Entity
    {
        public MasterListCascadeStatus()
        {
            MasterListCascadeBatches = new HashSet<MasterListCascadeBatch>();
            MasterListCascadeBatchDetails = new HashSet<MasterListCascadeBatchDetail>();
        }
        public int StatusID { get; set; }
        public string Status { get; set; }
        public virtual ICollection<MasterListCascadeBatch> MasterListCascadeBatches { get; set; }
        public virtual ICollection<MasterListCascadeBatchDetail> MasterListCascadeBatchDetails { get; set; }
    }
}
