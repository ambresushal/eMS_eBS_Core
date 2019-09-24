using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class WorkFlowStateUserMap : Entity
    {
        public int WFStateUserMapID { get; set; }
        public int UserID { get; set; }       
        public int WFStateID { get; set; }
        public int FolderID { get; set; }
        public int FolderVersionID { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public int TenantID { get; set; }
        public virtual FolderVersion FolderVersion { get; set; }
        public virtual Folder Folder { get; set; }
        public virtual WorkFlowVersionState WorkFlowVersionState { get; set; }
        public virtual User User { get; set; }
        public int ApprovedWFStateID { get; set; }
        public virtual WorkFlowVersionState WorkFlowState1 { get; set; }
        public int ApplicableTeamID { get; set; }
       
    }
}
