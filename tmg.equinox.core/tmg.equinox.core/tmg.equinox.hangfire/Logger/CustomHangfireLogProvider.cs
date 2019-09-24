using Hangfire.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.hangfire.Logger
{
    public class CustomHangfireLogProvider : ILogProvider
    {
        public ILog GetLogger(string name)
        {
            return new CustomHangfireLogger();
        }
    }
}
