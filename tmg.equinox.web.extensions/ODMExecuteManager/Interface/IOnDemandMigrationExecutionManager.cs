using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.web.ODMExecuteManager.Interface
{
    public interface IOnDemandMigrationExecutionManager
    {
        void Execute(int? batchID, bool runManualUpdateOnly);
        void ExecuteAsync(int? batchID, bool runManualUpdateOnly);
    }
}
