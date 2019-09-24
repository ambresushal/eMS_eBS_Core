using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.masterListCascade
{
    public class ElementDocumentRuleInputViewModel
    {
        public int FormDesignVersionID { get; set; }
        public int FolderVersionID { get; set; }
        public int FormInstanceID { get; set; }
        public string ElementJSONPath { get; set; }
        public string ElementValue { get; set; }
    }
}
