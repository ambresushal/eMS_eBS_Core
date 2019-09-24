using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.DocumentRule
{
    public class SourceDesignDetails
    {
        public string FormName { get; set; }
        public int FormDesignVersionId { get; set; }
        public int FormInstanceId { get; set;}
        public string RuleEventTree { get; set; }
        public int FolderVersionId { get; set; }
        public int FormDesignId { get; set; }
    }
}
