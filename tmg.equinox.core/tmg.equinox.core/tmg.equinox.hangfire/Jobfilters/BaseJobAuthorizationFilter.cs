using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Annotations;

namespace tmg.equinox.hangfire
{
    public class BaseJobAuthorizationFilter: IDashboardAuthorizationFilter
    {

        protected string _allowedRoleName;

        public BaseJobAuthorizationFilter(string allowedRoleName)
        {

            _allowedRoleName = allowedRoleName;
        }

        public virtual bool Authorize([NotNull] DashboardContext context)
        {
            return false;
        }
    }
}
