using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.backgroundjob;
using tmg.equinox.dependencyresolution;
using tmg.equinox.hangfire;

namespace tmg.equinox.web
{
    public class HangfireConfig
    {
        private static HangfireService Service
        {
            get
            {
                {
                    IBackgroundJobManager backgroundJobMaanger = UnityConfig.Resolve<IBackgroundJobManager>();
                    return new HangfireService(backgroundJobMaanger);
                }
            }
        }

        public static void Config(Owin.IAppBuilder app)
        {
            Service.UseDashboard(app, new DashboardOwinJobAuthorizationFilter(config.Config.SuperUserRoleName()));
        }
    }
}