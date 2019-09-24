using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.backgroundjob
{
    public class BackgroundWorkerManager : RunnableBase, IBackgroundWorkerManager, IDisposable
    {
        private readonly List<IBackgroundWorker> _backgroundJobs;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundWorkerManager"/> class.
        /// </summary>
        public BackgroundWorkerManager()
        {
            //_iocResolver = iocResolver;
            _backgroundJobs = new List<IBackgroundWorker>();
        }

        public override void Start()
        {
            base.Start();

            _backgroundJobs.ForEach(job => job.Start());
        }

        public override void Stop()
        {
            _backgroundJobs.ForEach(job => job.Stop());

            base.Stop();
        }

        public override void WaitToStop()
        {
            _backgroundJobs.ForEach(job => job.WaitToStop());

            base.WaitToStop();
        }

        public void Add(IBackgroundWorker worker)
        {
            _backgroundJobs.Add(worker);

            if (IsRunning)
            {
                worker.Start();
            }
        }

        private bool _isDisposed;

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _backgroundJobs.Clear();
        }
    }
}
