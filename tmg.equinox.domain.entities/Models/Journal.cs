using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class Journal: Entity
    {
        public int JournalID { get; set; }
        public int FormInstanceID { get; set; }
        public int FolderVersionID { get; set; }
        public string Description { get; set; }
        public string FieldName { get; set; }
        public string FieldPath { get; set; }
        public int ActionID { get; set; }
        public int AddedWFStateID { get; set; }
        public Nullable<int> ClosedWFStateID { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public virtual FormInstance FormInstance { get; set; }
        public virtual FolderVersion FolderVersion { get; set; }
        public virtual WorkFlowVersionState AddedWorkFlowState { get; set; }
        public virtual WorkFlowVersionState ClosedWorkFlowState { get; set; }
        public virtual JournalAction JournalAction { get; set; }
        public virtual ICollection<JournalResponse> JournalResponses { get; set; }
    }
}
