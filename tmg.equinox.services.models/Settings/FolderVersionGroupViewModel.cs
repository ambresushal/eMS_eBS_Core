using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Settings
{
    public class FolderVersionGroupViewModel : ViewModelBase
    {
        public int FolderVersionGroupID { get; set; }
        public string FolderVersionGroupName { get; set; }
        public bool IsActive { get; set; }
    }
}
