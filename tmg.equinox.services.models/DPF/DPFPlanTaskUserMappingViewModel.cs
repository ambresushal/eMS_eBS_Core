using System;
using System.Collections.Generic;

namespace tmg.equinox.applicationservices.viewmodels.DPF
{
    public class DPFPlanTaskUserMappingViewModel
    {
        public int ID { get; set; }
        //public int FormInstanceId { get; set; }
        public int WFStateID { get; set; }
        public int TaskID { get; set; }
        public System.DateTime AssignedDate { get; set; }
        public string AssignedUserName { get; set; }
        public string ManagerUserName { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime DueDate { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public DateTime? CompletedDate { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string TaskDescription { get; set; }
        public string Plan { get; set; }
        public string FolderName { get; set; }
        public string EffectiveDateString { get; set; }
        public System.DateTime EffectiveDate { get; set; }
        public string WorkflowState { get; set; }
        public string FolderVersionNumber { get; set; }
        public int FolderVersionID { get; set; }
        public int FolderID { get; set; }
        public List<int> FormInstanceIdList { get; set; }
        public List<string> FormInstanceNameList { get; set; }
        public string ViewID { get; set; }
        public string SectionID { get; set; }
        public string ViewLabels { get; set; }
        public string SectionLabels { get; set; }
        public int? Order { get; set; }
        public string Attachment { get; set; }
        public virtual ICollection<TaskCommentsViewModel> TaskCommentsMappings { get; set; }
        public string PlanTaskUserMappingDetails { get; set; }
        public string TaskComments { get; set; }
        public int? EstimatedTime { get; set; }
        public int? ActualTime { get; set; }
        public string filename { get; set; }
        public string AccountName { get; set; }

        public int Duration { get; set; }
 
    }

    public class TaskCommentsViewModel
    {
        public int ID { get; set; }
        public int TaskID { get; set; }
        public string Comments { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        //public virtual DPFPlanTaskUserMapping PlanTaskUserMappings { get; set; }
        public string Attachment { get; set; }
        public int FolderVersionID { get; set; }
        //public virtual FolderVersion FolderVersionMap { get; set; }
        public string filename { get; set; }
        public string PlanTaskUserMappingState { get; set; }
        //public virtual ICollection<DPFPlanTaskUserMapping> DPFPlanTaskUserMappings { get; set; }
    }

    public class PlanTaskUserMappingDetails
    {
        // public List<DesignDetails> DesignDetailsList { get; set; }
        public string SectionId { get; set; }
        public string FormDesignVersionId { get; set; }
        public string FormInstanceId { get; set; }
        public string SectionLabel { get; set; }
        public string FormDesignVersionLabel { get; set; }
        public string FormInstanceLabel { get; set; }
        public string TaskTraverseDetails { get; set; }
    }
    public class DesignDetails
    {
        public int SectionId { get; set; }
        public int FormDesignVersionId { get; set; }
        public int FormInstanceId { get; set; }
    }
}
