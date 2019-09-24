using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.config;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities;

namespace tmg.equinox.schema.sql
{
   public class CommonFunctions
    {
        private static readonly ILog _logger = LogProvider.For<CommonFunctions>();
        //public static string GetSchemaName(int designId)
        //{
        //    string schemaName = Config.GetReportingDatabaseSchemaName();
        //    if (designId == GlobalVariables.PBPDesignID)
        //        schemaName += "PBP";
        //    else if (designId == GlobalVariables.MedicalDesignID)
        //        schemaName += "SOT";
        //    else
        //        schemaName += "MAL";
        //    return schemaName;
        //}
    }
}
