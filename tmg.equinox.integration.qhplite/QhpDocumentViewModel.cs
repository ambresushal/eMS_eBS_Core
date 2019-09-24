using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite
{
    public class QhpDocumentViewModel
    {
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public string DocumentData { get; set; }
    }

    public class QhpDocumentGroupModel
    {
        public int DocumentID { get; set; }
        public JObject GroupMap { get; set; }

        public JObject DocumentData { get; set; }
    }
}
