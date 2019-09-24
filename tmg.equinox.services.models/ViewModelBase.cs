using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace tmg.equinox.applicationservices.viewmodels
{
    public class ViewModelBase
    {
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<RoleClaimModel> RoleClaim { get; set; }
    }

    /*TODO:This is not the best solution. We have implemented this to solve the circular reference problem. Need to find some more generic solution.*/
    [Serializable]
    public class RoleClaimModel
    {
        public string Resource { get; set; }

        public string Action { get; set; }

        public string ResourceType { get; set; }        
    }
}
