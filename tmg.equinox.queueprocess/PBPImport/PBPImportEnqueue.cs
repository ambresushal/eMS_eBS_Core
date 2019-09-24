using Hangfire.Logging;
using tmg.equinox.backgroundjob;
using tmg.equinox.hangfire;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.pbpimport;

namespace tmg.equinox.queueprocess.PBPImport
{
    public class PBPImportEnqueue : IPBPImportEnqueue
    {
        IBackgroundJobManager _hangFireJobManager;

        private static readonly core.logging.Logging.ILog _logger = core.logging.Logging.LogProvider.For<PBPImportEnqueue>();

        public PBPImportEnqueue(IBackgroundJobManager hangFireJobManager)
        {
            _hangFireJobManager = hangFireJobManager;
        }

        public void Enqueue(PBPImportQueueInfo PBPImportQueueInfo)
        {
            _logger.InfoFormat("This is queue : " + PBPImportQueueInfo.QueueId, PBPImportQueueInfo);
            IBackgroundJobManager background = new HangfireService(_hangFireJobManager).Get();
            IPBPImportEnqueueService _PBPImportEnqueueService = new PBPImportEnqueueService(background);
            _PBPImportEnqueueService.CreateJob(PBPImportQueueInfo);
        }
    }
}
