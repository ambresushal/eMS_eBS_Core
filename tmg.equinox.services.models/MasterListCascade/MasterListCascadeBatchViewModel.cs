using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.viewmodels
{
    public class MasterListCascadeBatchViewModel
    {
        public int MasterListCascadeBatchID { get; set; }
        public int MasterListCascadeID { get; set; }
        public DateTime QueuedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string ErrorMessage { get; set; }
        public string MasterListName { get; set; }
        public string QueuedBy { get; set; }
    }
}
