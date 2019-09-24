using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using tmg.equinox.backgroundjob;
using HangfireGlobalConfiguration = Hangfire.GlobalConfiguration;
using Hangfire.Logging;

namespace tmg.equinox.hangfire.Configuration
{
    public class HangfireConfiguration : BackgroundJobConfiguration, IConfiguration
    {
        IBackgroundJobServerFactory _server;
        ILogProviderFactory _logProvider;
        public HangfireConfiguration(IBackgroundJobServerFactory server, ILogProviderFactory hangfireLogProviderFactory)
        {
            _server = server;
            _logProvider = hangfireLogProviderFactory;
        }
        public IBackgroundJobServerFactory Server { get { return _server; } }

       //public  GlobalConfiguration { get { return HangfireGlobalConfiguration.Configuration; } }
        
        public ILogProviderFactory LogProvider { get { return _logProvider; } }


        public T JobGlobalConfiguration<T>()
        {           
            return (T)HangfireGlobalConfiguration.Configuration;
           
        }


      
    }
}
