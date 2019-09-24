using tmg.equinox.domain.entities.Models;
using System.Collections.Generic;
using tmg.equinox.schema.Base.Model;
using Newtonsoft.Json.Linq;

namespace tmg.equinox.schema.Base.Interface
{
    public interface ISQLStatement
    {
        void UpdateReportingDatabase(JData jData);

    }
}
