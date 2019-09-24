using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob.Base;

namespace tmg.equinox.backgroundjob
{
    public interface IBackgroundJobManager : IBackgroundWorker
    {
        Task EnqueueAsync<TJob, TArgs>(TArgs args, TimeSpan? delay = null)
           where TJob : IBackgroundJob<TArgs>;

        void Enqueue<TJob, TArgs>(TArgs args, TimeSpan? delay = null)
        where TJob : IBackgroundJob<TArgs>;

        void Recurring<TJob, TArgs>(TArgs args, string recurringJobName, Func<string> cronExpression, TimeZoneInfo timeZone = null, string queue = "default")
               where TJob : IBackgroundJob<TArgs>;        
    }
   
   
}
