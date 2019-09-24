using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.MasterList
{
    public class MasterListImportViewModel
    {
        public int FormInstanceId { get; set; }
        public string ViewName { get; set; }
        public string SectionName { get; set; }
        public string Message { get; set; }
        public int FolderVersionId { get; set; }
        public int FolderId { get; set; }
        public string FolderType { get; set; }

    }
}
