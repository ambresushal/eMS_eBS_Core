using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HangfireGlobalConfiguration = Hangfire.GlobalConfiguration;
using Hangfire;
using Hangfire.Logging;
using tmg.equinox.hangfire.Logger;
using tmg.equinox.backgroundjob;
using System.Configuration;

namespace tmg.equinox.hangfire.Configuration
{
   
    public class HangfireBackgroundJobServer : IBackgroundJobServerFactory
    {
        private BackgroundJobServer _server;
        public HangfireBackgroundJobServer()
        {
         
        }
        public bool IsCreated()
        {
            return _server == null ?  false : true;
        }
        
        private BackgroundJobServerOptions BackGroundJobServerOtions()
        {
            //todo read from config
            string hangFireQueues = ConfigurationManager.AppSettings["HangFireQueues"].ToString() ?? string.Empty;
            string[] hangFireQueuesList = hangFireQueues.Split(',').ToArray();

            var options = new BackgroundJobServerOptions
            {
                //Queues = new[] { "high", "low", "default", "exitvalidate" }
                Queues = hangFireQueuesList
                ,
                WorkerCount = Environment.ProcessorCount * 5
            };
            return options;
        }

        public void Dispose()
        {
            _server.Dispose();
        }

        public void SendStop()
        {
            _server.SendStop();
        }

        public void Start()
        {
            _server.Start();
        }

        public void Stop()
        {
            _server.Stop();
        }

        public void Create()
        {
            _server = new BackgroundJobServer(BackGroundJobServerOtions());
        }
    }
}
