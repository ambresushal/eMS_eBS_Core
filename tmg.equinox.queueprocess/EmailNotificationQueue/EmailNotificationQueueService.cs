using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;

namespace tmg.equinox.queueprocess.EmailNotificationQueue
{
    public class EmailNotificationQueueService : IEmailNotificationQueueService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;

        public EmailNotificationQueueService(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;
        }

        public void CreateJob(EmailNotificationQueueInfo notificationQueueInfo)
        {
            _backgroundJobManager.Recurring<EmailNotificationQueueBackgroundJob, EmailNotificationQueueInfo>(notificationQueueInfo, notificationQueueInfo.Name, () => Cron.MinuteInterval(30), TimeZoneInfo.Local);

        }

    }
}
