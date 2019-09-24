using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.RulesManager
{
    public class ElementRowModel
    {
        public string Element { get; set; }
        public int? Parent { get; set; }
        public string Section { get; set; }
        public int UIElementID { get; set; }
        public string UIElementName { get; set; }
        public bool IsContainer { get; set; }
    }
}
