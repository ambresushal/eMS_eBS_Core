using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.DashBoard
{
    public class UserRoleAssignmentViewModel : ViewModelBase
    {
        public int UserRoleID { get; set; }
        public string UserRoleName { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int ApplicableTeamID { get; set; }
        public string ApplicableTeamName { get; set; }
        public int ApplicableTeamUserMapID { get; set; }
    }
}
