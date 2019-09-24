using System.ServiceProcess;
using tmg.equinox.applicationservices.interfaces;

namespace MDMSyncService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ReportingCenterService ()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
