using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Settings
{
    public class WorkFlowSettingsViewModel
    {
        public bool IsManager { get; set; }
        public int? UserId { get; set; }
        public int ApplicableTeamID { get; set; }
        public string Username { get; set; }
    }

    public class ApplicableTeamMapModel {
        public string UsersList { get; set; }
        public bool IsManager { get; set; }
        public int UserID { get; set; }
    }
}
