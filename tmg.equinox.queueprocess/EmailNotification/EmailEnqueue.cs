using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;
using tmg.equinox.hangfire;

namespace tmg.equinox.queueprocess.EmailNotification
{
    public class EmailEnqueue : IEmailEnqueue
    {
        IBackgroundJobManager _hangFireJobManager;

        public EmailEnqueue(IBackgroundJobManager hangFireJobManager)
        {
            _hangFireJobManager = hangFireJobManager;
        }

        public void Enqueue(EmailQueueInfo emailQueueInfo)
        {
            IBackgroundJobManager background = new HangfireService(_hangFireJobManager).Get();
            IEmailEnqueueService _emailEnqueueService = new EmailEnqueueService(background);
            _emailEnqueueService.CreateJob(emailQueueInfo);
        }
    }
}
