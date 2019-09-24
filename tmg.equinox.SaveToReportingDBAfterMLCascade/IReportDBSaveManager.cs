using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.savetoreportingdbmlcascade
{
    public interface IReportDBSaveManager<T>
    {
        bool Execute(ReportingDBQueueInfo queueInfo);
    }
}
