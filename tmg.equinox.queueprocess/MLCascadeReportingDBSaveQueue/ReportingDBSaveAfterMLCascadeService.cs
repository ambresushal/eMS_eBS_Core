using Hangfire;
using tmg.equinox.backgroundjob;
using tmg.equinox.savetoreportingdbmlcascade;

namespace tmg.equinox.queueprocess.MLCascadeReportingDBSaveQueue
{
    public class ReportingDBSaveAfterMLCascadeService : IReportingDBEnqueueService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;

        public ReportingDBSaveAfterMLCascadeService(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;

        }

        [Queue("high")]
        public void CreateJob(ReportingDBQueueInfo queueInfo)
        {
            _backgroundJobManager.EnqueueAsync<ReportingDBBackgroundJob, ReportingDBQueueInfo>(queueInfo);
        }


    }
}
