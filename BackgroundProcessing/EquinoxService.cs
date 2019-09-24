using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.dependencyresolution;

namespace EquinoxProcessor
{
    public partial class EquinoxService : ServiceBase
    {
        ScheduleEquinoxService scheduleService;

        public static void Main(string[] args)
        {
            ServiceBase.Run(new EquinoxService());
        }

        public EquinoxService()
        {
            UnityConfig.RegisterComponents();
            string[] d = new string[2];
            this.OnStart(d);
            ServiceName = "EquinoxService_CBCDemo";
        }

        protected override void OnStart(string[] args)
        {
            scheduleService = new ScheduleEquinoxService();
            scheduleService.ScheduleIASService();
            scheduleService.ScheduleErrorLogService();
            scheduleService.ScheduleBatchService();
        }

        protected override void OnStop()
        {
            scheduleService.BatchSchedular.Dispose();
            scheduleService.IASSchedular.Dispose();
            scheduleService.ErrorLogSchedular.Dispose();
        }
    }
}
