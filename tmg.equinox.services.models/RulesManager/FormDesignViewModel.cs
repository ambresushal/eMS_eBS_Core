using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.RulesManager
{
    public class FormDesignViewModel
    {
        public int FormID { get; set; }
        public string FormName { get; set; }
        public string Group { get; set; }
        public string VersionNumber { get; set; }
        public int Status { get; set; }
    }
}
