using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public partial class PDVCHash : Entity
    {
        public string ProductID { get; set; }
        public string BenefitSet { get; set; }
        public string SEPYPFX { get; set; }
        public string DEDEPFX { get; set; }
        public string LTLTPFX { get; set; }
        public string DEDEHash { get; set; }
        public string LTLTMainHash { get; set; }
        public string SEPYHash { get; set; }
        public DateTime? EFF_DT { get; set; }
        public int? ProcessGovernance1up { get; set; }
        public int isNewSEPY { get; set; }
        public int isNewDEDE { get; set; }
        public int isNewLTLT { get; set; }
        public string LTLTHash { get; set; }
        public string LTSEHash { get; set; }
        public string LTIDHash { get; set; }
        public string LTIPHash { get; set; }
        public string LTPRHash { get; set; }
    }
}
