using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;

namespace tmg.equinox.queueprocess.PBPImportEmailNotificationQueue
{
    public class PBPImportEmailNotificationQueueService : IPBPImportEmailNotificationQueueService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;

        public PBPImportEmailNotificationQueueService(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;
        }

        public void CreateJob(PBPImportEmailNotificationQueueInfo notificationQueueInfo)
        {
            _backgroundJobManager.Recurring<PBPImportEmailNotificationQueueBackgroundJob, PBPImportEmailNotificationQueueInfo>(notificationQueueInfo, notificationQueueInfo.Name, () => Cron.MinuteInterval(120), TimeZoneInfo.Local);

        }

    }
}
