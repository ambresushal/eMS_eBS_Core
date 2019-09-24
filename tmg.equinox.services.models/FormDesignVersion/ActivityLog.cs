using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FormDesignVersion
{
    public class ActivityLog
    {
        public string ElementPath { get; set; }
        public string ElementLabel { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

    }
}
