using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.reporting;

namespace tmg.equinox.queueprocess.reporting
{
    public interface IReportEnqueueService
    {
        void CreateJob(ReportQueueInfo reportQueueInfo);
    }
}
