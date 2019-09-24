using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;
using Hangfire;

namespace tmg.equinox.queueprocess.EmailNotification
{
    public class EmailEnqueueService : IEmailEnqueueService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;

        public EmailEnqueueService(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;
        }
    
        public void CreateJob(EmailQueueInfo emailQueueInfo)
        {
            _backgroundJobManager.Recurring<EmailBackgroundJob, EmailQueueInfo>(emailQueueInfo,emailQueueInfo.Name,()=>Cron.Daily(), TimeZoneInfo.Local);
        }
    }
}
