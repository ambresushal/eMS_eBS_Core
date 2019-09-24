using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tmg.equinox.services.api.Models
{
    public class Documentadd
    {
        // this model is for create document
        public string DocumentName { get; set; }
        public int FolderversionID { get; set; }
        public bool IsCopy { get; set; }
        public string TemplateName { get; set; }
        //public int FormInstanceID { get; set; }
    }

    public class DocumentRef
    {
        // this model is for create document
        public int FolderversionID { get; set; }
        public int SourceFolderID { get; set; }
        public string SourceDocumentName { get; set; }
    }
}