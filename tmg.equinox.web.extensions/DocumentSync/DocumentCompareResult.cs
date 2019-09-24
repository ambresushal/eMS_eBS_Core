using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.documentcomparer
{
    public class DocumentCompareResult
    {
        public CompareDocument SourceDocument {get; set;}
        public CompareDocument TargetDocument {get; set;}
        public List<CompareResult> Results { get; set; }
        public bool CanSync { get; set; }
        public bool IsMatch { get; set; }
    }

    public class CompareDocument
    {
        public string DocumentType { get; set; }
        public string DocumentName { get; set; }
        public string FolderVerionNumber { get; set; }
        public string EffectiveDate { get; set; }
        public string FolderName { get; set; }
        public string AccountName { get; set; }
    }

    public enum CompareDocumentSource
    { 
      GENERATEREPORT,
      SYNCDOCUMENTS
    }
}
