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

namespace tmg.equinox.queueprocess.masterlistcascade
{
    public class MasterListCascadeEnqueue : IMasterListCascadeEnqueue
    {
        IBackgroundJobManager _hangFireJobManager;


        private static readonly core.logging.Logging.ILog _logger = core.logging.Logging.LogProvider.For<MasterListCascadeEnqueue>();

        public MasterListCascadeEnqueue(IBackgroundJobManager hangFireJobManager) {
            _hangFireJobManager = hangFireJobManager;
        }
   

        public void Enqueue(MasterListCascadeQueueInfo queueInfo)
        {
            _logger.InfoFormat("this is queue : " + queueInfo.QueueId,queueInfo);
            IBackgroundJobManager background = new HangfireService(_hangFireJobManager).Get();
            IMasterListCascadeEnqueueService _enqueueService = new MasterListCascadeEnqueueService(background);
            _enqueueService.CreateJob(queueInfo);
        }    

    }
}