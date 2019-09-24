using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.hangfire;

namespace tmg.equinox.queueprocess.PBPImportEmailNotificationQueue
{
    public class PBPImportEmailNotificationQueueBackgroundJob : BaseBackgroundJob<PBPImportEmailNotificationQueueInfo>
    {
        IPBPImportEmailNotificationService _emailNotificationService;
        public PBPImportEmailNotificationQueueBackgroundJob(IPBPImportEmailNotificationService emailNotificationService)
        {
            _emailNotificationService = emailNotificationService;
        }

        public override void Execute(PBPImportEmailNotificationQueueInfo info)
        {
            _emailNotificationService.Execute();
        }
    }
}
