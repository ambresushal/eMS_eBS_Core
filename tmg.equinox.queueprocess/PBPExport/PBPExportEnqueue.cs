using Hangfire.Logging;
using tmg.equinox.backgroundjob;
using tmg.equinox.hangfire;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.queueprocess.PBPExport;
using tmg.equinox.pbpexport;

namespace tmg.equinox.queueprocess.PBPExport
{
    public class PBPExportEnqueue : IPBPExportEnqueue
    {
        IBackgroundJobManager _hangFireJobManager;

        private static readonly core.logging.Logging.ILog _logger = core.logging.Logging.LogProvider.For<PBPExportEnqueue>();

        public PBPExportEnqueue(IBackgroundJobManager hangFireJobManager)
        {
            _hangFireJobManager = hangFireJobManager;
        }

        public void Enqueue(PBPExportQueueInfo PBPExportQueueInfo)
        {
            _logger.InfoFormat("This is queue : " + PBPExportQueueInfo.QueueId, PBPExportQueueInfo);
            IBackgroundJobManager background = new HangfireService(_hangFireJobManager).Get();
            IPBPExportEnqueueService _PBPImportEnqueueService = new PBPExportEnqueueService(background);
            _PBPImportEnqueueService.CreateJob(PBPExportQueueInfo);
        }
    }
}
