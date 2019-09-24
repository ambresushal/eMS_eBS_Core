using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.web.FindnReplace
{
    public class ReplaceCriteria
    {
        public string FindText { get; set; }
        public string ReplaceWith { get; set; }
        public string ReplaceWithIn { get; set; }
        public string SpecificPath { get; set; }
        public bool IsMatch { get; set; }
        public string SelectedInstances { get; set; }
    }
}
