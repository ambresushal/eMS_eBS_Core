using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class TaskComments : Entity
    {
        public int ID { get; set; }
        public int TaskID { get; set; }
        public string Comments { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public virtual DPFPlanTaskUserMapping PlanTaskUserMappings { get; set; }
        public string Attachment { get; set; }
        public int FolderVersionID { get; set; }
        public virtual FolderVersion FolderVersionMap { get; set; }
        public string filename { get; set; }
        public string PlanTaskUserMappingState { get; set; }
        //public virtual ICollection<DPFPlanTaskUserMapping> DPFPlanTaskUserMappings { get; set; }
    }
}
