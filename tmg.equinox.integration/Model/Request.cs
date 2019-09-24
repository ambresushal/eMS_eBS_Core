using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.Model
{
    public class Request
    {
        public string actionType { get; set; }
        public string contextId { get; set; }
        public List<string> nextApproverIds { get; set; }
        public string comments { get; set; }
    }
}
