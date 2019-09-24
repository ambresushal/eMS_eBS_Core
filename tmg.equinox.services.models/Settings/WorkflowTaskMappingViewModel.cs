using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Settings
{
   public  class WorkflowTaskMappingViewModel:ViewModelBase
    {
        public int WFStateTaskID { get; set; }
        public int TaskID { get; set; }
        public int WFStateID { get; set; }
        public string TaskDescription { get; set; }
        public bool IsActive { get; set; }
        public string AddedBy{ get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
    public class ApplicableTaskMapModel
    {
        public string TaskList { get; set; }
        public int TaskID { get; set; }
    }
}
