using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.schema.Base.Model;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IReportingDataService
    {
        bool Run(JData jData);
        bool RunAsync(JData jData);
        bool MigrateExistingData(List<JData> lstjData);
    }
}
