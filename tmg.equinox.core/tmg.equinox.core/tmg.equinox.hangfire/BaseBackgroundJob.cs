using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob;
using tmg.equinox.backgroundjob.Base;
using Hangfire;
using tmg.equinox.hangfire.Jobfilters;


namespace tmg.equinox.hangfire
{
    public class BaseBackgroundJob<TArgs> : BackgroundJob<TArgs>
    {
        [Queue("high"), AutomaticRetry(Attempts = 0)]
        public override void Execute(TArgs args)
        {
            return;
        }


    }

    public class BaseBackgroundJobLow<TArgs> : BackgroundJob<TArgs>
    {
        [Queue("low"), AutomaticRetry(Attempts = 0)]
        public override void Execute(TArgs args)
        {
            return;
        }


    }
}
