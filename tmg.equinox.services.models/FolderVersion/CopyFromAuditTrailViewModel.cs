using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersion
{
   public class CopyFromAuditTrailViewModel
    {
        
        public int CopyFromAuditTrailID { get; set; }      
        public string DestinationDocumentName { get; set; }       
        public string AccountName { get; set; }      
        public string FolderName { get; set; }
        public System.DateTime? FolderEffectiveDate { get; set; }      
        public string FolderVersionNumber { get; set; }
        public int FolderVersionID { get; set; }
        public int FolderID { get; set; } 
        public string SourceDocumentName { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime? AddedDate { get; set; }
        
    }
}
