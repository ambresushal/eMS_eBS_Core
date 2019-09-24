using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.queueprocess.EmailNotificationQueue
{
    public interface IEmailNotificationQueueService
    {
        void CreateJob(EmailNotificationQueueInfo notificationQueueInfo);
    }
}
