using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.hangfire;
using Hangfire;


namespace tmg.equinox.reportingprocess.NotifyTaskAssignment
{
    [Queue("high"), AutomaticRetry(Attempts = 0)]
    public class NotifyTaskAssignmentJob : BaseBackgroundJob<NotifyTaskAssignmentInfo>
    {
        IPlanTaskUserMappingService _planTaskUserMappingService;
        public NotifyTaskAssignmentJob(IPlanTaskUserMappingService planTaskUserMappingService)
        {
            _planTaskUserMappingService = planTaskUserMappingService;
        }

        [Queue("high"), AutomaticRetry(Attempts = 0)]
        public override void Execute(NotifyTaskAssignmentInfo info)
        {
            _planTaskUserMappingService.ExecuteNotifyTaskDueDateOverEmail();
        }
    }
}
