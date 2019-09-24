using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class BSTXSRC:Entity
    {
        public string PDBC_PFX { get; set; }
        public Byte[] BSTX_TEXT { get; set; }
        public string BSBS_TYPE { get; set; }
        public Int16 BSTX_SEQ_NO { get; set; }
        public int? ProcessGovernance1up { get; set; }
    }
}
