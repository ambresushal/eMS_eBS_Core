using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.dependencyresolution;
using System.IO;

namespace tmg.equinox.pbpimportservices
{
    public class PBPImportOperation
    {
        #region Private Members
        private AccessDbContext _accessDbContext;
        private string _mdfFileNameWithPath;
        private string _applicationConnectingString;
        private IPBPImportServices _pbpImportService;

        #endregion
        public PBPImportOperation(string mdfFileNameWithPath, string applicationConnectingString)
        {
            _mdfFileNameWithPath = mdfFileNameWithPath;
            _applicationConnectingString = applicationConnectingString;
            _accessDbContext = new AccessDbContext(mdfFileNameWithPath);
            UnityConfig.RegisterComponents();
            _pbpImportService = UnityConfig.Resolve<IPBPImportServices>();
        }

        #region Public Methods
        public void PerformImportOperation(int batchId)
        {
            try
            {
                SqlImportOperations objSqlImportOperations = new SqlImportOperations(_applicationConnectingString);

                DataTable sourceUsedTables = _accessDbContext.GetUsedTables();
                for (int i = 0; i < sourceUsedTables.Rows.Count; i++)
                {
                    try
                    {
                        string strSourceTableName = sourceUsedTables.Rows[i][2].ToString();
                        string strQuery = "SELECT * FROM " + strSourceTableName;
                        DataTable sourceTable = _accessDbContext.ExecuteSelectQuery(strQuery, null);
                        sourceTable.TableName = strSourceTableName;
                        sourceTable.Columns.Add(new DataColumn("PBPImportBatchID", typeof(Int32)));
                        string strTargetTableName = string.Format("[PBP].[{0}]", strSourceTableName);
                        foreach (DataRow dRow in sourceTable.Rows)
                        {
                            dRow["PBPImportBatchID"] = batchId;
                        }
                        sourceTable.AcceptChanges();

                        string createTableScript = SqlHelperClass.GetCreateTableSql(sourceTable, "PBP");

                        if (objSqlImportOperations.CreateTableStructure(createTableScript))
                        {
                            objSqlImportOperations.ImportDataToSqlServer(sourceTable, strTargetTableName, null);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void PerformImportOperationWithSequence(int batchId, IEnumerable<PBPImportTablesViewModel> collPBPImportTablesList, string username)
        {
            string message = string.Empty, strSourceTableName = string.Empty, strTargetTableName = string.Empty;
            try
            {
                SqlImportOperations objSqlImportOperations = new SqlImportOperations(_applicationConnectingString);
                DataTable sourceUsedTables = _accessDbContext.GetUsedTables();
                int totalRows = 0;

                foreach (PBPImportTablesViewModel objPBPImportTablesViewModel in collPBPImportTablesList)
                {
                    try
                    {
                        bool isTablePresent = false;
                        for (int i = 0; i < sourceUsedTables.Rows.Count; i++)
                        {
                            string strTalbePresent = sourceUsedTables.Rows[i][2].ToString();
                            if (objPBPImportTablesViewModel.PBPTableName.Contains(strTalbePresent))
                            {
                                isTablePresent = true;
                                break;
                            }
                        }
                        if (isTablePresent)
                        {
                            strSourceTableName = objPBPImportTablesViewModel.PBPTableName;
                            string strQuery = "SELECT * FROM " + strSourceTableName;
                            try
                            {
                                DataTable sourceTable = _accessDbContext.ExecuteSelectQuery(strQuery, null);
                                if (sourceTable != null)
                                {
                                    sourceTable.TableName = strSourceTableName;
                                    sourceTable.Columns.Add(new DataColumn("PBPImportBatchID", typeof(Int32)));
                                    strTargetTableName = string.Format("[PBP].[{0}]", strSourceTableName);
                                    foreach (DataRow dRow in sourceTable.Rows)
                                    {
                                        dRow["PBPImportBatchID"] = batchId;
                                    }
                                    totalRows = sourceTable.Rows.Count;
                                    sourceTable.AcceptChanges();

                                    objSqlImportOperations.ImportDataToSqlServer(sourceTable, strTargetTableName, objPBPImportTablesViewModel.PBPImportTableColumnsViewModel);
                         
                                    //Add Log Entry that the table is successfully imported
                                    message = "Inserted " + totalRows + " rows in " + strTargetTableName + " from " + strSourceTableName;
                                    _pbpImportService.AddPBPImportActivityLog(1, batchId, Path.GetFileName(_mdfFileNameWithPath), strTargetTableName, message, username);
                                }
                            }
                            catch (Exception ex)
                            {
                                //Add Log Entry that the table is unsuccessfully imported
                                message = "Error occured while inserting data in " + strTargetTableName + " from " + strSourceTableName + ": " + ex.Message + " " + ex.StackTrace;
                                _pbpImportService.AddPBPImportActivityLog(1, batchId, Path.GetFileName(_mdfFileNameWithPath), strTargetTableName, message, username);
                                //throw ex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //Add Log Entry that the table is unsuccessfully imported
                        message = "Error occured while inserting data in " + strTargetTableName + " from " + strSourceTableName + ": " + ex.Message + " " + ex.StackTrace;
                        _pbpImportService.AddPBPImportActivityLog(1, batchId, Path.GetFileName(_mdfFileNameWithPath), strTargetTableName, message, username);
                        //throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                //Add Log Entry that the table is unsuccessfully imported
                message = "Error occured while inserting data in " + strTargetTableName + " from " + strSourceTableName + ": " + ex.Message + " " + ex.StackTrace;
                _pbpImportService.AddPBPImportActivityLog(1, batchId, Path.GetFileName(_mdfFileNameWithPath), strTargetTableName, message, username);
                throw ex;
            }
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
