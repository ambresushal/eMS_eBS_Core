using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using tmg.equinox.applicationservices.FolderVersionDetail;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.infrastructure.util;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;
using System.Transactions;
using tmg.equinox.domain.entities.Utility;
using FolderVersionState = tmg.equinox.domain.entities.Enums.FolderVersionState;
using DocumentDesignTypes = tmg.equinox.domain.entities.Enums.DocumentDesignTypes;
using VersionType = tmg.equinox.domain.entities.Enums.VersionType;
using tmg.equinox.applicationservices.viewmodels.Settings;
using Newtonsoft.Json;
using tmg.equinox.applicationservices.viewmodels.EmailNotitication;
using System.Net.Mail;
using tmg.equinox.applicationservices.viewmodels.Report;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using tmg.equinox.applicationservices.viewmodels;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using System.Data.OleDb;
using System.Data;
using System.Data.Entity;

namespace tmg.equinox.applicationservices
{
    public class PBPImportHelperServices
    {
        #region private Members
        IUnitOfWorkAsync _unitOfWorkAsync = null;
        #endregion

        #region Public Mentods
        public PBPImportHelperServices(IUnitOfWorkAsync unitOfWorkAsync)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
        }

        public int GetFormDesignVersionID(string docName)
        {
            int formId = this._unitOfWorkAsync.RepositoryAsync<FormDesign>().Get()
                                         .Where(s => s.FormName.Equals(docName) && s.IsActive == true)
                                         .Select(s => s.FormID).FirstOrDefault();
            int FormDesignVersionID = this._unitOfWorkAsync.RepositoryAsync<FormDesignVersion>()
                                         .Query()
                                         .Filter(c => c.FormDesignID == formId)
                                         .OrderBy(c => c.OrderByDescending(d => d.FormDesignVersionID))
                                         .Get().Max(s => s.FormDesignVersionID);
            return FormDesignVersionID;
        }

        public int GetPBPViewFormInstanceID(int folderVersionId, int pBPFromdesignVersionID, int? formInstanceID)
        {
            int PBPViewFormInstanceID = this._unitOfWorkAsync.RepositoryAsync<FormInstance>().Get()
                                         .Where(s => s.FolderVersionID.Equals(folderVersionId)
                                         && s.IsActive == true
                                         && s.FormDesignVersionID.Equals(pBPFromdesignVersionID)
                                         && s.AnchorDocumentID == formInstanceID
                                         )
                                         .Select(s => s.FormInstanceID).FirstOrDefault();

            return PBPViewFormInstanceID;
        }

        public int GetMedicareFormInstanceID(int folderVersionId, int medicareFormdesignVersionID, string qID)
        {
            int MedicareFormInstanceID = this._unitOfWorkAsync.RepositoryAsync<FormInstance>().Get()
                                         .Where(s => s.FolderVersionID.Equals(folderVersionId)
                                         && s.IsActive == true
                                         && s.FormDesignVersionID.Equals(medicareFormdesignVersionID)
                                         && s.Name.Equals(qID)
                                         )
                                         .Select(s => s.FormInstanceID).FirstOrDefault();

            return MedicareFormInstanceID;
        }


        public PBPImportDetails GetFolderDetailByQID(string qID)
        {
            PBPImportDetails ViewModel = this._unitOfWorkAsync.RepositoryAsync<PBPImportDetails>().Get()
                                                .Where(s => s.QID.Equals(qID)
                                                       && s.IsActive == true
                                                ).FirstOrDefault();
            return ViewModel;
        }

        public ServiceResult UpdateImportQueueStatus(int PBPImportQueueID, domain.entities.Enums.ProcessStatusMasterCode status)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                PBPImportQueue itemToUpdateList = this._unitOfWorkAsync.RepositoryAsync<PBPImportQueue>().Get()
                                                .Where(s => s.PBPImportQueueID.Equals(PBPImportQueueID)
                                                ).FirstOrDefault();

