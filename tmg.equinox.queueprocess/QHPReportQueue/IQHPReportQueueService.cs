using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.queueprocess.QHPReportQueue
{
    public interface IQHPReportQueueService
    {
        void CreateJob(QHPReportQueueInfo qhpReportQueueInfo);
    }
}
