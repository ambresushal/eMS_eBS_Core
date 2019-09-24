using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using System.Data;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.repository.extensions;
using tmg.equinox.domain.entities.Enums;
using Newtonsoft.Json.Linq;
using tmg.equinox.domain.entities;
using System.Data.OleDb;
using tmg.equinox.schema.Base;
using System.IO;
using tmg.equinox.applicationservices.PBPImport;
//using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.applicationservices
{
    public class PBPExportService : IPBPExportServices
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private string UserName { get; set; }
        #endregion Private Members

        #region Constructor
        public PBPExportService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion

        public GridPagingResponse<PBPExportQueueViewModel> GetQueuedPBPExports(GridPagingRequest gridPagingRequest)
        {
            List<PBPExportQueueViewModel> PBPExportQueueList = null;
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                PBPExportQueueList = (from c in this._unitOfWork.RepositoryAsync<PBPExportQueue>().Get()
                                      select new PBPExportQueueViewModel
                                      {
                                          PBPExportQueueID = c.PBPExportQueueID,
                                          Description = c.Description,
                                          FileName = c.ExportName,
                                          ExportedDate = c.ExportedDate,
                                          ExportedBy = c.ExportedBy,
                                          Status = c.Status,
                                          PlanYear = c.PlanYear,
                                          PBPDatabase = c.PBPDataBase
                                      }).OrderByDescending(d => d.ExportedDate).ToList()
                                        .ApplySearchCriteria(criteria)
                                        .ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                        .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<PBPExportQueueViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, PBPExportQueueList);
        }

        public GridPagingResponse<PBPImportQueueViewModel> GetDatabaseNamesForPBPExports(GridPagingRequest gridPagingRequest)
        {
            List<PBPImportQueueViewModel> pbpDBs = null;
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);
                pbpDBs = (from c in this._unitOfWork.RepositoryAsync<PBPImportQueue>().Get()
                          join d in this._unitOfWork.RepositoryAsync<PBPDatabase>().Get()
                          on c.PBPDatabase1Up equals d.PBPDatabase1Up
                          //where c.Status == 4 //Completed
                          select new PBPImportQueueViewModel { PBPDataBase = d.DataBaseName, PBPDatabase1Up = d.PBPDatabase1Up }).Distinct().ToList()
                          .ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                          .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<PBPImportQueueViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, pbpDBs);
        }

        public IEnumerable<PBPDatabseDetailViewModel> GetDatabaseDetails(int PBPDatabase1Up)
        {
            List<PBPDatabseDetailViewModel> pbpDBDetails = new List<PBPDatabseDetailViewModel>();
            try
            {
                pbpDBDetails = (from c in this._unitOfWork.RepositoryAsync<PBPImportDetails>().Get()
                                join f in _unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                on c.FolderVersionId equals f.FolderVersionID
                                where c.PBPDatabase1Up == PBPDatabase1Up
                                select new PBPDatabseDetailViewModel
                                {
                                    PBPImportDetails1Up = c.PBPImportDetails1Up,
                                    PlanName = c.PlanName,
                                    PlanNumber = c.PlanNumber,
                                    FolderVersionNumber = f.FolderVersionNumber
                                }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return pbpDBDetails;
        }

        public void QueuePBPExport(string exportName, string description, string DBName, string userName, int PBPDatabase1Up)
        {
            PBPExportQueue model = new PBPExportQueue();
            try
            {
                string exportFilePath = System.Configuration.ConfigurationManager.AppSettings["PBPExportFiles"];
                string schemaPath = System.Configuration.ConfigurationManager.AppSettings["PBPExportMDBSchema"];
                model.Description = description;
                model.ExportName = exportName;
                model.ExportedBy = userName;
                model.ExportedDate = DateTime.Now;
                model.PlanYear = 0;
                model.Location = "";
                model.Status = 1; //Queued
                model.PBPDataBase = DBName;
                model.PBPFilePath = exportFilePath;
                model.PlanAreaFilePath = exportFilePath;
                model.VBIDFilePath = exportFilePath;
                model.ZipFilePath = exportFilePath;
                model.MDBSchemaPath = schemaPath;
                model.PBPDatabase1Up = PBPDatabase1Up;
                _unitOfWork.RepositoryAsync<PBPExportQueue>().Insert(model);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
        }

        public string GetZipFilePath(int PBPExportQueueID)
        {
            string filePath = string.Empty;
            try
            {
                filePath = (from e in this._unitOfWork.RepositoryAsync<PBPExportQueue>().Get()
                            where e.PBPExportQueueID == PBPExportQueueID
                            select e.ZipFilePath).FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return filePath ?? string.Empty;
        }

        public DataTable GetPBPExportDataTable()
        {
            DataTable dt = new DataTable();
            try
            {
                List<PBPExportQueueViewModel> PBPExportQueueList = (from c in this._unitOfWork.RepositoryAsync<PBPExportQueue>().Get()
                                                                    select new PBPExportQueueViewModel
                                                                    {
                                                                        PBPExportQueueID = c.PBPExportQueueID,
                                                                        Description = c.Description,
                                                                        FileName = c.ExportName,
                                                                        ExportedDate = c.ExportedDate,
                                                                        ExportedBy = c.ExportedBy,
                                                                        Status = c.Status,
                                                                        PlanYear = c.PlanYear,
                                                                        PBPDatabase = c.PBPDataBase
                                                                    }).OrderByDescending(d => d.ExportedDate).ToList();

                dt.Columns.Add("PBPExportQueueID");
                dt.Columns.Add("Description");
                dt.Columns.Add("Database");
                dt.Columns.Add("FileName");
                dt.Columns.Add("PlanYear");
                dt.Columns.Add("ExportedBy");
                dt.Columns.Add("ExportedDate");
                dt.Columns.Add("Status");

                foreach (PBPExportQueueViewModel model in PBPExportQueueList)
                {
                    DataRow row = dt.NewRow();
                    row["PBPExportQueueID"] = model.PBPExportQueueID;
                    row["Description"] = model.Description;
                    row["Database"] = model.PBPDatabase;
                    row["FileName"] = model.FileName;
                    row["PlanYear"] = model.PlanYear;
                    row["ExportedBy"] = model.ExportedBy;
                    row["ExportedDate"] = model.ExportedDate;
                    row["Status"] = ((ProcessStatusMasterCode)model.Status).ToString();
                    dt.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return dt;
        }

        public string GetJSONString(int formInstanceID)
        {
            FormInstanceDataMap formInstanceDataMap = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Get()
                                                      .Where(s => s.FormInstanceID == formInstanceID)
                                                      .Select(s => s).ToList().FirstOrDefault();
            if (formInstanceDataMap == null)
                return string.Empty;
            return formInstanceDataMap.FormData;
        }

        public List<FormInstanceViewModel> GetFormInstanceID_Name(int PBPDatabase1Up)
        {
            // Get latest folderVersionID for PBPDatabase1Up
            int folderVersionID = (from p in _unitOfWork.RepositoryAsync<PBPImportDetails>().Get()
                                   join f in _unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                   on p.FolderId equals f.FolderID
                                   where p.PBPDatabase1Up == PBPDatabase1Up
                                   select f.FolderVersionID).ToList().Max();

            List<FormInstanceViewModel> formInstancelist = (from f in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                                            where f.FormDesignID == (int)FormDesignID.PBPView && f.IsActive == true
                                                            && f.FolderVersionID == folderVersionID
                                                            select new FormInstanceViewModel { FormInstanceID = f.FormInstanceID, Name = f.Name }).ToList();
            return formInstancelist;
        }

        public IEnumerable<PBPExportToMDBMappingViewModel> GetExportMappings()
        {
            IEnumerable<PBPExportToMDBMappingViewModel> model = from m in this._unitOfWork.RepositoryAsync<PBPExportToMDBMapping>().Get()
                                                                where m.IsActive == true
                                                                select (new PBPExportToMDBMappingViewModel
                                                                {
                                                                    TableName = m.TableName,
                                                                    FieldName = m.FieldName,
                                                                    JsonPath = m.JsonPath,
                                                                    IsRepeater = m.IsRepeater,
                                                                    ElementName = m.ElementName,
                                                                    Length = m.Length
                                                                });

            return model;
        }

        public void GenerateMDBFile(int PBPExportQueueID, string userName)
        {
            bool isExceptionLogged = false;
            string tblName = string.Empty;
            string columnName = string.Empty;
            OleDbConnection connection = null;
            try
            {
                PBPExportQueueViewModel queueDetails = this.GetQueuedPBPExport(PBPExportQueueID);
                string folderPath = System.Configuration.ConfigurationManager.AppSettings["PBPExportFiles"] + "PBPExport_" + queueDetails.PBPExportQueueID;
                queueDetails.PBPFilePath = folderPath + @"\PBPExport_" + queueDetails.PBPExportQueueID + ".MDB";

                this.UpdatePBPFilePath(PBPExportQueueID, queueDetails.PBPFilePath, folderPath);
                System.IO.Directory.CreateDirectory(folderPath);
                File.Copy(queueDetails.MDBSchemaPath, queueDetails.PBPFilePath, true);

                //Directory dict = new Directory();

                List<FormInstanceViewModel> formInstanceLst = this.GetFormInstanceID_Name(queueDetails.PBPDatabase1Up);

                OleDbHelper oleDBHelperClass = new OleDbHelper();
                string connectingString = oleDBHelperClass.GetOleDbConnectingString(queueDetails.PBPFilePath);
                connection = new OleDbConnection(connectingString);
                //OleDbConnection connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + queueDetails.PBPFilePath + ";Persist Security Info=False;");
                connection.Open();
                var transaction = connection.BeginTransaction();

                try
                {
                    foreach (FormInstanceViewModel formInstance in formInstanceLst)
                    {
                        string json = this.GetJSONString(formInstance.FormInstanceID);
                        string QID = formInstance.Name;

                        Dictionary<string, object> dict = JsonHelper.DeserializeAndFlatten(json);
                        JObject source = JObject.Parse(json);

                        List<PBPExportToMDBMappingViewModel> mapping = this.GetExportMappings().ToList();
                        List<string> tables = mapping.Select(t => t.TableName).Distinct().ToList();

                        foreach (string tbl in tables.Where(t => !t.Contains("PBPMRX"))) //To Remove : Added Temporary condition for PBPMRX.
                        {
                            tblName = tbl;
                            string cmdText = "INSERT INTO " + tbl + "(";
                            string values = " VALUES (";
                            foreach (PBPExportToMDBMappingViewModel model in mapping.Where(t => t.TableName == tbl))
                            {
                                cmdText = cmdText + model.FieldName + ",";
                                values = values + "?,";
                            }
                            values = values.Remove(values.LastIndexOf(','), 1) + ")";
                            cmdText = cmdText.Remove(cmdText.LastIndexOf(','), 1) + ")" + values;

                            OleDbCommand cmd = new OleDbCommand(cmdText, connection);
                            cmd.Transaction = transaction;
                            cmd.Parameters.Clear();

                            foreach (PBPExportToMDBMappingViewModel model in mapping.Where(t => t.TableName == tbl && t.IsRepeater == false))
                            {
                                dynamic val;
                                columnName = model.FieldName;

                                if (model.FieldName == "QID")
                                    val = QID;
                                else
                                    val = dict.Where(t => t.Key == model.JsonPath).Select(t => t.Value).FirstOrDefault();
                                if (val != null)
                                {
                                    Type type = val.GetType();
                                    if (type.Equals(typeof(bool)))
                                        val = val == true ? "1" : "0";
                                }
                                if (val == string.Empty || val == null)
                                    val = 0;
                                if (val.ToString().Length > model.Length)
                                {
                                    string data = val.ToString();
                                    val = data.Substring(0, model.Length);
                                }
                                cmd.Parameters.AddWithValue(model.FieldName, val);
                            }
                            if (cmd.Parameters.Count > 0)
                                cmd.ExecuteNonQuery();

                            /**************************************************Populate REPEATER DATA**********************************************************/
                            List<string> repeaterList = (from m in mapping
                                                         where m.IsRepeater == true
                                                         && m.TableName == tbl
                                                         select m.JsonPath).Distinct().ToList();

                            bool isRowAddedToMDB = false;
                            foreach (string repeater in repeaterList)
                            {
                                //cmdText = "INSERT INTO " + tbl + "(";
                                //values = " VALUES (";
                                //foreach (PBPExportToMDBMappingViewModel model in mapping.Where(t => t.TableName == tbl && t.JsonPath == repeater))
                                //{
                                //    cmdText = cmdText + model.FieldName + ",";
                                //    values = values + "?,";
                                //}
                                //values = values.Remove(values.LastIndexOf(','), 1) + ")";
                                //cmdText = cmdText.Remove(cmdText.LastIndexOf(','), 1) + ")" + values;

                                //cmd = new OleDbCommand(cmdText, connection);
                                //cmd.Transaction = transaction;
                                //cmd.Parameters.Clear();

                                int rowCount = 0;
                                if (source.SelectToken(repeater) != null)
                                    rowCount = source.SelectToken(repeater).Count();

                                for (int row = 0; row < rowCount; row++)
                                {
                                    cmd.Parameters.Clear();
                                    foreach (PBPExportToMDBMappingViewModel model in mapping.Where(t => t.TableName == tbl && t.IsRepeater == true))
                                    {
                                        var repeaterRows = dict.Where(t => t.Key == model.JsonPath + "." + row + "." + model.ElementName).FirstOrDefault();
                                        cmd.Parameters.AddWithValue(model.FieldName, model.FieldName == "QID" ? QID : repeaterRows.Value);
                                    }
                                    //if (!isRowAddedToMDB)
                                    //{
                                    //    cmd.ExecuteNonQuery();
                                    //    isRowAddedToMDB = true;
                                    //}
                                }
                            }
                            /************************************************************************************************************/
                        }
                    }
                    transaction.Commit();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    connection.Close();
                    this.AddPBPExportActivityLog(PBPExportQueueID, tblName, columnName, userName, ex);
                    isExceptionLogged = true;
                    bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                    if (reThrow)
                        throw;
                }
            }
            catch (Exception ex)
            {
                if (!isExceptionLogged)
                    this.AddPBPExportActivityLog(PBPExportQueueID, string.Empty, string.Empty, userName, ex);
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }

        public void AddPBPExportActivityLog(int PBPExportQueueID, string tableName, string columnName, string userName, Exception ex)
        {
            string errorForTable = tableName.Length == 0 ? tableName : " ,tableName : " + tableName;
            string errorForColumn = columnName.Length == 0 ? columnName : " ,column : " + columnName;

            PBPExportActivityLog activitylog = new PBPExportActivityLog();
            activitylog.PBPEmportQueueID = PBPExportQueueID;
            activitylog.TableName = tableName;
            activitylog.Message = String.Concat("Error ocuurred for PBPExportQueueID: " + PBPExportQueueID + errorForTable + errorForColumn + " " + ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);
            activitylog.CreatedBy = userName;
            activitylog.CreatedDate = DateTime.Now;
            this._unitOfWork.RepositoryAsync<PBPExportActivityLog>().Insert(activitylog);
            this._unitOfWork.Save();
        }


        public PBPExportQueueViewModel GetQueuedPBPExport(int PBPExportQueueID)
        {

            PBPExportQueueViewModel result = (from c in _unitOfWork.RepositoryAsync<PBPExportQueue>().Get()
                                              where (c.PBPExportQueueID == PBPExportQueueID)
                                              select new PBPExportQueueViewModel
                                              {
                                                  PBPExportQueueID = c.PBPExportQueueID,
                                                  Description = c.Description,
                                                  FileName = c.ExportName,
                                                  ExportedDate = c.ExportedDate,
                                                  ExportedBy = c.ExportedBy,
                                                  Status = c.Status,
                                                  PlanYear = c.PlanYear,
                                                  PBPDatabase = c.PBPDataBase,
                                                  PBPDatabase1Up = c.PBPDatabase1Up,
                                                  MDBSchemaPath = c.MDBSchemaPath,
                                                  PBPFilePath = c.PBPFilePath
                                              }).ToList().FirstOrDefault();
            return result;
        }

        public void UpdatePBPFilePath(int PBPExportQueueID, string PBPFilePath, string ZipFilePath)
        {
            PBPExportQueue queueDetails = _unitOfWork.RepositoryAsync<PBPExportQueue>().Get()
                                           .Where(t => t.PBPExportQueueID == PBPExportQueueID).FirstOrDefault();

            queueDetails.PBPFilePath = PBPFilePath;
            queueDetails.ZipFilePath = ZipFilePath; //filePath.Replace(".MDB", ".ZIP");
            _unitOfWork.RepositoryAsync<PBPExportQueue>().Update(queueDetails);
            _unitOfWork.Save();
        }
    }
}
