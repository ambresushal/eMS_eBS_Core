using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.DPF
{
    public class DPFTaskStatusViewModel
    {
        public int TaskStatusID { get; set; }
        public string TaskStatusDescription { get; set; }
    }

    public class DPFTaskPriorityViewModel
    {
        public int ID { get; set; }
        public string Description { get; set; }
    }
}
