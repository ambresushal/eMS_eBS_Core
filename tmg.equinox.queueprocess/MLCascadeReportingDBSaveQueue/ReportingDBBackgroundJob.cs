using Hangfire;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.hangfire;
using tmg.equinox.savetoreportingdbmlcascade;

namespace tmg.equinox.queueprocess.MLCascadeReportingDBSaveQueue
{
    [Queue("high"), AutomaticRetry(Attempts = 0)]
    public class ReportingDBBackgroundJob : BaseBackgroundJob<ReportingDBQueueInfo>
    {
        IReportDBSaveManager<BaseJobInfo> _mlCascadeManager;
        IFolderVersionServices _mlService;
        public ReportingDBBackgroundJob(IReportDBSaveManager<BaseJobInfo> manager, IFolderVersionServices mlService)
        {
            _mlCascadeManager = manager;
            _mlService = mlService;
        }

        [Queue("high"), AutomaticRetry(Attempts = 0)]
        public override void Execute(ReportingDBQueueInfo queueInfo)
        {
            ReportingDBQueueManager queueManager = new ReportingDBQueueManager(_mlCascadeManager);
            queueManager.Execute(queueInfo);
        }
    }
}
