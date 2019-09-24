using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.document.rulebuilder.model
{
    public class ElementDesign
    {
        public int ID { get; set; }
        public string DisplayName { get; set; }
        public string UIElementFullName { get; set; }
        public string GeneratedName { get; set; }
        public int Sequence { get; set; }
        public int UIElementID { get; set; }
        public SectionDesign Section { get; set; }
        public RepeaterDesign Repeater { get; set; }
        public string DataType { get; set; }        
        public string Type { get; set; }               
        public string Value { get; set; }
        public string OptionLabel { get; set; }
        public string OptionLabelNo { get; set; }       
        public string DefaultValue { get; set; }               
        
    }
}
