using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class SEPY : Entity
    {
        public string SEPY_PFX { get; set; }
        public DateTime? SEPY_EFF_DT { get; set; }
        public DateTime? SEPY_TERM_DT { get; set; }
        public string SEPYHashcode { get; set; }
        public bool IsActive { get; set; }
        public string AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public bool? IsUsed { get; set; }
    }
}
