using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.schema.Base.Model
{
    public class JData
    {
        public int FormInstanceId { get; set; }
        public string FormInstanceName { get; set; }
        public int FormDesignId { get; set; }
        public int FormDesignVersionId { get; set; }
        public string FormData { get; set; }
        public int FolderID { get; set; }
        public int FolderVersionID { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool IsMasterList { get; set; }
        public int? AnchorDocumentID { get; set; }

    }
}
