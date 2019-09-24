using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.hangfire;
using tmg.equinox.integration.qhplite;

namespace tmg.equinox.queueprocess.QHPReportQueue
{
    
    public class QHPReportQueueBackgroundJob : BaseBackgroundJob<QHPReportQueueInfo>
    {
        IQHPReportQueueManager _qhpReportQueueManager;
        public QHPReportQueueBackgroundJob(IQHPReportQueueManager qhpReportQueueManager)
        {
            _qhpReportQueueManager = qhpReportQueueManager;
        }

        public override void Execute(QHPReportQueueInfo info)
        {
             _qhpReportQueueManager.Execute();
        }
    }
}
