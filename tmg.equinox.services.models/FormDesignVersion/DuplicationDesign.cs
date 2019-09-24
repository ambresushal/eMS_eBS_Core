using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FormDesignVersion
{
    public class DuplicationDesign
    {
        public string FullName { get; set; }
        public string UIElementName { get; set; }
        public string GeneratedName { get; set; }
        public string Type { get; set; }
        public string ParentUIElementName { get; set; }
        public int? ParentUIElementID { get; set; }
        public bool CheckDuplicate { get; set; }
    }
}
