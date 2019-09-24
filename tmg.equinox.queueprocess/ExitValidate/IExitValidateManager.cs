using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.reportingprocess.ExitValidate
{
    public interface IExitValidateManager<T>
    {
        bool Execute(T queueItem, int queueId);
    }
}
