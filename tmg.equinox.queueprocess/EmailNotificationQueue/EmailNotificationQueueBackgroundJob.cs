using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.hangfire;

namespace tmg.equinox.queueprocess.EmailNotificationQueue
{
    public class EmailNotificationQueueBackgroundJob : BaseBackgroundJob<EmailNotificationQueueInfo>
    {
        IEmailNotificationService _emailNotificationService;
        public EmailNotificationQueueBackgroundJob(IEmailNotificationService emailNotificationService)
        {
            _emailNotificationService = emailNotificationService;
        }

        public override void Execute(EmailNotificationQueueInfo info)
        {
            _emailNotificationService.Execute();
        }
    }
}
