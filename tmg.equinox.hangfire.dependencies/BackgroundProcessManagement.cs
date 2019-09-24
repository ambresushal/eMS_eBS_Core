using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;
using tmg.equinox.dependencyresolution;
using tmg.equinox.hangfire.Configuration;
using tmg.equinox.hangfire.Jobfilters;

namespace tmg.equinox.hangfire.dependencies
{
    public class  BackgroundProcessManagement
    {
        public static HangfireService Run()
        {

            return HangfireServiceInstance();

        }

        public  HangfireService Instance()
        {

            return HangfireServiceInstance();

        }

        private static HangfireService HangfireServiceInstance()
        {
            IBackgroundJobConfiguration backgroundJobConfiguration = new BackgroundJobConfiguration();
            backgroundJobConfiguration.IsJobExecutionEnabled = true;
            IConfiguration hangFireConfig = new HangfireConfiguration(new HangfireBackgroundJobServer(), new HangfireLogProviderFactory());
            //hangFireConfig.GlobalConfiguration.UseElmahLogProvider();
            GlobalJobFilters.Filters.Add(new HangfireStateFilter());
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

            var _hangFireJobManager = new HangfireBackgroundJobManager(backgroundJobConfiguration, hangFireConfig, new UnityJobActivator(UnityConfig.container));

            var hangfireService = new HangfireService(_hangFireJobManager);
            return hangfireService;
        }
        public static IBackgroundJobManager Get()
        {
           
            return HangfireServiceInstance().Get();

        }
    }
}
