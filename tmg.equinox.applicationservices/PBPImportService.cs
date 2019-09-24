using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.extensions;
using tmg.equinox.repository.interfaces;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using System.Data.OleDb;
using System.Data;
using System.Configuration;
using Newtonsoft.Json;
using System.IO;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using Newtonsoft.Json.Linq;
using tmg.equinox.applicationservices.FolderVersionDetail;
using System.Web.Configuration;

namespace tmg.equinox.applicationservices
{
    public class PBPImportService : IPBPImportService
    {
        #region Private Memebers
        public IUnitOfWorkAsync _unitOfWork { get; set; }
        //public string PBPIMPORTFILEPATH = ConfigurationManager.AppSettings["PBPIMPORTFILEPATH"].ToString();
        public string PBPTABLENAME = ConfigurationManager.AppSettings["PBPTABLENAME"].ToString();
        public string PBPPLAN_AREASTABLENAME = ConfigurationManager.AppSettings["PBPPLAN_AREASTABLENAME"].ToString();
        private AccessDbContext _accessDbContext;
        private string _mdfFileNameWithPath;
        private string _applicationConnectingString;
        private IFolderVersionServices _folderVersionService;
        private IUnitOfWorkAsync _unitOfWorkAsync;
        private ILoggingService _loggingService;
        private IDomainModelService _domainModelService;
        private string connectionString;
        private List<dynamic> _anchors;
        //private List<FormInstanceCollateralViewModel> _childInstances;
        string IMPORTFILEPATH = ConfigurationManager.AppSettings["PBPImportFiles"].ToString();
        #endregion Private Members

        #region Constructor

        public PBPImportService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        #endregion Constructor

        #region PBPImport

        public GridPagingResponse<PBPImportQueueViewModel> GetPBPImportQueueList(int tenantID, GridPagingRequest gridPagingRequest)
        {
            List<PBPImportQueueViewModel> PBPImportQueue = null;
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);
                var currentFolders = from s in "st"
                                     select new
                                     {
                                         FolderID = 0,
                                         FolderVersionID = 0,
                                         VersionCount = 0
                                     };

