using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersion
{
    public class ReportingFolderViewModel
    {
        public int FolderId { get; set; }
        public string FolderName { get; set; }
        public List<ReportingFolderVersionViewModel> FolderVersions { get; set; }
    }
}
