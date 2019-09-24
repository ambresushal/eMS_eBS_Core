using tmg.equinox.backgroundjob.Base;
using tmg.equinox.savetoreportingdbmlcascade;

namespace tmg.equinox.queueprocess.MLCascadeReportingDBSaveQueue
{
    public class ReportingDBQueueManager
    {

        IReportDBSaveManager<BaseJobInfo> _rptManager;
        public ReportingDBQueueManager(IReportDBSaveManager<BaseJobInfo> rptManager)
        {
            _rptManager = rptManager;

        }

        public bool Execute(ReportingDBQueueInfo queueInfo)
        {
            return _rptManager.Execute(queueInfo);
        }
    }
}