                PBPImportQueue = (from queue in this._unitOfWork.RepositoryAsync<PBPImportQueue>().Get()
                                  join db in this._unitOfWork.RepositoryAsync<PBPDatabase>().Get()
                                  on queue.PBPDatabase1Up equals db.PBPDatabase1Up
                                  select new PBPImportQueueViewModel
                                  {
                                      PBPImportQueueID = queue.PBPImportQueueID,
                                      Description = queue.Description,
                                      PBPFileName = queue.PBPFileName,
                                      PBPPlanAreaFileName = queue.PBPPlanAreaFileName,
                                      Year = queue.Year,
                                      ImportStartDate = queue.ImportStartDate,
                                      CreatedBy = queue.CreatedBy,
                                      Status = queue.Status,
                                      StatusCode = queue.Status,
                                      CreatedDate = queue.CreatedDate,
                                      PBPDatabase1Up = queue.PBPDatabase1Up,
                                      DataBaseName = db.DataBaseName
                                  }).OrderByDescending(s => s.CreatedDate).ToList().ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                 .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<PBPImportQueueViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, PBPImportQueue);
        }

        public ServiceResult AddPBPImportQueue(int tenantId, PBPImportQueueViewModel pBPImportQueueViewModel, out int PBPImportQueueID)
        {
            ServiceResult result = new ServiceResult();
            PBPImportQueueID = 0;
            try
            {

                PBPImportQueue AddToImport = new PBPImportQueue();
                AddToImport.Description = pBPImportQueueViewModel.Description;
                AddToImport.PBPFileName = pBPImportQueueViewModel.PBPFileName;
                AddToImport.PBPPlanAreaFileName = pBPImportQueueViewModel.PBPPlanAreaFileName;
                AddToImport.Location = pBPImportQueueViewModel.Location;
                AddToImport.PBPDatabase1Up = pBPImportQueueViewModel.PBPDatabase1Up;
                AddToImport.ImportStartDate = pBPImportQueueViewModel.ImportStartDate;
                AddToImport.ImportEndDate = pBPImportQueueViewModel.ImportEndDate;
                AddToImport.Status = (int)domain.entities.Enums.ProcessStatusMasterCode.InReview;
                AddToImport.Year = pBPImportQueueViewModel.Year;
                AddToImport.PBPDataBase = pBPImportQueueViewModel.PBPDataBase;
                AddToImport.CreatedBy = pBPImportQueueViewModel.CreatedBy;
                AddToImport.CreatedDate = DateTime.Now;
                AddToImport.IsFullMigration = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsFullMigration"]);
                this._unitOfWork.RepositoryAsync<PBPImportQueue>().Insert(AddToImport);
                this._unitOfWork.Save();
                PBPImportQueueID = AddToImport.PBPImportQueueID;

                result.Result = ServiceResultStatus.Success;
                //GetQID();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
            }
            return result;
        }

        public PlanConfigurationViewModel ProcessPlanConfiguration(List<PBPTableViewModel> PBPTableData, string CreateBy)
        {
            string FileName = "";
            int MatchIndex = 0, MisMatchIndex = 0; ;
            int PBPImportQueueID = 0;
            int PBPMatchConfig1Up = 0;
            List<PBPPlanViewModel> PlanList = null;
            ServiceResult result = new ServiceResult();
            PlanConfigurationViewModel PlanConfigrationLists = new PlanConfigurationViewModel();
            PBPPlanConfigViewModel MatchingPlan = new PBPPlanConfigViewModel();
            try
            {

                foreach (var item in PBPTableData)
                {

                    PBPImportQueueID = item.PBPImportQueueID;
                    PlanList = item.PBPPlanList;
                    PBPImportQueueViewModel ViewModel = GetPBPDataBaseID(PBPImportQueueID);
                    string DataBaseName = PBPDatabaseNameById(ViewModel.PBPDatabase1Up);
                    FileName = DataBaseName;
                    foreach (var PbPPlanItem in PlanList)
                        if (IsPlanPresentIneBS(PbPPlanItem.QID) == false)
                        {
                            PBPMatchConfig1Up = SavePBPConfig(PBPImportQueueID, PbPPlanItem, CreateBy);
                            MisMatchIndex = MisMatchIndex + 1;
                            PlanConfigrationLists.MisMatchPlanList.Add(new PBPPlanConfigViewModel
                            {
                                PBPImportQueueID = PBPImportQueueID,
                                PBPMatchConfig1Up = PBPMatchConfig1Up,
                                Index = MisMatchIndex,
                                QID = PbPPlanItem.QID,
                                PlanName = PbPPlanItem.PBPPlanName,
                                PlanNumber = PbPPlanItem.PBPPlanNumber,
                                ebsPlanName = "--",
                                eBsPlanNumber = "--",
                                DataBaseFileName = FileName
                            });
                            result.Result = ServiceResultStatus.Success;
                        }
                        else
                        {
                            PBPMatchConfig1Up = SavePBPConfig(PBPImportQueueID, PbPPlanItem, CreateBy);
                            MatchingPlan = GetMatchingPlanDetails(PbPPlanItem.QID);
                            if (MatchingPlan != null)
                            {
                                MatchingPlan.PBPMatchConfig1Up = PBPMatchConfig1Up;
                                MatchIndex = MatchIndex + 1;
                                MatchingPlan.Index = MatchIndex;
                                MatchingPlan.PBPImportQueueID = PBPImportQueueID;
                                PlanConfigrationLists.MatchPlanList.Add(MatchingPlan);
                            }
                        }
                }
            }

            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
            }
            return PlanConfigrationLists;
        }

        public int SavePBPConfig(int PBPImportQueueID, PBPPlanViewModel PBPPlanModel, string CreateBy)
        {
            int PBPMatchConfig1Up = 0;
            try
            {
                PBPMatchConfig AddToImport = new PBPMatchConfig();
                AddToImport.QID = PBPPlanModel.QID;
                AddToImport.PlanName = PBPPlanModel.PBPPlanName;
                AddToImport.PlanNumber = PBPPlanModel.PBPPlanNumber;
                AddToImport.ebsPlanName = PBPPlanModel.PBPPlanName;
                AddToImport.ebsPlanNumber = PBPPlanModel.PBPPlanNumber;
                AddToImport.PBPImportQueueID = PBPImportQueueID;
                AddToImport.CreatedDate = DateTime.Now.Date;
                AddToImport.IsActive = true;
                AddToImport.CreatedBy = CreateBy;
                this._unitOfWork.RepositoryAsync<PBPMatchConfig>().Insert(AddToImport);
                this._unitOfWork.Save();
                PBPMatchConfig1Up = AddToImport.PBPMatchConfig1Up;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return PBPMatchConfig1Up;
        }

        public ServiceResult DeletePBPConfig(int PBPImportQueueID, string UpdateBy)
        {
            ServiceResult result = new ServiceResult();
            List<PBPMatchConfig> itemToUpdateList = this._unitOfWork.RepositoryAsync<PBPMatchConfig>().Get()
                                           .Where(s => s.PBPImportQueueID.Equals(PBPImportQueueID)
                                           && s.IsActive == true).ToList();
            try
            {
                if (itemToUpdateList != null)
                {
                    foreach (var item in itemToUpdateList)
                    {
                        item.IsActive = false;
                        using (var scope = new TransactionScope())
                        {
                            this._unitOfWork.RepositoryAsync<PBPMatchConfig>().Update(item);
                            //this._unitOfWork.Save();
                            this._unitOfWork.Save();
                            scope.Complete();
                        }
                        result.Result = ServiceResultStatus.Success;
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
            }
            return result;
        }

        public bool IsPlanPresentIneBS(string qID)
        {
            bool IsExist = false;
            IList<PBPPlanConfigViewModel> PlanList = GetPBPPlanDetailsList();
            IsExist = PlanList.Where(s => s.PlanNumber.Equals(qID)).Any();
            return IsExist;
        }
        //Creaete
        public PlanConfigurationViewModel CreateMatchAndMisMatchPlanLists(int PBPImportQueueID)
        {
            int UnMatchIndex = 0;
            int MatchIndex = 0;
            PlanConfigurationViewModel ViewModel = new PlanConfigurationViewModel();
            IList<PBPPlanConfigViewModel> UnMatchPlanList = new List<PBPPlanConfigViewModel>();
            IList<PBPPlanConfigViewModel> MatchPlanList = new List<PBPPlanConfigViewModel>();
            FolderDetailsViewModel FolderDetailViewModel = GetFolderDetails();

            IList<PBPPlanConfigViewModel> PlanList = (from queue in this._unitOfWork.RepositoryAsync<PBPMatchConfig>()
                        .Get()
                        .Where(s => s.IsActive == true
                        && s.PBPImportQueueID.Equals(PBPImportQueueID)
                        )
                                                      select new PBPPlanConfigViewModel
                                                      {
                                                          PBPMatchConfig1Up = queue.PBPMatchConfig1Up,
                                                          PBPImportQueueID = queue.PBPImportQueueID,
                                                          QID = queue.QID,
                                                          PlanName = queue.PlanName,
                                                          PlanNumber = queue.PlanNumber,
                                                          ebsPlanName = queue.QID,
                                                          eBsPlanNumber = queue.ebsPlanNumber,
                                                          FormInstanceId = queue.FormInstanceID,
                                                          DocumentId = queue.DocId,
                                                          FolderId = queue.FolderId,
                                                          FolderVersionId = queue.FolderVersionId,
                                                          UserAction = queue.UserAction,
                                                      }).ToList();
            if (PlanList != null)
            {
                PlanList = PlanListHelper(PlanList, PBPImportQueueID);
            }

            foreach (var plan in PlanList)
            {
                if (IsPlanPresentIneBS(plan.QID) == false)
                {
                    UnMatchIndex = UnMatchIndex + 1;
                    plan.Index = UnMatchIndex;
                    plan.ebsPlanName = "--";
                    plan.eBsPlanNumber = "--";
                    plan.PBPImportQueueID = PBPImportQueueID;
                    UnMatchPlanList.Add(plan);
                }
                else
                {
                    PBPPlanConfigViewModel MatchPlanDetails = GetMatchingPlanDetailsByQID(plan.QID);
                    if (MatchPlanDetails != null)
                    {
                        MatchIndex = MatchIndex + 1;
                        MatchPlanDetails.Index = MatchIndex;
                        MatchPlanDetails.PBPImportQueueID = PBPImportQueueID;
                        MatchPlanList.Add(MatchPlanDetails);
                        if (MatchPlanList != null)
                        {
                            MatchPlanList = PlanListHelper(MatchPlanList, PBPImportQueueID);
                        }
                    }
                }
            }

            ViewModel.MatchPlanList = MatchPlanList;
            ViewModel.MisMatchPlanList = UnMatchPlanList;
            return ViewModel;
        }

        public IList<PBPPlanConfigViewModel> GetPBPPlanDetailsList()
        {
            IList<PBPPlanConfigViewModel> PlanList = null;

            PlanList = (from queue in this._unitOfWork.RepositoryAsync<PBPImportDetails>()
                        .Get()
                        .Where(s => s.IsActive == true)
                        select new PBPPlanConfigViewModel
                        {
                            QID = queue.QID,
                            PlanName = queue.PlanName,
                            PlanNumber = queue.PlanNumber
                        }).ToList();

            return PlanList;
        }

        public PBPPlanConfigViewModel GetMatchingPlanDetails(string qID)
        {
            PBPPlanConfigViewModel PlanDetails = new PBPPlanConfigViewModel();

            IList<Folder> FolderList = this._unitOfWork.RepositoryAsync<Folder>().Get().ToList();
            IList<FolderVersion> FolderVersionList = this._unitOfWork.RepositoryAsync<FolderVersion>().Get().ToList();
            IList<FormInstance> FormInstanceList = this._unitOfWork.RepositoryAsync<FormInstance>().Get().ToList();

            if (!string.IsNullOrEmpty(qID))
            {
                PlanDetails = (from queue in this._unitOfWork.RepositoryAsync<PBPImportDetails>().Get()
                               where (queue.IsActive == true && queue.PlanNumber.Equals(qID))
                               select new PBPPlanConfigViewModel
                               {
                                   QID = queue.QID,
                                   PlanName = queue.PlanName,
                                   PlanNumber = queue.PlanNumber,
                                   ebsPlanName = queue.ebsPlanName,
                                   eBsPlanNumber = queue.ebsPlanNumber,
                                   FormInstanceId = queue.FormInstanceID,
                                   FolderId = queue.FolderId,
                                   FolderVersionId = queue.FolderVersionId,
                                   //FolderVersionName= fl.Name +"-"+ flVersion.FolderVersionNumber,
                                   DocumentId = queue.DocId,
                                   Year = queue.Year,
                                   UserAction = (int)PBPUserActionList.UpdatePlan,
                                   PBPDataBase1Up = queue.PBPDatabase1Up
                               }).FirstOrDefault();

                PlanDetails.ProductDetails = ProductDetailsHelper(PlanDetails.FolderId, PlanDetails.FolderVersionId, PlanDetails.FormInstanceId);
                PlanDetails.DataBaseFileName = PBPDatabaseNameById(PlanDetails.PBPDataBase1Up);
            }
            return PlanDetails;
        }

        private PBPPlanConfigViewModel GetMatchingPlanDetailsByQID(string qID)
        {
            PBPPlanConfigViewModel MatchPlanDetails =
                    (from queue in this._unitOfWork.RepositoryAsync<PBPImportDetails>()
                        .Get()
                        .Where(s => s.IsActive == true && s.QID.Equals(qID))
                     select new PBPPlanConfigViewModel
                     {
                         QID = queue.QID,
                         PlanName = queue.PlanName,
                         PlanNumber = queue.PlanNumber,
                         ebsPlanName = queue.ebsPlanName,
                         eBsPlanNumber = queue.ebsPlanNumber,
                         FormInstanceId = queue.FormInstanceID,
                         FolderId = queue.FolderId,
                         FolderVersionId = queue.FolderVersionId,
                         //FolderVersionName= fl.Name +"-"+ flVersion.FolderVersionNumber,
                         DocumentId = queue.DocId,
                         Year = queue.Year,
                         UserAction = (int)PBPUserActionList.UpdatePlan,
                         PBPDataBase1Up = queue.PBPDatabase1Up
                     }).FirstOrDefault();
            return MatchPlanDetails;
        }

        public ServiceResult SavePBPPlanDetails(int tenantId, IList<PlanMatchingConfigurationViewModel> PlanConfigrationDetailList, string createdBy)
        {
            ServiceResult Result = new ServiceResult();
            try
            {
                PBPImportDetails AddToImport = null;
                int ImportID = PlanConfigrationDetailList.Select(s => s.PBPImportQueueID).FirstOrDefault();
                Result = DeletePBPConfig(ImportID, createdBy);

                PBPImportQueueViewModel ViewModel = GetPBPDataBaseID(ImportID);

                foreach (var item in PlanConfigrationDetailList)
                {
                    AddToImport = new PBPImportDetails();
                    AddToImport.PBPImportQueueID = item.PBPImportQueueID;
                    AddToImport.QID = item.PBPPlanNumber;
                    AddToImport.PlanName = item.PBPPlanName;
                    AddToImport.PlanNumber = item.PBPPlanNumber;
                    //AddToImport.ebsPlanName = item.EBSPlanName;
                    AddToImport.FormInstanceID = item.FormInstanceId;
                    AddToImport.DocId = item.DocumentId;
                    AddToImport.FolderId = item.FolderId;
                    AddToImport.FolderVersionId = item.FolderVersionId;
                    //AddToImport.Status = item.st;
                    AddToImport.UserAction = item.UserActionId;
                    AddToImport.PBPDatabase = item.DataBaseFileName;
                    AddToImport.CreatedBy = createdBy;
                    AddToImport.CreatedDate = DateTime.Now;
                    AddToImport.IsActive = true;
                    AddToImport.Status = (int)domain.entities.Enums.ProcessStatusMasterCode.Queued;
                    AddToImport.PBPDatabase1Up = ViewModel.PBPDatabase1Up;
                    AddToImport.IsIncludeInEbs = item.IsIncludeInEbs;
                    this._unitOfWork.RepositoryAsync<PBPImportDetails>().Insert(AddToImport);
                    this._unitOfWork.Save();
                    Result.Result = ServiceResultStatus.Success;
                }

                ServiceResult ImportQueueStatusResult = UpdateImportQueueStatus(ImportID, domain.entities.Enums.ProcessStatusMasterCode.Queued);
            }
            catch (Exception ex)
            {
                Result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return Result;
        }

        public ServiceResult UpdateImportQueueStatus(int PBPImportQueueID, domain.entities.Enums.ProcessStatusMasterCode status)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                PBPImportQueue itemToUpdateList = this._unitOfWork.RepositoryAsync<PBPImportQueue>().Get()
                                                .Where(s => s.PBPImportQueueID.Equals(PBPImportQueueID)
                                                ).FirstOrDefault();

                if (itemToUpdateList != null)
                {
                    itemToUpdateList.Status = (int)status;
                    using (var scope = new TransactionScope())
                    {
                        //this._unitOfWork.RepositoryAsync<PBPImportQueue>
                        this._unitOfWork.RepositoryAsync<PBPImportQueue>().Update(itemToUpdateList);
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

        public DataTable GetExportQueueDataTable()
        {
            List<PBPImportQueueViewModel> PBPImportQueueList = null;
            string ProcessStatus = string.Empty;
            DataTable ExportDataTable = null;

            PBPImportQueueList = (from queue in this._unitOfWork.RepositoryAsync<PBPImportQueue>().Get()
                                  join db in this._unitOfWork.RepositoryAsync<PBPDatabase>().Get()
                                  on queue.PBPDatabase1Up equals db.PBPDatabase1Up
                                  select new PBPImportQueueViewModel
                                  {
                                      PBPImportQueueID = queue.PBPImportQueueID,
                                      Description = queue.Description,
                                      PBPFileName = queue.PBPFileName,
                                      PBPPlanAreaFileName = queue.PBPPlanAreaFileName,
                                      Year = queue.Year,
                                      ImportStartDate = queue.ImportStartDate,
                                      CreatedBy = queue.CreatedBy,
                                      Status = queue.Status,
                                      StatusCode = queue.Status,
                                      CreatedDate = queue.CreatedDate,
                                      PBPDatabase1Up = queue.PBPDatabase1Up,
                                      DataBaseName = db.DataBaseName
                                  }).OrderByDescending(s => s.CreatedDate).ToList();
            if (PBPImportQueueList != null)
            {
                ExportDataTable = new DataTable();
                ExportDataTable.Columns.Add("PBPImportQueueID");
                ExportDataTable.Columns.Add("DatabaseName");
                ExportDataTable.Columns.Add("Description");
                ExportDataTable.Columns.Add("PBPFileName");
                ExportDataTable.Columns.Add("PBPPlanAreaFileName");
                //ExportDataTable.Columns.Add("Year");
                ExportDataTable.Columns.Add("CreatedDate");
                ExportDataTable.Columns.Add("CreatedBy");
                ExportDataTable.Columns.Add("Status");

                foreach (var item in PBPImportQueueList)
                {
                    DataRow row = ExportDataTable.NewRow();
                    row["PBPImportQueueID"] = item.PBPImportQueueID;
                    row["DatabaseName"] = item.DataBaseName;
                    row["Description"] = item.Description;
                    row["PBPFileName"] = item.PBPFileName.ToString();
                    row["PBPPlanAreaFileName"] = item.PBPPlanAreaFileName;
                    //row["Year"] = item.Year;
                    row["CreatedDate"] = item.CreatedDate.ToString();
                    row["CreatedBy"] = item.CreatedBy;
                    ProcessStatusKeyValuePair.TryGetValue(item.Status, out ProcessStatus);
                    row["Status"] = ProcessStatus;
                    ExportDataTable.Rows.Add(row);

                }
            }

            return ExportDataTable;
        }

        public List<PBPImportQueueViewModel> GetQueuedPBPImportList()
        {
            List<PBPImportQueueViewModel> PBPImportQueueList = null;
            try
            {
                PBPImportQueueList = (from queue in this._unitOfWork.RepositoryAsync<PBPImportQueue>().Get()
                                      join db in this._unitOfWork.RepositoryAsync<PBPDatabase>().Get()
                                      on queue.PBPDatabase1Up equals db.PBPDatabase1Up
                                      where (queue.Status.Equals((int)domain.entities.Enums.ProcessStatusMasterCode.Queued))
                                      select new PBPImportQueueViewModel
                                      {
                                          PBPImportQueueID = queue.PBPImportQueueID,
                                          Description = queue.Description,
                                          PBPFileName = queue.PBPFileName,
                                          PBPPlanAreaFileName = queue.PBPPlanAreaFileName,
                                          Year = queue.Year,
                                          ImportStartDate = queue.ImportStartDate,
                                          CreatedBy = queue.CreatedBy,
                                          Status = queue.Status,
                                          StatusCode = queue.Status,
                                          CreatedDate = queue.CreatedDate,
                                          PBPDatabase1Up = queue.PBPDatabase1Up,
                                          DataBaseName = db.DataBaseName,
                                          IsFullMigration = queue.IsFullMigration
                                      }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return PBPImportQueueList;
        }

        public List<PBPPlanConfigViewModel> GetPBPPlanDetailsForProcess(int pBPImportQueueID)
        {
            List<PBPPlanConfigViewModel> PlanDetailList = (from queue in this._unitOfWork.RepositoryAsync<PBPImportDetails>().Get()
                                                           where (queue.IsActive == true
                                                           && queue.PBPImportQueueID == pBPImportQueueID
                                                           && queue.Status.Equals((int)ProcessStatusMasterCode.Queued)
                                                           )
                                                           select new PBPPlanConfigViewModel
                                                           {
                                                               PBPMatchConfig1Up = queue.PBPImportDetails1Up,
                                                               PBPImportQueueID = queue.PBPImportQueueID,
                                                               QID = queue.QID,
                                                               PlanName = queue.PlanName,
                                                               PlanNumber = queue.PlanNumber,
                                                               ebsPlanName = queue.ebsPlanName,
                                                               eBsPlanNumber = queue.ebsPlanNumber,
                                                               FormInstanceId = queue.FormInstanceID,
                                                               FolderId = queue.FolderId,
                                                               FolderVersionId = queue.FolderVersionId,
                                                               DocumentId = queue.DocId,
                                                               Year = queue.Year,
                                                               UserAction = queue.UserAction,
                                                               PBPDataBase1Up = queue.PBPDatabase1Up
                                                           }).ToList();
            return PlanDetailList;
        }
        #endregion PBPImport

        #region Validate AccessFile,File Data

        public List<PBPTableViewModel> GetPlanDetailsFromAccessDataBase(string fileName, int PBPImportQueueID)
        {

            List<PBPTableViewModel> PBPTableDataList = new List<PBPTableViewModel>();
            string path = IMPORTFILEPATH;
            PBPImportQueueViewModel PBPImportQueueList = (from queue in this._unitOfWork.RepositoryAsync<PBPImportQueue>().Get()
                                                          select new PBPImportQueueViewModel
                                                          {
                                                              PBPImportQueueID = queue.PBPImportQueueID,
                                                              Description = queue.Description,
                                                              PBPFileName = queue.PBPFileName,
                                                              PBPPlanAreaFileName = queue.PBPPlanAreaFileName,
                                                              Year = queue.Year,
                                                              ImportStartDate = queue.ImportStartDate,
                                                              CreatedBy = queue.CreatedBy,
                                                              Status = queue.Status,
                                                              CreatedDate = queue.CreatedDate,
                                                          }).Where(s => s.PBPImportQueueID.Equals(PBPImportQueueID)).FirstOrDefault();
            try
            {
                if (PBPImportQueueList != null)
                {
                    AccessDBService Service = new AccessDBService(path + "\\" + PBPImportQueueList.PBPFileName);
                    PBPTableDataList.Add(new PBPTableViewModel
                    {
                        FileName = PBPImportQueueList.PBPFileName,
                        PBPImportQueueID = PBPImportQueueID,
                        PBPPlanList = Service.ReadPBPTableData(),
                    });
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return PBPTableDataList;
        }

        public ServiceResult ValidateFileScheme(string pbpFileName, string pBPAreaPlanFileName)
        {
            ServiceResult Result = new ServiceResult();
            string path = IMPORTFILEPATH;
            bool IsValidPBPTableScheme, IsValidPBPPlanAreaTableName;
            try
            {
                AccessDBService ServiceObj1 = new AccessDBService(path + "\\" + pbpFileName);
                IsValidPBPTableScheme = ServiceObj1.IsTableExist(PBPTABLENAME);
                ServiceObj1 = new AccessDBService(path + "\\" + pBPAreaPlanFileName);
                IsValidPBPPlanAreaTableName = ServiceObj1.IsTableExist(PBPPLAN_AREASTABLENAME);

                if (IsValidPBPTableScheme && IsValidPBPPlanAreaTableName)
                {
                    Result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    Result.Result = ServiceResultStatus.Failure;
                }
            }

            catch (Exception ex)
            {
                Result.Result = ServiceResultStatus.Failure;
            }
            return Result;
        }

        public ServiceResult ValidateQID(string fileName1, string fileName2)
        {
            ServiceResult Result = new ServiceResult();
            string path = IMPORTFILEPATH;
            PBPTableViewModel File_1_PBPTableList = new PBPTableViewModel();
            PBPTableViewModel File_2_PBPTableList = new PBPTableViewModel();
            try
            {
                AccessDBService ServiceObj1 = new AccessDBService(path + "\\" + fileName1);
                File_1_PBPTableList = new PBPTableViewModel
                {
                    FileName = fileName1,
                    PBPPlanList = ServiceObj1.ReadPBPTableData()
                };

                AccessDBService ServiceObj2 = new AccessDBService(path + "\\" + fileName2);
                File_2_PBPTableList = new PBPTableViewModel
                {
                    FileName = fileName2,
                    PBPPlanList = ServiceObj2.ReadPBPPlanAreaTableData()
                };

                bool IsMisMatch = File_1_PBPTableList.PBPPlanList.Where(item => !File_2_PBPTableList.PBPPlanList.Any(item2 => item2.QID == item.QID)).Any();

                if (IsMisMatch)
                {
                    Result.Result = ServiceResultStatus.Failure;
                }
                else
                {
                    Result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                Result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return Result;
        }
        #endregion Validate AccessFile,File Data

        #region PBPImportHelper
        private string ProductDetailsHelper(int folderid, int folderversionid, int forminstanceid)
        {
            string Str = string.Empty;
            List<Folder> FolderList;
            List<FolderVersion> FolderVersionList;
            List<FormInstance> FormInstanceList;
            FolderList = this._unitOfWork.RepositoryAsync<Folder>().Get()
                                    .ToList();
            FolderVersionList = this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                    .Where(S => S.IsActive == true)
                                    .ToList();
            FormInstanceList = this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                    .Where(s => s.IsActive == true)
                                    .ToList();
            string foldername = FolderList.Where(s => s.FolderID.Equals(folderid)).Select(s => s.Name).FirstOrDefault();

            string folderversionName = FolderVersionList.Where(s => s.FolderVersionID.Equals(folderversionid)).Select(s => s.FolderVersionNumber).FirstOrDefault();

            string fromInstName = FormInstanceList.Where(s => s.FormInstanceID.Equals(forminstanceid)).Select(s => s.Name).FirstOrDefault();

            Str = foldername != null ? foldername + "_" : null;

            Str += folderversionName != null ? folderversionName + "_" : null;

            Str += fromInstName != null ? fromInstName : null;

            return Str;
        }

        private IList<PBPPlanConfigViewModel> PlanListHelper(IList<PBPPlanConfigViewModel> PlanList, int PBPImportQueueID)
        {
            PBPImportQueueViewModel ViewModel = GetPBPDataBaseID(PBPImportQueueID);
            List<Folder> FolderList;
            List<FolderVersion> FolderVersionList;
            List<FormInstance> FormInstanceList;
            FolderList = this._unitOfWork.RepositoryAsync<Folder>().Get()
                                    .ToList();
            FolderVersionList = this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                    .Where(S => S.IsActive == true)
                                    .ToList();
            FormInstanceList = this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                    .Where(s => s.IsActive == true)
                                    .ToList();
            PBPDatabase PPBDataBaseName = this._unitOfWork.RepositoryAsync<PBPDatabase>()
               .Get()
               .Where(s => s.PBPDatabase1Up.Equals(ViewModel.PBPDatabase1Up))
               .FirstOrDefault();
            
            foreach (var item in PlanList.ToList())
            {
                PBPPlanConfigViewModel ConfigViewModel = GetMatchPlanByQID(item.QID, item.PBPImportQueueID);
                item.ProductDetails = ProductDetailsHelper(item.FolderId, item.FolderVersionId, item.FormInstanceId);
                item.DataBaseFileName = PPBDataBaseName.DataBaseName;
                item.PBPMatchConfig1Up = ConfigViewModel.PBPMatchConfig1Up;
                item.IsIncludeInEbs = ConfigViewModel.IsIncludeInEbs;
            }

            return PlanList;
        }

        private class FolderDetailsViewModel
        {
            public List<Folder> FolderList { get; set; }
            public List<FolderVersion> FolderVersionList { get; set; }
            public List<FormInstance> FormInstanceList { get; set; }
        }

        private FolderDetailsViewModel GetFolderDetails()
        {
            FolderDetailsViewModel ViewModel = new FolderDetailsViewModel();

            ViewModel.FolderList = this._unitOfWork.RepositoryAsync<Folder>().Get()
                                    .ToList();
            ViewModel.FolderVersionList = this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                    .Where(S => S.IsActive == true)
                                    .ToList();
            ViewModel.FormInstanceList = this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                    .Where(s => s.IsActive == true)
                                    .ToList();
            return ViewModel;

        }
        private PBPPlanConfigViewModel GetMatchPlanByQID(string qID, int PBPImportQueueID)
        {
            PBPPlanConfigViewModel ViewModel = (from data in this._unitOfWork.RepositoryAsync<PBPMatchConfig>()
                                                .Get()
                                                .Where(p => p.PBPImportQueueID.Equals(PBPImportQueueID)
                                                && p.QID.Equals(qID)
                                                && p.IsActive == true
                                                )
                                                select new PBPPlanConfigViewModel
                                                {
                                                    PBPMatchConfig1Up = data.PBPMatchConfig1Up,
                                                    IsIncludeInEbs = data.IsIncludeInEbs
                                                }).FirstOrDefault();

            return ViewModel;
        }
        #endregion PBPImportHelper

        #region UserAction
        public IList<UserActionViewModel> GetUserActionList()
        {
            List<UserActionViewModel> UserActionList = null;
            try
            {
                UserActionList = (from queue in this._unitOfWork.RepositoryAsync<PBPUserAction>().Get()
                                  .Where(s => s.IsActive == true
                                  && s.PBPUserActionID != (int)PBPUserActionList.UpdatePlan)
                                  select new UserActionViewModel
                                  {
                                      PBPUserActionID = queue.PBPUserActionID,
                                      UserAction = queue.UserAction
                                  }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return UserActionList;
        }
        #endregion

        #region PBPDataBase

        public GridPagingResponse<PBPDatabaseViewModel> GetPBPDatabaseList(int tenantID, GridPagingRequest gridPagingRequest)
        {
            int count = 0;
            List<PBPDatabaseViewModel> PBPDatabaseList = null;
            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);
                var currentDatabase = from s in "st"
                                      select new
                                      {
                                          FolderID = 0,
                                          FolderVersionID = 0,
                                          VersionCount = 0
                                      };
                PBPDatabaseList = (from queue in this._unitOfWork.RepositoryAsync<PBPDatabase>().Get()
                                   orderby queue.CreatedDate descending
                                   select new PBPDatabaseViewModel
                                   {
                                       PBPDatabase1Up = queue.PBPDatabase1Up,
                                       DataBaseName = queue.DataBaseName,
                                       DataBaseDescription = queue.DataBaseDescription,
                                       CreatedBy = queue.CreatedBy,
                                       CreatedDate = queue.CreatedDate,
                                       UpdatedBy = queue.UpdatedBy,
                                       UpdatedDate = queue.UpdatedDate,
                                       IsActive = queue.IsActive
                                   }).ToList().ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                     .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<PBPDatabaseViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, PBPDatabaseList);


        }

        public List<PBPDatabaseViewModel> GetPBPDatabaseNameList(int tenantID)
        {
            List<PBPDatabaseViewModel> PBPDatabaseList = null;
            try
            {
                PBPDatabaseList = (from queue in this._unitOfWork.RepositoryAsync<PBPDatabase>().Get()
                                   .Where(s => s.IsActive == true)
                                   select new PBPDatabaseViewModel
                                   {
                                       PBPDatabase1Up = queue.PBPDatabase1Up,
                                       DataBaseName = queue.DataBaseName,
                                   }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return PBPDatabaseList;
        }

        public PBPImportQueueViewModel GetPBPDataBaseID(int PBPImportQueueID)
        {
            PBPImportQueueViewModel ViewModel = (from queue in this._unitOfWork.RepositoryAsync<PBPImportQueue>().Get()
                                                   .Where(s => s.PBPImportQueueID == PBPImportQueueID)
                                                 select new PBPImportQueueViewModel
                                                 {
                                                     PBPImportQueueID = queue.PBPImportQueueID,
                                                     PBPDatabase1Up = queue.PBPDatabase1Up,
                                                 }).FirstOrDefault();
            return ViewModel;
        }

        public ServiceResult AddPBPDatabase(AddPBPDBNameViewModel list, string addedBy)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();
                {

                    PBPDatabase DBToAdd = new PBPDatabase();

                    DBToAdd.DataBaseName = list.PBPDataBaseName;
                    DBToAdd.DataBaseDescription = list.PBPDataBaseDescription;
                    DBToAdd.CreatedDate = DateTime.Now;
                    DBToAdd.CreatedBy = addedBy;
                    DBToAdd.IsActive = true;

                    this._unitOfWork.RepositoryAsync<PBPDatabase>().Insert(DBToAdd);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }

            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;

                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        public ServiceResult UpdatePlanConfig(int tenantId, IList<PBPMatchConfigViewModel> PBPMatchConfigList,string UpdateBy)
        {
            ServiceResult Result = new ServiceResult();

            try
            {
                if (PBPMatchConfigList != null)
                {
                    foreach (var item in PBPMatchConfigList.Where(s => s.UserAction != (int)PBPUserActionList.UpdatePlan))
                    {
                        PBPMatchConfig itemToUpdateList = this._unitOfWork.RepositoryAsync<PBPMatchConfig>().Get()
                                                                        .Where(s => s.PBPMatchConfig1Up.Equals(item.PBPMatchConfig1Up)
                                                                        && s.PBPImportQueueID.Equals(item.PBPImportQueueID)
                                                                        && s.IsActive == true).FirstOrDefault();

                        if (itemToUpdateList != null)
                        {

                            using (var scope = new TransactionScope())
                            {
                                itemToUpdateList.FolderId = item.FolderId;
                                itemToUpdateList.FolderVersionId = item.FolderVersionId;
                                itemToUpdateList.FormInstanceID = item.FormInstanceId;
                                itemToUpdateList.UserAction = item.UserAction;
                                itemToUpdateList.DocId = item.DocId;
                                this._unitOfWork.RepositoryAsync<PBPMatchConfig>().Update(itemToUpdateList);
                                this._unitOfWork.Save();
                                scope.Complete();
                            }
                            Result.Result = ServiceResultStatus.Success;
                        }
                    }
                }

                else
                {
                    Result.Result = ServiceResultStatus.Success;
                }
            }

            catch (Exception ex)
            {
                Result = ex.ExceptionMessages();
                Result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return Result;
        }

        public ServiceResult UpdatePBPDatabase(UpdatePBPDBNameViewModel update, string updatedby)
        {

            ServiceResult result = null;
            try
            {
                result = new ServiceResult();
                PBPDatabase database = _unitOfWork.RepositoryAsync<PBPDatabase>().FindById(update.PBPDatabase1Up);

                if (database != null)
                {
                    {
                        database.DataBaseName = update.PBPDataBaseName;
                        database.DataBaseDescription = update.PBPDataBaseDescription;
                        database.UpdatedBy = updatedby;
                        database.UpdatedDate = DateTime.Now;

                        this._unitOfWork.RepositoryAsync<PBPDatabase>().Update(database);
                        this._unitOfWork.Save();

                        result.Result = ServiceResultStatus.Success;
                    }

                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                    throw new Exception("Database Does Not exists");
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;

                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        public string PBPDatabaseNameById(int dbId)
        {
            string DataBaseName = this._unitOfWork.RepositoryAsync<PBPDatabase>().Get()
                                     .Where(s => s.IsActive == true
                                            && s.PBPDatabase1Up.Equals(dbId))
                                   .Select(s => s.DataBaseName)
                                    .FirstOrDefault();

            return DataBaseName;
        }

        public ServiceResult UpdateMatchPlanConfig(IList<PBPMatchConfigViewModel> PBPMatchConfigList, string UpdateBy)
        {
            ServiceResult Result = new ServiceResult();

            try
            {
                if (PBPMatchConfigList != null)
                {
                    foreach (var item in PBPMatchConfigList.Where(s => s.UserAction == (int)PBPUserActionList.UpdatePlan))
                    {
                        PBPMatchConfig itemToUpdateList = this._unitOfWork.RepositoryAsync<PBPMatchConfig>().Get()
                                                                        .Where(s => s.PBPMatchConfig1Up.Equals(item.PBPMatchConfig1Up)
                                                                        && s.PBPImportQueueID.Equals(item.PBPImportQueueID)
                                                                        && s.IsActive == true).FirstOrDefault();

                        if (itemToUpdateList != null)
                        {

                            using (var scope = new TransactionScope())
                            {
                                itemToUpdateList.IsIncludeInEbs = item.IsIncludeInEbs;
                                this._unitOfWork.RepositoryAsync<PBPMatchConfig>().Update(itemToUpdateList);
                                this._unitOfWork.Save();
                                scope.Complete();
                            }
                            Result.Result = ServiceResultStatus.Success;
                        }
                    }
                }

                else
                {
                    Result.Result = ServiceResultStatus.Success;
                }
            }

            catch (Exception ex)
            {
                Result = ex.ExceptionMessages();
                Result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return Result;
        }

        #endregion PBPDataBase

        #region Process Status

        public Dictionary<int, string> ProcessStatusKeyValuePair = new Dictionary<int, string>()
        {
            { 1,"Queued" },{ 2,"In Progress" },{ 3,"Errored" },{ 4,"Complete" },
            { 8,"In Review" },{ 7,"Scheduled" },{ 6,"Not Scheduled" },{ 5,"Finalized" }
        };
        #endregion

    }
}
