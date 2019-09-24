using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersion
{
    public class FolderVersionPropertiesViewModel : ViewModelBase
    {
        public string FormInstanceName { get; set; }
        public string FormDesignName { get; set; }
        public string VersionNumber { get; set; }
        public string DisplayText { get; set; }
    }
}
