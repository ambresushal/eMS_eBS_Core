using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Owin;
using System;
using tmg.equinox.backgroundjob;
using tmg.equinox.hangfire.Configuration;
using tmg.equinox.hangfire.Jobfilters;
using tmg.equinox.hangfire.Logger;
using HangfireGlobalConfiguration = Hangfire.GlobalConfiguration;

namespace tmg.equinox.hangfire
{
    public class HangfireService
    {
        IBackgroundJobManager _hangFireJobManager;
        public HangfireService(IBackgroundJobManager hangFireJobManager)
        {

            GlobalJobFilters.Filters.Add(new HangfireStateFilter());
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            _hangFireJobManager = hangFireJobManager;
        }

        public HangfireService()
        {
            IBackgroundJobConfiguration backgroundJobConfiguration = new BackgroundJobConfiguration();
            backgroundJobConfiguration.IsJobExecutionEnabled = true;
            IConfiguration hangFireConfig = new HangfireConfiguration(new HangfireBackgroundJobServer(), new HangfireLogProviderFactory());
            //hangFireConfig.GlobalConfiguration.UseElmahLogProvider();
            GlobalJobFilters.Filters.Add(new HangfireStateFilter());
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            _hangFireJobManager = new HangfireBackgroundJobManager(backgroundJobConfiguration, hangFireConfig, null);

        }
        public void UseDashboard(IAppBuilder app, BaseJobAuthorizationFilter dashboardAuthorizationFilter)
        {
            app.UseHangfireDashboard("/jobs", new DashboardOptions()
            {
                Authorization = new[] { (IDashboardAuthorizationFilter)dashboardAuthorizationFilter }
            }); ;
        }
        public void Start()
        {
            _hangFireJobManager.Start();
        }
        public IBackgroundJobManager Get()
        {

            return _hangFireJobManager;
        }
        public void Stop()
        {
            _hangFireJobManager.Stop();
        }
    }

    /*Staring Hangfire server fom UI then 
       //Calling from APP
            /*IBackgroundWorkerManager workerManager = new BackgroundWorkerManager();
             var hangfireService = new HangfireService();
             backgroundJobManager = hangfireService.Get();
             workerManager.Add(backgroundJobManager);
             workerManager.Start();
             app.UseHangfireDashboard();
             app.UseHangfireServer();*/

}
