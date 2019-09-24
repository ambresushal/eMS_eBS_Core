using Hangfire;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.backgroundjob;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.core.masterlistcascade;
using tmg.equinox.hangfire;
using tmg.equinox.hangfire.Jobfilters;
using tmg.equinox.reporting;
using tmg.equinox.reporting.Base;
using tmg.equinox.reporting.Base.Interface;

namespace tmg.equinox.queueprocess.masterlistcascade
{
    [Queue("mlcascade"), AutomaticRetry(Attempts = 0)]
    public class MasterListCascadeBackgroundJob : BaseBackgroundJob<MasterListCascadeQueueInfo>
    {
        IMasterListCascadeManager<BaseJobInfo> _mlCascadeManager;
        IMasterListCascadeService _mlService;
        public MasterListCascadeBackgroundJob(IMasterListCascadeManager<BaseJobInfo> manager,IMasterListCascadeService mlService)
        {
            _mlCascadeManager = manager;
            _mlService = mlService;
        }

        [Queue("mlcascade"), AutomaticRetry(Attempts = 0)]
        public override void Execute(MasterListCascadeQueueInfo queueInfo)
        {
            MasterListCascadeQueueManager queueManager = new MasterListCascadeQueueManager(_mlCascadeManager);
            queueManager.Execute(queueInfo,queueInfo.FormDesignID,queueInfo.FormDesignVersionID, queueInfo.FolderVersionID,queueInfo.MasterListCascadeBatchID,queueInfo.UserID,queueInfo.UserName);
        }
    }
}
