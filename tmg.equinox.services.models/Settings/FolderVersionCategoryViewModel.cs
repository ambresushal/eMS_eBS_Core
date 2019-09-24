using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Settings
{
    public class FolderVersionCategoryViewModel : ViewModelBase
    {
        public int FolderVersionCategoryID { get; set; }
        public string FolderVersionCategoryName { get; set; }
        public bool IsActive { get; set; }
        public int FolderVersionGroupID { get; set; }
        public string FolderVersionGroupName { get; set; }
    }
}
