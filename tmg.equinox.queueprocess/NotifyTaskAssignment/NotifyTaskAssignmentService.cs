using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;

namespace tmg.equinox.reportingprocess.NotifyTaskAssignment
{
    public class NotifyTaskAssignmentService : INotifyTaskAssignmentService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;

        public NotifyTaskAssignmentService(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;
        }

        public void CreateJob(NotifyTaskAssignmentInfo notifyTaskAssignmentInfo)
        {
            _backgroundJobManager.Recurring<NotifyTaskAssignmentJob, NotifyTaskAssignmentInfo>(notifyTaskAssignmentInfo, notifyTaskAssignmentInfo.Name, () => Cron.Daily(), TimeZoneInfo.Local);
        }
    }
}
