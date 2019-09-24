using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.pbpexport;

namespace tmg.equinox.queueprocess.PBPExport
{
    public interface IPBPExportEnqueueService
    {
        void CreateJob(PBPExportQueueInfo pbpQueueInfo);
    }
}
