using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tmg.equinox.services.api.Models
{
    public class ApplicableTeamUserMap
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
        public virtual User User { get; set; }
    }
}