using Hangfire;
using Hangfire.Logging;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;
using tmg.equinox.config;
using tmg.equinox.hangfire.Configuration;
using tmg.equinox.hangfire.Logger;
using HangfireBackgroundJob = Hangfire.BackgroundJob;

namespace tmg.equinox.hangfire
{
    public class HangfireBackgroundJobManager : BackgroundWorkerBase, IBackgroundJobManager
    {
        private readonly IBackgroundJobConfiguration _backgroundJobConfiguration;
        private readonly IConfiguration _hangfireConfiguration;
        private static ILog _logger =   LogProvider.For<HangfireBackgroundJobManager>();
        private IJobActivator _jobActivator;


        public HangfireBackgroundJobManager(
            IBackgroundJobConfiguration backgroundJobConfiguration,
            IConfiguration hangfireConfiguration)
        {
            _backgroundJobConfiguration = backgroundJobConfiguration;
            _hangfireConfiguration = hangfireConfiguration;

            Initialize();
        }

        public HangfireBackgroundJobManager(
            IBackgroundJobConfiguration backgroundJobConfiguration,
            IConfiguration hangfireConfiguration,
            IJobActivator jobActivator)
        {
            _backgroundJobConfiguration = backgroundJobConfiguration;
            _hangfireConfiguration = hangfireConfiguration;
            _jobActivator = jobActivator;

            Initialize();
        }

        private void Initialize()
        {
            // LogProvider.SetCurrentLogProvider(new CustomHangfireLogProvider());
             _hangfireConfiguration.LogProvider.CreateLogProvider();   
            if (_jobActivator !=null && _jobActivator.GetType() != typeof(JobActivatorNotImplemented))
            {
                var jobActivator = (JobActivator)_jobActivator;
                _hangfireConfiguration.JobGlobalConfiguration<IGlobalConfiguration>().UseActivator(jobActivator);
            }
            _hangfireConfiguration.JobGlobalConfiguration<IGlobalConfiguration>().UseSqlServerStorage(Config.TMGDB());
        }

        public override void Start()
        {
            base.Start();


            if (_hangfireConfiguration.Server.IsCreated() == false && _backgroundJobConfiguration.IsJobExecutionEnabled)
            {
                _hangfireConfiguration.Server.Create();
                
            }
 
            _logger.Info("Job Started");


         }

        public override void WaitToStop()
        {   
            if (_hangfireConfiguration.Server != null)
            {
                try
                {
                    _hangfireConfiguration.Server.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.ErrorException(ex.ToString(), ex);
                }
            }

            base.WaitToStop();
        }


