using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.data.Models
{
    public partial class PDPX : Entity
    {
        public string PDBC_TYPE { get; set; }
        public string PDBC_PFX { get; set; }
        public bool IsUsed { get; set; }
        public bool isActive { get; set; }
    }
}
