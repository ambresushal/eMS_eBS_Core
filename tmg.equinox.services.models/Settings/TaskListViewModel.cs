using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Settings
{
   public class TaskListViewModel:ViewModelBase
    {
        public int TaskID { get; set; }
        public string TaskDescription { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsStandardTask { get; set; }
        public int WFStateID { get; set; }
    }
}