        /// <summary>
        /// Calling methods in background Asyn
        /// Example:
        ///  public class ReportingEnqueueService : IReportEnqueueService
        ///
        ///    private readonly IBackgroundJobManager _backgroundJobManager;
        ///
        ///    public ReportingEnqueueService(IBackgroundJobManager backgroundJobManager)
        ///  {
        ///      _backgroundJobManager = backgroundJobManager;
        ///
        ///  }
        ///
        ///  [Queue("high")]
        ///  public void CreateJob(ReportQueueInfo reportQueueInfo)
        ///  {
        ///       //reportQueueInfo.AssemblyName = "tmg.equinox.applicationservices";
        ///      //reportQueueInfo.ClassName = "ReportCustomQueue";
        /// 
        ///      _backgroundJobManager.EnqueueAsync<ReportBackgroundJob, ReportQueueInfo>(reportQueueInfo);
        /// 
        ///   }
        ///  }
        ///    public class ReportBackgroundJob : BaseBackgroundJob<ReportQueueInfo>
        ///    {
        ///         public override void Execute(ReportQueueInfo rqinfo)
        ///         {
        ///            ReportQueueManager reportQueueManager = new ReportQueueManager();
        ///                reportQueueManager.Execute(new ReportQueueInfo { QueueId = rqinfo.QueueId, JobId = rqinfo.JobId, Status = rqinfo.Status, FeatureId = rqinfo.FeatureId.ToString(), Name = rqinfo.Name, FileName = rqinfo.FileName, FilePath = rqinfo.FilePath, AssemblyName = "tmg.equinox.applicationservices", ClassName = "ReportCustomQueue" });
        ///         }
        ///    }
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <typeparam name="TArgs"></typeparam>
        /// <param name="args"></param>
        /// <param name="delay">Calling methods with delay</param>
        /// <returns></returns>
        public Task EnqueueAsync<TJob, TArgs>(TArgs args,
            TimeSpan? delay = null) where TJob : IBackgroundJob<TArgs>
        {
            _logger.Info("Job Enqueued");
            if (!delay.HasValue)
                HangfireBackgroundJob.Enqueue<TJob>(job => job.Execute(args));
            else
                HangfireBackgroundJob.Schedule<TJob>(job => job.Execute(args), delay.Value);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Calling methods in background 
        /// Example:
        ///  public class ReportingEnqueueService : IReportEnqueueService
        ///
        ///    private readonly IBackgroundJobManager _backgroundJobManager;
        ///
        ///    public ReportingEnqueueService(IBackgroundJobManager backgroundJobManager)
        ///  {
        ///      _backgroundJobManager = backgroundJobManager;
        ///
        ///  }
        ///
        ///  [Queue("high")]
        ///  public void CreateJob(ReportQueueInfo reportQueueInfo)
        ///  {
        ///       //reportQueueInfo.AssemblyName = "tmg.equinox.applicationservices";
        ///      //reportQueueInfo.ClassName = "ReportCustomQueue";
        /// 
        ///      _backgroundJobManager.EnqueueAsync<ReportBackgroundJob, ReportQueueInfo>(reportQueueInfo);
        /// 
        ///   }
        ///  }
        ///    public class ReportBackgroundJob : BaseBackgroundJob<ReportQueueInfo>
        ///    {
        ///         public override void Execute(ReportQueueInfo rqinfo)
        ///         {
        ///            ReportQueueManager reportQueueManager = new ReportQueueManager();
        ///                reportQueueManager.Execute(new ReportQueueInfo { QueueId = rqinfo.QueueId, JobId = rqinfo.JobId, Status = rqinfo.Status, FeatureId = rqinfo.FeatureId.ToString(), Name = rqinfo.Name, FileName = rqinfo.FileName, FilePath = rqinfo.FilePath, AssemblyName = "tmg.equinox.applicationservices", ClassName = "ReportCustomQueue" });
        ///         }
        ///    }
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <typeparam name="TArgs"></typeparam>
        /// <param name="args"></param>
        /// <param name="delay">Calling methods with delay</param>
        /// <returns></returns>
        public void Enqueue<TJob, TArgs>(TArgs args, TimeSpan? delay = default(TimeSpan?)) where TJob : IBackgroundJob<TArgs>
        {
              if (!delay.HasValue)
                HangfireBackgroundJob.Enqueue<TJob>(job => job.Execute(args));
            else
                HangfireBackgroundJob.Schedule<TJob>(job => job.Execute(args), delay.Value);
        }



        /// <summary>
        /// Perform Recurercy task
        ///  Example:
        ///  public class ReportingEnqueueService : IReportEnqueueService
        ///
        ///    private readonly IBackgroundJobManager _backgroundJobManager;
        ///
        ///    public ReportingEnqueueService(IBackgroundJobManager backgroundJobManager)
        ///  {
        ///      _backgroundJobManager = backgroundJobManager;
        ///
        ///  }
        ///
        ///  [Queue("high")]
        ///  public void CreateJob(ReportQueueInfo reportQueueInfo)
        ///  {
        ///       //reportQueueInfo.AssemblyName = "tmg.equinox.applicationservices";
        ///      //reportQueueInfo.ClassName = "ReportCustomQueue";
        /// 
        ///      _backgroundJobManager.Recurring<ReportBackgroundJob, ReportQueueInfo>(reportQueueInfo,Cron.Daily);
        /// 
        ///   }
        ///  }
        ///    public class ReportBackgroundJob : BaseBackgroundJob<ReportQueueInfo>
        ///    {
        ///         public override void Execute(ReportQueueInfo rqinfo)
        ///         {
        ///            ReportQueueManager reportQueueManager = new ReportQueueManager();
        ///                reportQueueManager.Execute(new ReportQueueInfo { QueueId = rqinfo.QueueId, JobId = rqinfo.JobId, Status = rqinfo.Status, FeatureId = rqinfo.FeatureId.ToString(), Name = rqinfo.Name, FileName = rqinfo.FileName, FilePath = rqinfo.FilePath, AssemblyName = "tmg.equinox.applicationservices", ClassName = "ReportCustomQueue" });
        ///         }
        ///    }
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <typeparam name="TArgs"></typeparam>
        /// <param name="args"></param>
        /// <param name="recurringJobName">Provide Unique Job Name</param>
        /// <param name="cronExpression">Cron.Daily</param>
        /// <param name="timeZone"></param>
        /// <param name="queue"></param>
        public void Recurring<TJob, TArgs>(TArgs args, string recurringJobName, Func<string> cronExpression, TimeZoneInfo timeZone = null, string queue = "default")
            where TJob : IBackgroundJob<TArgs>
        {
            RecurringJob.AddOrUpdate<TJob>(recurringJobName,job => job.Execute(args), cronExpression, timeZone, queue);
        }
    }
}
