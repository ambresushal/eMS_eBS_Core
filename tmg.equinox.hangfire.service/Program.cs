using Hangfire.Logging;
using System;
using System.ComponentModel;
using System.Timers;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.backgroundjob;
using tmg.equinox.config;
using tmg.equinox.dependencyresolution;
using tmg.equinox.hangfire.dependencies;
using Topshelf;


namespace tmg.equinox.hangfire.service
{
    class Program
    {
        private static ILog _logger = LogProvider.For<Program>();

        static void Main(string[] args)
        {
            try
            {
                var ServiceName = Config.GetHangfireServiceName();
                //Topshelf
                HostFactory.Run(x =>
                {
//need to remove following commented lines after discussion with Jamir.
                    //x.Service<ServiceManager>(s =>
                    //{
                    //    UnityConfig.RegisterComponents();

                    //// IBackgroundJobServerFactory _backgroundJobServerFactory = UnityConfig.Resolve<IBackgroundJobServerFactory>();
                    //// ILogProviderFactory _logProviderFactory = UnityConfig.Resolve<ILogProviderFactory>();

                    //// IBackgroundJobManager _backgroundJobManager = UnityConfig.Resolve<IBackgroundJobManager>();
                    ////s.ConstructUsing(name => new HangfireService(_backgroundJobManager));



                    //s.ConstructUsing(name => new ServiceManager());
                    //    s.WhenStarted(rs => rs.Start());
                    //    s.WhenStopped(rs => rs.Stop());


                    //});


                    x.Service<HangfireService>(s =>
                    {
                        UnityConfig.RegisterComponents();

                        IBackgroundJobServerFactory _backgroundJobServerFactory = UnityConfig.Resolve<IBackgroundJobServerFactory>();
                        ILogProviderFactory _logProviderFactory = UnityConfig.Resolve<ILogProviderFactory>();

                        IBackgroundJobManager _backgroundJobManager = UnityConfig.Resolve<IBackgroundJobManager>();
                        s.ConstructUsing(name => new HangfireService(_backgroundJobManager));

                        s.ConstructUsing(name => BackgroundProcessManagement.Run());
                        s.WhenStarted(rs => rs.Start());
                        s.WhenStopped(rs => rs.Stop());
                    });

                    x.RunAsLocalSystem();

                    //if (ServiceName==null)

                    //x.SetDescription(ServiceName);
                    //x.SetDisplayName(ServiceName);
                    //x.SetServiceName(ServiceName);
                    x.SetDescription("eBenefitSyncBackgoundService");
                    x.SetDisplayName("eBenefitSyncBackgoundService");
                    x.SetServiceName("eBenefitSyncBackgoundService");

                });
            }
            catch (Exception ex)
            {
                _logger.ErrorException("HangfireServiceProgram", ex);
            }
         //   Console.ReadKey();
        }
    }
 

    public class ServiceManager 
    {
        private readonly BackgroundProcessManagement _hangfireService;
        private readonly PBPExportServiceSingleThread  _exportService;
        private bool isExportListedInQueue = Config.ContainHangFireQueues("export");

        public ServiceManager()
        {
            this._hangfireService =  new BackgroundProcessManagement();
            if (isExportListedInQueue)
                this._exportService =  new PBPExportServiceSingleThread();
        }

        public void Start()
        {
            _hangfireService.Instance().Start();
            if (isExportListedInQueue)
                _exportService.Start();
        }

        public void Stop()
        {
            _hangfireService.Instance().Stop();
            if (isExportListedInQueue)
                _exportService.Stop();
        }
    }
    /// <summary>
    /// always run in single thread
    /// </summary>
    public class PBPExportServiceSingleThread 
    {
     //   System.Timers.Timer timer;
        private BackgroundWorker worker;
        private static ILog _logger = LogProvider.For<PBPExportServiceSingleThread>();
        public PBPExportServiceSingleThread()
        {
         //   timer = new System.Timers.Timer();
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
        }
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            DoWork();
        }

        private void DoWork()
        {

            try
            {
                IPBPExportServices pbpExportServices = UnityConfig.Resolve<IPBPExportServices>();
                pbpExportServices.GenerateMDBFile();
         //       Thread.Sleep(200000); // every 3 min 
            }
            catch(Exception ex)
            {
                _logger.ErrorException("DoWork", ex);
            }

        }

        public void Start()
        {
            /*  System.Timers.Timer timer = new System.Timers.Timer(2000);
              timer.Elapsed += timer_Elapsed;
              timer.Start();*/
            worker.RunWorkerAsync();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {           
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync();
            }
        }

        public void Stop()
        {
       //     timer.Stop();
            worker.Dispose();
        }
    }
}
