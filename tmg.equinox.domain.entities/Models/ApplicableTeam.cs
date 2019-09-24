using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class ApplicableTeam : Entity
    {
        public ApplicableTeam()
        {
            this.WorkFlowStateFolderVersionMaps = new List<WorkFlowStateFolderVersionMap>();
            this.ApplicableTeamUserMaps = new List<ApplicableTeamUserMap>();
            
        }
        public int ApplicableTeamID { get; set; }
        public string ApplicableTeamName { get; set; }
        public int TenantID { get; set; }
        public virtual ICollection<WorkFlowStateFolderVersionMap> WorkFlowStateFolderVersionMaps { get; set; }
        public virtual ICollection<ApplicableTeamUserMap> ApplicableTeamUserMaps { get; set; }

    }
}
