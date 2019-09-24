using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
   public class GuUpdateUIElementValueViewModel
    {
        public int GlobalUpdateId { get; set; }
        public int UIElementID { get; set; }
        public int TenantId { get; set; }
        public int FormDesignVersionId { get; set; }
        public int UIElementTypeId { get; set; }
        public string ElementType { get; set; }
        public string ElementHeader { get; set; }
        public IEnumerable<GuRuleRowModel> Rules { get; set; }
        public bool AreRulesModified { get; set; }
        public bool IsPropertyGridModified { get; set; } 
    }
}
