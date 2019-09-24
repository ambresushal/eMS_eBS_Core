using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.web.ODMExecuteManager.Interface;
using tmg.equinox.web.ODMExecuteManager.Model;

namespace tmg.equinox.web.ODMExecuteManager
{

    public class OnDemandMigrationExecutionManager : IOnDemandMigrationExecutionManager
    {
        private IUnitOfWorkAsync _unitOfWork;
        private static readonly ILog _logger = LogProvider.For<OnDemandMigrationExecutionManager>();

        public OnDemandMigrationExecutionManager(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Execute(int? batchID, bool runManualUpdateOnly)
        {
            ExecuteOnDemandMigrationQueue(batchID, runManualUpdateOnly);
        }

        public void ExecuteAsync(int? batchID, bool runManualUpdateOnly)
        {
            Task.Factory.StartNew(() => ExecuteOnDemandMigrationQueue(batchID, runManualUpdateOnly));
        }

        private void ExecuteOnDemandMigrationQueue(int? batchID, bool runManualUpdateOnly)
        {
            string[] viewTypes = AppSettingsManager.GetViewTypes();
            IMigrationService migrationService = new MigrationService(_unitOfWork);
            List<MigrationBatchs> migrationBatchs = migrationService.GetMigrationBatchs(batchID);
            foreach (MigrationBatchs migrationBatch in migrationBatchs)
            {
                //Update Batch queue status as in progress
                migrationService.UpdateMigrationBatchsStatus(migrationBatch, MigrationBatchStatus.InProgress);
                List<AccessFile> files = migrationService.GetFilesToMigrate(migrationBatch.BatchId);
                if (files != null && files.Count > 0)
                {
                    try
                    {
                        foreach (AccessFile file in files)
                        {
                            foreach (string viewType in viewTypes)
                            {
                                List<MigrationPlanItem> qidListToMigrate = migrationService.GetMigrationListForFile(migrationBatch.BatchId, file.FileID, viewType);

                                IAccessService accessService = new AccessService();
                                string mdbPath = AppSettingsManager.AccessFilePath + file.FileName;
                                List<string> fileQIDList = qidListToMigrate.Select(q => q.QID).ToList(); //accessService.GetQIDList(mdbPath);
                                foreach (var qid in qidListToMigrate)
                                {
                                    if (fileQIDList.Contains(qid.QID))
                                    {
                                        MigrationMapper mapper = migrationService.GetMigrationMapper(migrationBatch.BatchId, qid.FormDesignVersionId, viewType);
                                        if (mapper != null)
                                        {
                                            List<TableInfo> tables = mapper.GetPBPTables();
                                            JObject qidInstance = accessService.GetQIDData(mdbPath, tables, qid.QID);
                                            JObject formInstance = migrationService.GetFormInstanceJSON(qid.FormInstanceId);
                                            if (viewType == "SOT")
                                            {
                                                if (formInstance["DMSection"] != null)
                                                    formInstance["DMSection"].Parent.Remove();

                                                formInstance.Add(new JProperty("DMSection", JToken.FromObject(qidInstance)));
                                            }
                                            if (runManualUpdateOnly == false)
                                            {
                                                mapper.ProcessMappings(qidInstance, formInstance);

                                                // Expression Rule Execution of ViewType ==SOT
                                                if (viewType == "SOT")
                                                {
                                                    try
                                                    {
                                                        RuleExecutor executor = new RuleExecutor();
                                                        formInstance = executor.ProcessExpressionRules(qid, formInstance, migrationService);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        _logger.Error("Migration process expression rule execution has been closed. Due to error [" + ex.Message + "]");
                                                    }
                                                }
                                            }
                                            
                                                List<ManualDataUpdates> manualDataUpdates = migrationService.GetManualDataUpdates(qid.QID, qid.ViewType);
                                                if (manualDataUpdates != null)
                                                    mapper.ProcessManualDataUpdates(manualDataUpdates, ref formInstance);
                                            
                                            migrationService.UpdateDocumentJSON(qid.FormInstanceId, formInstance.ToString(Newtonsoft.Json.Formatting.None));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        migrationService.UpdateMigrationBatchsStatus(migrationBatch, MigrationBatchStatus.Fail);
                        _logger.Error(string.Format("Exception occures for batch id:{0},message:{1}", migrationBatch.BatchId, ex.Message));
                        throw ex;
                    }
                }

                migrationService.UpdateMigrationBatchsStatus(migrationBatch, MigrationBatchStatus.Completed);
                _logger.Info(string.Format("Migration Batch id:{0} has been done successfully!", migrationBatch.BatchId));
            }
        }
    }

}
