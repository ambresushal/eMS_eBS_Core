using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class DPFPlanTaskUserMapping : Entity
    {
        public int ID { get; set; }
        //public int FormInstanceId { get; set; }
        public int WFStateID { get; set; }
        public int TaskID { get; set; }
        public DateTime AssignedDate { get; set; }
        public string AssignedUserName { get; set; }
        public string ManagerUserName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool MarkInterested { get; set; }
        //public virtual FormInstance FormInstances { get; set; }
        public virtual WorkFlowStateMaster WorkFlowStateMasters { get; set; }
        public virtual WorkflowTaskMap WorkflowTaskMaps { get; set; }
        //public virtual FormDesignVersion FormDesignVersionMaps { get; set; }
        // public virtual UIElement UIElementMaps { get; set; }
        //public virtual TaskComments TaskCommentsMaps { get; set; }
        public bool LateStatusDone { get; set; }
        //public string ViewID { get; set; }
        //public string SectionID { get; set; }
        public int? Order { get; set; }
        public int? Duration { get; set; }
        //public string Attachment { get; set; }
        public virtual ICollection<TaskComments> TaskCommentsMappings { get; set; }
        public string PlanTaskUserMappingDetails { get; set; }
        public int FolderVersionID { get; set; }
        public virtual FolderVersion FolderVersionMap { get; set; }
        public int? EstimatedTime { get; set; }
        public int? ActualTime { get; set; }
    }
}