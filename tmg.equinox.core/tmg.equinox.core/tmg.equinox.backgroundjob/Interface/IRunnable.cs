using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.backgroundjob
{

    public interface IRunnable
    {
        /// <summary>
        /// Starts the service.
        /// </summary>
        void Start();

        /// <summary>
        /// Sends stop command to the service.
        /// Service may return immediately and stop asynchronously.
        /// </summary>
        void Stop();

        /// <summary>
        /// Waits the service to stop.
        /// </summary>
        void WaitToStop();
    }
}
