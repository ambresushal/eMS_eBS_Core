using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
  public partial  class PBPMatchConfig: Entity
    {
        public int PBPMatchConfig1Up { get; set; }
        public int PBPImportQueueID { get; set; }
        public string QID { get; set; }
        public string PlanName { get; set; }
        public string PlanNumber { get; set; }
        public string ebsPlanName { get; set; }
        public string ebsPlanNumber { get; set; }
        public int FormInstanceID { get; set; }
        public int DocId { get; set; }
        public int Year { get; set; }
        public int FolderId { get; set; }
        public int FolderVersionId { get; set; }
        public int UserAction { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public virtual Folder Folder { get; set; }
        public virtual FolderVersion FolderVersion { get; set; }
        public virtual FormInstance FormInstance { get; set; }
        public bool IsIncludeInEbs { get; set; }
        public bool IsTerminateVisible { get; set; }
        public bool IsProxyUsed { get; set; }
        public bool IsEGWPPlan { get; set; }
    }
}
