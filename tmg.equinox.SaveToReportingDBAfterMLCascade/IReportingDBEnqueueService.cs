namespace tmg.equinox.savetoreportingdbmlcascade
{
    public interface IReportingDBEnqueueService
    {
        void CreateJob(ReportingDBQueueInfo queueInfo);
    }
}
