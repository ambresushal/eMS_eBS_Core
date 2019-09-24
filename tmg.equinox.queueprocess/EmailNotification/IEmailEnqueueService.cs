using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.queueprocess.EmailNotification
{
    public interface IEmailEnqueueService
    {
        void CreateJob(EmailQueueInfo reportQueueInfo);
    }
}
