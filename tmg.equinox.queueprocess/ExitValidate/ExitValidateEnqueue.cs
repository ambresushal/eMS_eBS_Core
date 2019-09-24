using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.hangfire;

namespace tmg.equinox.queueprocess.exitvalidate
{
    public class ExitValidateEnqueue : IExitValidateEnqueue
    {
        IBackgroundJobManager _hangFireJobManager;
        
        private static readonly core.logging.Logging.ILog _logger = core.logging.Logging.LogProvider.For<ExitValidateEnqueue>();

        public ExitValidateEnqueue(IBackgroundJobManager hangFireJobManager)
        {
            _hangFireJobManager = hangFireJobManager;
        }
        
        public void Enqueue(ExitValidateQueueInfo queueInfo)
        {
            _logger.InfoFormat("this is queue : " + queueInfo.QueueId, queueInfo);
            IBackgroundJobManager background = new HangfireService(_hangFireJobManager).Get();
            IExitValidateEnqueueService _enqueueService = new ExitValidateEnqueueService(background);
            _enqueueService.CreateJob(queueInfo);
        }

    }
}
