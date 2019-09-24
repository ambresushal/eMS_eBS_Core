using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.models;
using tmg.equinox.schema.Base.Model;

namespace tmg.equinox.schema.Base.Interface
{
    public interface ISchemaRepository
    {
        List<ReportingTableInfo> PopulateSchemaFromDatabase(int jsonDesignId, int jsonDesignVersionId, bool force = false);
        IList<ReportingTableInfo> GetTables(JsonDesign jsonDesign);
        void CreateOrUpdateTableInformation(ReportingTableInfo table, List<ReportingTableInfo> tables);
        bool CheckDesignExists(JsonDesign jsonDesign);

        string isSchemaExistForDesignID(JsonDesign jsonDesign);

        IList<ReportingTableInfo> GetTables();
        List<ReportingTableDetails> GetRawTableData();
        void Log(SchemaVersionActivityLog schemaVersionActivityLog);
        void Log(List<SchemaVersionActivityLog> schemaVersionActivityLog);
    }
}
