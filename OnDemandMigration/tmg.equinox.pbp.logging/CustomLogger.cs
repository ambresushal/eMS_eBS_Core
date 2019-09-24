using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.pbp.logging
{
    public class CustomLogger : Logger
    {
        public void SetContext(int batchId, int formInstanceId, int formDesignVersionId, string qid )
        {
            MappedDiagnosticsContext.Set("batchid", batchId);
            MappedDiagnosticsContext.Set("forminstanceid", formInstanceId);
            MappedDiagnosticsContext.Set("formdesignversionid", formDesignVersionId);
            MappedDiagnosticsContext.Set("qid", qid);
        }
    }
}
