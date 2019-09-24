using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.documentcomparer
{
    public class CompareResult
    {
        public string RootSectionName { get; set; }
        public List<string> ParentSections { get; set; }
        public string Path { get; set; }
    }
}
