using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tmg.equinox.web.Report
{
    public class ReportQueueInfo
    {
        public string JobName { get; set; }
        public long JobId { get; set; }
        public string UserId { get; set; }
        public string FeatureId { get; set; }

        public string Status { get; set; }

        public string AssemblyName { get; set; }
        public string ClassName { get; set; }

        public string Error { get; set; } 
              
        public int ReportQueueId { get; set; }
    }
}