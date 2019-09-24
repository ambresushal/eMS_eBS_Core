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

            var queueNameList = ConfigurationManager.AppSettings["HangFireQueues"];
            string hangFireQueues = string.Empty;
            if (queueNameList != null)
            {
                hangFireQueues = queueNameList.ToString() ?? string.Empty;
            }
         
            string[] hangFireQueuesList = hangFireQueues.Split(',').ToArray();
            if (String.IsNullOrEmpty(hangFireQueues))
            {
                hangFireQueuesList = new[] { "export", "import", "mlcascade","report","high","low","defualt" };
            }
            //todo read from config
            var options = new BackgroundJobServerOptions
            {
                Queues = hangFireQueuesList,
                WorkerCount = Environment.ProcessorCount * 5
            };
            //Set WorkQueue Count for EV service
            if (hangFireQueuesList.Contains("exitvalidate"))
            {
                var eVHangFireQueuesCount = ConfigurationManager.AppSettings["HangFireQueuesCount"];
                string evWorkQueCount = string.Empty;
                if (eVHangFireQueuesCount != null)
                {
                    evWorkQueCount = eVHangFireQueuesCount.ToString() ?? string.Empty;
                    if (!String.IsNullOrEmpty(evWorkQueCount.ToString()))
                    {
                        options.WorkerCount = Convert.ToInt32(evWorkQueCount);
                    }
                    else
                        options.WorkerCount = 1;
                }
                else
                options.WorkerCount = 1;
            }
            
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
