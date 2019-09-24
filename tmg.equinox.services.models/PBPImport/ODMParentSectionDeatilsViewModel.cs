using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
   public class ODMParentSectionDeatilsViewModel
    {
        public string Label { get; set; }
        public int UIElementID { get; set; }
        public string UIElementName { get; set; }
        public string GeneratedName { get; set; }

        public string SelectSection { get; set; }

        public int SectionId { get; set; }

        public string SectionName { get; set; }
    }
}
