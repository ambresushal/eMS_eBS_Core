using Hangfire;
using System;
using tmg.equinox.backgroundjob;
using tmg.equinox.pbpexport;

namespace tmg.equinox.queueprocess.PBPExport
{
    public class PBPExportEnqueueService : IPBPExportEnqueueService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;

        public PBPExportEnqueueService(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;
        }

        [Queue("export")]
        public void CreateJob(PBPExportQueueInfo pBPExportQueueInfo)
        {
            _backgroundJobManager.EnqueueAsync<PBPExportBackgroundJob, PBPExportQueueInfo>(pBPExportQueueInfo);
        }
    }
}
