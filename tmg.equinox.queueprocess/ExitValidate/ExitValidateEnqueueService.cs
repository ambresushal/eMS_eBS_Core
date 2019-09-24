using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;

namespace tmg.equinox.queueprocess.exitvalidate
{
    public class ExitValidateEnqueueService : IExitValidateEnqueueService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;

        public ExitValidateEnqueueService(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;

        }

        [Queue("exitvalidate")]
        public void CreateJob(ExitValidateQueueInfo queueInfo)
        {
            _backgroundJobManager.EnqueueAsync<ExitValidateBackgroundJob, ExitValidateQueueInfo>(queueInfo);
        }

        [Queue("exitvalidatelow")]
        public void CreateJobWithLowpriority(ExitValidateQueueInfo queueInfo)
        {
            _backgroundJobManager.EnqueueAsync<ExitValidateBackgroundJobLowPriority, ExitValidateQueueInfo>(queueInfo);
        }
    }
}
