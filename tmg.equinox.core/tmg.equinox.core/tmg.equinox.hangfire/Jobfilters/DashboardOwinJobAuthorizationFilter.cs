using Hangfire.Dashboard;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.hangfire
{
    public class DashboardOwinJobAuthorizationFilter: BaseJobAuthorizationFilter
    {

        public DashboardOwinJobAuthorizationFilter(string allowedRoleName):base(allowedRoleName)
        {

        }

        public override bool Authorize(DashboardContext context)
        {
            bool isAllowed = true;
            //var owinContext = new OwinContext(context.GetOwinEnvironment());
            
            //// Allow all authenticated users to see the Dashboard 
            //if (owinContext.Authentication.User.Identity.IsAuthenticated)
            //{
            //    isAllowed = owinContext.Authentication.User.IsInRole(_allowedRoleName);
            //}
            return isAllowed;
           
        }
    }
}
