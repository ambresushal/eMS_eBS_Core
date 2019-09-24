using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.web.ODMExecuteManager.Interface;
using tmg.equinox.web.ODMExecuteManager.Model;

namespace tmg.equinox.web.ODMExecuteManager
{
    public class MigrationService : IMigrationService
    {
        private IUnitOfWorkAsync _unitOfWork;
        public MigrationService(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public List<MigrationBatchs> GetMigrationBatchs(int? batchID)
        {
            if (batchID == null)
                return _unitOfWork.RepositoryAsync<MigrationBatchs>().Get().AsEnumerable().Where(x => x.IsActive == true && x.Status == Enum.GetName(typeof(MigrationBatchStatus), MigrationBatchStatus.Queued)).ToList();
            else
                return _unitOfWork.RepositoryAsync<MigrationBatchs>().Get().AsEnumerable().Where(x => x.BatchId == batchID).ToList();
        }
        public JObject GetFormInstanceJSON(int formInstanceId)
        {
            JObject defaultJSON = new JObject();

            var formInstance = (from fi in _unitOfWork.RepositoryAsync<FormInstanceDataMap>().Get()
                                where fi.FormInstanceID == formInstanceId
                                select fi).First();
            if (formInstance != null)
                defaultJSON = JObject.Parse(formInstance.FormData);

            return defaultJSON;
        }

        public void UpdateDocumentJSON(int formInstanceId, string documentJSON)
        {
            var formInstance = (from fi in _unitOfWork.RepositoryAsync<FormInstanceDataMap>().Get()
                                where fi.FormInstanceID == formInstanceId
                                select fi).First();
            if (formInstance != null)
            {
                formInstance.FormData = documentJSON;

                _unitOfWork.RepositoryAsync<FormInstanceDataMap>().Update(formInstance);
                _unitOfWork.Save();
            }
        }

        public List<AccessFile> GetFilesToMigrate(int batchID)
        {
            List<AccessFile> files = new List<AccessFile>();

            var accessFiles = from accessFile in _unitOfWork.RepositoryAsync<AccessFiles>().Get().Where(x => x.BatchId == batchID)
                              select new AccessFile
                              {
                                  FileID = accessFile.FileID,
                                  BatchID = accessFile.BatchId,
                                  FileName = accessFile.FileName,
                              };
            if (accessFiles != null && accessFiles.Count() > 0)
            {
                files = accessFiles.ToList();
            }
            return files;
        }

        public List<MigrationPlanItem> GetMigrationListForFile(int batchId, int fileId, string viewType)
        {
            List<MigrationPlanItem> plans = new List<MigrationPlanItem>();
            var migrationPlans = from migrationPlan in _unitOfWork.RepositoryAsync<MigrationPlans>().Get()
                                 where migrationPlan.BatchId == batchId && migrationPlan.FileID == fileId && migrationPlan.ViewType == viewType
                                 select new MigrationPlanItem
                                 {
                                     MigrationPlanID = migrationPlan.MigrationPlanID,
                                     BatchID = migrationPlan.BatchId,
                                     FileID = migrationPlan.FileID,
                                     FolderId = migrationPlan.FolderId,
                                     FolderVersionId = migrationPlan.FolderVersionId,
                                     FormInstanceId = migrationPlan.FormInstanceId,
                                     FormDesignVersionId = migrationPlan.FormDesignVersionId,
                                     ViewType = migrationPlan.ViewType,
                                     QID = migrationPlan.QID
                                 };
            if (migrationPlans != null && migrationPlans.Count() > 0)
            {
                plans = migrationPlans.ToList();
            }
            return plans;

        }

        public bool IsSectionRulesExecute(int batchID, string viewType, string sectionName)
        {
            var migrationBatchSection = (from mbs in _unitOfWork.RepositoryAsync<MigrationBatchSection>().Get()
                                                           where mbs.BatchId == batchID && mbs.ViewType == viewType && mbs.SectionGeneratedName == sectionName
                                                           select mbs).FirstOrDefault();
            if (migrationBatchSection != null)
                return true;
            else
                return false;
        }
        public List<ManualDataUpdates> GetManualDataUpdates(string qid, string viewType)
        {
            return _unitOfWork.RepositoryAsync<ManualDataUpdates>().Get().Where(x => x.QID == qid && x.ViewType == viewType).ToList();
        }
        public MigrationMapper GetMigrationMapper(int batchId, int formDesignVersionId, string viewType)
        {
            MigrationMapper mapper = null;
            List<MigrationFieldItem> FieldItems = new List<MigrationFieldItem>();

            var fieldItems = from pbpFieldItem in _unitOfWork.RepositoryAsync<BenefitMapping>().Get()
                             join section in _unitOfWork.RepositoryAsync<MigrationBatchSection>().Get().Where(x => x.BatchId == batchId)
                             on new { pbpFieldItem.ViewType, pbpFieldItem.SectionGeneratedName } equals new { section.ViewType, section.SectionGeneratedName }
                             where pbpFieldItem.FormDesignVersionID == formDesignVersionId
                             && pbpFieldItem.IsActive == true
                             && pbpFieldItem.ViewType == viewType
                             select new MigrationFieldItem
                             {
                                 Dictionaryitems = (from pbpDictItem in pbpFieldItem.BenefitsDictionaries
                                                    select new FieldItem
                                                    {
                                                        Codes = pbpDictItem.Codes,
                                                        CODE_VALUES = pbpDictItem.CODE_VALUES,
                                                        ID = pbpDictItem.ID,
                                                        FIELD_TITLE = pbpDictItem.FIELD_TITLE,
                                                        FILE = pbpDictItem.FILE,
                                                        LENGTH = pbpDictItem.LENGTH,
                                                        NAME = pbpDictItem.NAME,
                                                        TITLE = pbpDictItem.TITLE,
                                                        TYPE = pbpDictItem.TYPE,
                                                        YEAR = pbpDictItem.YEAR
                                                    }).ToList(),
                                 MappingID = pbpFieldItem.MappingID,
                                 PBPFile = pbpFieldItem.PBPFile,
                                 ColumnName = pbpFieldItem.ColumnName,
                                 DataType = pbpFieldItem.DataType,
                                 Length = pbpFieldItem.Length,
                                 FieldTitle = pbpFieldItem.FieldTitle,
                                 Title = pbpFieldItem.Title,
                                 Codes = pbpFieldItem.Codes,
                                 Code_Values = pbpFieldItem.Code_Values,
                                 FormDesignVersionID = pbpFieldItem.FormDesignVersionID,
                                 FormDesignID = pbpFieldItem.FormDesignID,
                                 TableName = pbpFieldItem.TableName,
                                 MappingType = pbpFieldItem.MappingType,
                                 DocumentPath = pbpFieldItem.DocumentPath,
                                 ElementType = pbpFieldItem.ElementType,
                                 IsArrayElement = pbpFieldItem.IsArrayElement,
                                 ViewType = pbpFieldItem.ViewType,
                                 SOTDocumentPath = pbpFieldItem.SOTDocumentPath,
                                 SOTPrefix = pbpFieldItem.SOTPrefix,
                                 SOTSuffix = pbpFieldItem.SOTSuffix,
                                 IfBlankThenValue = pbpFieldItem.IfBlankThenValue,
                                 IsYesNoField = pbpFieldItem.IsYesNoField,
                                 IsCheckBothFields = pbpFieldItem.IsCheckBothFields,
                                 SetSimilarValues = pbpFieldItem.SetSimilarValues,
                                 SectionGeneratedName = pbpFieldItem.SectionGeneratedName
                             };

            if (fieldItems != null && fieldItems.Count() > 0)
            {
                FieldItems = fieldItems.ToList();
            }

            if (FieldItems != null && FieldItems.Count() > 0)
                mapper = new MigrationMapper(FieldItems);

            return mapper;
        }

        public void UpdateMigrationBatchsStatus(MigrationBatchs migrationBatch, MigrationBatchStatus status)
        {
            if (migrationBatch != null)
            {
                if (status == MigrationBatchStatus.InProgress)
                    migrationBatch.Status = "In Progress";
                else if (status == MigrationBatchStatus.Completed)
                {
                    migrationBatch.Status = "Completed";
                    migrationBatch.ExecutedDate = DateTime.Now;
                    migrationBatch.IsActive = false;
                }
                else
                {
                    migrationBatch.Status = "Fail";
                    migrationBatch.ExecutedDate = DateTime.Now;
                    migrationBatch.IsActive = false;
                }
            }
            _unitOfWork.RepositoryAsync<MigrationBatchs>().Update(migrationBatch);
            _unitOfWork.Save();
        }
    }

}
