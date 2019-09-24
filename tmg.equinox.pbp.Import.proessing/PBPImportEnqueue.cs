using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;
using tmg.equinox.hangfire;

namespace tmg.equinox.pbp.Import.processing
{
   public class PBPImportEnqueue
    {
        public PBPImportEnqueue() { }

        public void Enqueue(PBPImportQueueInfo reportQueueInfo)
        {
            IBackgroundJobManager background = new HangfireService().Get();
            var service = new PBPImportEnqueueService(background);
            service.CreateJob(reportQueueInfo);
        }
    }
}
