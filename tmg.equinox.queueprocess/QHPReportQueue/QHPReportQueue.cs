using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;
using tmg.equinox.hangfire;

namespace tmg.equinox.queueprocess.QHPReportQueue
{ 
    public class QHPReportQueue : IQHPReportQueue
    {
        IBackgroundJobManager _hangFireJobManager;

        public QHPReportQueue(IBackgroundJobManager hangFireJobManager)
        {
            _hangFireJobManager = hangFireJobManager;
        }

        public void ExecuteQHPReportQueue(QHPReportQueueInfo qhpReportQueueInfo)
        {
            IBackgroundJobManager background = new HangfireService(_hangFireJobManager).Get();
            IQHPReportQueueService _qhpReportQueueService = new QHPReportQueueService(background);
            _qhpReportQueueService.CreateJob(qhpReportQueueInfo);
        }
    }
}
