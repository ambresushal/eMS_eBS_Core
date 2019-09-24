using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class WorkflowTaskMap: Entity
    {
        public int WFStateTaskID { get; set; }
        public int TaskID { get; set; }
        public int TenantID { get; set; }
        public int WFStateID { get; set; }
       // public string TaskDescription { get; set; }
        public bool IsActive { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public virtual TaskList Tasklist { get; set; }
        public virtual WorkFlowStateMaster WorkFlowStateMaster { get; set; }
        public virtual ICollection<DPFPlanTaskUserMapping> DPFPlanTaskUserMappings { get; set; }
    }
}
