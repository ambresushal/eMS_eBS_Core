using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.queueprocess.exitvalidate
{
    public interface IExitValidateEnqueueService
    {
        void CreateJob(ExitValidateQueueInfo queueInfo);
        void CreateJobWithLowpriority(ExitValidateQueueInfo queueInfo);
    }
}
