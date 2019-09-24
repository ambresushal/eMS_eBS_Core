using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.pbpimport;

namespace tmg.equinox.queueprocess.PBPImport
{
    public interface IPBPImportEnqueue
    {
        void Enqueue(PBPImportQueueInfo pbpQueueInfo);
    }
}
