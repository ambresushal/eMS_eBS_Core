using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.web.FindnReplace
{
    public class DocumentInfo
    {
        public int FolderId { get; set; }
        public int FolderVersionId { get; set; }
        public int FormInstanceId { get; set; }
        public int FormDesignVersionId { get; set; }
        public string CurrentSection { get; set; }
    }
}
