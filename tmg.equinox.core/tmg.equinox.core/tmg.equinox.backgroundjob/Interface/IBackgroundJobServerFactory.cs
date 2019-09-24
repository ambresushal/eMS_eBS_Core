using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.backgroundjob
{

    public interface IBackgroundJobServerFactory
    {
        bool IsCreated();
        void Create();
        void Dispose();
        void SendStop();
        void Start();
        void Stop();
    }
}
