using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using tmg.equinox.pbp.dataaccess;

namespace tmg.equinox.applicationservices.pbp
{
    public class PBPMigrationService : IPBPMigrationService
    {
        public JObject GetDefaultJSONForFormDesignVersion(int formDesignVersionId)
        {
            JObject defaultJSON = new JObject();
            using (PBPMigrationModel model = new PBPMigrationModel())
            {
                var formDesignJSONs = from fdJSON in model.FormDesignVersionJSONs where fdJSON.FormDesignVersionID == formDesignVersionId select fdJSON;
                if (formDesignJSONs != null && formDesignJSONs.Count() > 0)
                {
                    string defaultJSSONStr = formDesignJSONs.First().DefaultJSON;
                    defaultJSON = JObject.Parse(defaultJSSONStr);
                }
            } 
            return defaultJSON;
        }

        public List<PBPFile> GetFilesToMigrate()
        {
            List<PBPFile> files = new List<PBPFile>();
            using (PBPMigrationModel model = new PBPMigrationModel())
            {
                var accessFiles = from accessFile in model.AccessPBPFiles
                                  where (accessFile.IsActive == true )
                                  select new PBPFile
                                  {
                                      FileID = accessFile.FileID,
                                      FileName = accessFile.FileName,
                                      IsActive = true
                                  };
                if (accessFiles != null && accessFiles.Count() > 0)
                {
                    files = accessFiles.ToList();
                }
            }
            return files;
        }

        public List<MigrationPlanItem> GetMigrationListForFile(int fileId,string viewType)
        {
            List<MigrationPlanItem> plans = new List<MigrationPlanItem>();
            using (PBPMigrationModel model = new PBPMigrationModel())
            {
                var migrationPlans = from migrationPlan in model.MigrationPlans
                                     where migrationPlan.FileID == fileId && migrationPlan.IsActive == true && migrationPlan.ViewType == viewType
                                     select new MigrationPlanItem
                                     {
                                         FileID = migrationPlan.FileID,
                                         FolderId = migrationPlan.FolderId,
                                         FolderVersionId = migrationPlan.FolderVersionId,
                                         FormInstanceId = migrationPlan.FormInstanceId,
                                         FormDesignVersionId = migrationPlan.FormDesignVersionId,
                                         MigrationPlanID = migrationPlan.MigrationPlanID,
                                         IsActive = true,
                                         QID = migrationPlan.QID,
                                         ViewType = migrationPlan.ViewType
                                     };
                if (migrationPlans != null && migrationPlans.Count() > 0)
                {
                    plans = migrationPlans.ToList();
                }
            }
            return plans;

        }

        public MigrationMapper GetMigrationMapper(int formDesignVersionId,string viewType)
        {
            MigrationMapper mapper = null;
            List<MigrationFieldItem> pbpFieldItems = new List<MigrationFieldItem>();
            using (PBPMigrationModel model = new PBPMigrationModel())
            {
                var fieldItems = from pbpFieldItem in model.PBPBenefitMappings
                                 where pbpFieldItem.FormDesignVersionID == formDesignVersionId && pbpFieldItem.IsActive == true && pbpFieldItem.ViewType == viewType
                                 select new MigrationFieldItem
                                 {
                                     Dictionaryitems = (from pbpDictItem in pbpFieldItem.PBPBenefitsDictionaries
                                                        select new PBPFieldItem
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
                                     Codes = pbpFieldItem.Codes,
                                     Code_Values = pbpFieldItem.Code_Values,
                                     DataType = pbpFieldItem.DataType,
                                     ColumnName = pbpFieldItem.ColumnName,
                                     DocumentPath = pbpFieldItem.DocumentPath,
                                     ElementType = pbpFieldItem.ElementType,
                                     FieldTitle = pbpFieldItem.FieldTitle,
                                     FormDesignID = pbpFieldItem.FormDesignID,
                                     FormDesignVersionID = pbpFieldItem.FormDesignVersionID,
                                     Length = pbpFieldItem.Length,
                                     MappingID = pbpFieldItem.MappingID,
                                     MappingType = pbpFieldItem.MappingType,
                                     PBPFile = pbpFieldItem.PBPFile,
                                     TableName = pbpFieldItem.TableName,
                                     Title = pbpFieldItem.Title,
                                     IsArrayElement = pbpFieldItem.IsArrayElement,
                                     ViewType = pbpFieldItem.ViewType,
                                     SOTDocumentPath = pbpFieldItem.SOTDocumentPath,
                                     SOTPrefix = pbpFieldItem.SOTPrefix,
                                     SOTSuffix= pbpFieldItem.SOTSuffix,
                                     IfBlankThenValue = pbpFieldItem.IfBlankThenValue,
                                     IsYesNoField = pbpFieldItem.IsYesNoField,
                                     IsCheckBothFields = pbpFieldItem.IsCheckBothFields,
                                     SetSimilarValues = pbpFieldItem.SetSimilarValues
                                 };
                if (fieldItems != null && fieldItems.Count() > 0)
                {
                    pbpFieldItems = fieldItems.ToList();
                    mapper = new MigrationMapper(pbpFieldItems);
                }
            }
            return mapper;
        }

        public int GetNewBatchId()
        {
            int batchId = 0;
            using (PBPMigrationModel model = new PBPMigrationModel())
            {
                MigrationBatch batch = new MigrationBatch();
                model.MigrationBatches.Add(batch);
                model.SaveChanges();
                batchId = batch.BatchId;
            }
            return batchId;
        }

        public void ExecuteSQL(string query)
        {
            using (PBPMigrationModel model = new PBPMigrationModel())
            {
                model.Database.ExecuteSqlCommand(query);
            }
        }
        
    }
}
