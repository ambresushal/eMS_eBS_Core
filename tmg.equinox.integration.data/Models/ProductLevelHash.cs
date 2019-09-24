using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public partial class ProductLevelHash : Entity
    {
        public string ProductID { get; set; }
        public string PDPDHash { get; set; }
        public string EBCLHash { get; set; }
        public string BSBSHash { get; set; }
        public string PDDSHash { get; set; }
        public string EBCLPFX { get; set; }
        public string BSBSPFX { get; set; }
        public int isNewEBCL { get; set; }
        public int isNewBSBS { get; set; }
        public bool? IsUsingNewBSBS { get; set; }
        public int? ProcessGovernance1up { get; set; }        
    }
}
