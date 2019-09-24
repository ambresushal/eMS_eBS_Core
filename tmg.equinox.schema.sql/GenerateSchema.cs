using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Transactions;
using tmg.equinox.config;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.models;
using tmg.equinox.schema.Base.Interface;
using tmg.equinox.schema.Base.Model;

namespace tmg.equinox.schema.sql
{
    public class GenerateSchema : IGenerateSchema
    {
        private static readonly ILog _logger = LogProvider.For<GenerateSchema>();
        Server _server;
        Database _database;
        Schema _tableSchema;
        ISchemaRepository _schemaRepository = new SQLSchemaRepository();
        JsonDesign _jsonDesign = new JsonDesign();
        bool _isDesignExists;
        List<ReportingTableInfo> _previousMetaData;
        public GenerateSchema(ISchemaRepository schemaRepository, JsonDesign jsonDesign, Server server, Database database)
        {
            _schemaRepository = schemaRepository;
            _jsonDesign = jsonDesign;
            _server = server;
            _database = database;
        }

        public void CreateSchema(List<ReportingTableInfo> tables)
        {
            _isDesignExists = _schemaRepository.CheckDesignExists(_jsonDesign);
            _previousMetaData = _schemaRepository.PopulateSchemaFromDatabase(_jsonDesign.JsonDesignId, _jsonDesign.PreviousJsonDesignVersionId, true);

            GetTableSchemaInstance(_jsonDesign.TableSchemaName);
            _schemaRepository.PopulateSchemaFromDatabase(_jsonDesign.JsonDesignId, _jsonDesign.JsonDesignVersionId);
            _logger.Debug("Populated database information from database metadata.");

            foreach (var table in tables)
            {
                CreateOrUpdateTables(table);
                _schemaRepository.CreateOrUpdateTableInformation(table, tables);
            }
            if (_jsonDesign.PreviousJsonDesignVersionId != 0)
                GetDifferenceBetweenPrevNewDesign();
        }

