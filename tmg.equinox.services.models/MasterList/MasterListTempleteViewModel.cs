using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.MasterList
{
    public class MasterListTemplateViewModel : ViewModelBase
    {
        public int MasterListTemplate1Up { get; set; }
        public string MLSectionName { get; set; }
        public string FilePath { get; set; }
        public bool IsActive { get; set; }
    }
}
