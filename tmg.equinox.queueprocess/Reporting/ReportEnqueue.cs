using Hangfire.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.hangfire;
//using tmg.equinox.infrastructure.logging;
using tmg.equinox.reporting;

namespace tmg.equinox.queueprocess.reporting
{
    public class ReportEnqueue : IReportEnqueue
    {
        IBackgroundJobManager _hangFireJobManager;


        private static readonly core.logging.Logging.ILog _logger = core.logging.Logging.LogProvider.For<ReportEnqueue>();

        public ReportEnqueue(IBackgroundJobManager hangFireJobManager) {
            _hangFireJobManager = hangFireJobManager;
        }
   

        public void Enqueue(ReportQueueInfo reportQueueInfo)
        {
            _logger.InfoFormat("this is queue : " + reportQueueInfo.QueueId,reportQueueInfo);
            IBackgroundJobManager background = new HangfireService(_hangFireJobManager).Get();
            IReportEnqueueService _reportEnqueueService = new ReportingEnqueueService(background);
            _reportEnqueueService.CreateJob(reportQueueInfo);
        }    

    }
}