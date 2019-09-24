using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;
using tmg.equinox.hangfire;

namespace tmg.equinox.queueprocess.EmailNotificationQueue
{
    public class EmailNotificationQueue : IEmailNotificationQueue
    {
        IBackgroundJobManager _hangFireJobManager;

        public EmailNotificationQueue(IBackgroundJobManager hangFireJobManager)
        {
            _hangFireJobManager = hangFireJobManager;
        }

        public void ExecuteEmailNotificationQueue(EmailNotificationQueueInfo emailNotificationQueueInfo)
        {
            IBackgroundJobManager background = new HangfireService(_hangFireJobManager).Get();
            IEmailNotificationQueueService _emailNotificationQueueService = new EmailNotificationQueueService(background);
            _emailNotificationQueueService.CreateJob(emailNotificationQueueInfo);
        }
    }
}
