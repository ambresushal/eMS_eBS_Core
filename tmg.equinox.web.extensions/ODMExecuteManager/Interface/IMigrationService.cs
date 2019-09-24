using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.web.ODMExecuteManager.Model;

namespace tmg.equinox.web.ODMExecuteManager.Interface
{
    public interface IMigrationService
    {
        List<MigrationBatchs> GetMigrationBatchs(int? batchID);
        List<AccessFile> GetFilesToMigrate(int batchID);
        bool IsSectionRulesExecute(int batchID, string viewType, string sectionName);
        List<MigrationPlanItem> GetMigrationListForFile(int batchId, int fileId, string viewType);
        MigrationMapper GetMigrationMapper(int batchId, int formDesignVersionId, string viewType);
        JObject GetFormInstanceJSON(int formInstanceId);
        void UpdateDocumentJSON(int formInstanceId, string documentJSON);
        void UpdateMigrationBatchsStatus(MigrationBatchs migrationBatch, MigrationBatchStatus status);
        List<ManualDataUpdates> GetManualDataUpdates(string qid, string viewType);
    }
}
