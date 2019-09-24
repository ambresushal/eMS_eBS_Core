using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
    public class PlanMatchingConfigurationViewModel : ViewModelBase
    {
        public int PBPImportQueueID { get; set; }
        public int Id { get; set; }
        public string PBPPlanNumber { get; set; }
        public string PBPPlanName { get; set; }
        public string EBSPlanNumber { get; set; }
        public string EBSPlanName { get; set; }
        public int MatchStatus { get; set; }
        public int UserActionId { get; set; }
        public Int32 FolderVersionId { get; set; }
        public string FolderVersionName { get; set; }
        public bool IsIncludeInEbs { get; set; }
        public bool Ismatching { get; set; }
        public Int32 Index { get; set; }
        public int FolderId { get; set; }
        public string FormInstanceName { get; set; }
        public int FormInstanceId  { get; set; }
        public int DocumentId { get; set; }
        public string DataBaseFileName { get; set;}
    }
}
