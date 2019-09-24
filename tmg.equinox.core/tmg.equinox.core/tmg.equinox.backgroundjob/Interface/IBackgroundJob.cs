using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.backgroundjob
{
    public interface IBackgroundJob<in TArgs>
    {
        /// <summary>
        /// Executes the job with the <see cref="args"/>.
        /// </summary>
        /// <param name="args">Job arguments.</param>
        void Execute(TArgs args);
    }
}