        private void GetDifferenceBetweenPrevNewDesign()
        {
            var currentMetaData = _schemaRepository.GetTables(_jsonDesign);

            List<SchemaVersionActivityLog> removeTableOrColLog = new List<SchemaVersionActivityLog>();

            var query = from o in _previousMetaData
                        join e in currentMetaData
                          on new { o.SchemaName, o.Name } equals new { e.SchemaName, e.Name } into pp
                        from e in pp.DefaultIfEmpty()
                        where e == null
                        select new ReportingTableInfo { SchemaName = o.SchemaName, Name = o.Name, DesignType = o.DesignType, Label = o.Label, DocumentPath = o.DocumentPath };


            query = query.ToList();

            //table missing
            foreach (var missingTable in query)
            {

                removeTableOrColLog.Add(createLogData(missingTable, RPTConstant.ObjectType_Table,
                            RPTConstant.Op_Removed, missingTable.Name, null));
            }
            //colum missing
            var queryCol =
                 from o in _previousMetaData
                 join e in currentMetaData
                     on new { o.SchemaName, o.Name, } equals new { e.SchemaName, e.Name }
                 where (o.Columns.Count > e.Columns.Count)
                 select new ReportingTableInfo { SchemaName = o.SchemaName, Name = o.Name, DesignType = o.DesignType, Label = o.Label, PreColumns = o.Columns, Columns = e.Columns };


            queryCol = queryCol.ToList();


            //table missing
            foreach (var table in queryCol)
            {
                foreach (var col in table.PreColumns)
                {
                    if (table.Columns.Where(m => m.Name == col.Name).ToList().Count() == 0) //not contain
                    {
                        removeTableOrColLog.Add(createLogData(table, RPTConstant.ObjectType_Field,
                             RPTConstant.Op_Removed, col.Name, col));
                    }
                }
            }

            _schemaRepository.Log(removeTableOrColLog);

        }
        private void GetTableSchemaInstance(string schemaName)
        {
            try
            {
                if (_database.Schemas.Contains(schemaName))
                {
                    _tableSchema = _database.Schemas[schemaName];
                    _logger.Debug("Get table schema [" + schemaName + "] instance.");
                }
                else
                {
                    _tableSchema = new Schema(_database, schemaName);
                    _tableSchema.Create();
                    _logger.Debug("Created table schema [" + schemaName + "].");
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Exception occurred while creating table schema.", ex);
                throw ex;
            }
        }

        private SchemaVersionActivityLog createLogData(ReportingTableInfo tableInfo, string objectType, string operation, string value, ReportingTableColumnInfo column)
        {
            // var valuePath = "";
            //if (tableInfo!=null)
            //{
            //    valuePath = string.Format("Source:{0}.{1}.{2}-{3}", tableInfo.DesignType, tableInfo.Label, tableInfo.DocumentPath, value);
            //}
            //else
            //{
            //    valuePath = value;
            //}
            var schemaVersionActivityLog = new SchemaVersionActivityLog
            {
                CreationDate = DateTime.Now,
                DesignVersion = _jsonDesign.VersionNumber,
                ObjectType = objectType,
                Operation = operation,
                Value = value,
                DesignVersionId = _jsonDesign.JsonDesignVersionId,
                DesignType = tableInfo.DesignType,
                Label = (column == null) ? tableInfo.Label : column.Label,
                ValuePath = (column == null) ? tableInfo.DocumentPath : column.valuePath,
                CustomType = (column == null) ? "" : column.CustomType
            };
            return schemaVersionActivityLog;
        }
        private void CreateOrUpdateTables(ReportingTableInfo tableInfo)
        {
            try
            {
                string tableName = tableInfo.Name;
                if (!_database.Tables.Contains(tableName, _tableSchema.Name))
                {
                    Table table = new Table(_database, tableName, _tableSchema.Name);

                    CreateOrUpdateTableColumns(tableInfo.Columns, ref table, tableInfo);
                    table.Create();
                    _logger.Debug("Created table structure [" + tableName + "].");

                    if (_isDesignExists)
                    {
                        _schemaRepository.Log(createLogData(tableInfo, RPTConstant.ObjectType_Table,
                            RPTConstant.Op_Add, tableName, null));
                    }
                    var query = from row in tableInfo.Columns.AsEnumerable()
                                where row.isForiegnKey == true
                                select row;

                    List<ReportingTableColumnInfo> fkDetails = query.ToList();
                    if (fkDetails.Count > 0) CreateForeginKey(ref table, fkDetails);
                }
                else
                {
                    Table table = _database.Tables[tableName, _tableSchema.Name];
                    var isColumnAddedinExistingTable = CreateOrUpdateTableColumns(tableInfo.Columns, ref table, tableInfo);
                    table.Alter();
                    if (isColumnAddedinExistingTable && _isDesignExists)
                    {
                        _schemaRepository.Log(createLogData(tableInfo, RPTConstant.ObjectType_Table,
                                RPTConstant.Op_Change, tableName, null));
                    }
                    _logger.Debug("Altered table structure [" + tableName + "].");
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Exception occurred while creating table [" + tableInfo.Name + "].", ex);
                throw ex;
            }
        }

        private bool CreateOrUpdateTableColumns(ICollection<ReportingTableColumnInfo> lstColumns, ref Table table, ReportingTableInfo tableInfo)
        {
            var isChange = false;
            foreach (ReportingTableColumnInfo col in lstColumns)
            {
                if (!table.Columns.Contains(col.Name))
                {
                    Column column = new Column();

                    column.Parent = table;
                    column.Name = col.Name;
                    column.DataType = GetDataType(col.DataType, col.Length);
                    column.Nullable = true; //col.isNullable;

                    //if (col.DataType == "string" || col.DataType == "varchar" || col.DataType == "nvarchar")
                    //    if (col.Length == "")
                    //        writeLog(table.Schema + "," + table.Name + "," + col.Name);

                    if (col.IsIdentity)
                    {
                        column.Nullable = false;
                        column.Identity = true;
                        column.IdentityIncrement = col.IdentityIncrement;
                        column.IdentitySeed = col.IdentitySeed;
                        _logger.Debug("Set identity on column [" + column.Name + "] on table [" + table.Name + "].");
                    }
                    table.Columns.Add(column);
                    _logger.Debug("Column created [" + column.Name + "] on table [" + table.Name + "].");

                    if (_isDesignExists)
                    {
                        _schemaRepository.Log(createLogData(tableInfo, RPTConstant.ObjectType_Field,
                                    RPTConstant.Op_Add, col.Name, col));
                    }
                    // Add indexes 
                    if (col.IsPrimaryKey)
                    {
                        Index index = new Index(table, "PK_" + table.Name + "Table");
                        index.IndexKeyType = IndexKeyType.DriPrimaryKey;
                        index.IndexedColumns.Add(new IndexedColumn(index, col.Name));
                        table.Indexes.Add(index);
                        _logger.Debug("Created primary key on table [" + table.Name + "].");
                    }

                    if (col.isUnique)
                    {
                        Index index = new Index(table, "Un_" + col.Name + "Column_" + table.Name + "Table");
                        index.IndexKeyType = IndexKeyType.DriUniqueKey;
                        index.IndexedColumns.Add(new IndexedColumn(index, col.Name));
                        table.Indexes.Add(index);
                        _logger.Debug("Unique key index created on column [" + column.Name + "] on table [" + table.Name + "].");
                    }
                    isChange = true;
                }
                else
                {
                    Column column = table.Columns[col.Name];
                    if (_isDesignExists)
                    {
                        //get the previous Datatype 
                        var newLen = "";
                        var isDataTypeChange = isDataTypeHasChanged(table.Schema, table.Name, col.Name, col.DataType, col.Length, out newLen);

                        if (isDataTypeChange || newLen != "")
                        {
                            column.DataType = setNewDataTypeIfDataTypeisChanged(col, newLen);
                            column.Alter();

                            string value = string.Format("{0}.{1}.{2}.{3}", table.Schema, table.Name, col.Name, column.DataType.ToString());

                            if (newLen != "")
                            {
                                value = string.Format("{0}.NewLen.", value, newLen);
                            }
                            _schemaRepository.Log(createLogData(tableInfo, RPTConstant.ObjectType_DataType,
                                RPTConstant.Op_Change, value, col));
                        }
                    }
                    //_schemaRepository.Log(createLogData(RPTConstant.ObjectType_Table,
                    //        RPTConstant.Op_Change, string.Format("{0}.{1}", _tableSchema.Name, tableName)));
                    //todoNJ : get the previous formdesign Table info
                    /*
                    _schemaRepository.Log(createLogData(RPTConstant.ObjectType_Field,
                                 RPTConstant.Op_Add, string.Format("{0}.{1}.{2}", table.Schema, table.Name, col.Name)));
                        */

                    //check the other properties and if change the change those like
                    //column.Name = "changedName";
                    //column.Alter();
                }
            }
            return isChange;
        }
        private bool isDataTypeHasChanged(string schemaname, string tableName, string columnName, string dataType, string newLen, out string lengthChange)
        {
            bool isChange = false;
            var findtable = _previousMetaData.Find(m => (m.SchemaName == schemaname && m.Name == tableName));
            lengthChange = "";
            if (findtable != null)
            {
                if (findtable.Columns != null)
                {
                    var findcol = findtable.Columns.Where(m => m.Name == columnName).FirstOrDefault();
                    if (findcol != null)
                    {
                        if (findcol.DataType != dataType)
                        {
                            isChange = true;
                        }
                        else //checking len if old len =100 and new len == 1000, return 1000 or if old len = 1000 and newlen = 100 the return 1000
                        {
                            if (findcol.Length == null || newLen == null)
                            {
                                return isChange;
                            }
                            if (newLen == findcol.Length)
                            {
                                return isChange;
                            }
                            if (newLen == "max")
                            {
                                lengthChange = newLen;
                            }
                            else if (findcol.Length == "max")
                            {
                                lengthChange = "max";
                            }
                            else if (Convert.ToInt32(newLen) > Convert.ToInt32(findcol.Length))
                            {
                                lengthChange = newLen;
                            }
                            else
                            {
                                lengthChange = findcol.Length;
                            }
                        }
                    }
                }
            }
            return isChange;
        }
        private DataType setNewDataTypeIfDataTypeisChanged(ReportingTableColumnInfo col, string newLen)
        {
            var newDataType = GetDataType(col.DataType, col.Length);

            if (newDataType == DataType.Bit || newDataType == DataType.Int || newDataType == DataType.Numeric(0, 18) || newDataType == DataType.DateTime || newDataType == DataType.Bit || newDataType == DataType.Float || newDataType == DataType.Decimal(0, 18))
            {
                col.DataType = "nvarchar";
                if (newDataType == DataType.Int || newDataType == DataType.Numeric(0, 18) || newDataType == DataType.Float || newDataType == DataType.Decimal(0, 18))
                {
                    col.Length = "2000";
                }
                else
                {
                    col.Length = "100";
                }
                newDataType = GetDataType(col.DataType, col.Length);
            }
            else
            {
                if (col.Length != null)
                {
                    if (col.Length != "")
                    {
                        col.Length = newLen;
                    }
                }
            }
            return newDataType;
        }
        private void writeLog(string message)
        {
            string path = "D:\\MissingFieldLength.txt";
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }
        private void CreateForeginKey(ref Table table, List<ReportingTableColumnInfo> fkDetails)
        {
            try
            {
                foreach (ReportingTableColumnInfo col in fkDetails)
                {
                    if (col.isForiegnKey)
                    {
                        ForeignKey fk = new ForeignKey(table, "FK_" + table.Name + col.Name);
                        ForeignKeyColumn fkc = new ForeignKeyColumn(fk, col.Name, col.ForiegnKeyColumnName);
                        fk.Columns.Add(fkc);
                        fk.ReferencedTable = col.ForiegnKeyTableName;
                        fk.ReferencedTableSchema = table.Schema;
                        fk.Create();
                        _logger.Debug("Created foregin key on table [" + table.Name + "].");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Exception occurred while creating foregin key on table [" + table.Name + "].", ex);
                throw ex;
            }
        }

        private DataType GetDataType(string dataType, string dataSize)
        {
            DataType type = DataType.Char(1);

            switch (dataType.ToLower())
            {
                case "string":
                case "varchar":
                case "nvarchar":
                    if (dataSize == "max")
                        type = DataType.NVarCharMax;
                    else if (Convert.ToInt32(dataSize) >= 4000)
                        type = DataType.NVarCharMax;
                    else if (dataSize != "")
                        type = DataType.NVarChar(Convert.ToInt32(dataSize));
                    else
                        type = DataType.NVarChar(2000);
                    break;
                case "int":
                    type = DataType.Int;
                    break;
                case "long":
                    type = DataType.Numeric(0, 18);
                    break;
                case "date":
                    type = DataType.DateTime;
                    break;
                case "bool":
                    type = DataType.Bit;
                    break;
                case "decimal":
                    type = DataType.Decimal(0, 18);
                    break;
                case "float":
                    type = DataType.Float;
                    break;
            }

            return type;
        }

    }
}
