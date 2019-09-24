using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.CompareSync
{
    public class DocumentElementBaseModel
    {
        public int UIElementID { get; set; }
        public string Label { get; set; }
        public string UIElementPath { get; set; }
        public bool isChecked { get; set; }
        public string GeneratedName { get; set; }
        public string ElementType { get; set; }
        public string FilterValue { get; set; }
    }
}
