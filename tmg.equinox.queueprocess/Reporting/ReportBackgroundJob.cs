using Hangfire;
using tmg.equinox.backgroundjob;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.hangfire;
using tmg.equinox.hangfire.Jobfilters;
using tmg.equinox.reporting;
using tmg.equinox.reporting.Base;
using tmg.equinox.reporting.Base.Interface;

namespace tmg.equinox.queueprocess.reporting
{

    [Queue("report"), AutomaticRetry(Attempts = 0)]
    public class ReportBackgroundJob : BaseBackgroundJob<ReportQueueInfo>
    {
        IReportManager<BaseJobInfo> _reportManager;
        public ReportBackgroundJob(IReportManager<BaseJobInfo> reportManager)
        {
            _reportManager = reportManager;
        }

        [Queue("report"), AutomaticRetry(Attempts = 0)]
        public override void Execute(ReportQueueInfo rqinfo)
        {
            //Business Logic goes here
            //Call Report executer class in tmg.equinox.reporting

            ReportQueueManager reportQueueManager = new ReportQueueManager(_reportManager);
            reportQueueManager.Execute(new ReportQueueInfo
                                {
                                    QueueId = rqinfo.QueueId, JobId = rqinfo.JobId,
                                    Status = rqinfo.Status, FeatureId = rqinfo.FeatureId.ToString(),
                                    Name = rqinfo.Name, FileName = rqinfo.FileName,
                                    FilePath = rqinfo.FilePath, AssemblyName = "tmg.equinox.applicationservices",
                                    ClassName = "ReportCustomQueue"
                                });
        }
    }
}
