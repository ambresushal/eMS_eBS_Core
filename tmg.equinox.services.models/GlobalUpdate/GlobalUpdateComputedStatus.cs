using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
    public class GlobalUpdateComputedStatus
    {
        public int GlobalUpdateId { get; set; }
        public string ExecutionStatusText { get; set; }
        public string ExecutionStatusSymbol { get; set; }
        public string ExecutionStatus { get; set; }
    }
}
