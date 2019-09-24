using Hangfire;
using System;
using tmg.equinox.backgroundjob;

namespace tmg.equinox.queueprocess.QHPReportQueue
{
    public class QHPReportQueueService : IQHPReportQueueService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;

        public QHPReportQueueService(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;
        }

        public void CreateJob(QHPReportQueueInfo qhpReportQueueInfo)
        {
            _backgroundJobManager.Recurring<QHPReportQueueBackgroundJob, QHPReportQueueInfo>(qhpReportQueueInfo, qhpReportQueueInfo.Name, () => Cron.MinuteInterval(1), TimeZoneInfo.Local);
            //_backgroundJobManager.Enqueue<QHPReportQueueBackgroundJob, QHPReportQueueInfo>(qhpReportQueueInfo);
        }

    }
}
