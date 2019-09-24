using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersion
{
    public class FolderViewModel
    {
        public int DocId { get; set; }
        public string FormName { get; set; }
        public int FolderVersionID { get; set; }
        public int DocumentViews { get; set; }
        public int FormDesignVersionId { get; set; }

        public int FolderID { get; set; }
        public string FolderName { get; set; }
        public string MarketSegment { get; set; }
        public string PrimaryContact { get; set; }
    }
}
