using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.DashBoard
{
    public class ViewModelForProofingTasks : ViewModelBase
    {

        public int MappingRowID { get; set; }
        public string Account { get; set; }
        public int TenantID { get; set; }
        public int FolderId { get; set; }
        public int FolderVersionId { get; set; }
        public int FormInstanceId { get; set; }
        public int TaskId { get; set; }
        public string Folder { get; set; }
        public string FolderVersion { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Workflow { get; set; }
        public string Plan { get; set; }
        public string Task { get; set; }
        public string Assignment { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? Completed { get; set; }
        public bool MarkInterested { get; set; }
        public string LastPageNoForGrid { get; set; }
        public string View { get; set; }
        public string Section { get; set; }
        public int? Order { get; set; }
        public string Priority { get; set; }
        public string Attachments { get; set; }
        public int ID { get; set; }
        public string PlanTaskUserMappingDetails { get; set; }
        public int? EstimatedTime { get; set; }
        public int? ActualTime { get; set; }
        public int? FolderVersionWFStateID { get; set; }
        public int? TaskWFStateID { get; set; }
    }
}
