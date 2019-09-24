using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;
using tmg.equinox.hangfire;

namespace tmg.equinox.reportingprocess.NotifyTaskAssignment
{
    public class NotifyTaskAssignment : INotifyTaskAssignment
    {
        IBackgroundJobManager _hangFireJobManager;

        public NotifyTaskAssignment(IBackgroundJobManager hangFireJobManager)
        {
            _hangFireJobManager = hangFireJobManager;
        }

        public void Enqueue(NotifyTaskAssignmentInfo notifyTaskAssignmentInfo)
        {
            IBackgroundJobManager background = new HangfireService(_hangFireJobManager).Get();
            INotifyTaskAssignmentService _notifyTaskAssignmentService = new NotifyTaskAssignmentService(background);
            _notifyTaskAssignmentService.CreateJob(notifyTaskAssignmentInfo);
        }
    }
}
