using Hangfire;
using System;
using tmg.equinox.backgroundjob;
using tmg.equinox.pbpimport;

namespace tmg.equinox.queueprocess.PBPImport
{
    public class PBPImportEnqueueService : IPBPImportEnqueueService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;

        public PBPImportEnqueueService(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;
        }

        [Queue("import")]
        public void CreateJob(PBPImportQueueInfo pBPImportQueueInfo)
        {
            _backgroundJobManager.EnqueueAsync<PBPImportBackgroundJob, PBPImportQueueInfo>(pBPImportQueueInfo);
        }
    }
}
