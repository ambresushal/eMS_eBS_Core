using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class PBPImportDetails : Entity
    {
        public int PBPImportDetails1Up { get; set; }
        public int PBPImportQueueID { get; set; }
        public string QID { get; set; }
        public string PlanName { get; set; }
        public string PlanNumber { get; set; }
        public string ebsPlanName { get; set; }
        public string ebsPlanNumber { get; set; }
        public int FormInstanceID { get; set; }
        public int DocId { get; set; }
        public int FolderId { get; set; }
        public int FolderVersionId { get; set; }
        public int Status { get; set; }
        public int UserAction { get; set; }
        public int Year { get; set; }
        public string PBPDatabase { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public bool IsEGWPPlan { get; set; }
        public int PBPDatabase1Up { get; set; }
        public bool IsIncludeInEbs { get; set; }
    }
}
