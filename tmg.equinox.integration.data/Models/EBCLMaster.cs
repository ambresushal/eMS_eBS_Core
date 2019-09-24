using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class EBCLMaster:Entity
    {
        public string PDBC_PFX { get; set; }
        public string EBCL_TYPE { get; set; }
        public string EBCL_YEAR_IND { get; set; }
        public Int16? ACAC_ACC_NO { get; set; }
        public string EBCL_PFX_MAX { get; set; }
        public string EBCL_ZERO_AMT_IND { get; set; }
    }
}
