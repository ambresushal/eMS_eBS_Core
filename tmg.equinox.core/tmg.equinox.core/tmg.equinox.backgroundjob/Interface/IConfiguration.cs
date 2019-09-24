using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.backgroundjob
{
    public interface IConfiguration
    {

        IBackgroundJobServerFactory Server { get; }

        T JobGlobalConfiguration<T>();

        ILogProviderFactory LogProvider { get; }

    }
}
