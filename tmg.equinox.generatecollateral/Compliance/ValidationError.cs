using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.generatecollateral
{
    public class ValidationError
    {
        public int No { get; set; }
        public string Error { get; set; }

        public string ComplianceType { get; set; }
    }
}
