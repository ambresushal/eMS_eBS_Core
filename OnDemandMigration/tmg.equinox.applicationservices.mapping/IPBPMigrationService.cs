using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.pbp.dataaccess;

namespace tmg.equinox.applicationservices.pbp
{
    public interface IPBPMigrationService
    {
        List<PBPFile> GetFilesToMigrate();

        List<MigrationPlanItem> GetMigrationListForFile(int fileId, string viewType);
        MigrationMapper GetMigrationMapper(int formDesignVersionId, string viewType);

        JObject GetDefaultJSONForFormDesignVersion(int formDesignVersionId);

        int GetNewBatchId();

        void ExecuteSQL(string query);
    }
}
