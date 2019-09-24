using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class ReturnConfigData:Entity
    {
        public int PDPD_PDDS { get; set; }
        public int DEDE { get; set; }
        public int LTLT { get; set; }
        public int AssignServRule { get; set; }
        public int SEPY { get; set; }
    }
}
