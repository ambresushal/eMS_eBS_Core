using Hangfire;
using tmg.equinox.backgroundjob;
using tmg.equinox.hangfire.Jobfilters;
using tmg.equinox.pbpimport;

namespace tmg.equinox.pbp.Import.processing
{
    [Queue("high"), HangfireStateFilter()]
  public  class PBPImportBackgroundJob : BackgroundJob<PBPImportQueueInfo>
    {
        public PBPImportBackgroundJob() { }

        [Queue("high")]
        public override void Execute(PBPImportQueueInfo rqinfo)
        {
            //Business Logic goes here
            //Call Report executer class in tmg.equinox.reporting
            PBPImportQueueManager _pBPImportQueueManager = new PBPImportQueueManager();
            _pBPImportQueueManager.Execute(new QueueItem { QueueId = rqinfo.QueueId, JobId = rqinfo.JobId });
        }
    }
}
