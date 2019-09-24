using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.queueprocess.PBPImportEmailNotificationQueue
{
    public interface IPBPImportEmailNotificationQueueService
    {
        void CreateJob(PBPImportEmailNotificationQueueInfo notificationQueueInfo);
    }
}
