using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;
using tmg.equinox.hangfire;

namespace tmg.equinox.queueprocess.PBPImportEmailNotificationQueue
{
    public class PBPImportEmailNotificationQueue : IPBPImportEmailNotificationQueue
    {
        IBackgroundJobManager _hangFireJobManager;

        public PBPImportEmailNotificationQueue(IBackgroundJobManager hangFireJobManager)
        {
            _hangFireJobManager = hangFireJobManager;
        }

        public void ExecuteEmailNotificationQueue(PBPImportEmailNotificationQueueInfo emailNotificationQueueInfo)
        {
            IBackgroundJobManager background = new HangfireService(_hangFireJobManager).Get();
            IPBPImportEmailNotificationQueueService _emailNotificationQueueService = new PBPImportEmailNotificationQueueService(background);
            _emailNotificationQueueService.CreateJob(emailNotificationQueueInfo);
        }
    }
}
