using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
   public partial class ApplicableTeamUserMap : Entity
    {
       public int ApplicableTeamUserMapID { get; set; }
       public int ApplicableTeamID { get; set; }
       public int? UserID { get; set; }
       public bool IsTeamManager { get; set; }
       public string AddedBy { get; set; }
       public string UpdatedBy { get; set; }
       public bool IsDeleted { get; set; }
       public System.DateTime AddedDate { get; set; }
       public System.DateTime UpdatedDate { get; set; }
       public virtual ApplicableTeam ApplicableTeam { get; set; }
       public virtual User User { get; set; }
    }

}
