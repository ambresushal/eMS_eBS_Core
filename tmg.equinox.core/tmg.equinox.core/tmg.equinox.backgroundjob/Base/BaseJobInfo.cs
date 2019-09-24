using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.backgroundjob.Base
{
    public class BaseJobInfo
    {
        public string Name { get; set; }
        public int QueueId { get; set; }
        public string JobId { get; set; }
        public string UserId { get; set; }
        public string FeatureId { get; set; }

        public string Status { get; set; }

        public string AssemblyName { get; set; }
        public string ClassName { get; set; }

        public string Error { get; set; }

    }
}
