﻿using System;
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
using Newtonsoft.Json;
using tmg.equinox.domain.entities;
using System.Data.OleDb;
using tmg.equinox.schema.Base;
using System.IO;
using tmg.equinox.applicationservices.PBPImport;
using tmg.equinox.queueprocess.PBPExport;
using tmg.equinox.backgroundjob;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.pbpexport;
using System.Transactions;
//using tmg.equinox.ruleprocessor.formdesignmanager;
//using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
//using tmg.equinox.ruleprocessor;
//using tmg.equinox.expressionbuilder;
using System.Collections;
using System.Configuration;
using tmg.equinox.applicationservices.viewmodels.exitvalidate;
using System.Threading;
using tmg.equinox.notification;
//using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.applicationservices
{
    public class PBPExportService : IPBPExportServices
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private IFormDesignService _formDesignService;
        private IFolderVersionServices _folderVersionService;
        private IFormInstanceService _formInstanceService;
        private IFormInstanceDataServices _formInstanceDataService;
        private IMasterListService _masterListService;
        private IUIElementService _uiElementService;
        private IExitValidateService _exitValidateService;
        static readonly object _lockExportobject = new object();
        private string UserName { get; set; }
        IBackgroundJobManager _hangFireJobManager;
        
        private static readonly ILog _logger = LogProvider.For<PBPImportService>();
        private IExportPreQueueService _iExportPreQueueService;
        string[] TableQIDList = { "PBPB20", "PBPC", "PBPC_OON", "PBPC_POS", "PBPD_OON", "PBPD_OPT", "STEP10B", "STEP17B","STEP7B","STEP7F","STEP17A","STEP18A","STEP18B", "STEP16B", "STEP16A" };
        #endregion Private Members

        #region Constructor
        public PBPExportService(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PBPExportService(IUnitOfWorkAsync unitOfWork, IBackgroundJobManager hangFireJobManager, IUIElementService uiElementService, IFormDesignService formDesignService, IFolderVersionServices folderVersionService, IFormInstanceService formInstanceService, IFormInstanceDataServices formInstanceDataService, IMasterListService masterListService, IExportPreQueueService exportPreQueueService, IExitValidateService exitValidateService)
        {
            this._unitOfWork = unitOfWork;
            this._hangFireJobManager = hangFireJobManager;
            _uiElementService = uiElementService;
            _formDesignService = formDesignService;
            _folderVersionService = folderVersionService;
            _formInstanceService = formInstanceService;
            _formInstanceDataService = formInstanceDataService;
            _masterListService = masterListService;
            _iExportPreQueueService = exportPreQueueService;
            _exitValidateService = exitValidateService;
            
        }

        public void InitializeVariables(IUnitOfWorkAsync unitOfWork)
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
                                      join status in this._unitOfWork.RepositoryAsync<ProcessStatusMaster>().Get()
                                      on c.Status equals status.ProcessStatus1Up
                                      select new PBPExportQueueViewModel
                                      {
                                          PBPExportQueueID = c.PBPExportQueueID,
                                          Description = c.Description,
                                          FileName = c.ExportName,
                                          ExportedDate = c.ExportedDate,
                                          ExportedBy = c.ExportedBy,
                                          Status = c.Status,
                                          StatusText = status.ProcessStatusName == "Errored" ? "Failed" : status.ProcessStatusName,
                                          PlanYear = c.PlanYear == 0 ? System.DateTime.Now.Year : c.PlanYear,
                                          PBPDatabase = c.PBPDataBase,
                                      }).OrderByDescending(d => d.ExportedDate).ToList()
                                        .ApplySearchCriteria(criteria)
                                        .ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                        .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);

                foreach (PBPExportQueueViewModel model in PBPExportQueueList.Where(t => t.Status == 3)) // Get Error log
                    model.ErrorMessage = this.GetExportErrorLog(model.PBPExportQueueID);
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
                          where c.Status == 4 //Completed
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

        public GridPagingResponse<PBPDatabseDetailViewModel> GetDatabaseDetails(int PBPDatabase1Up, GridPagingRequest gridPagingRequest)
        {
            List<PBPDatabseDetailViewModel> pbpDBDetails = null;
            int count = 0;
            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);
                List<int> folderVersionIDs = (from p in _unitOfWork.RepositoryAsync<PBPImportDetails>().Get()
                                              where p.PBPDatabase1Up == PBPDatabase1Up
                                              & p.IsActive == true
                                              group p by p.FolderId into g
                                              select g.Select(m => m.FolderVersionId).Max()).ToList();

                pbpDBDetails = (from c in this._unitOfWork.RepositoryAsync<PBPImportDetails>().Get()
                                join f in _unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                on c.FolderId equals f.FolderID
                                where c.PBPDatabase1Up == PBPDatabase1Up
                                && c.IsActive == true
                                && folderVersionIDs.Contains(f.FolderVersionID)
                                select new PBPDatabseDetailViewModel
                                {
                                    PlanName = c.PlanName,
                                    PlanNumber = c.PlanNumber,
                                    FolderVersionNumber = f.FolderVersionNumber
                                }).Distinct().ToList()
                           .ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                          .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<PBPDatabseDetailViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, pbpDBDetails);
        }


        public ServiceResult QueuePBPExport(string exportName, string description, string DBName, string userName, int PBPDatabase1Up, bool scheduleForProcessing, int? currentUserId, string runRuleInWindowsService)
        {
            ServiceResult result = null;
            PBPExportQueue model = new PBPExportQueue();
            try
            {
                string exportFilePath = System.Configuration.ConfigurationManager.AppSettings["PBPExportFiles"];
                string schemaPath = System.Configuration.ConfigurationManager.AppSettings["PBPExportMDBSchema"];
                model.Description = description;
                model.ExportName = exportName;
                model.ExportedBy = userName;
                model.ExportedDate = DateTime.Now;
                //model.PlanYear = 0;
                model.Location = "";
                model.Status = 1; //Queued
                model.PBPDataBase = DBName;
                model.PBPFilePath = exportFilePath;
                model.PlanAreaFilePath = exportFilePath;
                model.VBIDFilePath = exportFilePath;
                model.ZipFilePath = exportFilePath;
                model.MDBSchemaPath = schemaPath;
                model.PBPDatabase1Up = PBPDatabase1Up;
                var PlanYear = this._unitOfWork.RepositoryAsync<PBPImportDetails>().Get()
                           .Where(s => s.PBPDatabase1Up.Equals(model.PBPDatabase1Up)
                           && s.Year > 0).OrderByDescending(s => s.PBPImportDetails1Up)
                           .FirstOrDefault();
                if (PlanYear != null)
                {
                    model.PlanYear = PlanYear.Year;
                    schemaPath = schemaPath.Replace("2018", PlanYear.Year.ToString());
                    model.MDBSchemaPath = schemaPath;
                }
                else
                {
                    model.PlanYear = 0;
                }
                _unitOfWork.RepositoryAsync<PBPExportQueue>().Insert(model);
                _unitOfWork.Save();
                if (scheduleForProcessing == false && runRuleInWindowsService.Equals("Yes"))
                {
                    _iExportPreQueueService.PreQueueExport(model.PBPExportQueueID, model.PBPDatabase1Up, userName, currentUserId);
                }
                if (scheduleForProcessing == true)
                {
                    PBPExportEnqueue PBPExportEnqueue = new PBPExportEnqueue(_hangFireJobManager);
                    PBPExportEnqueue.Enqueue(new PBPExportQueueInfo { QueueId = model.PBPExportQueueID, UserId = currentUserId.Value.ToString(), FeatureId = model.PBPExportQueueID.ToString(), UserName = userName, RunExportRulesInWindowsService = runRuleInWindowsService, pbpDatabase1Up = PBPDatabase1Up, Name = "PBP Export for PBPExportQueueID: " + model.PBPExportQueueID.ToString(), AssemblyName = "tmg.equinox.applicationservices", ClassName = "PBPExportCustomQueue" });
                }
                result = new ServiceResult();
                result.Result = ServiceResultStatus.Success;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem()
                {
                    Messages = new string[] { model.PBPExportQueueID.ToString() }
                });
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        public void ScheduleForPBPExportQueue(int pbpExportQueueID, string currentUserName)
        {
            try
            {
                var rec = from expQueue in _unitOfWork.RepositoryAsync<PBPExportQueue>().Get()
                          where expQueue.PBPExportQueueID == pbpExportQueueID && expQueue.Status == 1
                          select expQueue;
                if (rec != null && rec.Count() > 0)
                {
                    PBPExportEnqueue PBPExportEnqueue = new PBPExportEnqueue(_hangFireJobManager);
                    PBPExportEnqueue.Enqueue(new PBPExportQueueInfo { QueueId = pbpExportQueueID, UserId = currentUserName, FeatureId = pbpExportQueueID.ToString(), Name = "PBP Export for PBPExportQueueID: " + pbpExportQueueID.ToString(), AssemblyName = "tmg.equinox.applicationservices", ClassName = "PBPExportCustomQueue" });
                }
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
                                                                    join status in this._unitOfWork.RepositoryAsync<ProcessStatusMaster>().Get()
                                                                    on c.Status equals status.ProcessStatus1Up
                                                                    select new PBPExportQueueViewModel
                                                                    {
                                                                        PBPExportQueueID = c.PBPExportQueueID,
                                                                        Description = c.Description,
                                                                        FileName = c.ExportName,
                                                                        ExportedDate = c.ExportedDate,
                                                                        ExportedBy = c.ExportedBy,
                                                                        StatusText = status.ProcessStatusName == "Errored" ? "Failed" : status.ProcessStatusName,
                                                                        PlanYear = c.PlanYear,
                                                                        PBPDatabase = c.PBPDataBase
                                                                    }).OrderByDescending(d => d.ExportedDate).ToList();

                dt.Columns.Add("Export Id");
                dt.Columns.Add("Export Name");
                dt.Columns.Add("Description");
                dt.Columns.Add("Database Name");
                dt.Columns.Add("Plan Year");
                dt.Columns.Add("Exported Date & Time");
                dt.Columns.Add("Exported By");
                dt.Columns.Add("Export Status");

                foreach (PBPExportQueueViewModel model in PBPExportQueueList)
                {
                    DataRow row = dt.NewRow();
                    row["Export Id"] = model.PBPExportQueueID;
                    row["Export Name"] = model.FileName;
                    row["Description"] = model.Description;
                    row["Database Name"] = model.PBPDatabase;
                    row["Plan Year"] = model.PlanYear;
                    row["Exported By"] = model.ExportedBy;
                    row["Exported Date & Time"] = model.ExportedDate;
                    row["Export Status"] = model.StatusText;
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

        public List<FormInstanceViewModel> GetFormInstanceID_Name(int PBPDatabase1Up, int formDesignID)
        {
            // Get latest folderVersionID for PBPDatabase1Up
            List<int> folderVersionIDs = (from p in _unitOfWork.RepositoryAsync<PBPImportDetails>().Get()
                                          where p.PBPDatabase1Up == PBPDatabase1Up
                                          group p by p.FolderId into g
                                          select g.Select(m => m.FolderVersionId).Max()).ToList();

            List<int> FormInstanceIDList = (from p in _unitOfWork.RepositoryAsync<PBPImportDetails>().Get()
                                            where p.PBPDatabase1Up == PBPDatabase1Up
                                            && folderVersionIDs.Contains(p.FolderVersionId)
                                            && p.IsActive == true
                                            && p.IsEGWPPlan == false
                                            select p.FormInstanceID).Distinct().ToList();

            List<FormInstanceViewModel> formInstanceList = (from f in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                                            join p in _unitOfWork.RepositoryAsync<PBPImportDetails>().Get()
                                                            on (int)f.AnchorDocumentID equals p.FormInstanceID
                                                            where FormInstanceIDList.Contains(f.AnchorDocumentID ?? 0)
                                                            && f.AnchorDocumentID != f.FormInstanceID
                                                            && p.IsActive == true
                                                            && f.IsActive == true
                                                            && p.IsEGWPPlan == false
                                                            && folderVersionIDs.Contains(f.FolderVersionID)
                                                            && f.FormDesignID.Equals(formDesignID)
                                                            select new FormInstanceViewModel
                                                            {
                                                                FormInstanceID = f.FormInstanceID,
                                                                Name = p.QID,
                                                                FormDesignVersionID = f.FormDesignVersionID,
                                                                FolderVersionId = f.FolderVersionID,
                                                                DocId = f.AnchorDocumentID ?? 0
                                                            }).Distinct().ToList();

            if (formInstanceList == null) return new List<FormInstanceViewModel>();
            return formInstanceList;
        }

        public List<FormInstanceViewModel> GetFormInstanceID_Name(int PBPDatabase1Up)
        {
            // Get latest folderVersionID for PBPDatabase1Up
            List<int> folderVersionIDs = (from p in _unitOfWork.RepositoryAsync<PBPImportDetails>().Get()
                                          where p.PBPDatabase1Up == PBPDatabase1Up
                                          group p by p.FolderId into g
                                          select g.Select(m => m.FolderVersionId).Max()).ToList();

            List<int> FormInstanceIDList = (from p in _unitOfWork.RepositoryAsync<PBPImportDetails>().Get()
                                            where p.PBPDatabase1Up == PBPDatabase1Up
                                            && folderVersionIDs.Contains(p.FolderVersionId)
                                            && p.IsActive == true
                                            select p.FormInstanceID).Distinct().ToList();

            List<FormInstanceViewModel> formInstanceList = (from f in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                                            join p in _unitOfWork.RepositoryAsync<PBPImportDetails>().Get()
                                                            on (int)f.FormInstanceID equals p.FormInstanceID
                                                            where FormInstanceIDList.Contains(f.AnchorDocumentID ?? 0)
                                                            && f.AnchorDocumentID == f.FormInstanceID
                                                            && p.IsActive == true
                                                            && f.IsActive == true
                                                            && folderVersionIDs.Contains(f.FolderVersionID)
                                                            select new FormInstanceViewModel
                                                            {
                                                                FormInstanceID = f.FormInstanceID,
                                                                Name = p.QID,
                                                                FormDesignVersionID = f.FormDesignVersionID,
                                                                FolderVersionId = f.FolderVersionID,
                                                                DocId = f.AnchorDocumentID ?? 0
                                                            }).Distinct().ToList();

            if (formInstanceList == null) return new List<FormInstanceViewModel>();
            return formInstanceList;
        }

        public IEnumerable<PBPExportToMDBMappingViewModel> GetExportMappings(int year)
        {
            IEnumerable<PBPExportToMDBMappingViewModel> model = from m in this._unitOfWork.RepositoryAsync<PBPExportToMDBMapping>().Get()
                                                                where m.IsActive == true
                                                                && m.Year == year
                                                                select (new PBPExportToMDBMappingViewModel
                                                                {
                                                                    TableName = m.TableName,
                                                                    FieldName = m.FieldName,
                                                                    JsonPath = m.JsonPath,
                                                                    IsRepeater = m.IsRepeater,
                                                                    Length = m.Length,
                                                                    MappingType = m.MappingType,
                                                                    IsActive = m.IsActive,
                                                                    IsBlankAllow = m.IsBlankAllow,
                                                                    DefaultValue = m.DefaultValue,
                                                                    IsCustomRule = m.IsCustomRule
                                                                });

            return model;
        }

        /// <summary>
        /// Method is called by windows service to run in single thread
        /// </summary>
        public void GenerateMDBFile()
        {
            try
            {
                do
                {
                    PBPExportQueueViewModel queue = GetInprogressQueuedPBPExport();
                    if (queue != null)
                    {
                        try
                        {
                            GenerateMDBFile(queue.PBPExportQueueID, queue.UserId);
                        }
                        catch(Exception ex)
                        {
                            _logger.ErrorException("GenerateMDBFile() queueId " + queue.PBPExportQueueID.ToString(), ex);
                        }
                    }

                    Thread.Sleep(50000); // every 50 sec
                } while (1 == 1);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("GenerateMDBFile() Please Restart Hangfire Export Service", ex);
                throw ex;
            }


        }
        private PBPExportQueueViewModel GetInprogressQueuedPBPExport()
        {

            var result = (from c in _unitOfWork.RepositoryAsync<PBPExportQueue>().Get()
                          where (c.Status == (int)domain.entities.Enums.ProcessStatusMasterCode.InProgress)
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
                              PBPFilePath = c.PBPFilePath,
                              UserId = c.ExportedBy
                          }).OrderBy(m => m.PBPExportQueueID).ToList().FirstOrDefault();
            return result;
        }
        public void GenerateMDBFile(int PBPExportQueueID, string userName)
        {
            bool isExceptionLogged = false;
            string tblName = string.Empty;
            string tblName1 = string.Empty;
            string FieldName = string.Empty;
            //OleDbConnection connection = null;
            ServiceResult result;
            string PBPFile = "PBP{YEAR}.MDB";
            string PLANAreaFile = "PBPPLANS{YEAR}.MDB";
            string VBIDFile = "VBID_PBP{YEAR}.MDB";
            PBPSoftwareVersionVeiwModel ViewModel = null;
            try
            {
                //  result = this.UpdateExportQueueStatus(PBPExportQueueID, ProcessStatusMasterCode.InProgress);
                PBPExportQueueViewModel queueDetails = this.GetQueuedPBPExport(PBPExportQueueID);
                string pbpPlanAreaFilePath = System.Configuration.ConfigurationManager.AppSettings["PBPImportFiles"] + GetPBPPlanAreaFileName(queueDetails.PBPDatabase1Up);
                string folderPath = System.Configuration.ConfigurationManager.AppSettings["PBPExportFiles"] + "PBPExport_" + queueDetails.PBPExportQueueID + @"\";
                string stagingFolderPath = System.Configuration.ConfigurationManager.AppSettings["PBPExportFiles"] + @"Staging\";

                string year = queueDetails.PlanYear.ToString();
                PBPFile = PBPFile.Replace("{YEAR}", year);
                PLANAreaFile = PLANAreaFile.Replace("{YEAR}", year);
                VBIDFile = VBIDFile.Replace("{YEAR}", year);
                queueDetails.PBPFilePath = stagingFolderPath + PBPFile;
                queueDetails.VBIDFilePath = stagingFolderPath + VBIDFile;
                queueDetails.PlanAreaFilePath = stagingFolderPath + PLANAreaFile;

                string finalPBPFilePath = folderPath + PBPFile;
                string finalVBIDFilePath = folderPath + VBIDFile;
                string finalPlanAreaFilePath = folderPath + PLANAreaFile;

                bool IsLicenseVersion = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsLicenseVersion"].ToString());
                this.AddPBPExportActivityLog(PBPExportQueueID, tblName, IsLicenseVersion.ToString() + queueDetails.PlanYear, userName, null);
                ViewModel = this.GetPBPSoftwareVersion(IsLicenseVersion, queueDetails.PlanYear);

                if (Directory.Exists(stagingFolderPath))
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(stagingFolderPath);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                }
                else
                {
                    System.IO.Directory.CreateDirectory(stagingFolderPath);
                }

                File.Copy(queueDetails.MDBSchemaPath, queueDetails.PBPFilePath, true);
                File.Copy(queueDetails.MDBSchemaPath.Replace(PBPFile, VBIDFile), queueDetails.VBIDFilePath, true);
                if (File.Exists(pbpPlanAreaFilePath))
                    File.Copy(pbpPlanAreaFilePath, queueDetails.PlanAreaFilePath, true);

                List<FormInstanceViewModel> formInstanceLst = this.GetFormInstanceID_Name(queueDetails.PBPDatabase1Up, (int)FormDesignID.PBPView);


                List<PBPExportToMDBMappingViewModel> mapping = this.GetExportMappings(int.Parse(year)).ToList();

                OleDbHelper oleDBHelperClass = new OleDbHelper();
                string connectingString = oleDBHelperClass.GetOleDbConnectingString(queueDetails.PBPFilePath);
                //connection = new OleDbConnection(connectingString);
                //connection.Open();
                FormInstanceViewModel formInstanceData = new FormInstanceViewModel();
                try
                {
                    int totalInstance = formInstanceLst.Count();
                    foreach (FormInstanceViewModel formInstance in formInstanceLst)
                    {
                        formInstanceData = formInstance;
                        this.AddPBPExportActivityLog(PBPExportQueueID, "", "Total :" + totalInstance.ToString() + " FormInstanceID - " + formInstanceData.FormInstanceID + " & Name- " + formInstanceData.Name + " Db:" + queueDetails.PBPDatabase1Up.ToString(), userName, null);

                        string json = this.GetJSONString(formInstance.FormInstanceID);
                        string QID = formInstance.Name;

                        JObject source = JObject.Parse(json);
                        if (source["POSGroups"]["POSGroupsBase1"] != null && source["POSGroups"]["POSGroupsBase1"] is JArray && ((JArray)source["POSGroups"]["POSGroupsBase1"]).Count() > 0)
                        {
                            List<JToken> va = (source.SelectToken("POSGroups.POSGroupsBase1")).OrderBy(S => S["POSGroupID"]).ToList();
                            source.SelectToken("POSGroups.POSGroupsBase1").Replace(JToken.Parse(JsonConvert.SerializeObject(va)));
                        }
                        if (source["POSGroups"]["POSGroupsBase2"] != null && source["POSGroups"]["POSGroupsBase2"] is JArray && ((JArray)source["POSGroups"]["POSGroupsBase2"]).Count() > 0)
                        {
                            List<JToken> val1 = (source.SelectToken("POSGroups.POSGroupsBase2")).OrderBy(S => S["POSGroupID"]).ToList();
                            source.SelectToken("POSGroups.POSGroupsBase2").Replace(JToken.Parse(JsonConvert.SerializeObject(val1)));
                        }
                        Dictionary<string, object> dict = JsonHelper.DeserializeAndFlatten(source.ToString());


                        List<string> MDBtables = mapping.Select(t => t.TableName).Distinct().ToList();
                        List<string> tables1 = MDBtables;
                        //CheckSectionVisibility(source, ref tables1);

                        foreach (string tbl in tables1)
                        {
                            tblName = tbl;
                            
                            if(tblName.ToLower().Contains("_base"))
                            {
                                tblName1 = tbl;
                                tblName = GetTableName(tblName);
                            }

                            string cmdText = "INSERT INTO " + tblName + "(";
                            string values = " VALUES (";


                            Dictionary<string, string> commandParam = new Dictionary<string, string>();
                            List<PBPExportToMDBMappingViewModel> tableMapping = (from map in mapping
                                                                                 where map.TableName == tbl
                                                                                 && map.IsRepeater == false
                                                                                 orderby map.FieldName
                                                                                 select map).ToList();

                            foreach (PBPExportToMDBMappingViewModel model in tableMapping)
                            {
                                var val = "";
                                FieldName = model.FieldName;
                                if (model.FieldName == "QID")
                                    val = QID;
                                else if (model.FieldName == "PBP_D_OPT_OON_TYPE_ID")
                                {
                                    if(model.JsonPath.Contains("OONOptional"))
                                    {
                                        val = "1";
                                    }
                                    else if (model.JsonPath.Contains("OONStepup"))
                                        val = "2";
                                }
                                else if (FieldName.Equals("PBP_D_OPT_OON_CAT_ID"))
                                {
                                    val = getValueForCatID(model.JsonPath);
                                }
                                else
                                {
                                    var Value = dict.Where(t => t.Key == model.JsonPath).Select(t => t.Value).FirstOrDefault();
                                    
                                    if (Value != null && !string.IsNullOrEmpty(Value.ToString()) && Value.ToString() == "NULL")
                                        Value = null;

                                    if (Value != null)
                                    {
                                        if (!string.IsNullOrEmpty(Value.ToString()))
                                        {
                                            val = RemoveNotApplicable(model, Value.ToString());
                                            val = ValueFormatter(model, val);
                                        }
                                        //if (string.IsNullOrEmpty(val) && !string.IsNullOrEmpty(model.DefaultValue))
                                        //{
                                        //    val = SetDefaultValue(model, val);
                                        //}
                                        else
                                        {
                                            val = Value.ToString();
                                        }
                                    }

                                }
                                if (model.MappingType == "MULTISELECT")
                                    val = ConvertMultiSelectValue(dict, model, false, null);
                                if (model.MappingType == "MULTIVALUECSV")
                                    val = ConvertMultiSelectCSVValue(dict, model, false, string.Empty);

                                if (val != null)
                                {

                                    if (val.Equals("true") || val.Equals("True"))
                                    {
                                        val = "1";
                                    }
                                    else if (val.Equals("false") || val.Equals("False"))
                                    {
                                        val = "0";
                                    }
                                }
                                if (model.IsCustomRule)
                                {
                                    val = ApplySort(val);
                                }

                                /*******************************************Special Case for PLAN_TYPE & ORG_TYPE***********************************************/
                                if (tbl == "PBP")
                                {
                                    if (model.FieldName == "PBP_A_PLAN_TYPE" || model.FieldName == "PBP_A_ORG_TYPE")
                                    {
                                        if (val.ToString().Length == 1)
                                            val = "0" + val;
                                    }
                                }

                                if (val.ToString().Length > model.Length)
                                {
                                    string data = val.ToString();
                                    val = data.Substring(0, model.Length);
                                }
                                if (val != string.Empty && val != null)
                                {
                                    if (!commandParam.ContainsKey(model.FieldName))
                                    {
                                        commandParam.Add(model.FieldName, val);
                                    }
                                }
                                //cmd.Parameters.AddWithValue(model.FieldName, val);
                            }

                            //   OleDbCommand cmd = new OleDbCommand();
                            //   cmd.Parameters.Clear();
                            var cmdTextCOlumn = "";

                            foreach (string key in commandParam.Keys)
                            {
                                cmdText = cmdText + key + ",";
                                values = values + "?,";
                                //    cmd.Parameters.AddWithValue(key, commandParam[key]);
                            }
                            if (commandParam.Count() > 0)
                            {
                                if (commandParam.Count() == 1 && commandParam.Keys.First() == "QID" && TableQIDList.Contains(tblName))
                                {
                                }
                                else if (tblName == "PBPD_OON" && (commandParam.Count() <= 3 || !commandParam.ContainsKey("QID") || !commandParam.ContainsKey("PBP_D_OPT_OON_IDENTIFIER") || !commandParam.ContainsKey("PBP_D_OPT_OON_TYPE_ID") || !commandParam.ContainsKey("PBP_D_OPT_OON_CAT_ID")))
                                {
                                }
                                else if ((tblName.Contains("STEP") || tblName == "PBPD_OPT") && (!commandParam.ContainsKey("QID") || !commandParam.ContainsKey("PBP_D_OPT_IDENTIFIER")))
                                {
                                }
                                else if ((tblName.Contains("STEP") && commandParam.Count() == 2 && commandParam.ContainsKey("QID")  && commandParam.ContainsKey("PBP_D_OPT_IDENTIFIER")))
                                {
                                }
                                else
                                {
                                    values = values.Remove(values.LastIndexOf(','), 1) + ")";
                                    cmdText = cmdText.Remove(cmdText.LastIndexOf(','), 1) + ")" + values;



                                    using (OleDbConnection connection = new OleDbConnection(connectingString))
                                    {
                                        using (OleDbCommand cmdNew = new OleDbCommand())
                                        {
                                            foreach (string key in commandParam.Keys)
                                            {
                                                //cmdText = cmdText + key + ",";
                                                //values = values + "?,";
                                                cmdNew.Parameters.AddWithValue(key, commandParam[key]);
                                            }

                                            connection.Open();
                                            cmdNew.CommandText = cmdText;
                                            cmdNew.Connection = connection;
                                            cmdNew.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                List<string> columns = (from map in mapping
                                                        where map.TableName == tbl
                                                        orderby map.FieldName
                                                        select map.FieldName).ToList();
                                foreach (string column in columns)
                                {
                                    if (!cmdText.Contains(column + ","))
                                    {
                                        cmdText = cmdText + column + ",";
                                        values = values + "?,";
                                    }
                                }
                                values = values.Remove(values.LastIndexOf(','), 1) + ")";
                                cmdText = cmdText.Remove(cmdText.LastIndexOf(','), 1) + ")" + values;
                                cmdTextCOlumn = cmdText;

                                // cmd.CommandText = cmdText;
                                //  cmd.Connection = connection;
                            }
                            /**************************************************Populate REPEATER DATA**********************************************************/

                            if (IsTableNeededToIncludeInExport(source, tbl, QID, PBPExportQueueID).Equals(true))
                            {
                                List<string> repeaterList = (from m in mapping
                                                             where m.IsRepeater == true
                                                             && m.TableName == tbl
                                                             select m.JsonPath).Distinct().ToList();
                                string[] repeaters = null;
                                List<string> lstRepeaters = new List<string>();
                                foreach (string repeater in repeaterList)
                                {
                                    repeaters = repeater.Split('[').ToArray();
                                    if (repeaters[0].Length > 0)
                                        lstRepeaters.Add(repeaters[0]);
                                }

                                DataTable dTable = new DataTable();

                                if (lstRepeaters.Count() > 0)
                                {
                                    lstRepeaters = lstRepeaters.Distinct().OrderBy(t => t).ToList();
                                    dTable = this.GetDataTableSchema(tbl, lstRepeaters, mapping);
                                }

                                int gapcnt = 0;
                                int iclcnt = 0;
                                int cnt1 = 0;
                                int rowcnt = 0;
                                foreach (string repeaterName in lstRepeaters)
                                {
                                    //i = 1;
                                    string columnVal = string.Empty;
                                    int rowCount = 0;
                                    if (source.SelectToken(repeaterName) != null)
                                        rowCount = source.SelectToken(repeaterName).Count();

                                    int cnt = 0;
                                    if (repeaterName.StartsWith("GapCoverage."))
                                    {
                                        cnt1 = iclcnt;
                                        if (gapcnt <= rowCount)
                                            gapcnt = rowCount;
                                        rowcnt = rowCount + iclcnt;
                                    }
                                    else if (repeaterName.StartsWith("PreICL.") || repeaterName.StartsWith("MedicareMedicaidPreICL.") || repeaterName.StartsWith("PostOOPThreshold."))
                                    {
                                        cnt1 = gapcnt;
                                        if (iclcnt <= rowCount)
                                            iclcnt = rowCount;
                                        rowcnt = rowCount + gapcnt;
                                    }

                                    if (repeaterName.Contains("PreICL.") || repeaterName.Contains("GapCoverage.")
                                        || repeaterName.Contains("MedicareMedicaidPreICL.") || repeaterName.StartsWith("PostOOPThreshold."))
                                    {
                                        cnt = cnt1;
                                        rowCount = rowcnt;
                                    }
                                    else
                                    {
                                        cnt1 = 0;
                                        rowcnt = 0;
                                    }

                                    List<PBPExportToMDBMappingViewModel> repeaterColumns = mapping.Where(t => t.TableName == tbl && t.IsRepeater == true && t.JsonPath.Contains(repeaterName)).OrderBy(t => t.FieldName).ToList();
                                    int Index = 0;
                                    for (int row = cnt; row < rowCount; row++)
                                    {
                                        DataRow DTrow = null;
                                        foreach (PBPExportToMDBMappingViewModel model in repeaterColumns)
                                        {
                                            columnVal = string.Empty;
                                            FieldName = model.FieldName;
                                            if (model.TableName.Equals("PBPMRX_T") && QID.Equals("H4506003000"))
                                            {

                                            }

                                            string repeaterElemetPath = model.JsonPath.Replace("[0]", "." + (cnt1 == 0 || (gapcnt != 0 && iclcnt != 0) == false ? row : Index).ToString()); // replace [0] with actual row Number
                                            dynamic repeaterRow = dict.Where(t => t.Key == repeaterElemetPath).FirstOrDefault();

                                            if (model.MappingType == "MULTISELECT")
                                            {
                                                string columnData = string.Empty;
                                                var selectedOptions = dict.Where(t => t.Key.Contains(repeaterElemetPath));
                                                foreach (var option in selectedOptions)
                                                {
                                                    if (option.Value != null)
                                                    {
                                                        if (!string.IsNullOrEmpty(option.Value.ToString()))
                                                            columnData = columnData + option.Value + ",";
                                                    }
                                                }
                                                if (model.TableName.Equals("PBPC_POS") && model.FieldName.Equals("PBP_C_POS_OUPT_BENCAT_BENS"))
                                                {
                                                    if (columnData != null)
                                                    {
                                                        if (!string.IsNullOrEmpty(columnData))
                                                        {
                                                            columnData = columnData.TrimEnd(',');
                                                            columnVal = Get_PBP_C_POS_OUPT_BENCAT_BENS(model, columnData);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    columnVal = ConvertMultiSelectValue(dict, model, true, columnData);
                                                }
                                            }
                                            else if (model.MappingType == "MULTIVALUECSV")
                                                columnVal = ConvertMultiSelectCSVValue(dict, model, true, repeaterElemetPath);
                                            else if (repeaterRow.Value != null)
                                                columnVal = repeaterRow.Value.ToString();

                                            if (columnVal.ToString().Length > model.Length)
                                            {
                                                //string data = repeaterRow.Value == null ? "" : repeaterRow.Value.ToString();
                                                columnVal = columnVal.Substring(0, model.Length);
                                            }


                                            if (dTable.Rows.Count <= row)
                                            {
                                                DTrow = dTable.NewRow();
                                                dTable.Rows.Add(DTrow);
                                            }
                                            else
                                                DTrow = dTable.Rows[row];

                                            //if (model.FieldName == "MRX_TIER_POST_ID")
                                            //    columnVal = i++.ToString();

                                            if (string.IsNullOrEmpty(columnVal.Trim()) && model.IsBlankAllow == false)
                                                columnVal = null;

                                            DTrow[model.FieldName] = model.FieldName == "QID" ? QID : ValueFormatter(model, columnVal);
                                        }
                                        Index = Index + 1;
                                    }
                                }
                                if (tbl.Equals("PBPMRX_T"))
                                {
                                    dTable = RemoveBlankRowIn_PBPMRX_T(dTable);
                                }
                                else if (tbl.Equals("PBPMRX_P"))
                                {
                                    dTable = RemoveBlankRowIn_PBPMRX_P(dTable);
                                }

                                // cmd.Parameters.Clear();

                                using (OleDbConnection connection = new OleDbConnection(connectingString))
                                {
                                    using (OleDbCommand cmdNew = new OleDbCommand())
                                    {
                                        connection.Open();
                                        cmdNew.Connection = connection;

                                        foreach (DataRow row in dTable.Rows)
                                        {
                                            cmdNew.Parameters.Clear();
                                            foreach (DataColumn dc in dTable.Columns)
                                            {
                                                cmdNew.Parameters.AddWithValue(dc.ColumnName, row[dc.ColumnName]);
                                            }
                                            if (cmdNew.Parameters.Count > 0)
                                            {
                                                cmdNew.CommandText = cmdTextCOlumn;
                                                cmdNew.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                            }
                            //}
                        }
                        /************************************************************************************************************/
                        DeleteQIDFromTable(QID, source, connectingString);
                        queueDetails.FolderVersionID = formInstance.FolderVersionId;
                        /**************PROCESS VBIDS***************/
                        VBIDExportService vbidService = new VBIDExportService(_unitOfWork, _uiElementService, _formDesignService, _folderVersionService, _formInstanceService, _formInstanceDataService, _masterListService, _exitValidateService, null);
                        vbidService.ProcessVBIDExport(queueDetails, formInstanceData.DocId, QID, int.Parse(year));
                    }

                    this.AddPBPExportActivityLog(PBPExportQueueID, tblName, ViewModel.PBPSoftwareVersionName + ViewModel.TestQaVesrion + connectingString , userName, null);
                    AddPBPSoftwareVersion(ViewModel, connectingString);
                    //UpdateTrimAllFields(mapping, connection);
                    //transaction.Commit();
                    ServiceResult Result = this.ProcessPlanAreaFile(pbpPlanAreaFilePath, formInstanceLst);
                    this.UpdateExportQueueStatus(PBPExportQueueID, ProcessStatusMasterCode.Complete);
                    System.IO.Directory.CreateDirectory(folderPath);
                    File.Copy(queueDetails.PBPFilePath, finalPBPFilePath, true);
                    File.Copy(queueDetails.VBIDFilePath, finalVBIDFilePath, true);
                    if (File.Exists(queueDetails.PlanAreaFilePath))
                    {
                        File.Copy(queueDetails.PlanAreaFilePath,finalPlanAreaFilePath, true);
                    }
                    result = this.UpdatePBPFilePath(PBPExportQueueID, finalPBPFilePath, folderPath);

                }
                catch (Exception ex)
                {
                    this.AddPBPExportActivityLog(PBPExportQueueID, tblName, "FormInstanceID- " + formInstanceData.FormInstanceID + " & Name- " + formInstanceData.Name + "& Table Name= " + tblName + " Field= " + FieldName, userName, ex);
                    isExceptionLogged = true;
                    this.UpdateExportQueueStatus(PBPExportQueueID, ProcessStatusMasterCode.Errored);
                    bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                    if (reThrow)
                        throw ex;
                }
            }
            catch (Exception ex)
            {
                if (!isExceptionLogged)
                {
                    this.AddPBPExportActivityLog(PBPExportQueueID, string.Empty, string.Empty, userName, ex);
                }
                this.UpdateExportQueueStatus(PBPExportQueueID, ProcessStatusMasterCode.Errored);
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            finally
            {
                //   if (connection != null)
                //     connection.Close();
            }
        }

        private string getValueForCatID(string jsonPath)
        {
            if (jsonPath.Contains("17b"))
                return "17b";
            if (jsonPath.Contains("17a"))
                return "17a";
            if (jsonPath.Contains("18b"))
                return "18b";
            if (jsonPath.Contains("18a"))
                return "18a";
            if (jsonPath.Contains("16a"))
                return "16a";
            if (jsonPath.Contains("16b"))
                return "16b";
            if (jsonPath.Contains("7b"))
                return "7b";
            if (jsonPath.Contains("7f"))
                return "7f";
            if (jsonPath.Contains("10b"))
                return "10b";

            return string.Empty;

        }

        public IEnumerable<string> GetHiddenSectionList()
        {
            int parentUIElementID = (from u in this._unitOfWork.RepositoryAsync<UIElement>().Get()
                                     where u.FormID == 2409 && u.GeneratedName == "VBIDView"
                                     select u.UIElementID).FirstOrDefault();
            
            var HideSectionList = (from u in this._unitOfWork.RepositoryAsync<UIElement>().Get()
                                   where u.Visible == false && u.ParentUIElementID == parentUIElementID
                                   select u.GeneratedName).ToList();

            return HideSectionList;
        }

        public string ExitValidateGenerateMDBFile(int ExitValidateQueueID, string userName)
        {
            string resultString = "";
            bool isExceptionLogged = false;
            string tblName = string.Empty;
            string FieldName = string.Empty;
            ServiceResult result;
            string PBPFile = "PBP{YEAR}.MDB";
            string PLANAreaFile = "PBPPLANS{YEAR}.MDB";
            string VBIDFile = "VBID_PBP{YEAR}.MDB";
            PBPSoftwareVersionVeiwModel ViewModel = null;
            try
            {
                ExitValidateViewModel evModel = _exitValidateService.GetExitValidateMappings(ExitValidateQueueID);
                int PBPDatabase1Up = _exitValidateService.GetPBPDatabase1Up(evModel.AnchorDocumentID);
                string pbpPlanAreaFileName = GetPBPPlanAreaFileName(PBPDatabase1Up);
                string year = evModel.Year.ToString("yyyy");
                string pbpPlanAreaFilePath = System.Configuration.ConfigurationManager.AppSettings["PBPImportFiles"] + pbpPlanAreaFileName;
                string folderPath = string.Format(System.Configuration.ConfigurationManager.AppSettings["ExitValidateFiles"],year) + "ExitValidate_" +
                    evModel.FormInstanceID + "_" + evModel.QueueID + "_" + evModel.UserID + "_" + evModel.AddedDate.ToString("yyyyMMddhhmmss") + @"\";
                string mdbSchemaPath = System.Configuration.ConfigurationManager.AppSettings["PBPExitValidateExportSchema"] ?? string.Empty;

                PBPFile = PBPFile.Replace("{YEAR}", year.Equals("2020") ? "2020" : "2019");
                PLANAreaFile = PLANAreaFile.Replace("{YEAR}", year.Equals("2020") ? "2020" : "2019");
                VBIDFile = VBIDFile.Replace("{YEAR}", year.Equals("2020") ? "2020" : "2019");
                evModel.ExitValidateFilePath = folderPath + PBPFile;
                evModel.VBIDFilePath = folderPath + VBIDFile;
                evModel.PlanAreaFilePath = folderPath + PLANAreaFile;
                mdbSchemaPath = year.Equals("2020") ? mdbSchemaPath.Replace("2019", "2020") : mdbSchemaPath;

                bool IsLicenseVersion = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsLicenseVersion"].ToString());
                ViewModel = this.GetPBPSoftwareVersion(IsLicenseVersion, int.Parse(year));

                result = _exitValidateService.UpdateExitValidateFilePath(evModel.QueueID, evModel, folderPath);
                System.IO.Directory.CreateDirectory(folderPath);

                string folderPathCopy = string.Format(System.Configuration.ConfigurationManager.AppSettings["ExitValidateMDBPath"], year) ?? string.Empty;

                _exitValidateService.DeleteDirectory(folderPathCopy);
                File.Copy(mdbSchemaPath, evModel.ExitValidateFilePath, true);
                File.Copy(mdbSchemaPath.Replace(PBPFile, VBIDFile), evModel.VBIDFilePath, true);
                if (File.Exists(pbpPlanAreaFilePath))
                    File.Copy(pbpPlanAreaFilePath, evModel.PlanAreaFilePath, true);
                else
                    File.Copy(mdbSchemaPath.Replace(PBPFile, PLANAreaFile), evModel.PlanAreaFilePath, true);
                List<PBPExportToMDBMappingViewModel> mapping = this.GetExportMappings(int.Parse(year)).ToList();
                OleDbHelper oleDBHelperClass = new OleDbHelper();
                string connectingString = oleDBHelperClass.GetOleDbConnectingString(evModel.ExitValidateFilePath);

                try
                {
                    string json = this.GetJSONString(evModel.FormInstanceID);

                    JObject source = JObject.Parse(json);

                    string squishId = source.SelectToken("SectionA.AdditionalFields.PBPASQUISHID").ToString();

                    if (!string.IsNullOrEmpty(squishId) && squishId != evModel.Name)
                    {
                        evModel.Name = squishId;
                    }
                    string QID = string.IsNullOrEmpty(evModel.QID) ? evModel.Name : evModel.QID;
                    resultString = QID;
                    var token = source.SelectToken("SectionA.AdditionalFields.ABSTRAID");
                    var prop = token.Parent as JProperty;
                    prop.Value = string.Empty;

                    token = source.SelectToken("SectionA.AdditionalFields.UPLOADED");
                    prop = token.Parent as JProperty;
                    prop.Value = string.Empty;

                    if ((source["OONGroups"]["OONGroupsBase1"] != null && source["OONGroups"]["OONGroupsBase2"] != null && source["OONGroups"]["OONGroupsBase1"] is JArray && source["OONGroups"]["OONGroupsBase2"] is JArray) && ((JArray)(source["OONGroups"]["OONGroupsBase1"])).Count < ((JArray)(source["OONGroups"]["OONGroupsBase2"])).Count)
                    {
                        throw new Exception("OON Group Base 2 has more rows than OON Group Base1, which is not expected.");
                    }

                    if (source["POSGroups"]["POSGroupsBase1"] != null && source["POSGroups"]["POSGroupsBase1"] is JArray && ((JArray)source["POSGroups"]["POSGroupsBase1"]).Count() > 0)
                    {
                        List<JToken> va = (source.SelectToken("POSGroups.POSGroupsBase1")).OrderBy(S => S["POSGroupID"]).ToList();
                        source.SelectToken("POSGroups.POSGroupsBase1").Replace(JToken.Parse(JsonConvert.SerializeObject(va)));
                    }
                    if (source["POSGroups"]["POSGroupsBase2"] != null && source["POSGroups"]["POSGroupsBase2"] is JArray && ((JArray)source["POSGroups"]["POSGroupsBase2"]).Count() > 0)
                    {
                        List<JToken> val1 = (source.SelectToken("POSGroups.POSGroupsBase2")).OrderBy(S => S["POSGroupID"]).ToList();
                        source.SelectToken("POSGroups.POSGroupsBase2").Replace(JToken.Parse(JsonConvert.SerializeObject(val1)));
                    }
                    Dictionary<string, object> dict = JsonHelper.DeserializeAndFlatten(source.ToString());

                    List<string> MDBtables = mapping.Select(t => t.TableName).Distinct().ToList();
                    List<string> tables1 = MDBtables;
                    //CheckSectionVisibility(source, ref tables1);

                    foreach (string tbl in tables1)
                    {
                        tblName = tbl;
                        if (tblName.ToLower().Contains("_base"))
                        {
                            tblName = GetTableName(tblName);
                        }

                        string cmdText = "INSERT INTO " + tblName + "(";
                        string values = " VALUES (";

                        Dictionary<string, string> commandParam = new Dictionary<string, string>();
                        List<PBPExportToMDBMappingViewModel> tableMapping = (from map in mapping
                                                                             where map.TableName == tbl
                                                                             && map.IsRepeater == false
                                                                             orderby map.FieldName
                                                                             select map).ToList();

                        foreach (PBPExportToMDBMappingViewModel model in tableMapping)
                        {
                            var val = "";
                            FieldName = model.FieldName;
                            if (model.FieldName == "QID")
                                val = QID;
                            else if (model.FieldName == "PBP_D_OPT_OON_TYPE_ID")
                            {
                                if (model.JsonPath.Contains("OONOptional"))
                                {
                                    val = "1";
                                }
                                else if (model.JsonPath.Contains("OONStepup"))
                                    val = "2";
                            }
                            else if (FieldName.Equals("PBP_D_OPT_OON_CAT_ID"))
                            {
                                val = getValueForCatID(model.JsonPath);
                            }
                            else
                            {
                                var Value = dict.Where(t => t.Key == model.JsonPath).Select(t => t.Value).FirstOrDefault();
                                if (FieldName.Equals("PBP_C_POS_REFER_MC_SUBCATS") && QID.Equals("H0712020000"))
                                {

                                }

                                if (Value != null && !string.IsNullOrEmpty(Value.ToString()) && Value.ToString() == "NULL")
                                    Value = null;

                                if (Value != null)
                                {
                                    if (!string.IsNullOrEmpty(Value.ToString()))
                                    {
                                        val = RemoveNotApplicable(model, Value.ToString());
                                        val = ValueFormatter(model, val);
                                    }
                                    //if (string.IsNullOrEmpty(val) && !string.IsNullOrEmpty(model.DefaultValue))
                                    //{
                                    //    val = SetDefaultValue(model, val);
                                    //}
                                    else
                                    {
                                        val = Value.ToString();
                                    }
                                }

                            }
                            if (model.MappingType == "MULTISELECT")
                                val = ConvertMultiSelectValue(dict, model, false, null);
                            if (model.MappingType == "MULTIVALUECSV")
                                val = ConvertMultiSelectCSVValue(dict, model, false, string.Empty);

                            if (val != null)
                            {

                                if (val.Equals("true") || val.Equals("True"))
                                {
                                    val = "1";
                                }
                                else if (val.Equals("false") || val.Equals("False"))
                                {
                                    val = "0";
                                }
                            }
                            if (model.IsCustomRule)
                            {
                                val = ApplySort(val);
                            }

                            /*******************************************Special Case for PLAN_TYPE & ORG_TYPE***********************************************/
                            if (tbl == "PBP")
                            {
                                if (model.FieldName == "PBP_A_PLAN_TYPE" || model.FieldName == "PBP_A_ORG_TYPE")
                                {
                                    if (val.ToString().Length == 1)
                                        val = "0" + val;
                                }
                            }
                            
                            if (val.ToString().Length > model.Length)
                            {
                                if (val.ToString().Split('.')[0].Length > model.Length)
                                {
                                    result = _exitValidateService.AddExitValidatePBPExportError(evModel, model);
                                }
                                val = val.ToString().Substring(0, model.Length);
                            }
                            if (val != string.Empty && val != null)
                            {
                                if (!commandParam.ContainsKey(model.FieldName))
                                {
                                    commandParam.Add(model.FieldName, val);
                                }
                            }
                            //cmd.Parameters.AddWithValue(model.FieldName, val);
                        }

                        //OleDbCommand cmd = new OleDbCommand();
                        //cmd.Parameters.Clear();
                        var cmdTextCOlumn = "";

                        foreach (string key in commandParam.Keys)
                        {
                            cmdText = cmdText + key + ",";
                            values = values + "?,";
                            //cmd.Parameters.AddWithValue(key, commandParam[key]);
                        }
                        if (commandParam.Count() > 0)
                        {
                            if (commandParam.Count() == 1 && commandParam.Keys.First() == "QID" && TableQIDList.Contains(tblName))
                            {
                            }
                            //else if (tblName == "PBPD_OON" && commandParam.Count() <= 3)
                            //{
                            //}
                            else if (tblName == "PBPD_OON" && (commandParam.Count() <= 3 || !commandParam.ContainsKey("QID") || !commandParam.ContainsKey("PBP_D_OPT_OON_IDENTIFIER") || !commandParam.ContainsKey("PBP_D_OPT_OON_TYPE_ID") || !commandParam.ContainsKey("PBP_D_OPT_OON_CAT_ID")))
                            {
                            }
                            else if ((tblName.Contains("STEP") || tblName == "PBPD_OPT") && (!commandParam.ContainsKey("QID") || !commandParam.ContainsKey("PBP_D_OPT_IDENTIFIER")))
                            {

                            }
                            else
                            {
                                values = values.Remove(values.LastIndexOf(','), 1) + ")";
                                cmdText = cmdText.Remove(cmdText.LastIndexOf(','), 1) + ")" + values;

                                using (OleDbConnection connection = new OleDbConnection(connectingString))
                                {
                                    using (OleDbCommand cmdNew = new OleDbCommand())
                                    {
                                        foreach (string key in commandParam.Keys)
                                        {
                                            //cmdText = cmdText + key + ",";
                                            //values = values + "?,";
                                            cmdNew.Parameters.AddWithValue(key, commandParam[key]);
                                        }

                                        connection.Open();
                                        cmdNew.CommandText = cmdText;
                                        cmdNew.Connection = connection;
                                        cmdNew.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        else
                        {
                            List<string> columns = (from map in mapping
                                                    where map.TableName == tbl
                                                    orderby map.FieldName
                                                    select map.FieldName).ToList();
                            foreach (string column in columns)
                            {
                                if (!cmdText.Contains(column + ","))
                                {
                                    cmdText = cmdText + column + ",";
                                    values = values + "?,";
                                }
                            }
                            values = values.Remove(values.LastIndexOf(','), 1) + ")";
                            cmdText = cmdText.Remove(cmdText.LastIndexOf(','), 1) + ")" + values;
                            cmdTextCOlumn = cmdText;
                            //cmd.CommandText = cmdText;
                            //cmd.Connection = connection;
                        }
                        /**************************************************Populate REPEATER DATA**********************************************************/

                        if (IsTableNeededToIncludeInExport(source, tbl, QID, evModel.QueueID).Equals(true))
                        {
                            List<string> repeaterList = (from m in mapping
                                                         where m.IsRepeater == true
                                                         && m.TableName == tbl
                                                         select m.JsonPath).Distinct().ToList();
                            string[] repeaters = null;
                            List<string> lstRepeaters = new List<string>();
                            foreach (string repeater in repeaterList)
                            {
                                repeaters = repeater.Split('[').ToArray();
                                if (repeaters[0].Length > 0)
                                    lstRepeaters.Add(repeaters[0]);
                            }

                            DataTable dTable = new DataTable();

                            if (lstRepeaters.Count() > 0)
                            {
                                lstRepeaters = lstRepeaters.Distinct().OrderBy(t => t).ToList();
                                dTable = this.GetDataTableSchema(tbl, lstRepeaters, mapping);
                            }

                            int gapcnt = 0;
                            int iclcnt = 0;
                            int cnt1 = 0;
                            int rowcnt = 0;
                            foreach (string repeaterName in lstRepeaters)
                            {
                                //i = 1;
                                string columnVal = string.Empty;
                                int rowCount = 0;
                                if (source.SelectToken(repeaterName) != null)
                                    rowCount = source.SelectToken(repeaterName).Count();

                                int cnt = 0;
                                if (repeaterName.StartsWith("GapCoverage."))
                                {
                                    cnt1 = iclcnt;
                                    if (gapcnt <= rowCount)
                                        gapcnt = rowCount;
                                    rowcnt = rowCount + iclcnt;
                                }
                                else if (repeaterName.StartsWith("PreICL.") || repeaterName.StartsWith("MedicareMedicaidPreICL."))
                                {
                                    cnt1 = gapcnt;
                                    if (iclcnt <= rowCount)
                                        iclcnt = rowCount;
                                    rowcnt = rowCount + gapcnt;
                                }

                                if (repeaterName.Contains("PreICL.") || repeaterName.Contains("GapCoverage.")
                                    || repeaterName.Contains("MedicareMedicaidPreICL."))
                                {
                                    cnt = cnt1;
                                    rowCount = rowcnt;
                                }
                                else
                                {
                                    cnt1 = 0;
                                    rowcnt = 0;
                                }

                                List<PBPExportToMDBMappingViewModel> repeaterColumns = mapping.Where(t => t.TableName == tbl && t.IsRepeater == true && t.JsonPath.Contains(repeaterName)).OrderBy(t => t.FieldName).ToList();
                                int Index = 0;
                                for (int row = cnt; row < rowCount; row++)
                                {
                                    DataRow DTrow = null;
                                    foreach (PBPExportToMDBMappingViewModel model in repeaterColumns)
                                    {
                                        columnVal = string.Empty;
                                        FieldName = model.FieldName;
                                        if (model.TableName.Equals("PBPMRX_T") && QID.Equals("H4506003000"))
                                        {

                                        }

                                        string repeaterElemetPath = model.JsonPath.Replace("[0]", "." + (cnt1 == 0 || (gapcnt != 0 && iclcnt != 0) == false ? row : Index).ToString()); // replace [0] with actual row Number
                                        dynamic repeaterRow = dict.Where(t => t.Key == repeaterElemetPath).FirstOrDefault();

                                        if (model.MappingType == "MULTISELECT")
                                        {
                                            string columnData = string.Empty;
                                            var selectedOptions = dict.Where(t => t.Key.Contains(repeaterElemetPath));
                                            foreach (var option in selectedOptions)
                                            {
                                                if (option.Value != null)
                                                {
                                                    if (!string.IsNullOrEmpty(option.Value.ToString()))
                                                        columnData = columnData + option.Value + ",";
                                                }
                                            }
                                            if (model.TableName.Equals("PBPC_POS") && model.FieldName.Equals("PBP_C_POS_OUPT_BENCAT_BENS"))
                                            {
                                                if (columnData != null)
                                                {
                                                    if (!string.IsNullOrEmpty(columnData))
                                                    {
                                                        columnData = columnData.TrimEnd(',');
                                                        columnVal = Get_PBP_C_POS_OUPT_BENCAT_BENS(model, columnData);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                columnVal = ConvertMultiSelectValue(dict, model, true, columnData);
                                            }
                                        }
                                        else if (model.MappingType == "MULTIVALUECSV")
                                            columnVal = ConvertMultiSelectCSVValue(dict, model, true, repeaterElemetPath);
                                        else if (repeaterRow.Value != null)
                                            columnVal = repeaterRow.Value.ToString();

                                        if (columnVal.ToString().Length > model.Length)
                                        {
                                            //string data = repeaterRow.Value == null ? "" : repeaterRow.Value.ToString();
                                            columnVal = columnVal.Substring(0, model.Length);
                                        }


                                        if (dTable.Rows.Count <= row)
                                        {
                                            DTrow = dTable.NewRow();
                                            dTable.Rows.Add(DTrow);
                                        }
                                        else
                                            DTrow = dTable.Rows[row];

                                        //if (model.FieldName == "MRX_TIER_POST_ID")
                                        //    columnVal = i++.ToString();

                                        DTrow[model.FieldName] = model.FieldName == "QID" ? QID : ValueFormatter(model, columnVal);
                                    }
                                    Index = Index + 1;
                                }
                            }
                            if (tbl.Equals("PBPMRX_T"))
                            {
                                dTable = RemoveBlankRowIn_PBPMRX_T(dTable);
                            }
                            else if (tbl.Equals("PBPMRX_P"))
                            {
                                dTable = RemoveBlankRowIn_PBPMRX_P(dTable);
                            }
                            using (OleDbConnection connection = new OleDbConnection(connectingString))
                            {
                                using (OleDbCommand cmdNew = new OleDbCommand())
                                {
                                    connection.Open();
                                    cmdNew.Connection = connection;

                                    foreach (DataRow row in dTable.Rows)
                                    {
                                        cmdNew.Parameters.Clear();
                                        foreach (DataColumn dc in dTable.Columns)
                                        {
                                            cmdNew.Parameters.AddWithValue(dc.ColumnName, row[dc.ColumnName]);
                                        }
                                        if (cmdNew.Parameters.Count > 0)
                                        {
                                            cmdNew.CommandText = cmdTextCOlumn;
                                            cmdNew.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }
                        //}
                    }
                    /************************************************************************************************************/

                    int rowsAffected = 0;

                    DeleteQIDFromTable(QID, source, connectingString);
                    AddPBPSoftwareVersion(ViewModel, connectingString);

                    string pbpPlanConnectionString = connectingString;

                    using (OleDbConnection conn = new OleDbConnection(connectingString))
                    {
                        conn.Open();
                        rowsAffected = _exitValidateService.UpdatePBPFileRow(conn);
                    }

                    connectingString = oleDBHelperClass.GetOleDbConnectingString(evModel.VBIDFilePath);
                    AddPBPSoftwareVersion(ViewModel, connectingString);

                    connectingString = oleDBHelperClass.GetOleDbConnectingString(evModel.PlanAreaFilePath);
                    using (OleDbConnection conn = new OleDbConnection(connectingString))
                    {
                        conn.Open();
                        rowsAffected = _exitValidateService.DeleteExtraRowsPlanAreaFile(QID, conn);
                        if (rowsAffected == 0)
                        {
                            rowsAffected = _exitValidateService.AddPlanAreaFileRow(source, conn);
                        }
                    }
                    if (rowsAffected == 0)
                    {
                        using (OleDbConnection connPbp = new OleDbConnection(pbpPlanConnectionString))
                        {
                            connPbp.Open();
                            string squishQID = source.SelectToken("SectionA.AdditionalFields.PBPASQUISHID").ToString();
                            string pbpQuery = "UPDATE PBP SET QID ='" + squishQID + "'";
                            OleDbCommand cmdPBP = new OleDbCommand(pbpQuery, connPbp);
                            cmdPBP.ExecuteNonQuery();
                        }
                    }

                    /**************PROCESS VBIDS***************/
                    VBIDExportService vbidService = new VBIDExportService(_unitOfWork, _uiElementService, _formDesignService, _folderVersionService, _formInstanceService, _formInstanceDataService, _masterListService, _exitValidateService, evModel);
                    vbidService.ProcessVBIDExportForExitValidate(evModel, QID, int.Parse(year));

                    File.Copy(evModel.ExitValidateFilePath, folderPathCopy + PBPFile, true);
                    File.Copy(evModel.VBIDFilePath, folderPathCopy + VBIDFile, true);
                    File.Copy(evModel.PlanAreaFilePath, folderPathCopy + PLANAreaFile, true);
                }
                catch (Exception ex)
                {
                    this.AddPBPExportActivityLog(evModel.QueueID, tblName, "FormInstanceID- " + evModel.FormInstanceID + " & Name- " + evModel.Name + "& Table Name= " + tblName + " Field= " + FieldName, userName, ex);
                    isExceptionLogged = true;
                  
                    this.UpdateExportQueueStatus(evModel.QueueID, ProcessStatusMasterCode.Errored);
                    bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                    if (reThrow)
                        throw;
                }
            }
            catch (Exception ex)
            {
                if (!isExceptionLogged)
                {
                    this.AddPBPExportActivityLog(ExitValidateQueueID, string.Empty, string.Empty, userName, ex);
                    this.UpdateExportQueueStatus(ExitValidateQueueID, ProcessStatusMasterCode.Errored);
                    _exitValidateService.SendEVCompletionotification(ExitValidateQueueID, true,0);
                }
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            
            return resultString;
        }

        

        private void DeleteQIDFromTable(string QID, JObject source, string connectingString)
        {
            string[] TableList = { "PBPB20", "PBPC", "PBPC_OON", "PBPC_POS", "PBPD_OON", "PBPD_OPT" };

            string PlanTypePath = "SectionA.SectionA1.PlanType";
            JToken PlayTypeToken = source.SelectToken(PlanTypePath);
            string PlanType = string.Empty;
            if (PlayTypeToken != null)
                PlanType = PlayTypeToken.ToString();

            var commandtext = "";
            if (!string.IsNullOrEmpty(PlanType))
            {
                if (PlanType.Equals("29"))
                {
                    foreach (var item in TableList)
                    {
                        //try
                        // {
                        //OleDbCommand Cmd = new OleDbCommand(string.Con    cat("DELETE FROM ", item.ToString(), " where QID='", QID, "';"), Conn);
                        //Cmd.ExecuteNonQuery();
                        var deleteText = string.Concat("DELETE FROM ", item.ToString(), " where QID='", QID, "'");
                        RunSQLCommand(connectingString, deleteText);
                        //commandtext = string.Format("{0} {1}", commandtext, deleteText);
                        /* }
                         catch (Exception ex)
                         {
                             bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                             if (reThrow)
                                 throw;
                         }*/

                    }
                }
            }

        }
        private void CheckSectionVisibility(JObject json, ref List<string> tables1)
        {
            string plantype, otherCat, serviceCat, OptBenPkg, section6 = string.Empty;
            plantype = json.SelectToken("SectionA.SectionA1.PlanType") == null ? string.Empty : json.SelectToken("SectionA.SectionA1.PlanType").ToString();
            section6 = json.SelectToken("SectionA.SectionA6.IsyourorganizationfilingastandardbidforSectionDofthePBP") == null ? string.Empty : json.SelectToken("SectionA.SectionA6.IsyourorganizationfilingastandardbidforSectionDofthePBP").ToString();
            if (plantype == "4" || plantype == "48" || plantype == "29")
                section6 = "2";
            OptBenPkg = json.SelectToken("OptionalSupplementalBenefitPackages.OptionalSupplementalPackages") == null ? string.Empty : json.SelectToken("OptionalSupplementalBenefitPackages.OptionalSupplementalPackages").ToString();

            if (!((plantype != "49" || plantype != "48" || plantype != "32" || plantype != "30" || plantype != "29" || plantype != "20") && section6 != "" && section6 != "1" && OptBenPkg != ""))
            {
                tables1.Remove("PBPD_OPT_Base1");
                tables1.Remove("PBPD_OPT_Base2");
                tables1.Remove("PBPD_OPT");
            }

            otherCat = json.SelectToken("OptionalSupplementalBenefitPackages.OptionalSupplementalServiceCategories1.SelecttheotherservicecategoriesincludedinthispackageiethatareNOTdeclar") == null ? string.Empty
                                : json.SelectToken("OptionalSupplementalBenefitPackages.OptionalSupplementalServiceCategories1.SelecttheotherservicecategoriesincludedinthispackageiethatareNOTdeclar").ToString();

            OptBenPkg = json.SelectToken("OptionalSupplementalBenefitPackages.OptionalSupplementalPackages") == null ? string.Empty : json.SelectToken("OptionalSupplementalBenefitPackages.OptionalSupplementalPackages").ToString();

            int OptBenPkgNum = 0;
            bool isNum = Int32.TryParse(OptBenPkg, out OptBenPkgNum);
            if (isNum)
            {
                if (!(otherCat.Contains("16a") && OptBenPkgNum > 1))
                {
                    tables1.Remove("STEP16A_Base1");
                    tables1.Remove("STEP16A_Base2");
                }
            }
            else
            {
                tables1.Remove("STEP16A_Base1");
                tables1.Remove("STEP16A_Base2");
            }

            if (!otherCat.Contains("16b"))
            {
                tables1.Remove("STEP16B_Base1");
                tables1.Remove("STEP16B_Base2");
            }

            serviceCat = json.SelectToken("OptionalSupplementalBenefitPackages.OptionalSupplementalServiceCategories1.Selecttheservicecategoriesincludedinthispackagethathaveoptionalsupplem") == null ? string.Empty
                                : json.SelectToken("OptionalSupplementalBenefitPackages.OptionalSupplementalServiceCategories1.Selecttheservicecategoriesincludedinthispackagethathaveoptionalsupplem").ToString();

            if (!((((plantype != "32" || plantype != "30" || plantype != "29" || plantype != "20") && section6 != "" && section6 != "1")) && ((otherCat.Contains("16a")) || (otherCat.Contains("16b")))))
            {
                tables1.Remove("PBPD_OON_Base1");
                tables1.Remove("PBPD_OON_Base2");
                tables1.Remove("PBPD_OON_Base3");
                tables1.Remove("PBPD_OON_Base4");
            }
        }

        private string ConvertMultiSelectCSVValue(Dictionary<string, object> dict, PBPExportToMDBMappingViewModel model, bool isRepeater, string repetareJsonPath)
        {
            string CSV = string.Empty;
            string jsonPath = string.Empty;
            if (isRepeater)
                jsonPath = repetareJsonPath;
            else
                jsonPath = model.JsonPath;

            var selectedOptions = dict.Where(t => t.Key.Contains(jsonPath))
                                  .OrderBy(s => s.Value);

            if (selectedOptions != null && selectedOptions.Count() > 0)
            {
                foreach (var option in selectedOptions)
                {
                    if (option.Value != null)
                    {
                        if (!String.IsNullOrWhiteSpace(option.Value.ToString()))
                        {
                            CSV = CSV + option.Value + ";";
                        }
                    }
                }
            }

            CSV = CSV.Replace(",", ";");
            CSV = CSV.Replace(";;", ";");            
            //CSV = CSV.TrimEnd(';');
            CSV = CSV.Replace("[", string.Empty);
            try
            {
                string alphaNumericSort = ConfigurationManager.AppSettings["ExportAlphaNumericSort"] ?? string.Empty;
                if (alphaNumericSort == "Yes")
                {
                    if (!String.IsNullOrEmpty(CSV))
                    {
                        string[] CSVArr = CSV.Split(';').Where(v => !String.IsNullOrEmpty(v)).ToArray();
                        if (CSVArr.Length > 1)
                        {
                            Array.Sort(CSVArr, new AlphanumComparatorFast());
                            CSV = String.Join(";", CSVArr);
                            CSV += string.Join(CSV, ";");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while sorting codes for " + jsonPath;
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
            return CSV;
        }

        private void UpdateTrimAllFields(List<PBPExportToMDBMappingViewModel> mapping, OleDbConnection connection)
        {
            List<string> tables = mapping.Select(t => t.TableName).Distinct().ToList();

            foreach (string tbl in tables)
            {
                string cmdText = "UPDATE " + tbl + " SET ";
                string values = string.Empty;
                List<string> columns = (from map in mapping
                                        where map.TableName == tbl
                                        orderby map.FieldName
                                        select map.FieldName).ToList();

                foreach (string column in columns)
                    //values = values + column + " = TRIM( " + column + " ), ";

                    values = values + column + " = replace(" + column + " , 'blank', ' ' ), ";

                values = values.Remove(values.LastIndexOf(','), 1);
                cmdText += values;
                OleDbCommand cmd = new OleDbCommand(cmdText, connection);
                cmd.ExecuteNonQuery();
            }
        }

        private DataTable GetDataTableSchema(string tableName, List<string> lstRepeaters, List<PBPExportToMDBMappingViewModel> mapping)
        {
            DataTable dataTable = new DataTable();
            List<string> columns = new List<string>();
            foreach (string repeaterName in lstRepeaters)
            {
                List<PBPExportToMDBMappingViewModel> models = mapping.Where(t => t.TableName == tableName && t.IsRepeater == true && t.JsonPath.Contains(repeaterName)).OrderBy(t => t.FieldName).ToList();
                foreach (PBPExportToMDBMappingViewModel model in models)
                {
                    if (!columns.Contains(model.FieldName))
                    {
                        columns.Add(model.FieldName);
                    }
                }
            }
            if (columns.Count() > 0)
            {
                columns = columns.OrderBy(t => t).ToList();
                foreach (string col in columns)
                    dataTable.Columns.Add(col);
            }
            return dataTable;
        }

        public dynamic ConvertMultiSelectValue(Dictionary<string, object> dict, PBPExportToMDBMappingViewModel model, bool isRepeater, dynamic repeaterData)
        {
            dynamic result = "";
            string CSV = "";
            if (!isRepeater)
            {
                var selectedOptions = dict.Where(t => t.Key.Contains(model.JsonPath))
                                      .OrderBy(s => s.Value);
                foreach (var option in selectedOptions)
                    if (option.Value != null)
                    {
                        if (!String.IsNullOrWhiteSpace(option.Value.ToString()))
                        {
                            if (!CSV.Contains(option.Value.ToString()))
                                CSV = CSV + option.Value + ",";
                        }
                    }
            }
            if (isRepeater)
                CSV = repeaterData.ToString();
            //CSV = CSV.TrimEnd(';');
            if (CSV != null && CSV.Length > 0)
            {
                string targetVal = "0";
                string[] strArr = CSV.Split(',');
                if (strArr != null && strArr.Length > 0)
                {
                    foreach (string str in strArr)
                    {
                        long intVal = 0;
                        bool isInt = Int64.TryParse(str, out intVal);
                        if (isInt)
                        {
                            int num = Convert.ToInt32(str, 2);
                            int res = Convert.ToInt32(targetVal, 2);
                            targetVal = Convert.ToString(res + num, 2);
                        }
                    }
                    result = targetVal;
                }
            }
            if (CSV.Length > 0 && result.ToString().Length < model.Length)
            {
                long intVal = 0;
                bool isInt = Int64.TryParse(result, out intVal);
                if (isInt)
                    result = string.Format("{0}", intVal.ToString("D" + model.Length.ToString()));// Pad a Number with Leading Zeros
            }
            return result;
        }

        public void AddPBPExportActivityLog(int PBPExportQueueID, string tableName, string planDetails, string userName, Exception ex)
        {
            string errorForTable = tableName.Length == 0 ? tableName : " ,tableName : " + tableName;
            string errorForPlan = planDetails.Length == 0 ? planDetails : " Plan with : " + planDetails;
            string message = "Error occurred for Export ID: " + PBPExportQueueID + errorForPlan + errorForTable + " -> ";
            PBPExportActivityLog activitylog = new PBPExportActivityLog();
            activitylog.PBPExportQueueID = PBPExportQueueID;
            activitylog.TableName = tableName;
            if (ex != null)
            {
                activitylog.Message = String.Concat("Error occurred for PBPExportQueueID: " + PBPExportQueueID + errorForPlan + errorForTable + " -> " + ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);
            }
            else
            {
                activitylog.Message = String.Concat("Debug LPBPExportQueueID: " + PBPExportQueueID + errorForPlan + errorForTable);

            }
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

        public ServiceResult UpdatePBPFilePath(int PBPExportQueueID, string PBPFilePath, string ZipFilePath)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                using (var scope = new TransactionScope())
                {
                    PBPExportQueue queueDetails = this._unitOfWork.RepositoryAsync<PBPExportQueue>().Find(PBPExportQueueID);

                    queueDetails.PBPFilePath = PBPFilePath;
                    queueDetails.ZipFilePath = ZipFilePath;
                    _unitOfWork.RepositoryAsync<PBPExportQueue>().Update(queueDetails);
                    _unitOfWork.Save();
                    scope.Complete();
                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                _logger.ErrorException("Error in UpdateExportQueue : ", ex);
                throw ex;
            }
            return result;
        }

        public ServiceResult UpdateExportQueue(int PBPExportQueueID, PBPExportQueueViewModel model)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                using (var scope = new TransactionScope())
                {
                    PBPExportQueue itemToUpdate = this._unitOfWork.RepositoryAsync<PBPExportQueue>().Find(model.PBPExportQueueID);

                    if (itemToUpdate != null)
                    {
                        itemToUpdate.ImportStatus = model.ImportStatus;
                        itemToUpdate.JobId = model.JobId;
                        itemToUpdate.ErrorMessage = model.ErrorMessage;

                        this._unitOfWork.RepositoryAsync<PBPExportQueue>().Update(itemToUpdate);
                        this._unitOfWork.Save();
                        scope.Complete();
                        result.Result = ServiceResultStatus.Success;
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                _logger.ErrorException("Error in UpdateExportQueue : ", ex);
                throw ex;
            }
            return result;
        }

        public ServiceResult UpdateExportQueueStatus(int PBPExportQueueID, domain.entities.Enums.ProcessStatusMasterCode status)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                using (var scope = new TransactionScope())
                {
                    PBPExportQueue itemToUpdate = this._unitOfWork.RepositoryAsync<PBPExportQueue>().Find(PBPExportQueueID);

                    if (itemToUpdate != null)
                    {
                        itemToUpdate.Status = (int)status;

                        this._unitOfWork.RepositoryAsync<PBPExportQueue>().Update(itemToUpdate);
                        this._unitOfWork.Save();
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


        public PBPExportQueueViewModel GetQueuedOrProcessingPBPExport()
        {
            PBPExportQueueViewModel model = null;
            try
            {
                var PBPExportQueueList = (from c in this._unitOfWork.RepositoryAsync<PBPExportQueue>().Get()
                                          join status in this._unitOfWork.RepositoryAsync<ProcessStatusMaster>().Get()
                                          on c.Status equals status.ProcessStatus1Up
                                          where c.Status < 3
                                          select new PBPExportQueueViewModel
                                          {
                                              PBPExportQueueID = c.PBPExportQueueID,
                                              Description = c.Description,
                                              FileName = c.ExportName,
                                              ExportedDate = c.ExportedDate,
                                              ExportedBy = c.ExportedBy,
                                              Status = c.Status,
                                              StatusText = status.ProcessStatusName == "Errored" ? "Failed" : status.ProcessStatusName,
                                              PlanYear = c.PlanYear == 0 ? System.DateTime.Now.Year : c.PlanYear,
                                              PBPDatabase = c.PBPDataBase,
                                          });

                if (PBPExportQueueList != null && PBPExportQueueList.Count() > 0)
                {
                    model = PBPExportQueueList.First();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return model;
        }

        public string GetPBPPlanAreaFileName(int PBPDatabase1Up)
        {
            string fileName = _unitOfWork.Repository<PBPImportQueue>().Get()
                               .Where(t => t.PBPDatabase1Up == PBPDatabase1Up && t.Status == 4)
                               .OrderByDescending(t => t.PBPImportQueueID)
                               .Select(t => t.PBPPlanAreaFileName).FirstOrDefault();
            return fileName;
        }

        public string GetExportErrorLog(int PBPExportQueueID)
        {
            string errorMsg = string.Empty;
            try
            {
                var errMessages = (from err in _unitOfWork.RepositoryAsync<PBPExportActivityLog>().Get()
                                   where err.PBPExportQueueID == PBPExportQueueID
                                   select err.Message.Replace("'", "").Replace(">", "-")).Distinct().ToList();

                foreach (string err in errMessages)
                    errorMsg = err + "; ";
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return errorMsg;
        }

        public bool CheckFolderIsQueued(int folderID)
        {
            bool isQueued = false;
            try
            {
                List<PBPImportDetailsViewModel> pbpImportDetails = (from importDetails in _unitOfWork.RepositoryAsync<PBPImportDetails>().Get()
                                                                    join exportDetails in _unitOfWork.RepositoryAsync<PBPExportQueue>().Get()
                                                                    on importDetails.PBPDatabase1Up equals exportDetails.PBPDatabase1Up
                                                                    where exportDetails.Status == 1 && importDetails.IsActive == true
                                                                    select new PBPImportDetailsViewModel
                                                                    {
                                                                        FolderId = importDetails.FolderId,
                                                                    }).ToList();
                if (pbpImportDetails != null && pbpImportDetails.Count > 0)
                    isQueued = pbpImportDetails.Exists(x => x.FolderId == folderID);
            }
            catch (Exception ex)
            {
                isQueued = false;
            }
            return isQueued;
        }

        public List<ResourceLock> CheckExportFolderIsLocked(int pbpDatabase1Up, int? currentUserId, string userName)
        {
            List<ResourceLock> lockDetails = new List<ResourceLock>();
            try
            {
                List<PBPImportDetailsViewModel> pbpImportDetails = (from importDetails in _unitOfWork.RepositoryAsync<PBPImportDetails>().Get()
                                                                    where importDetails.PBPDatabase1Up == pbpDatabase1Up && importDetails.IsActive == true
                                                                    select new PBPImportDetailsViewModel
                                                                    {
                                                                        FolderId = importDetails.FolderId,
                                                                    }).ToList();

                var resourceLockHolder = new ResourceLockHolder();
                if (pbpImportDetails != null && pbpImportDetails.Count > 0)
                {
                    // Get Folder/Documents that are locked.
                    IEnumerable<ResourceLock> details = resourceLockHolder.GetDocument(); 
                    if (details != null && details.Count() > 0)
                    {
                        var lockObj = details.Select(x => new { x.FolderID, x.LockedBy }).Distinct();
                        var importDetailObj = pbpImportDetails.Select(x => new { x.FolderId }).Distinct();
                        lockDetails = (from folderLock in lockObj
                                       join importDetails in importDetailObj
                                       on folderLock.FolderID equals importDetails.FolderId
                                       join user in this._unitOfWork.RepositoryAsync<User>().Get()
                                       on folderLock.LockedBy equals user.UserID
                                       join fldr in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                       on folderLock.FolderID equals fldr.FolderID
                                       select new ResourceLock
                                       {
                                           Folder = fldr,
                                           LockedBy = folderLock.LockedBy,
                                           LockedUserName = user.UserName,
                                       }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                lockDetails = new List<ResourceLock>();
            }
            return lockDetails;
        }

        private PBPSoftwareVersionVeiwModel GetPBPSoftwareVersion(bool IsLicenseVersion, int planYear)
        {
            PBPSoftwareVersionVeiwModel ViewModel = new PBPSoftwareVersionVeiwModel();
            try
            {
                ViewModel = (from list in _unitOfWork.Repository<PBPSoftwareVersion>().Get()
                                         .Where(t => t.IsActive.Equals(true)
                                         && t.IsLicenseVersion.Equals(IsLicenseVersion)
                                         && t.PBPSoftwareVersionName.Contains(planYear.ToString())
                                         )
                             select new PBPSoftwareVersionVeiwModel
                             {
                                 PBPSoftwareVersionName = list.PBPSoftwareVersionName,
                                 TestQaVesrion = list.TestQaVesrion
                             })
                             .FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return ViewModel;
        }


        private void RunSQLCommand(string connectingString, string sqlStatment)
        {
            using (OleDbConnection connection = new OleDbConnection(connectingString))
            {
                using (OleDbCommand cmdNew = new OleDbCommand())
                {
                    connection.Open();
                    cmdNew.CommandText = sqlStatment;
                    cmdNew.Connection = connection;
                    cmdNew.ExecuteNonQuery();
                }
            }
        }
        private void AddPBPSoftwareVersion(PBPSoftwareVersionVeiwModel ViewModel, string connectingString)
        {
            try
            {
                string PBPVersionStr = string.Empty;
                if (ViewModel != null)
                {
                    PBPVersionStr = "DELETE FROM VERSION";
                    RunSQLCommand(connectingString, PBPVersionStr);

                    //OleDbCommand cmdPBPVersion = new OleDbCommand(PBPVersionStr);
                    // cmdPBPVersion.ExecuteNonQuery();
                    PBPVersionStr = "INSERT INTO VERSION (PBPVERSION,TEST_QA_VERSION)VALUES ('" + ViewModel.PBPSoftwareVersionName + "','" + ViewModel.TestQaVesrion + "')";
                    RunSQLCommand(connectingString, PBPVersionStr);

                    /*    using (OleDbConnection connection = new OleDbConnection(connectingString))
                        {
                            using (OleDbCommand cmdNew = new OleDbCommand())
                            {
                                connection.Open();
                                cmdNew.CommandText = PBPVersionStr;
                                cmdNew.Connection = connection;
                                cmdNew.ExecuteNonQuery();
                            }
                        }
                        */

                    //  cmdPBPVersion = new OleDbCommand(PBPVersionStr, conn);
                    //   cmdPBPVersion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                this.AddPBPExportActivityLog(-9999, "Version", ViewModel.PBPSoftwareVersionName + ViewModel.TestQaVesrion + connectingString, "superuser", ex);
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
        }

        string RemoveNotApplicable(PBPExportToMDBMappingViewModel viewModel, string val)
        {
            if (!string.IsNullOrWhiteSpace(val))
            {
                //string Val = string.Empty;
                //List<string> NotApplicableColumnList = new List<string>();
                //NotApplicableColumnList.Add("PBP_B3_NOTES");
                //NotApplicableColumnList.Add("PBP_B4A_NOTES");
                //NotApplicableColumnList.Add("PBP_B4B_NOTES");
                //NotApplicableColumnList.Add("PBP_B4C_NOTES");
                //NotApplicableColumnList.Add("PBP_B7A_NOTES");
                //NotApplicableColumnList.Add("PBP_B1A_NOTES");
                //NotApplicableColumnList.Add("PBP_B1B_NOTES");
                //NotApplicableColumnList.Add("PBP_B10A_NOTES");
                //NotApplicableColumnList.Add("PBP_B10B_NOTES");
                //NotApplicableColumnList.Add("PBP_B11A_NOTES");
                //NotApplicableColumnList.Add("PBP_B11C_NOTES");
                //NotApplicableColumnList.Add("MRX_B_NOTES");
                //NotApplicableColumnList.Add("PBP_B16A_NOTES");
                //NotApplicableColumnList.Add("PBP_B17A_NOTES");
                //NotApplicableColumnList.Add("PBP_B18A_NOTES");
                //NotApplicableColumnList.Add("PBP_B18B_NOTES");
                ////NotApplicableColumnList.Add("PBP_B2_HOSP_BEN_PERIOD_OTH");
                //NotApplicableColumnList.Add("PBP_B4A_NOTES");
                //NotApplicableColumnList.Add("PBP_B4B_NOTES");
                //NotApplicableColumnList.Add("PBP_B4C_NOTES");
                //NotApplicableColumnList.Add("PBP_B7A_NOTES");
                //NotApplicableColumnList.Add("PBP_B7C_NOTES");
                //NotApplicableColumnList.Add("PBP_B9A_NOTES");
                //NotApplicableColumnList.Add("PBP_B9B_NOTES");
                //NotApplicableColumnList.Add("PBP_B9C_NOTES");
                //NotApplicableColumnList.Add("PBP_B9D_NOTES");
                //NotApplicableColumnList.Add("PBP_B8B_DRS_NOTES");
                //NotApplicableColumnList.Add("PBP_B8B_TMC_NOTES");

                if (viewModel.FieldName.Contains("_NOTES") && val == "Not Applicable")
                    val = string.Empty;
                return val;
            }
            else
            {
                return val;
            }
        }

        private string ValueFormatter(PBPExportToMDBMappingViewModel viewModel, string val)
        {
            string Value = string.Empty;
            if (viewModel.TableName.Equals("PBPC_POS") && (viewModel.FieldName.Equals("PBP_C_POS_OUTPT_COINS_YN") || viewModel.FieldName.Equals("PBP_C_POS_OUTPT_COPAY_YN")))
            {
                if (val.ToUpper().Equals("YES") || val.ToUpper().Equals("Y"))
                {
                    Value = "1";
                }
                else if (val.ToUpper().Equals("NO") || val.ToUpper().Equals("N"))
                {
                    Value = "2";
                }
                else
                {
                    Value = val;
                }
            }
            else
            {
                Value = val;
            }

            return Value;
        }

        private string Get_PBP_C_POS_OUPT_BENCAT_BENS(PBPExportToMDBMappingViewModel viewModel, string val)
        {
            string Value = string.Empty;

            if (val.Equals("Medicare Covered,Non Medicare Covered"))
            {
                Value = "11";
            }
            else if (val.Equals("Medicare Covered"))
            {
                Value = "01";
            }
            else if (val.Equals("Non Medicare Covered"))
            {
                Value = "10";
            }
            else
            {
                Value = "";
            }
            return Value;
        }

        private void RemoveDuplicates(DataTable dt)
        {
            DataTable DeleteRow = new DataTable();
            if (dt.Rows.Count > 0)
            {
                DeleteRow = dt.Clone();
                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    if (i == 0)
                    {
                        break;
                    }
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (Convert.ToString(dt.Rows[i]["MRX_TIER_ID"]) == Convert.ToString(dt.Rows[j]["MRX_TIER_ID"])
                            && dt.Rows[i]["MRX_TIER_TYPE_ID"].ToString() == dt.Rows[j]["MRX_TIER_TYPE_ID"].ToString()
                            && dt.Rows[i]["MRX_TIER_BENEFIT_TYPE"].ToString() == dt.Rows[j]["MRX_TIER_BENEFIT_TYPE"].ToString())
                        {
                            dt.Rows[i].Delete();
                            break;
                        }
                    }
                }
                dt.AcceptChanges();
            }
        }

        private bool IsTableNeededToIncludeInExport(JObject source, string tblName, string qID, int PBPExportQueueID)
        {
            if (tblName.Equals("PBPC_OON") && qID.Equals("H7326001000"))
            {

            }
            bool IsInsert = true;
            try
            {
                string[] TableList = { "PBPMRX_P", "PBPMRX_T", "PBPC_OON", "PBPC_POS" };
                if (TableList.Contains(tblName))
                {
                    string PlanTypePath = "SectionA.SectionA1.PlanType";
                    JToken PlayTypeToken = source.SelectToken(PlanTypePath);
                    string PlanType = string.Empty;
                    if (PlayTypeToken != null)
                        PlanType = PlayTypeToken.ToString();

                    string NumberOfGroupsPath = "OONNumberofGroups.IndicatethenumberofOutofNetworkgroupingsofferedexcludingInpatientHospi";
                    JToken NumberOfGroupsToken = source.SelectToken(NumberOfGroupsPath);
                    string NumberOfGroups = string.Empty;
                    if (NumberOfGroupsToken != null)
                        NumberOfGroups = NumberOfGroupsToken.ToString();

                    string NumberOfPosGroupPath = "POSNumberofGroups.IndicatethenumberofPointofServicegroupingsofferedexcludingInpatientHos";
                    JToken NumberOfPosGroupToken = source.SelectToken(NumberOfPosGroupPath);
                    string NumberOfPosGroup = string.Empty;
                    if (NumberOfPosGroupToken != null)
                        NumberOfPosGroup = NumberOfPosGroupToken.ToString();

                    if (tblName.Equals("PBPC_OON"))
                    {
                        if (!string.IsNullOrEmpty(NumberOfGroups))
                        {
                            if (Convert.ToInt32(NumberOfGroups) > 0)
                            {
                                IsInsert = true;
                            }
                            else
                            {
                                IsInsert = false;
                            }
                        }
                        else
                        {
                            IsInsert = false;
                        }
                    }

                    if (tblName.Equals("PBPC_POS"))
                    {
                        if (!string.IsNullOrEmpty(PlanType) && !string.IsNullOrEmpty(NumberOfPosGroup))
                        {
                            if (PlanType != "29" && Convert.ToInt32(NumberOfPosGroup) > 0)
                            {
                                IsInsert = true;
                            }
                            else
                            {
                                IsInsert = false;
                            }
                        }
                        else
                        {
                            IsInsert = false;
                        }
                    }

                    if (tblName.Equals("PBPMRX_P") || tblName.Equals("PBPMRX_T"))
                    {
                        string MRXPath = "MedicareRxGeneral.MedicareRxGeneral1.DoesyourplanofferaMedicarePrescriptiondrugPartDbenefit";
                        JToken MRXToken = source.SelectToken(MRXPath);
                        string MRXValue = string.Empty;
                        if (MRXToken != null)
                            MRXValue = MRXToken.ToString();
                        if (MRXValue.Equals("1"))
                        {
                            IsInsert = true;
                        }
                        else
                        {
                            IsInsert = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddPBPExportActivityLog(PBPExportQueueID, "Error in Method IsTableNeededToIncludeInExport() for Table " + tblName, qID, null, ex);
            }
            return IsInsert;
        }

        private DataTable RemoveBlankRowIn_PBPMRX_T(DataTable td)
        {
            DataTable TempDataTable = new DataTable();
            TempDataTable = td.Clone();
            foreach (DataRow row in td.Rows)
            {
                if (!string.IsNullOrEmpty(row["MRX_TIER_ID"].ToString())
                    && !string.IsNullOrEmpty(row["MRX_TIER_TYPE_ID"].ToString())
                    && !string.IsNullOrEmpty(row["MRX_TIER_BENEFIT_TYPE"].ToString()))
                {
                    TempDataTable.ImportRow(row);
                }
            }
            return TempDataTable;
        }

        private DataTable RemoveBlankRowIn_PBPMRX_P(DataTable td)
        {
            DataTable TempDataTable = new DataTable();
            TempDataTable = td.Clone();
            foreach (DataRow row in td.Rows)
            {
                if (!string.IsNullOrEmpty(row["MRX_TIER_POST_ID"].ToString())
                    && !string.IsNullOrEmpty(row["MRX_TIER_POST_TYPE_ID"].ToString())
                    && !string.IsNullOrEmpty(row["MRX_TIER_POST_BENEFIT_TYPE"].ToString()))
                {
                    TempDataTable.ImportRow(row);
                }
            }
            return TempDataTable;
        }

        public int GetExportYear(int ExportQueueId)
        {
            return this._unitOfWork.RepositoryAsync<PBPExportQueue>().Get()
                 .Where(s => s.PBPExportQueueID.Equals(ExportQueueId))
                 .Select(s => s.PlanYear)
                 .FirstOrDefault();
        }


        private ServiceResult ProcessPlanAreaFile(string filePath, List<FormInstanceViewModel> formInstanceLst)
        {
            ServiceResult Result = new ServiceResult();
            OleDbHelper odedbHelper = new OleDbHelper();
            OleDbConnection conn = null;
            //OleDbCommand cmd = null;
            string ConnectionStr = odedbHelper.GetOleDbConnectingString(filePath);
            string QIDs = string.Empty;
            try
            {
                if (formInstanceLst.Count > 0)
                {
                    foreach (var item in formInstanceLst)
                    {
                        QIDs += "'" + item.Name + "',";
                    }
                    using (conn = new OleDbConnection(ConnectionStr))
                    {
                        conn.Open();
                        QIDs = QIDs.Remove(QIDs.Length - 1);
                        string CommandText = "Delete from PLAN_AREAS Where PBP_A_SQUISH_ID Not In(" + QIDs + ")";
                        //cmd = new OleDbCommand(CommandText, conn);
                        using (OleDbCommand cmd = new OleDbCommand(CommandText))
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = CommandText;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                Result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {

                //bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                //if (reThrow)
                //    Result.Result = ServiceResultStatus.Failure;
                //throw;
            }
            finally
            {
                conn.Close();

            }
            return Result;
        }

        private ServiceResult ProcessPlanAreaFile(string filePath, string QID)
        {
            ServiceResult Result = new ServiceResult();
            OleDbHelper odedbHelper = new OleDbHelper();
            string ConnectionStr = odedbHelper.GetOleDbConnectingString(filePath);
            string QIDs = string.Empty;

            try
            {
                using (OleDbConnection conn = new OleDbConnection(ConnectionStr))
                {
                    conn.Open();
                    QIDs = QIDs.Remove(QIDs.Length - 1);
                    string CommandText = "Delete from PLAN_AREAS Where PBP_A_SQUISH_ID Not In(" + QID + ")";
                    //cmd = new OleDbCommand(CommandText, conn);
                    using (OleDbCommand cmd = new OleDbCommand(CommandText))
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = CommandText;
                        cmd.ExecuteNonQuery();
                    }
                }
                Result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {

                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    Result.Result = ServiceResultStatus.Failure;
                throw;
            }
            return Result;
        }


        private string ApplySort(string CSV)
        {
            if (!String.IsNullOrEmpty(CSV))
            {
                string[] CSVArr = CSV.Split(';').Where(v => !String.IsNullOrEmpty(v)).ToArray();
                if (CSVArr.Length > 1)
                {
                    Array.Sort(CSVArr, new AlphanumComparatorFast());
                    CSV = String.Join(";", CSVArr);
                    CSV += string.Join(CSV, ";");
                }
            }
            return CSV;
        }

        public void PreProccessingLog(int PBPExportQueueID, string message, string userName, Exception ex)
        {
            PBPExportActivityLog activitylog = new PBPExportActivityLog();
            activitylog.PBPExportQueueID = PBPExportQueueID;
            activitylog.TableName = "";
            string innerException = string.Empty;
            if (ex != null)
                innerException = ex.InnerException == null ? "" : ex.InnerException.Message;
            activitylog.Message = message + " " + (ex == null ? "" : ex.Message + "->" + innerException + ": " + ex.StackTrace);
            activitylog.CreatedBy = userName;
            activitylog.CreatedDate = DateTime.Now;
            this._unitOfWork.RepositoryAsync<PBPExportActivityLog>().Insert(activitylog);
            this._unitOfWork.Save();
        }

        public void PreProcessingLogs(List<PBPExportLogViewModel> logs)
        {
            if(logs != null && logs.Count > 0)
            {
                foreach(PBPExportLogViewModel log in logs)
                {
                    PBPExportActivityLog activitylog = new PBPExportActivityLog();
                    activitylog.PBPExportQueueID = log.ExportQueueID;
                    activitylog.TableName = "";
                    string innerException = string.Empty;
                    Exception ex = log.Exception;
                    if (ex != null)
                        innerException = ex.InnerException == null ? "" : ex.InnerException.Message;
                    activitylog.Message = log.Message + " " + (ex == null ? "" : ex.Message + "->" + innerException + ": " + ex.StackTrace);
                    activitylog.CreatedBy = log.UserName;
                    activitylog.CreatedDate = log.LogTime;
                    this._unitOfWork.RepositoryAsync<PBPExportActivityLog>().Insert(activitylog);
                }
                this._unitOfWork.Save();
            }
        }

        private string GetTableName(string tblName)
        {   
            tblName = tblName.Replace("_Base10", "");
            tblName = tblName.Replace("_Base11", "");
            tblName = tblName.Replace("_Base12", "");
            tblName = tblName.Replace("_Base13", "");
            tblName = tblName.Replace("_Base14", "");
            tblName = tblName.Replace("_Base15", "");
            tblName = tblName.Replace("_Base16", "");
            tblName = tblName.Replace("_Base17", "");
            tblName = tblName.Replace("_Base18", "");
            tblName = tblName.Replace("_Base19", "");
            tblName = tblName.Replace("_Base20", "");
            tblName = tblName.Replace("_Base21", "");
            tblName = tblName.Replace("_Base22", "");
            tblName = tblName.Replace("_Base23", "");
            tblName = tblName.Replace("_Base24", "");
            tblName = tblName.Replace("_Base25", "");
            tblName = tblName.Replace("_Base26", "");
            tblName = tblName.Replace("_Base27", "");
            tblName = tblName.Replace("_Base28", "");
            tblName = tblName.Replace("_Base29", "");
            tblName = tblName.Replace("_Base30", "");
            tblName = tblName.Replace("_Base31", "");
            tblName = tblName.Replace("_Base32", "");
            tblName = tblName.Replace("_Base33", "");
            tblName = tblName.Replace("_Base34", "");
            tblName = tblName.Replace("_Base35", "");
            tblName = tblName.Replace("_Base36", "");
            tblName = tblName.Replace("_Base37", "");
            tblName = tblName.Replace("_Base38", "");
            tblName = tblName.Replace("_Base39", "");
            tblName = tblName.Replace("_Base40", "");
            tblName = tblName.Replace("_Base41", "");
            tblName = tblName.Replace("_Base42", "");
            tblName = tblName.Replace("_Base43", "");
            tblName = tblName.Replace("_Base44", "");
            tblName = tblName.Replace("_Base45", "");
            tblName = tblName.Replace("_Base46", "");
            tblName = tblName.Replace("_Base47", "");
            tblName = tblName.Replace("_Base48", "");
            tblName = tblName.Replace("_Base49", "");
            tblName = tblName.Replace("_Base50", "");
            tblName = tblName.Replace("_Base51", "");
            tblName = tblName.Replace("_Base52", "");
            tblName = tblName.Replace("_Base53", "");
            tblName = tblName.Replace("_Base54", "");
            tblName = tblName.Replace("_Base55", "");
            tblName = tblName.Replace("_Base56", "");
            tblName = tblName.Replace("_Base57", "");
            tblName = tblName.Replace("_Base58", "");
            tblName = tblName.Replace("_Base59", "");
            tblName = tblName.Replace("_Base60", "");
            tblName = tblName.Replace("_Base61", "");
            tblName = tblName.Replace("_Base62", "");
            tblName = tblName.Replace("_Base63", "");
            tblName = tblName.Replace("_Base64", "");
            tblName = tblName.Replace("_Base65", "");
            tblName = tblName.Replace("_Base66", "");
            tblName = tblName.Replace("_Base67", "");
            tblName = tblName.Replace("_Base68", "");
            tblName = tblName.Replace("_Base69", "");
            tblName = tblName.Replace("_Base70", "");
            tblName = tblName.Replace("_Base71", "");
            tblName = tblName.Replace("_Base72", "");
            tblName = tblName.Replace("_Base73", "");
            tblName = tblName.Replace("_Base74", "");
            tblName = tblName.Replace("_Base75", "");
            tblName = tblName.Replace("_Base76", "");
            tblName = tblName.Replace("_Base77", "");
            tblName = tblName.Replace("_Base78", "");
            tblName = tblName.Replace("_Base79", "");
            tblName = tblName.Replace("_Base80", "");
            tblName = tblName.Replace("_Base81", "");
            tblName = tblName.Replace("_Base82", "");
            tblName = tblName.Replace("_Base83", "");
            tblName = tblName.Replace("_Base84", "");
            tblName = tblName.Replace("_Base85", "");
            tblName = tblName.Replace("_Base86", "");
            tblName = tblName.Replace("_Base87", "");
            tblName = tblName.Replace("_Base88", "");
            tblName = tblName.Replace("_Base89", "");
            tblName = tblName.Replace("_Base90", "");
            tblName = tblName.Replace("_Base1", "");
            tblName = tblName.Replace("_Base2", "");
            tblName = tblName.Replace("_Base3", "");
            tblName = tblName.Replace("_Base4", "");
            tblName = tblName.Replace("_Base5", "");
            tblName = tblName.Replace("_Base6", "");
            tblName = tblName.Replace("_Base7", "");
            tblName = tblName.Replace("_Base8", "");
            tblName = tblName.Replace("_Base9", "");


            tblName = tblName.Replace("_BASE10", "");
            tblName = tblName.Replace("_BASE11", "");
            tblName = tblName.Replace("_BASE12", "");
            tblName = tblName.Replace("_BASE13", "");
            tblName = tblName.Replace("_BASE14", "");
            tblName = tblName.Replace("_BASE15", "");
            tblName = tblName.Replace("_BASE16", "");
            tblName = tblName.Replace("_BASE17", "");
            tblName = tblName.Replace("_BASE18", "");
            tblName = tblName.Replace("_BASE19", "");
            tblName = tblName.Replace("_BASE20", "");
            tblName = tblName.Replace("_BASE21", "");
            tblName = tblName.Replace("_BASE22", "");
            tblName = tblName.Replace("_BASE23", "");
            tblName = tblName.Replace("_BASE24", "");
            tblName = tblName.Replace("_BASE25", "");
            tblName = tblName.Replace("_BASE26", "");
            tblName = tblName.Replace("_BASE27", "");
            tblName = tblName.Replace("_BASE28", "");
            tblName = tblName.Replace("_BASE29", "");
            tblName = tblName.Replace("_BASE30", "");
            tblName = tblName.Replace("_BASE31", "");
            tblName = tblName.Replace("_BASE32", "");
            tblName = tblName.Replace("_BASE33", "");
            tblName = tblName.Replace("_BASE34", "");
            tblName = tblName.Replace("_BASE35", "");
            tblName = tblName.Replace("_BASE36", "");
            tblName = tblName.Replace("_BASE37", "");
            tblName = tblName.Replace("_BASE38", "");
            tblName = tblName.Replace("_BASE39", "");
            tblName = tblName.Replace("_BASE40", "");
            tblName = tblName.Replace("_BASE41", "");
            tblName = tblName.Replace("_BASE42", "");
            tblName = tblName.Replace("_BASE43", "");
            tblName = tblName.Replace("_BASE44", "");
            tblName = tblName.Replace("_BASE45", "");
            tblName = tblName.Replace("_BASE46", "");
            tblName = tblName.Replace("_BASE47", "");
            tblName = tblName.Replace("_BASE48", "");
            tblName = tblName.Replace("_BASE49", "");
            tblName = tblName.Replace("_BASE50", "");
            tblName = tblName.Replace("_BASE51", "");
            tblName = tblName.Replace("_BASE52", "");
            tblName = tblName.Replace("_BASE53", "");
            tblName = tblName.Replace("_BASE54", "");
            tblName = tblName.Replace("_BASE55", "");
            tblName = tblName.Replace("_BASE56", "");
            tblName = tblName.Replace("_BASE57", "");
            tblName = tblName.Replace("_BASE58", "");
            tblName = tblName.Replace("_BASE59", "");
            tblName = tblName.Replace("_BASE60", "");
            tblName = tblName.Replace("_BASE61", "");
            tblName = tblName.Replace("_BASE62", "");
            tblName = tblName.Replace("_BASE63", "");
            tblName = tblName.Replace("_BASE64", "");
            tblName = tblName.Replace("_BASE65", "");
            tblName = tblName.Replace("_BASE66", "");
            tblName = tblName.Replace("_BASE67", "");
            tblName = tblName.Replace("_BASE68", "");
            tblName = tblName.Replace("_BASE69", "");
            tblName = tblName.Replace("_BASE70", "");
            tblName = tblName.Replace("_BASE71", "");
            tblName = tblName.Replace("_BASE72", "");
            tblName = tblName.Replace("_BASE73", "");
            tblName = tblName.Replace("_BASE74", "");
            tblName = tblName.Replace("_BASE75", "");
            tblName = tblName.Replace("_BASE76", "");
            tblName = tblName.Replace("_BASE77", "");
            tblName = tblName.Replace("_BASE78", "");
            tblName = tblName.Replace("_BASE79", "");
            tblName = tblName.Replace("_BASE80", "");
            tblName = tblName.Replace("_BASE81", "");
            tblName = tblName.Replace("_BASE82", "");
            tblName = tblName.Replace("_BASE83", "");
            tblName = tblName.Replace("_BASE84", "");
            tblName = tblName.Replace("_BASE85", "");
            tblName = tblName.Replace("_BASE86", "");
            tblName = tblName.Replace("_BASE87", "");
            tblName = tblName.Replace("_BASE88", "");
            tblName = tblName.Replace("_BASE89", "");
            tblName = tblName.Replace("_BASE90", "");
            tblName = tblName.Replace("_BASE1", "");
            tblName = tblName.Replace("_BASE2", "");
            tblName = tblName.Replace("_BASE3", "");
            tblName = tblName.Replace("_BASE4", "");
            tblName = tblName.Replace("_BASE5", "");
            tblName = tblName.Replace("_BASE6", "");
            tblName = tblName.Replace("_BASE7", "");
            tblName = tblName.Replace("_BASE8", "");
            tblName = tblName.Replace("_BASE9", "");

            return tblName;
        }
    }
    public class PBPSoftwareVersionVeiwModel
    {
        public string PBPSoftwareVersionName { get; set; }
        public string TestQaVesrion { get; set; }
    }

    public class AlphanumComparatorFast : IComparer
    {
        List<string> GetList(string s1)
        {
            List<string> SB1 = new List<string>();
            string st1, st2, st3;
            st1 = "";
            bool flag = char.IsDigit(s1[0]);
            foreach (char c in s1)
            {
                if (flag != char.IsDigit(c) || c == '\'')
                {
                    if (st1 != "")
                        SB1.Add(st1);
                    st1 = "";
                    flag = char.IsDigit(c);
                }
                if (char.IsDigit(c))
                {
                    st1 += c;
                }
                if (char.IsLetter(c))
                {
                    st1 += c;
                }


            }
            SB1.Add(st1);
            return SB1;
        }
        public int Compare(object x, object y)
        {
            string s1 = x as string;
            if (s1 == null)
            {
                return 0;
            }
            string s2 = y as string;
            if (s2 == null)
            {
                return 0;
            }
            if (s1 == s2)
            {
                return 0;
            }
            int len1 = s1.Length;
            int len2 = s2.Length;
            int marker1 = 0;
            int marker2 = 0;

            // Walk through two the strings with two markers.
            List<string> str1 = GetList(s1);
            List<string> str2 = GetList(s2);
            while (str1.Count != str2.Count)
            {
                if (str1.Count < str2.Count)
                {
                    str1.Add("");
                }
                else
                {
                    str2.Add("");
                }
            }
            int x1 = 0; int res = 0; int x2 = 0; string y2 = "";
            bool status = false;
            string y1 = ""; bool s1Status = false; bool s2Status = false;
            //s1status ==false then string ele int;
            //s2status ==false then string ele int;
            int result = 0;
            for (int i = 0; i < str1.Count && i < str2.Count; i++)
            {
                status = int.TryParse(str1[i].ToString(), out res);
                if (res == 0)
                {
                    y1 = str1[i].ToString();
                    s1Status = false;
                }
                else
                {
                    x1 = Convert.ToInt32(str1[i].ToString());
                    s1Status = true;
                }

                status = int.TryParse(str2[i].ToString(), out res);
                if (res == 0)
                {
                    y2 = str2[i].ToString();
                    s2Status = false;
                }
                else
                {
                    x2 = Convert.ToInt32(str2[i].ToString());
                    s2Status = true;
                }
                //checking --the data comparision
                if (!s2Status && !s1Status)    //both are strings
                {
                    result = str1[i].CompareTo(str2[i]);
                }
                else if (s2Status && s1Status) //both are intergers
                {
                    if (x1 == x2)
                    {
                        if (str1[i].ToString().Length < str2[i].ToString().Length)
                        {
                            result = 1;
                        }
                        else if (str1[i].ToString().Length > str2[i].ToString().Length)
                            result = -1;
                        else
                            result = 0;
                    }
                    else
                    {
                        int st1ZeroCount = str1[i].ToString().Trim().Length - str1[i].ToString().TrimStart(new char[] { '0' }).Length;
                        int st2ZeroCount = str2[i].ToString().Trim().Length - str2[i].ToString().TrimStart(new char[] { '0' }).Length;
                        if (st1ZeroCount > st2ZeroCount)
                            result = -1;
                        else if (st1ZeroCount < st2ZeroCount)
                            result = 1;
                        else
                            result = x1.CompareTo(x2);

                    }
                }
                else
                {
                    result = str1[i].CompareTo(str2[i]);
                }
                if (result == 0)
                {
                    continue;
                }
                else
                    break;

            }
            return result;
        }
                
    }
}