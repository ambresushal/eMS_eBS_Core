using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class GlobalUpdateStatus : Entity
    {

        public GlobalUpdateStatus()
        {
            this.GlobalUpdates = new List<GlobalUpdate>();
        }

        public int GlobalUpdateStatusID { get; set; }
        public string GlobalUpdatestatus { get; set; }
        public bool? IsActive { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public virtual ICollection<GlobalUpdate> GlobalUpdates { get; set; }
    }
}
