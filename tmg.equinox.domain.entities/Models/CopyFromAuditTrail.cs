using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class CopyFromAuditTrail:Entity
    {
        public int CopyFromAuditTrailID { get; set; }
        public int DestinationDocumentID { get; set; }
        public int? AccountID { get; set; }
        public int FolderID { get; set; }
        public System.DateTime? EffectiveDate { get; set; }
        public int SourceFolderVersionID { get; set; }
        public int DestinationFolderVersionID { get; set; }      
        public int SourceDocumentID { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime ? AddedDate { get; set; }       

    }
}
