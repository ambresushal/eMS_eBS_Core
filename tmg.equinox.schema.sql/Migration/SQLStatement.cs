using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.config;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.models;
using tmg.equinox.schema.Base;
using tmg.equinox.schema.Base.Interface;
using tmg.equinox.schema.Base.Model;

namespace tmg.equinox.schema.sql.Migration
{
    public class SQLStatement : ISQLStatement
    {
        private static readonly ILog _logger = LogProvider.For<SQLStatement>();
        IReportingDataRepository _reportingDataRepository;
        List<ReportingTableInfo> _tableCollection;
        IJsonData _jsData;
        JData _jData;
        private IMDMSyncDataService _mDMSyncDataService;
        public SQLStatement(IReportingDataRepository reportingDataRepository, IMDMSyncDataService mDMSyncDataService)
        {
            _reportingDataRepository = reportingDataRepository;
            _mDMSyncDataService = mDMSyncDataService;

        }
        public void UpdateReportingDatabase(JData jData)
        {
            List<ReportingTableInfo> _tableCollection;
            try
            {
                _jData = jData;
                _jsData = new JsonData(_jData);
                _tableCollection = _reportingDataRepository.GetTableInformation(_jData);

                // var option = new TransactionOptions();
                //   option.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                //   option.Timeout = TimeSpan.FromMinutes(600);

                _logger.Info("Transaction scope has been started.");
                // Temporary commentted transaction to get maximum data for reporting.
                //using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, option))
                //{
                // _reportingDataRepository.ExecuteQueryOnReportingDB(GetStatementForFormInstanceDetails());
                var sqlStatement = "BEGIN BEGIN TRY BEGIN TRANSACTION ";
                foreach (ReportingTableInfo table in _tableCollection)
                {
                    //if (table.Name == "LimitInformationDetail_MLServiceList" || table.Name == "LimitInformationDetail")
                    //{
                    //    Console.Write("got it");
                    //}
                    //else
                    //{
                    //    continue;
                    //}

                    JToken jToken = _jsData.GetTableData(table.Name, table.DocumentPath);
                    if (jToken != null)
                    {
                        if (jToken.Type == JTokenType.Array && jToken.Children().Count() <= 0)
                        {
                            sqlStatement = string.Format("{0} ; {1}", sqlStatement, DeletedFormInstanceFromReportingDatabase(table.SchemaName, table.Name));
                        }
                        else if (jToken.HasValues == false)
                            sqlStatement = string.Format("{0} ; {1}", sqlStatement, DeletedFormInstanceFromReportingDatabase(table.SchemaName, table.Name));
                        else
                        {                            
                            if (_jsData.ifRecordsExists(jToken))
                            {
                                //if (table.Name.Contains("_")) {
                                    sqlStatement = string.Format("{0} ; {1}", sqlStatement, DeletedFormInstanceFromReportingDatabase(table.SchemaName, table.Name));
                                // }
                                int totalRow = jToken.Count();
                                for (int index = 0; index < totalRow; index++)
                                {
                                    if (table.Name.Contains("_"))
                                    {                                        
                                        string dataSourceName = table.Name.Split('_')[table.Name.Split('_').Length - 1];
                                        if (jToken[index][dataSourceName] != null)
                                        {
                                            int countChild = jToken[index][dataSourceName].Count();                                            
                                            for (int childIndx = 0; childIndx < countChild; childIndx++)
                                            {
                                                if (jToken[index][dataSourceName].Type == JTokenType.Array)
                                                {
                                                    sqlStatement = string.Format("{0} ; {1}", sqlStatement, GetStatement(table, jToken[index][dataSourceName][childIndx], index, false));
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        sqlStatement = string.Format("{0} ; {1}", sqlStatement, GetStatement(table, jToken[index], index));
                                    }
                                }
                            }
                            else
                            {
                                if (jToken.Type != JTokenType.String)
                                    sqlStatement = string.Format("{0} ; {1}", sqlStatement, GetStatement(table, jToken, null));
                            }

                            //if (table.Name != "PBPList")
                            //    sqlStatement = string.Format("{0} ; {1}", sqlStatement, RemoveDeletedRecordsFromReportingDatabase(table, jToken));
                        }

                    }
                }

               
                sqlStatement = string.Format("{0} ; {1}", sqlStatement, GetStatementForFormInstanceDetails(_jsData.GetContractNumber()));
                _logger.Debug("SQL Statement Updation for FormInstancID  '" + jData.FormInstanceId + "' is Beign.");
                sqlStatement = string.Format("{0} ; {1}", sqlStatement , " COMMIT TRANSACTION END TRY BEGIN CATCH ROLLBACK TRANSACTION DECLARE    @ErMessage NVARCHAR(2048),    @ErSeverity INT,   @ErState INT  SELECT   @ErMessage = ERROR_MESSAGE(),   @ErSeverity = ERROR_SEVERITY(),   @ErState = ERROR_STATE()  RAISERROR (@ErMessage,             @ErSeverity,             @ErState ) END CATCH END");
                //sqlStatement = sqlStatement + " COMMIT TRANSACTION END TRY BEGIN CATCH ROLLBACK TRANSACTION END CATCH END";
                _reportingDataRepository.ExecuteQueryOnReportingDB(sqlStatement);
                _logger.Debug("SQL Statement Updation for FormInstancID  '" + jData.FormInstanceId + "' is Successful.");

                //tran.Complete();
                _logger.Info("Transaction scope has been committed.");
                //}
                //UpdateMDMSyncStatus(jData.FormInstanceId, (int)MSMSyncStatus.Completed);
                _mDMSyncDataService.UpdateDocumentUpdateTracker(jData.FormInstanceId, (int)MSMSyncStatus.Completed);
            }
            catch (Exception ex)
            {
                _mDMSyncDataService.AddLogForMDMProcess(new MDMLog() { ForminstanceID = jData.FormInstanceId, FormDesignID = 0, FormDesignVersionID = 0, AddedDate = DateTime.Now, Error = ex.Message, ErrorDescription = ex.StackTrace.ToString() });
                //UpdateMDMSyncStatus(jData.FormInstanceId, (int)MSMSyncStatus.Errored);
                _mDMSyncDataService.UpdateDocumentUpdateTracker(jData.FormInstanceId, (int)MSMSyncStatus.Errored);
                _logger.ErrorException("ReportingCenter database data update transaction fail. FormInstanceId=[" + jData.FormInstanceId.ToString() + "],FormDesignId=[" + jData.FormDesignId.ToString() + "],FormDesignVersionId=[" + jData.FormDesignVersionId.ToString() + "]", ex);
                //throw ex;
            }
        }

        private void UpdateMDMSyncStatus(int formInstanceId, int status)
        {
            try
            {
                var documentTracker = _mDMSyncDataService.GetDocumentUpdateTrackerStatusByFormInstanceId(formInstanceId);
                if (documentTracker != null)
                {
                    documentTracker.Status = status;
                    documentTracker.UpdatedDate = DateTime.Now;
                    if (status == (int)MSMSyncStatus.Completed)
                    {
                        documentTracker.OldJsonHash = documentTracker.CurrentJsonHash;
                    }
                    _mDMSyncDataService.UpdateDocumentUpdateTracker(documentTracker);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        private string DeletedFormInstanceFromReportingDatabase(string tableSchema, string tableName)
        {
            StringBuilder deleteChildStatement = new StringBuilder();
            deleteChildStatement.Append("IF((SELECT top 1 Name from rpt.reportingtableinfo where parentname = '" + tableName + "' and CHARINDEX('_',Name) > 0  and SchemaName = '" + tableSchema + "') <>'')");
            deleteChildStatement.Append(" BEGIN DECLARE @tablename" + tableName  + " Varchar(200) Set  @tablename" + tableName + " = " + "(SELECT SchemaName+ '.'+ Name FROM rpt.reportingtableinfo WHERE parentname = '" + tableName + "' and CHARINDEX('_',Name) > 0  and SchemaName = '" + tableSchema + "')");
            deleteChildStatement.Append(" DECLARE @query" + tableName + " AS nvarchar(500); SET @query" + tableName + "  = 'DELETE FROM ' +  @tablename" + tableName  + "+ '  WHERE FormInstanceId = " + _jData.FormInstanceId + "' EXECUTE sp_executesql @query" + tableName + "; END");
            return deleteChildStatement.ToString() + " ; " + string.Format("DELETE FROM [{0}].[{1}]  WHERE FormInstanceID={2}", tableSchema, tableName, _jData.FormInstanceId);
        }
        private string GetStatement(ReportingTableInfo table, JToken jToken, int? sequence, bool updateRequired = true)
        {
            if (sequence == null)
            {
                string whereClauseForSequence = table.Name != "PBPList" ? GetWhereClauseForSequence(jToken, sequence) : "";
                if (updateRequired && _reportingDataRepository.CheckFormInstanceIdExists(table, whereClauseForSequence, _jData.FormInstanceId))
                    return UpdateStatement(table, jToken, sequence);
                else
                    return InsertStatement(table, jToken, sequence);
            }
            return InsertStatement(table, jToken, sequence);
        }
        private string GetStatementForFormInstanceDetails(string ContractNumber)
        {
            string query = "";
            _jData.AnchorDocumentID = _jData.AnchorDocumentID == null ? 0 : _jData.AnchorDocumentID;
            if (_reportingDataRepository.CheckFormInstanceIdExists(_jData.FormInstanceId))
            {
                query = "UPDATE [RPT].[FormInstanceDetail] SET [FolderID] =" + _jData.FolderID + ",[FolderVersionID] = " + _jData.FolderVersionID + ",[EffectiveDate] =" + GetDataFormat(_jData.EffectiveDate ?? DateTime.Now) + ",[FormDesignID] = " + _jData.FormDesignId + ",[FormDesignVersionID] = " + _jData.FormDesignVersionId + ",[IsMasterList] = " + (_jData.IsMasterList == true ? "1" : "0") + ", [AnchorDocumentID] = " + _jData.AnchorDocumentID + ", [ContractNo] = '" + ContractNumber + "' WHERE [FormInstanceID] = " + _jData.FormInstanceId;
            }
            else
            {
                query = "INSERT INTO [RPT].[FormInstanceDetail]([InsertDate],[FormInstanceID],[FormInstanceName],[FolderID],[FolderVersionID],[EffectiveDate],[FormDesignID],[FormDesignVersionID],[IsMasterList],[AnchorDocumentID],[ContractNo])";
                query += "VALUES(getdate() , " + _jData.FormInstanceId + ",'" + _jData.FormInstanceName + "', " + _jData.FolderID + ", " + _jData.FolderVersionID + "," + GetDataFormat(_jData.EffectiveDate ?? DateTime.Now) + ", " + _jData.FormDesignId + ", " + _jData.FormDesignVersionId + ", " + (_jData.IsMasterList == true ? "1" : "0") + "," + _jData.AnchorDocumentID + ",'" + ContractNumber + "'" + ")";
            }
            return query;
        }

        private string GetDataFormat(DateTime date)
        {
            return string.Format("DATEFROMPARTS({0}, {1}, {2})", date.Year.ToString(), date.Month.ToString(), date.Day.ToString());
        }
        private string RemoveDeletedRecordsFromReportingDatabase(ReportingTableInfo table, JToken jToken)
        {
            string sqlStatements = "";
            DataTable dtFormInstanceData = _reportingDataRepository.GetFormInstanceData(table, _jData.FormInstanceId);
            if (dtFormInstanceData != null)
            {
                foreach (DataRow dr in dtFormInstanceData.Rows)
                {
                    List<JToken> lstTokens = jToken.FindTokens("RowIDProperty");
                    if (lstTokens.Count > 0)
                    {
                        bool recordFound = false;
                        foreach (JToken jt in lstTokens)
                        {
                            if (dtFormInstanceData.Columns.Contains("Sequence"))
                            {
                                if (dr["Sequence"].ToString() == jt.ToString())
                                {
                                    recordFound = true;
                                    break;
                                }
                            }
                        }
                        if (!recordFound)
                        {
                            sqlStatements = string.Format("{0} ; {1}", sqlStatements, DeletedRecordFromReportingDatabase(table, dr));
                        }
                    }
                }

            }
            return sqlStatements;
        }

        private string DeletedRecordFromReportingDatabase(ReportingTableInfo table, DataRow dr)
        {
            if (dr.Table.Columns.Contains("Sequence"))
            {
                string sequence = "0";
                if (dr["Sequence"]!=null && dr["Sequence"].ToString() !=null && dr["Sequence"].ToString() != "" ) {
                    sequence = dr["Sequence"].ToString();
                }

                var statement = string.Format("DELETE FROM [{0}].[{1}]   WHERE FormInstanceId = {2} AND [Sequence] = {3}", table.SchemaName, table.Name, dr["FormInstanceId"].ToString(), sequence);
                //_reportingDataRepository.ExecuteQueryOnReportingDB(statement);
                return statement;
            }
            else
            {
                return "";
            }
        }
        private string GetWhereClauseForSequence(JToken jToken, int? sequence)
        {
            if (sequence != null)
            {
                if (jToken.FindTokens("RowIDProperty").Count > 0)
                {
                    int numValue;
                    if (Int32.TryParse(jToken["RowIDProperty"].ToString(), out numValue))
                        return string.Format("AND [Sequence] = {0}", numValue.ToString());
                    else
                        return string.Format("AND [Sequence] = {0}", sequence.ToString());
                }
                else
                    return string.Format("AND [Sequence] = {0}", sequence.ToString());
            }
            else
                return "";
        }
        private string InsertStatement(ReportingTableInfo table, JToken jToken, int? sequence)
        {
            var statement = "";
            var statementReferenceTable = "";
            var validationLogStatement = "";
            if (!string.IsNullOrEmpty(table.ParentName))
            {
                statementReferenceTable = prepareSelectReferenceTableStatement(table.SchemaName, table.ParentName);
            }
            statement = string.Format("INSERT INTO [{0}].[{1}] ", table.SchemaName, table.Name);

            var insertColumn = "";
            var insertValues = "";
            var tableName = string.Format("[{0}].[{1}] ", table.SchemaName, table.Name);


            var colFormInstance = table.Columns.Where(a => a.Name.ToString() == "FormInstanceId").FirstOrDefault();
            insertColumn = Concanate(insertColumn, "[" + colFormInstance.Name + "]", null, null, null, ref validationLogStatement);
            insertValues = Concanate(insertValues, _jData.FormInstanceId.ToString(), colFormInstance.DataType, null, null, ref validationLogStatement);


            foreach (var col in table.Columns)
            {
                if (col.Name != "ID")//if (col.Name != table.Name + "ID")
                {
                    if (col.Name == "FormInstanceId")
                    {
                        continue;
                    }
                    else
                    {
                        insertColumn = Concanate(insertColumn, "[" + col.Name + "]", null, null, null, ref validationLogStatement);
                        if (col.isForiegnKey)
                        {
                            insertValues = Concanate(insertValues, prepareSelectReferenceTableStatement(table.SchemaName, col.ReferenceTable), col.DataType, null, null, ref validationLogStatement);
                        }
                        else
                        {
                            if (col.Name == "Sequence")
                            {
                                /* List<JToken> lstTokens = jToken.FindTokens("RowIDProperty");
                                 if (lstTokens.Count <= 0)
                                     insertValues = Concanate(insertValues, sequence.ToString(), col.DataType, null, null, ref validationLogStatement);
                                 else
                                 {
                                     int numValue;
                                     if (Int32.TryParse(jToken["RowIDProperty"].ToString(), out numValue))
                                         insertValues = Concanate(insertValues, numValue.ToString(), col.DataType, null, null, ref validationLogStatement);
                                     else
                                         insertValues = Concanate(insertValues, sequence.ToString(), col.DataType, null, null, ref validationLogStatement);
                                 }*/
                                insertValues = Concanate(insertValues, sequence.ToString(), col.DataType, null, null, ref validationLogStatement);
                            }
                            else
                            {
                                insertValues = Concanate(insertValues, jToken[col.Name] != null ? jToken[col.Name].ToString() : "null", col.DataType, col, tableName, ref validationLogStatement);
                            }
                        }
                    }
                }
            }

            statement = string.Format("{0} ({1}) Values ({2} );", statement, insertColumn, insertValues);
            statement = string.Format("{0} {1}", statement, validationLogStatement);
            _logger.Debug("Prepared update statement [" + statement + "]");
            return statement;
        }
        private string Concanate(string statement, string values, string valueType, ReportingTableColumnInfo col, string tableName, ref string validationLogStatement)
        {

            if (col != null)
                DoTechnicalValidation(valueType, ref values, col, tableName, ref validationLogStatement);



            if (values == "null")
            {
                if (!string.IsNullOrEmpty(statement))
                {
                    return string.Format("{0},{1}", statement, values);
                }
                else
                {
                    return string.Format("{0}{1}", statement, values);
                }
            }
            else if (string.IsNullOrEmpty(statement))
                return string.Format("{0}{1}", statement, values.Replace("'", "''"));
            else if (valueType == "string" || valueType == "date")
                return string.Format("{0},'{1}'", statement, values.Replace("'", "''"));
            else if (valueType == "bool")
                return string.Format("{0},{1}", statement,(values == "true" || values == "True" || values == "Yes" || values == "yes") ? "1" : "0");
            else if (values.Trim() == "")
                return string.Format("{0},{1}", statement, "NULL");
            else
                return string.Format("{0},{1}", statement, GetNullOrReplaceComma(values));

        }


        private string GetNullOrReplaceComma(string value)
        {
            if (value.ToLower() == "not applicable" || value.ToLower() == "na" || value.ToLower() == "blank")
            {
                return "null";
            }
            return value.Replace(",", "");
        }
        private string prepareSelectReferenceTableStatement(string SchemaName, string parentTableName)
        {
            var statement = string.Format("(SELECT TOP 1 ID FROM [{0}].[{1}] WHERE FormInstanceId = {2})", SchemaName, parentTableName, _jData.FormInstanceId);
            //_logger.Debug("Prepared stagement [" + statement + "] for forgien key reference.");
            return statement;
        }
        private string UpdateStatement(ReportingTableInfo table, JToken jToken, int? sequence)
        {
            var statement = string.Format("UPDATE [{0}].[{1}] ", table.SchemaName, table.Name);
            var tableName = string.Format("[{0}].[{1}] ", table.SchemaName, table.Name);
            string whereClauseForSequence = string.Empty;
            string updateColumn = "SET ";

            var validationLogStatement = "";
            foreach (var col in table.Columns)
            {
                if (col.Name != "ID" && col.Name != "FormInstanceId" && col.Name != "Sequence")
                {
                    if (col.isForiegnKey)
                        updateColumn = string.Format("{0} [{1}] = {2},", updateColumn, col.Name, prepareSelectReferenceTableStatement(table.SchemaName, table.ParentName));
                    else
                    {
                        //get the datatype : have a case statement : get the value : check the value is matching with datatype : if not do log and mark Value null and finally update the Validation flag false
                        string columnName = col.Name;
                        if (col.valuePath != null && col.valuePath.IndexOf('.') != -1)
                        {
                            columnName = col.valuePath.Split('.')[col.valuePath.Split('.').Length - 1];
                        }

                        updateColumn = string.Format("{0} [{1}] = {2},", updateColumn, col.Name, GetColumnValue((jToken[columnName] != null ? jToken[columnName].ToString() : ""), col.DataType, col, tableName, ref validationLogStatement));
                    }
                }

                if (col.Name == "Sequence")
                {
                    int numValue;
                    if (jToken["RowIDProperty"] != null && Int32.TryParse(jToken["RowIDProperty"].ToString(), out numValue))
                        whereClauseForSequence = string.Format("AND [Sequence] = {0}", numValue.ToString());
                    else
                    {
                        if (sequence == null)
                        {
                            whereClauseForSequence = string.Format("AND [Sequence] = {0}", 0);
                        }
                        else
                        {
                            whereClauseForSequence = string.Format("AND [Sequence] = {0}", sequence.ToString());
                        }
                    }

                }
            }

            statement = string.Format("{0} {1} WHERE [FormInstanceId] = {2} {3} ;", statement, updateColumn.Remove(updateColumn.Length - 1), _jData.FormInstanceId.ToString(), whereClauseForSequence);
            //_logger.Debug("Prepared update statement [" + statement + "]");
            return statement;
        }
        string GetColumnValue(string value, string dataType, ReportingTableColumnInfo col, string tableName, ref string validationLogStatement)
        {
            if (value != null && value != "" && value != "System.Collections.Generic.List`1[System.Object]")
            {
                DoTechnicalValidation(dataType, ref value, col, tableName, ref validationLogStatement);
                if (value == "null")
                    return value;
                else if (dataType == "string" || dataType == "date")
                    value = string.Format("'{0}'", value.Replace("'", "''"));
                else if (dataType == "bool")
                    value = string.Format("{0}", (value == "true" || value == "True" || value == "Yes" || value == "yes") ? "1" : "0");
                else
                    value = string.Format("{0}", value.Replace("'", "''"));
            }
            else
            {
                value = "null";
            }
            return value;
        }

        private string concanetValidationLogStatement(string validationLogStatement, string reportingTableColumnInfoID, string errorMessage)
        {
            var insertValidationLogStatement = string.Format("INSERT into rpt.ValidationLog (ReportingTableColumnInfoID,creationDate,DesignVersionId,  FormInstanceId,ErrorMessage) values ('{0}',getdate(),'{1}','{2}','{3}');", reportingTableColumnInfoID, _jData.FormDesignVersionId, _jData.FormInstanceId, errorMessage);
            /* if (validationLogStatement == "") performance issue : due to this in report "excel sheet" will show duplicate record
             {
                 validationLogStatement = string.Format("DELETE FROM rpt.ValidationLog where DesignVersionId = {0} and ReportingTableColumnInfoID ={1} and FormInstanceId={2};", _jData.FormDesignVersionId.ToString(), reportingTableColumnInfoID, _jData.FormInstanceId);

             }*/
            validationLogStatement = string.Format("{0} {1}", validationLogStatement, insertValidationLogStatement);
            return validationLogStatement;
        }
        private string DoTechnicalValidation(string dataType, ref string value, ReportingTableColumnInfo col, string tableName, ref string validationLogStatement)
        {
            string errroMessage = "";
            if (col.IsPrimaryKey)
            {
                return "";
            }
            switch (dataType.ToLower())
            {
                case "string":
                case "varchar":
                case "nvarchar":
                    if (!string.IsNullOrEmpty(col.Length))
                    {
                        int len;
                        if (Int32.TryParse(col.Length, out len))
                        {
                            if (value.Length > len)
                            {
                                errroMessage = string.Format("Value Length({0}) is greater then configured({1}) ", value.Length.ToString(), len.ToString());
                                value = value.Substring(0, len - 1);
                                validationLogStatement = concanetValidationLogStatement(validationLogStatement, col.ID.ToString(), errroMessage);
                            }
                        }
                    }
                    break;
                case "int":
                    int validIntValue;
                    if (!Int32.TryParse(value, out validIntValue))
                    {
                        errroMessage = string.Format("Value expected as INT and passed as {0}", value.Length == 0 ? "blank" : value);
                        validationLogStatement = concanetValidationLogStatement(validationLogStatement, col.ID.ToString(), errroMessage);
                        value = "null";
                    }
                    break;
                case "long":
                    long validLongValue;
                    if (!long.TryParse(value, out validLongValue))
                    {
                        errroMessage = string.Format("Value expected as LONG and passed as {0}", value.Length == 0 ? "blank" : value);
                        validationLogStatement = concanetValidationLogStatement(validationLogStatement, col.ID.ToString(), errroMessage);

                        value = "null";
                    }
                    break;
                case "date":
                    DateTime validDateValue;
                    if (!DateTime.TryParse(value, out validDateValue))
                    {
                        errroMessage = string.Format("Value expected as DATETIME and passed as {0}", value.Length == 0 ? "blank" : value);
                        validationLogStatement = concanetValidationLogStatement(validationLogStatement, col.ID.ToString(), errroMessage);

                        value = "null";
                    }
                    break;
                case "bool":
                    bool validBoolValue;
                    if (value == "0" || value == "1")
                    {
                        value = value == "0" ? "false" : "true";
                    }
                    if (value.ToLower().Trim() == "yes" || value.ToLower().Trim() == "no")
                    {
                        value = value.ToLower().Trim() == "no" ? "false" : "true";
                    }
                    if (!bool.TryParse(value, out validBoolValue))
                    {
                        errroMessage = string.Format("Value expected as Boolean Flag and passed as {0}", value.Length == 0 ? "blank" : value);
                        validationLogStatement = concanetValidationLogStatement(validationLogStatement, col.ID.ToString(), errroMessage);

                        value = "null";
                    }
                    break;
                case "decimal":
                    decimal validDecValue;
                    if (!decimal.TryParse(value, out validDecValue))
                    {
                        errroMessage = string.Format("Value expected as DECIMAL and passed as {0}", value.Length == 0 ? "blank" : value);
                        validationLogStatement = concanetValidationLogStatement(validationLogStatement, col.ID.ToString(), errroMessage);

                        value = "null";
                    }
                    break;
                case "float":
                    float validFloatValue;
                    if (!float.TryParse(value, out validFloatValue))
                    {
                        errroMessage = string.Format("Value expected as FLOAT and passed as {0}", value.Length == 0 ? "blank" : value);
                        validationLogStatement = concanetValidationLogStatement(validationLogStatement, col.ID.ToString(), errroMessage);

                        value = "null";
                    }
                    break;
            }
            return value;
        }

        //arithmatic overflow

    }
}
