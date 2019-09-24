using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.viewmodels.ReportingCenter;
using tmg.equinox.caching.Interfaces;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.models;
using tmg.equinox.schema.Base.Interface;
using tmg.equinox.schema.Base.Model;

namespace tmg.equinox.schema.sql
{

    public class RPTConstant
    {
        public static string ObjectType_Table = "Table";
        public static string ObjectType_Field = "Field";
        public static string ObjectType_DataType = "DataType";
        public static string Op_Add = "Added";
        public static string Op_Removed = "Removed";
        public static string Op_Change = "Changed";
    }
    public class SQLSchemaRepository : ISchemaRepository
    {
        private IRptUnitOfWorkAsync _unitOfWork;
        List<ReportingTableInfo> _lstReportingTableInfo = new List<ReportingTableInfo>();
        private static readonly ILog _logger = LogProvider.For<SQLSchemaRepository>();
        public SQLSchemaRepository(ICacheProvider cacheProvider, IRptUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public SQLSchemaRepository()
        {
        }

        public List<ReportingTableInfo> PopulateSchemaFromDatabase(int jsonDesignId, int jsonDesignVersionId, bool force = false)
        {
            _lstReportingTableInfo = new List<ReportingTableInfo>();

            List<ReportingTableDetails> rawData = (from rt in this._unitOfWork.RepositoryAsync<ReportingTableInfo>().Query().Get()
                                                   join rtc in this._unitOfWork.RepositoryAsync<ReportingTableColumnInfo>().Get() on rt.ID equals rtc.ReportingTableInfo_ID
                                                   where (rt.DesignId == jsonDesignId  && rt.DesignVersionId == jsonDesignVersionId )
                                                   select new ReportingTableDetails
                                                   {
                                                       TableID = rt.ID,
                                                       TableName = rt.Name,
                                                       TableSchemaName = rt.SchemaName,
                                                       TableParentName = rt.ParentName,
                                                       TableDesignId = rt.DesignId,
                                                       TableDesignVersionId = rt.DesignVersionId,
                                                       TableCreationDate = rt.CreationDate,
                                                       TableLabel = rt.Label,
                                                       TableDescription = rt.Description,
                                                       TableDesignType = rt.DesignType,
                                                       TableDocumentPath = rt.DocumentPath,
                                                       DesignVersionNumber = rt.DesignVersionNumber,
                                                       ColumnID = rtc.ID,
                                                       ColumnReportingTableInfo_ID = (long)rtc.ReportingTableInfo_ID,
                                                       ColumnName = rtc.Name,
                                                       ColumnDataType = rtc.DataType,
                                                       ColumnLength = rtc.Length,
                                                       ColumnisNullable = rtc.isNullable,
                                                       ColumnIsPrimaryKey = rtc.IsPrimaryKey,
                                                       ColumnisUnique = rtc.isUnique,
                                                       ColumnIsIdentity = rtc.IsIdentity,
                                                       ColumnIdentityIncrement = rtc.IdentityIncrement,
                                                       ColumnIdentitySeed = rtc.IdentitySeed,
                                                       ColumnisForiegnKey = rtc.isForiegnKey,
                                                       ColumnForiegnKeyTableName = rtc.ForiegnKeyTableName,
                                                       ColumnForiegnKeyColumnName = rtc.ForiegnKeyColumnName,
                                                       ColumnvaluePath = rtc.valuePath,
                                                       ColumnReferenceTable = rtc.ReferenceTable,
                                                       ColumnCustomType = rtc.CustomType,
                                                       ColumnLabel = rtc.Label,
                                                       ColumnDescription = rtc.Description
                                                   }).ToList();


            var distinctTableNames = rawData.Select(p => p.TableName).Distinct().ToList();

            List<ReportingTableInfo> lstReportingTableInfo = new List<ReportingTableInfo>();

            foreach (var tableName in distinctTableNames)
            {
                var query = from row in rawData.AsEnumerable()
                            where row.TableName == tableName.ToString()
                            select row;
                if (force == false)
                {
                    _lstReportingTableInfo.Add(GetReportingTableInfo(query.ToList()));
                }
                else
                {
                    lstReportingTableInfo.Add(GetReportingTableInfo(query.ToList()));
                }
            }
            _logger.Debug("Populate schema from  database.");
            return lstReportingTableInfo;
            _logger.Debug("Populate schema from  database.");
        }


        public IList<ReportingTableInfo> GetTables()
        {
            List<ReportingTableDetails> rawData = (from rt in this._unitOfWork.RepositoryAsync<ReportingTableInfo>().Query().Get()
                                                   join rtc in this._unitOfWork.RepositoryAsync<ReportingTableColumnInfo>().Get() on rt.ID equals rtc.ReportingTableInfo_ID
                                                   select new ReportingTableDetails
                                                   {
                                                       TableID = rt.ID,
                                                       TableName = rt.DesignVersionNumber + "-" + rt.Name,
                                                       TableSchemaName = rt.SchemaName,
                                                       TableParentName =  rt.DesignVersionNumber + "-" + rt.ParentName,
                                                       TableDesignId = rt.DesignId,
                                                       TableDesignVersionId = rt.DesignVersionId,
                                                       TableCreationDate = rt.CreationDate,
                                                       TableLabel = rt.Label,
                                                       TableDescription = rt.Description,
                                                       TableDesignType = rt.DesignType,
                                                       TableDocumentPath = rt.DocumentPath,
                                                       DesignVersionNumber = rt.DesignVersionNumber,
                                                       ColumnID = rtc.ID,
                                                       ColumnReportingTableInfo_ID = (long)rtc.ReportingTableInfo_ID,
                                                       ColumnName = rtc.Name,
                                                       ColumnDataType = rtc.DataType,
                                                       ColumnLength = rtc.Length,
                                                       ColumnisNullable = rtc.isNullable,
                                                       ColumnIsPrimaryKey = rtc.IsPrimaryKey,
                                                       ColumnisUnique = rtc.isUnique,
                                                       ColumnIsIdentity = rtc.IsIdentity,
                                                       ColumnIdentityIncrement = rtc.IdentityIncrement,
                                                       ColumnIdentitySeed = rtc.IdentitySeed,
                                                       ColumnisForiegnKey = rtc.isForiegnKey,
                                                       ColumnForiegnKeyTableName = rtc.ForiegnKeyTableName,
                                                       ColumnForiegnKeyColumnName = rtc.ForiegnKeyColumnName,
                                                       ColumnvaluePath = rtc.valuePath,
                                                       ColumnReferenceTable = rtc.ReferenceTable,
                                                       ColumnCustomType = rtc.CustomType,
                                                       ColumnLabel = rtc.Label,
                                                       ColumnDescription = rtc.Description
                                                   }).ToList();


            var distinctTableNames = rawData.Select(p => p.TableName).Distinct().ToList();

            foreach (var tableName in distinctTableNames)
            {
                var query = from row in rawData.AsEnumerable()
                            where row.TableName == tableName.ToString()
                            select row;

                _lstReportingTableInfo.Add(GetReportingTableInfo(query.ToList()));
            }

            return _lstReportingTableInfo;
        }

        public List<ReportingTableDetails> GetRawTableData()
        {
            List<ReportingTableDetails> rawData = (from rt in this._unitOfWork.RepositoryAsync<ReportingTableInfo>().Query().Get()
                                                   join rtc in this._unitOfWork.RepositoryAsync<ReportingTableColumnInfo>().Get() on rt.ID equals rtc.ReportingTableInfo_ID
                                                   select new ReportingTableDetails
                                                   {
                                                    
                                                       TableSchemaName = rt.SchemaName,
                                                       TableParentName = rt.ParentName,
                                                       TableID = rt.ID,
                                                       TableName = rt.Name,
                                                       TableLabel = rt.Label,
                                                       TableDescription = rt.Description,
                                                       TableDesignType = rt.DesignType,
                                                       DesignVersionNumber = rt.DesignVersionNumber,
                                                       ColumnID = rtc.ID,
                                                       ColumnReportingTableInfo_ID = (long)rtc.ReportingTableInfo_ID,
                                                       ColumnName = rtc.Name,
                                                       ColumnDataType = rtc.DataType,
                                                       ColumnLength = rtc.Length,
                                                       ColumnisNullable = rtc.isNullable,
                                                       ColumnIsPrimaryKey = rtc.IsPrimaryKey,
                                                       ColumnisUnique = rtc.isUnique,
                                                       ColumnIsIdentity = rtc.IsIdentity,
                                                       ColumnIdentityIncrement = rtc.IdentityIncrement,
                                                       ColumnIdentitySeed = rtc.IdentitySeed,
                                                       ColumnisForiegnKey = rtc.isForiegnKey,
                                                       ColumnForiegnKeyTableName = rtc.ForiegnKeyTableName,
                                                       ColumnForiegnKeyColumnName = rtc.ForiegnKeyColumnName,
                                                       //ColumnvaluePath = rtc.valuePath,
                                                       ColumnReferenceTable = rtc.ReferenceTable,
                                                       ColumnCustomType = rtc.CustomType,
                                                       ColumnLabel = rtc.Label,
                                                       ColumnDescription = rtc.Description,
                                                       //  TableDesignId = rt.DesignId,
                                                       //    TableDesignVersionId = rt.DesignVersionId,
                                                       //   TableCreationDate = rt.CreationDate
                                                   }).OrderBy(m => new { m.TableSchemaName, m.TableName, m.ColumnName }).ToList();



            return rawData;
        }

        public IList<ReportingTableInfo> GetTables(JsonDesign jsonDesign)
        {
            if (_lstReportingTableInfo.Count() <= 0)
                if (AllReportingTablesCache.reportingTableCollection.TryGetValue(jsonDesign.JsonDesignVersionId, out _lstReportingTableInfo))
                {
                    return _lstReportingTableInfo;
                }
                else
                {
                    _lstReportingTableInfo = new List<ReportingTableInfo>();
                    PopulateSchemaFromDatabase(jsonDesign.JsonDesignId, jsonDesign.JsonDesignVersionId);
                    AllReportingTablesCache.reportingTableCollection.Add(jsonDesign.JsonDesignVersionId, _lstReportingTableInfo);
                }

            return _lstReportingTableInfo;
        }

        ReportingTableInfo GetReportingTableInfo(List<ReportingTableDetails> tableDetails)
        {
            ReportingTableInfo reportingTableInfo = new ReportingTableInfo();
            reportingTableInfo.ID = (int)tableDetails.FirstOrDefault().TableID;
            reportingTableInfo.Name = tableDetails.FirstOrDefault().TableName;
            reportingTableInfo.SchemaName = tableDetails.FirstOrDefault().TableSchemaName;
            reportingTableInfo.ParentName = tableDetails.FirstOrDefault().TableParentName;
            reportingTableInfo.DesignId = (int)tableDetails.FirstOrDefault().TableDesignId;
            reportingTableInfo.DesignVersionId = (int)tableDetails.FirstOrDefault().TableDesignVersionId;
            reportingTableInfo.CreationDate = tableDetails.FirstOrDefault().TableCreationDate;
            reportingTableInfo.Label = tableDetails.FirstOrDefault().TableLabel;
            reportingTableInfo.Description = tableDetails.FirstOrDefault().TableDescription;
            reportingTableInfo.DesignType = tableDetails.FirstOrDefault().TableDesignType;
            reportingTableInfo.DocumentPath = tableDetails.FirstOrDefault().TableDocumentPath;
            reportingTableInfo.DesignVersionNumber = tableDetails.FirstOrDefault().DesignVersionNumber;

            List<ReportingTableColumnInfo> colReportingTableColumnInfo = new List<ReportingTableColumnInfo>();
            foreach (ReportingTableDetails columnInfo in tableDetails)
            {
                ReportingTableColumnInfo reportingTableColumnInfo = new ReportingTableColumnInfo();
                reportingTableColumnInfo = GetReportingTableColumnInfo(columnInfo);
                colReportingTableColumnInfo.Add(reportingTableColumnInfo);
            }
            reportingTableInfo.Columns = colReportingTableColumnInfo;
            return reportingTableInfo;
        }

        ReportingTableColumnInfo GetReportingTableColumnInfo(ReportingTableDetails columnInfo)
        {
            ReportingTableColumnInfo reportingTableColumnInfo = new ReportingTableColumnInfo();
            reportingTableColumnInfo.ID = (int)columnInfo.ColumnID;
            reportingTableColumnInfo.ReportingTableInfo_ID = (int)columnInfo.ColumnReportingTableInfo_ID;
            reportingTableColumnInfo.Name = columnInfo.ColumnName;
            reportingTableColumnInfo.DataType = columnInfo.ColumnDataType;
            reportingTableColumnInfo.Length = columnInfo.ColumnLength;
            reportingTableColumnInfo.isNullable = columnInfo.ColumnisNullable;
            reportingTableColumnInfo.IsPrimaryKey = columnInfo.ColumnIsPrimaryKey;
            reportingTableColumnInfo.isUnique = columnInfo.ColumnisUnique;
            reportingTableColumnInfo.IsIdentity = columnInfo.ColumnIsIdentity;
            reportingTableColumnInfo.IdentityIncrement = columnInfo.ColumnIdentityIncrement;
            reportingTableColumnInfo.IdentitySeed = columnInfo.ColumnIdentitySeed;
            reportingTableColumnInfo.isForiegnKey = columnInfo.ColumnisForiegnKey;
            reportingTableColumnInfo.ForiegnKeyTableName = columnInfo.ColumnForiegnKeyTableName;
            reportingTableColumnInfo.ForiegnKeyColumnName = columnInfo.ColumnForiegnKeyColumnName;
            reportingTableColumnInfo.valuePath = columnInfo.ColumnvaluePath;
            reportingTableColumnInfo.ReferenceTable = columnInfo.ColumnReferenceTable;
            reportingTableColumnInfo.CustomType = columnInfo.ColumnCustomType;
            reportingTableColumnInfo.Label = columnInfo.ColumnLabel;
            reportingTableColumnInfo.Description = columnInfo.ColumnDescription;
            return reportingTableColumnInfo;
        }

        public void CreateOrUpdateTableInformation(ReportingTableInfo table, List<ReportingTableInfo> tables)
        {
            try
            {
                ReportingTableInfo objTable;
                if (!IsTableExists(table, tables))
                    objTable = InsertTabelMetadata(table);
                else
                {
                    objTable = GetTabelMetadataFromDB(table);

                    foreach (ReportingTableColumnInfo column in table.Columns)
                    {
                        if (!IsColumnExists(table, column))
                        {
                            column.ReportingTableInfo_ID = objTable.ID;
                            InsertColumnMetadata(column);
                        }
                        else
                        {
                            //Need to check other properties of that column and think about the migration.
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Exception occurred while maintaining table metadata [" + table.Name + "].", ex);
                throw ex;
            }
        }

        private void InsertColumnMetadata(ReportingTableColumnInfo column)
        {
            try
            {
                this._unitOfWork.Repository<ReportingTableColumnInfo>().Insert(column);
                this._unitOfWork.Save();
                _logger.Debug("Insert column [" + column.Name + "] metadata into WellCare database.");
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Exception occurred while inserting column [" + column.Name + "] metadata into WellCare database.", ex);
                throw ex;
            }
        }

        private ReportingTableInfo InsertTabelMetadata(ReportingTableInfo table)
        {
            try
            {
                this._unitOfWork.Repository<ReportingTableInfo>().Insert(table);
                this._unitOfWork.Save();
                _logger.Debug("Insert table [" + table.Name + "] metadata into WellCare database.");
                return table;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Exception occurred while inserting table [" + table.Name + "] metadata into WellCare database.", ex);
                throw ex;
            }
        }

        private ReportingTableInfo GetTabelMetadataFromDB(ReportingTableInfo table)
        {
            try
            {
                ReportingTableDetails RT1 = (from x in this._unitOfWork.Repository<ReportingTableInfo>().Get()
                                             where (x.Name == table.Name && x.DesignId == table.DesignId && x.SchemaName == table.SchemaName)
                                             select new ReportingTableDetails
                                             {
                                                 TableID = x.ID
                                             }).ToList().FirstOrDefault();

                table.ID = Convert.ToInt32(RT1.TableID);
                _logger.Debug("Get table [" + table.Name + "] metadata from WellCare database.");
                return table;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Exception occurred while getting table [" + table.Name + "] metadata from WellCare database.", ex);
                throw ex;
            }
        }

        bool IsTableExists(ReportingTableInfo table, List<ReportingTableInfo> tables)
        {
            if (_lstReportingTableInfo.Find(x => x.Name.Equals(table.Name) && x.DesignId == table.DesignId && x.DesignVersionId == table.DesignVersionId) != null)
                return true;
            else
                return false;

        }

        bool IsColumnExists(ReportingTableInfo table, ReportingTableColumnInfo column)
        {
            ReportingTableInfo currentTable = _lstReportingTableInfo.Where(x => x.Name.ToString().Equals(table.Name) && x.SchemaName.ToString().Equals(table.SchemaName) && x.DesignId == table.DesignId && x.DesignVersionId == table.DesignVersionId).FirstOrDefault();
            if (currentTable != null)
                if (currentTable.Columns.ToList().Find(x => x.Name.Equals(column.Name)) != null)
                    return true;
                else
                    return false;
            else
                return false;
        }

        public bool CheckDesignExists(JsonDesign jsonDesign)
        {
            if (this._unitOfWork.RepositoryAsync<ReportingTableInfo>().Get().Any(t => t.DesignId == jsonDesign.JsonDesignId))
                return true;
            else
                return false;
        }

        public string isSchemaExistForDesignID(JsonDesign jsonDesign) {
            return this._unitOfWork.RepositoryAsync<ReportingTableInfo>().Get().Where(t => t.DesignId == jsonDesign.JsonDesignId).Select(r => r.SchemaName).FirstOrDefault();               
        }

        public void Log(SchemaVersionActivityLog schemaVersionActivityLog)
        {
            this._unitOfWork.RepositoryAsync<SchemaVersionActivityLog>().Insert(schemaVersionActivityLog);
        }
        public void Log(List<SchemaVersionActivityLog> schemaVersionActivityLog)
        {
            foreach (var log in schemaVersionActivityLog)
            {
                this._unitOfWork.RepositoryAsync<SchemaVersionActivityLog>().Insert(log);
            }
            this._unitOfWork.Save();
        }
    }

    public static class AllReportingTablesCache
    {
        public static IDictionary<int, List<ReportingTableInfo>> reportingTableCollection = new Dictionary<int, List<ReportingTableInfo>>();
    }
}