                if (itemToUpdateList != null)
                {
                    itemToUpdateList.Status = (int)status;
                    using (var scope = new TransactionScope())
                    {
                        repository.UIFrameworkContext Context = new repository.UIFrameworkContext();
                        Context.Entry(itemToUpdateList).State = EntityState.Modified;
                        Context.SaveChanges();
                        scope.Complete();
                    }
                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }
        #endregion
    }

    public class ValueFormatterHelper
    {
        public ValueFormatterHelper()
        {

        }
    }

    public class OleDbHelper
    {
        public string GetOleDbConnectingString(string mdfDbPath)
        {
            string strOledbConnectingString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + mdfDbPath + "; Persist Security Info=True;";
            return strOledbConnectingString;
        }
    }

    public class AccessDbContext
    {
        #region private Members
        private OleDbDataAdapter _dataAdapter;
        private OleDbConnection _connection;
        private OleDbHelper _oleDBHelperClass;
        #endregion

        #region Public 
        public string _connectingString;
        #endregion

        #region Constructor
        public AccessDbContext(string mdfDbPath)
        {
            _dataAdapter = new OleDbDataAdapter();
            _oleDBHelperClass = new OleDbHelper();
            _connectingString = _oleDBHelperClass.GetOleDbConnectingString(mdfDbPath);
            _connection = new OleDbConnection(_connectingString);
        }
        #endregion

        #region Private Methods
        private OleDbConnection OpenConnection()
        {
            if (_connection.State == ConnectionState.Closed || _connection.State == ConnectionState.Broken)
            {
                _connection.Open();
            }

            return _connection;
        }
        #endregion

        #region Public Methods
        public DataTable ExecuteSelectQuery(string query, OleDbParameter[] parameters)
        {
            DataSet ds = new DataSet();
            DataTable dt = null;

            using (OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = OpenConnection();
                cmd.CommandText = query;
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                _dataAdapter.SelectCommand = cmd;
                _dataAdapter.Fill(ds);
                dt = ds.Tables[0];
            }

            return dt;
        }

        public string ExecuteReader(string query, OleDbParameter[] parameters)
        {
            OleDbDataReader sRdr;
            string result = string.Empty;

            using (OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = OpenConnection();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.uspGetEntityClass";
                cmd.Parameters.AddRange(parameters);
                sRdr = cmd.ExecuteReader();
                result = Convert.ToString(sRdr.GetString(0).ToString());
            }

            return result;
        }

        public DataTable GetUsedTables()
        {
            DataTable dt = null;
            string[] restrictions = new string[4];
            restrictions[3] = "Table";

            OleDbConnection connection = OpenConnection();
            dt = connection.GetSchema("Tables", restrictions);

            return dt;
        }
        #endregion
    }

    public class AccessDBService
    {
        #region Public Members
        public string OledbConnectingString;
        #endregion

        #region Public Methods
        public AccessDBService(string mdfDbPath)
        {
            OleDbHelper Helper = new OleDbHelper();
            OledbConnectingString = Helper.GetOleDbConnectingString(mdfDbPath);
        }

        public List<PBPPlanViewModel> ReadPBPTableData()
        {
            List<PBPPlanViewModel> PBPPlanList = new List<PBPPlanViewModel>();
            OleDbConnection conn;
            OleDbCommand cmd;
            OleDbDataReader Reader;
            conn = new OleDbConnection(OledbConnectingString);
            cmd = new OleDbCommand("SELECT DISTINCT (QID),PBP_A_PLAN_NAME,PBP_A_CONTRACT_NUMBER" + " FROM " + "PBP", conn);
            try
            {
                conn.Open();
                Reader = cmd.ExecuteReader();
                if (Reader.HasRows)
                {
                    while (Reader.Read())
                    {
                        PBPPlanList.Add(new PBPPlanViewModel
                        {
                            QID = Reader["QID"].ToString(),
                            PBPPlanName = Reader["PBP_A_PLAN_NAME"].ToString(),
                            //PBPPlanNumber = Reader["PBP_A_CONTRACT_NUMBER"].ToString(),
                            PBPPlanNumber = Reader["QID"].ToString(),
                        });
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            finally
            {
                conn.Close();
            }
            return PBPPlanList;
        }

        public List<PBPPlanViewModel> ReadPBPPlanAreaTableData()
        {
            List<PBPPlanViewModel> PBPPlanList = new List<PBPPlanViewModel>();
            OleDbConnection conn;
            OleDbCommand cmd;
            OleDbDataReader Reader;
            conn = new OleDbConnection(OledbConnectingString);
            cmd = new OleDbCommand("SELECT DISTINCT (PBP_A_SQUISH_ID)" + " FROM " + "PLAN_AREAS", conn);
            try
            {
                conn.Open();
                Reader = cmd.ExecuteReader();
                if (Reader.HasRows)
                {
                    while (Reader.Read())
                    {
                        PBPPlanList.Add(new PBPPlanViewModel
                        {
                            QID = Reader["PBP_A_SQUISH_ID"].ToString(),
                        });
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            finally
            {
                conn.Close();
            }
            return PBPPlanList;
        }

        public bool IsTableExist(string tableName)
        {
            bool IsExist = false;
            OleDbConnection conn;
            try
            {
                conn = new OleDbConnection(OledbConnectingString);
                conn.Open();
                var schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                conn.Close();
                if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == tableName.ToLower()))
                {
                    IsExist = true;
                }
                else
                {
                    IsExist = false;
                }
            }
            catch (Exception ex)
            {
                IsExist = false;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return IsExist;
        }
        #endregion
    }

    public class SqlImportOperations
    {
        #region Private Members
        private string _connectionString;
        #endregion

        #region Constructor
        public SqlImportOperations(string connectingString)
        {
            _connectionString = connectingString;
        }
        #endregion

        #region Public Method
        public bool CreateTableStructure(string strCreateTableScript)
        {
            bool isSuccessful = false;
            if (!string.IsNullOrEmpty(strCreateTableScript))
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    try
                    {
                        con.Open();

                        using (SqlCommand command = new SqlCommand(strCreateTableScript, con))
                            command.ExecuteNonQuery();
                        isSuccessful = true;
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message);
                    }
                }
            }
            return isSuccessful;
        }

        public bool ImportDataToSqlServer(DataTable sourceDataTable)
        {
            bool isSuccessfull = false;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    //Set the database table name
                    sqlBulkCopy.DestinationTableName = "PBP.PBPDATAMAP";
                    List<string> colNameTemp = new List<string>();
                    foreach (DataColumn dcol in sourceDataTable.Columns)
                        sqlBulkCopy.ColumnMappings.Add(dcol.ColumnName, dcol.ColumnName);

                    con.Open();
                    sqlBulkCopy.WriteToServer(sourceDataTable);
                    con.Close();
                }
            }
            return isSuccessfull;
        }
        public bool ImportDataToSqlServer(DataTable sourceDataTable, string destinationTableName, List<PBPImportTableColumnsViewModel> PBPImportTableColumnsViewModel)
        {
            bool isSuccessfull = false;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    //Set the database table name
                    sqlBulkCopy.DestinationTableName = destinationTableName;
                    List<string> colNameTemp = new List<string>();
                    foreach (DataColumn dcol in sourceDataTable.Columns)
                    {
                        string destinationColName = dcol.ColumnName;
                        if (PBPImportTableColumnsViewModel != null)
                        {
                            PBPImportTableColumnsViewModel destiObject = PBPImportTableColumnsViewModel.FirstOrDefault(x => x.PBPImportTableColumnName.Equals(dcol.ColumnName) || x.PBPImportTableColumnName.Contains(dcol.ColumnName));
                            if (destiObject != null && destiObject.PBPImportTableColumnName != null)
                            {
                                colNameTemp.Add(destiObject.PBPImportTableColumnName);
                                destinationColName = destiObject.PBPImportTableColumnName;
                            }
                            else
                            {
                                destinationColName = destinationColName;
                            }

                        }
                        sqlBulkCopy.ColumnMappings.Add(dcol.ColumnName, dcol.ColumnName);
                    }
                    con.Open();
                    sqlBulkCopy.WriteToServer(sourceDataTable);
                    con.Close();
                }
            }
            return isSuccessfull;
        }

        #endregion

        #region Private Method
        #endregion
    }

    public class PBPImportActivityLogServices
    {
        #region Private Members
        IUnitOfWorkAsync _unitOfWorkAsync = null;
        #endregion Private Members

        #region Constructor
        public PBPImportActivityLogServices(IUnitOfWorkAsync unitOfWorkAsync)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
        }
        #endregion

        #region Public Methods
        public void AddPBPImportActivityLog(int PBPImportQueueID, string methodName, string Message, string TableName, string QID, Exception Ex)
        {
            try
            {
                string Exceptiostr = string.Empty;
                PBPImportActivityLogViewModel viewModel = new PBPImportActivityLogViewModel();
                viewModel.PBPImportQueueID = PBPImportQueueID;
                viewModel.Message = !string.IsNullOrEmpty(methodName) ? "Error in Methood " + methodName + "\n" : string.Empty;
                viewModel.Message += (!string.IsNullOrEmpty(Message) ? "Message : -" + Message : string.Empty);
                viewModel.Message += !string.IsNullOrEmpty(TableName) ? "\n Table Name: -" + TableName + "\n" : null;
                viewModel.Message += !string.IsNullOrEmpty(QID) ? "Exception for QID :-" + QID : string.Empty;
                viewModel.CreatedBy = "TMG Super User";

                try
                {
                    Exceptiostr = "\n" + "Excepion: - " + Ex != null ? Ex.Message : null + "\n"
                    + "Inner Exception : -" + Ex != null ? Ex.InnerException.Message : null;
                }
                catch { }

                PBPImportActivityLog activitylog = new PBPImportActivityLog();
                activitylog.PBPImportQueueID = viewModel.PBPImportQueueID;
                activitylog.FileName = viewModel.FileName;
                activitylog.TableName = viewModel.TableName;
                activitylog.Message = viewModel.Message + " " + Exceptiostr;
                activitylog.CreatedBy = viewModel.CreatedBy;
                activitylog.CreatedDate = DateTime.Now;
                this._unitOfWorkAsync.RepositoryAsync<PBPImportActivityLog>().Insert(activitylog);
                this._unitOfWorkAsync.Save();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
        }
        #endregion Public Methods
    }
}
