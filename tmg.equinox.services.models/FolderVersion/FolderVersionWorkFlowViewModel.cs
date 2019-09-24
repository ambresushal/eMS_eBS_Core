using System;
using System.Collections.Generic;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersion
{
    public class FolderVersionWorkFlowViewModel : ViewModelBase
    {
        public int FolderVersionID { get; set; }
        public int FolderVersionWorkFlowStateID { get; set; }
        public int WorkflowStateID { get; set; }
        public int TenantID { get; set; }
        public int ApprovalStatusID { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UserName { get; set; }
        public int Sequence { get; set; }
        public int? WFStateGroupID { get; set; }
    }
}
