using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.backgroundjob
{
    public abstract class BackgroundWorkerBase : RunnableBase, IBackgroundWorker
    {
        public override void Start()
        {
            base.Start();

        }

        public override void Stop()
        {
            base.Stop();

        }

        public override void WaitToStop()
        {
            base.WaitToStop();

        }
    }
}
