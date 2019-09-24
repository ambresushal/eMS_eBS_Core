using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
   public class TaskList:Entity
    {
        public int TenantID { get; set; }
        public int TaskID { get; set; }
        public string TaskDescription { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public virtual Tenant Tenant { get; set; }
        public bool IsStandardTask { get; set; }
        public virtual ICollection<DPFPlanTaskUserMapping> DPFPlanTaskUserMappings { get; set; }
        public virtual ICollection<WorkflowTaskMap> applicableWorkflowTaskMap { get; set; }
    }
}
