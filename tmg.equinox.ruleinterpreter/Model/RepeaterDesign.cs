using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.document.rulebuilder.model
{
    public class RepeaterDesign
    {
        public int ID { get; set; }
        public string DisplayName { get; set; }
        public string UIElementFullName { get; set; }
        public string GeneratedName { get; set; }
        public int Sequence { get; set; }
        public int UIElementID { get; set; }
        public List<ElementDesign> Elements { get; set; }
    }
}
