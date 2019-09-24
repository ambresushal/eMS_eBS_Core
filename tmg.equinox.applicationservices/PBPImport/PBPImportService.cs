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
using tmg.equinox.applicationservices.PBPImport;
using tmg.equinox.backgroundjob;
using tmg.equinox.queueprocess.PBPImport;
using tmg.equinox.pbpimport;
using tmg.equinox.core.logging.Logging;
using System.Data.Entity;
using tmg.equinox.emailnotification.Model;
using tmg.equinox.emailnotification;
using tmg.equinox.pbpimport.Interfaces;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.applicationservices.viewmodels.Portfolio;
using FolderVersionState = tmg.equinox.domain.entities.Enums.FolderVersionState;

namespace tmg.equinox.applicationservices
{
    public class PBPImportService : IPBPImportService
    {
        #region Private Memebers
        public IUnitOfWorkAsync _unitOfWork { get; set; }
        IBackgroundJobManager _hangFireJobManager;
        private static readonly ILog _logger = LogProvider.For<PBPImportService>();

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
        Func<string, IUnitOfWorkAsync> _coreunitOfWork;
        #endregion Private Members

        #region Constructor

        public PBPImportService(IUnitOfWorkAsync unitOfWork, IBackgroundJobManager hangFireJobManager)
        {
            this._unitOfWork = unitOfWork;
            _hangFireJobManager = hangFireJobManager;
        }

        public PBPImportService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        #region PBPImport

        public void InitializeVariables(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public GridPagingResponse<PBPImportQueueViewModel> GetPBPImportQueueList(int tenantID, GridPagingRequest gridPagingRequest)
        {
            List<PBPImportQueueViewModel> PBPImportQueueList = null;
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

                PBPImportQueueList = (from queue in this._unitOfWork.RepositoryAsync<PBPImportQueue>().Get()
                                      join db in this._unitOfWork.RepositoryAsync<PBPDatabase>().Get()
                                      on queue.PBPDatabase1Up equals db.PBPDatabase1Up
                                      join status in this._unitOfWork.RepositoryAsync<ProcessStatusMaster>().Get()
                                        on queue.Status equals status.ProcessStatus1Up
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
                                          StatusText = status.ProcessStatusName,
                                          CreatedDate = queue.CreatedDate,
                                          PBPDatabase1Up = queue.PBPDatabase1Up,
                                          DataBaseName = db.DataBaseName,
                                          JobId = queue.JobId,
                                          PBPFileDisplayName = queue.PBPFileDisplayName,
                                          PBPPlanAreaFileDisplayName = queue.PBPPlanAreaFileDisplayName,
                                          //JobLocation = queue.JobLocation
                                      }).ToList();
                foreach (var item in PBPImportQueueList)
                {
                    item.ErrorMessage = GetActivityLog(item.PBPImportQueueID);
                }

                PBPImportQueueList = PBPImportQueueList.ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                 .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<PBPImportQueueViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, PBPImportQueueList);
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
                AddToImport.UpdatedBy = pBPImportQueueViewModel.CreatedBy;
                AddToImport.UpdatedDate = DateTime.Now;
                AddToImport.IsFullMigration = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsFullMigration"]);
                AddToImport.JobId = pBPImportQueueViewModel.JobId;
                AddToImport.PBPFileDisplayName = pBPImportQueueViewModel.PBPFileDisplayName;
                AddToImport.PBPPlanAreaFileDisplayName = pBPImportQueueViewModel.PBPPlanAreaFileDisplayName;
                //AddToImport.JobLocation = pBPImportQueueViewModel.JobLocation;
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

        public PlanConfigurationViewModel ProcessPlanConfiguration(List<PBPTableViewModel> PBPTableData, string CreateBy,int PBPImportQueueID, int PBPDataBase1Up)
        {
            PlanConfigurationViewModel PlanConfigrationLists = new PlanConfigurationViewModel();
            try
            {
                List<PBPPlanViewModel> PlanList = PBPTableData.Select(s => s.PBPPlanList).FirstOrDefault();
                foreach (var PbPPlanItem in PlanList)
                {
                    if (IsPlanPresentIneBS(PbPPlanItem.QID, PBPDataBase1Up) == false)
                    {
                        int PBPMatchConfig1Up = SavePBPConfig(PBPImportQueueID, PbPPlanItem, CreateBy, false);
                    }
                    else
                    {
                        if (PbPPlanItem.IsEGWPPlan)
                        {
                            SaveAllMatchingEGWPPlanDetail(PBPDataBase1Up, PbPPlanItem.QID, PBPImportQueueID, CreateBy, PbPPlanItem);
                        }
                        else
                        {
                            PBPPlanConfigViewModel MatchingPlan = GetMatchingPlanDetails(PbPPlanItem.QID, PBPDataBase1Up);
                            if (MatchingPlan != null)
                            {
                                PbPPlanItem.FolderId = MatchingPlan.FolderId;
                                PbPPlanItem.FolderVersionId = MatchingPlan.FolderVersionId;
                                PbPPlanItem.FormInstanceId = MatchingPlan.FormInstanceId;
                                PbPPlanItem.DocId = MatchingPlan.DocumentId;
                                PbPPlanItem.UserAction = (int)PBPUserActionList.UpdatePlan;
                                PbPPlanItem.ebsPlanName = MatchingPlan.ebsPlanName;
                                PbPPlanItem.ebsPlanNumber = MatchingPlan.eBsPlanNumber;
                                //save matching plan details
                                MatchingPlan.PBPMatchConfig1Up = SavePBPConfig(PBPImportQueueID, PbPPlanItem, CreateBy, true);
                            }
                        }
                    }
                }
                
                //get missing plan from list
                SaveMissMatchQIdInAccessFile(PBPTableData, PBPDataBase1Up, PBPImportQueueID, CreateBy);
                //get proxcy plan to delte
                ProxyPlanListForTerminatePlanfromeBS(PBPImportQueueID, PBPDataBase1Up, CreateBy);
                PlanConfigrationLists = CreateMatchAndMisMatchPlanLists(PBPImportQueueID);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return PlanConfigrationLists;
        }

        public int SavePBPConfig(int PBPImportQueueID, PBPPlanViewModel PBPPlanModel, string CreateBy, bool IsmatchPlan)
        {
            int PBPMatchConfig1Up = 0;
            try
            {
                if (IsmatchPlan == false)
                {
                    PBPMatchConfig AddToImport = new PBPMatchConfig();
                    AddToImport.QID = PBPPlanModel.QID;
                    AddToImport.PlanName = PBPPlanModel.PBPPlanName;
                    AddToImport.PlanNumber = PBPPlanModel.PBPPlanNumber;
                    AddToImport.ebsPlanName = PBPPlanModel.ebsPlanName;
                    AddToImport.ebsPlanNumber = PBPPlanModel.ebsPlanNumber;
                    AddToImport.PBPImportQueueID = PBPImportQueueID;
                    AddToImport.CreatedDate = DateTime.Now.Date;
                    AddToImport.IsActive = true;
                    AddToImport.CreatedBy = CreateBy;
                    AddToImport.UserAction = PBPPlanModel.UserAction;
                    AddToImport.FolderId = PBPPlanModel.FolderId;
                    AddToImport.FolderVersionId = PBPPlanModel.FolderVersionId;
                    AddToImport.FormInstanceID = PBPPlanModel.FormInstanceId;
                    AddToImport.DocId = PBPPlanModel.DocId;
                    AddToImport.IsTerminateVisible = PBPPlanModel.IsTerminateVisiable;
                    AddToImport.UpdatedDate = DateTime.Now;
                    AddToImport.UpdatedBy = CreateBy;
                    AddToImport.Year = PBPPlanModel.Year;
                    AddToImport.IsEGWPPlan = PBPPlanModel.IsEGWPPlan;
                    this._unitOfWork.RepositoryAsync<PBPMatchConfig>().Insert(AddToImport);
                    this._unitOfWork.Save();
                    PBPMatchConfig1Up = AddToImport.PBPMatchConfig1Up;
                }
                else
                {
                    PBPMatchConfig AddToImport = new PBPMatchConfig();
                    AddToImport.FolderId = PBPPlanModel.FolderId;
                    AddToImport.FolderVersionId = PBPPlanModel.FolderVersionId;
                    AddToImport.FormInstanceID = PBPPlanModel.FormInstanceId;
                    AddToImport.DocId = PBPPlanModel.DocId;
                    AddToImport.QID = PBPPlanModel.QID;
                    AddToImport.PlanName = PBPPlanModel.PBPPlanName;
                    AddToImport.PlanNumber = PBPPlanModel.PBPPlanNumber;
                    AddToImport.ebsPlanName = PBPPlanModel.ebsPlanName;
                    AddToImport.ebsPlanNumber = PBPPlanModel.ebsPlanNumber;
                    AddToImport.PBPImportQueueID = PBPImportQueueID;
                    AddToImport.CreatedDate = DateTime.Now.Date;
                    AddToImport.IsActive = true;
                    AddToImport.CreatedBy = CreateBy;
                    AddToImport.IsIncludeInEbs = true;
                    AddToImport.UserAction = (int)PBPUserActionList.UpdatePlan;
                    AddToImport.Year = PBPPlanModel.Year;
                    AddToImport.IsEGWPPlan = PBPPlanModel.IsEGWPPlan;
                    this._unitOfWork.RepositoryAsync<PBPMatchConfig>().Insert(AddToImport);
                    this._unitOfWork.Save();
                    PBPMatchConfig1Up = AddToImport.PBPMatchConfig1Up;

                }
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

        public bool IsPlanPresentIneBS(string qID, int dataBaseId)
        {
            bool IsExist = false;
            IList<PBPPlanConfigViewModel> PlanList = GetPBPPlanDetailsList();
            IsExist = PlanList.Where(s => s.PlanNumber.Equals(qID)
                              && (s.PBPDataBase1Up == dataBaseId || s.PBPDataBase1Up == 0))
                              .Any();
            return IsExist;
        }
        //Creaete
        public PlanConfigurationViewModel CreateMatchAndMisMatchPlanLists(int PBPImportQueueID)
        {
            int UnMatchIndex = 0, MatchIndex = 0;
            PlanConfigurationViewModel ViewModel = new PlanConfigurationViewModel();


       IList<PBPPlanConfigViewModel> PlanList = (from queue in this._unitOfWork.RepositoryAsync<PBPImportQueue>()
                                    .Get()
                                    .Where(s=>s.PBPImportQueueID.Equals(PBPImportQueueID))
                                    join pbpdatabase in this._unitOfWork.RepositoryAsync<PBPDatabase>()
                                    .Get()
                                    on queue.PBPDatabase1Up equals pbpdatabase.PBPDatabase1Up
                                    join match in this._unitOfWork.RepositoryAsync<PBPMatchConfig>()
                                    .Get()
                                    on queue.PBPImportQueueID equals match.PBPImportQueueID
                                    join action in this._unitOfWork.RepositoryAsync<PBPUserAction>()
                                    .Get()
                                    on match.UserAction equals action.PBPUserActionID
                                    into myData from rt in myData.DefaultIfEmpty()
                                    where match.IsActive==true
                                        select new PBPPlanConfigViewModel
                                        {
                                            PBPMatchConfig1Up = match.PBPMatchConfig1Up,
                                            PBPImportQueueID = queue.PBPImportQueueID,
                                            QID = match.QID,
                                            PlanName = match.PlanName,
                                            PlanNumber = match.PlanNumber,
                                            ebsPlanName = match.ebsPlanName,
                                            eBsPlanNumber = match.ebsPlanNumber,
                                            FormInstanceId = match.FormInstanceID,
                                            DocumentId = match.DocId,
                                            FolderId = match.FolderId,
                                            FolderVersionId = match.FolderVersionId,
                                            IsIncludeInEbs = match.IsIncludeInEbs,
                                            IsTerminateVisible = match.IsTerminateVisible,
                                            IsProxyUsed = match.IsProxyUsed,
                                            Year = queue.Year,
                                            PBPDataBase1Up = pbpdatabase.PBPDatabase1Up,
                                            DataBaseFileName = pbpdatabase.DataBaseName,
                                            UserActionText = rt.UserAction,
                                            UserAction = rt.PBPUserActionID > 0? rt.PBPUserActionID :0,
                                            IsEGWPPlan= match.IsEGWPPlan
                                        }).ToList();

            if (PlanList != null)
            {
                PlanList = PlanListHelper(PlanList, PBPImportQueueID);
            }
            ViewModel.MatchPlanList = PlanList.Where(s => s.UserAction.Equals((int)PBPUserActionList.UpdatePlan))
                                      .OrderBy(s => s.QID)
                                      .ToList();
            foreach (var matchitem in ViewModel.MatchPlanList.ToList())
            {
                MatchIndex = MatchIndex + 1;
                matchitem.Index = MatchIndex;
            }

            ViewModel.MisMatchPlanList = PlanList.Where(s => s.UserAction != (int)PBPUserActionList.UpdatePlan)
                                        .OrderByDescending(s => s.QID)
                                        .ToList();
            foreach (var matchitem in ViewModel.MisMatchPlanList.ToList())
            {
                UnMatchIndex = UnMatchIndex + 1;
                matchitem.Index = UnMatchIndex;
            }
            return ViewModel;
        }

        public IList<PBPPlanConfigViewModel> GetPBPPlanDetailsList()
        {
            //IList<PBPPlanConfigViewModel> PlanList = null;
            IList<PBPPlanConfigViewModel> PlanList = (from queue in this._unitOfWork.RepositoryAsync<PBPImportDetails>()
                        .Get()
                        .Where(s => s.IsActive == true
                               &&
                               s.Status == (int)ProcessStatusMasterCode.Complete
                               )
                                                      select new PBPPlanConfigViewModel
                                                      {
                                                          QID = queue.QID,
                                                          PlanName = queue.PlanName,
                                                          PlanNumber = queue.PlanNumber,
                                                          PBPDataBase1Up = queue.PBPDatabase1Up,
                                                          FolderId = queue.FolderId,
                                                          FolderVersionId = queue.FolderVersionId,
                                                          FormInstanceId = queue.FormInstanceID,
                                                          DocumentId = queue.DocId,
                                                          ebsPlanName = queue.ebsPlanName,
                                                          eBsPlanNumber = queue.ebsPlanNumber,
                                                          IsActive = queue.IsActive
                                                      }).ToList();

            return PlanList;
        }

        public PBPPlanConfigViewModel GetMatchingPlanDetails(string qID, int PbpDataBaseId)
        {
            PBPPlanConfigViewModel PlanDetails = new PBPPlanConfigViewModel();
            
            if (!string.IsNullOrEmpty(qID))
            {
                PlanDetails = (from queue in this._unitOfWork.RepositoryAsync<PBPImportDetails>().Get()
                               join flder in this._unitOfWork.RepositoryAsync<Folder>().Get()
                               on queue.FolderId equals flder.FolderID
                               join fldrVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                               on flder.FolderID equals fldrVersion.FolderID
                               join forminst in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                               on queue.FormInstanceID equals forminst.FormInstanceID
                               where (queue.IsActive == true
                               && queue.PlanNumber.Equals(qID)
                               && queue.PBPDatabase1Up.Equals(PbpDataBaseId)  
                               && flder.IsPortfolio==true
                               && fldrVersion.IsActive==true
                               && forminst.IsActive==true
                               && forminst.FormDesignID.Equals(2359)
                               )
                               select new PBPPlanConfigViewModel
                               {
                                   QID = queue.QID,
                                   PlanName = queue.PlanName,
                                   PlanNumber = queue.PlanNumber,
                                   ebsPlanName = queue.ebsPlanName,
                                   eBsPlanNumber = queue.ebsPlanNumber,
                                   FormInstanceId = queue.FormInstanceID,
                                   FolderId = queue.FolderId,
                                   Year = queue.Year,
                                   UserAction = (int)PBPUserActionList.UpdatePlan,
                                   PBPDataBase1Up = queue.PBPDatabase1Up,
                                   FolderVersionId=fldrVersion.FolderVersionID,
                                   DocumentId=forminst.DocID,
                                   ProductDetails=flder.Name +"_"+fldrVersion.FolderVersionNumber,
                                   FolderVersion= flder.Name + "_" + fldrVersion.FolderVersionNumber,
                               })
                               .OrderByDescending(s=>s.FolderVersionId)
                               .FirstOrDefault();
            }
            return PlanDetails;
        }

        public ServiceResult SavePBPPlanDetails(int ImportID, string createdBy)
        {
            ServiceResult Result = new ServiceResult();
            try
            {
                if (ImportID > 0)
                {
                    PBPImportQueue QueueList = this._unitOfWork.RepositoryAsync<PBPImportQueue>().Get()
                                                .Where(s => s.PBPImportQueueID.Equals(ImportID))
                                                .FirstOrDefault();
                        
                    List<PBPImportQueueViewModel> PBPImportQueueList = (from queue in this._unitOfWork.RepositoryAsync<PBPImportQueue>().Get()
                                          join db in this._unitOfWork.RepositoryAsync<PBPDatabase>().Get()
                                          on queue.PBPDatabase1Up equals db.PBPDatabase1Up
                                          join status in this._unitOfWork.RepositoryAsync<ProcessStatusMaster>().Get()
                                          on queue.Status equals status.ProcessStatus1Up
                                          where queue.PBPImportQueueID!= ImportID
                                          && (queue.Status==(int)ProcessStatusMasterCode.InProgress
                                              || queue.Status == (int)ProcessStatusMasterCode.Queued
                                             )
                                          && queue.PBPDatabase1Up.Equals(QueueList.PBPDatabase1Up)
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
                                              StatusText = status.ProcessStatusName,
                                              CreatedDate = queue.CreatedDate,
                                              PBPDatabase1Up = queue.PBPDatabase1Up,
                                              DataBaseName = db.DataBaseName,
                                              JobId = queue.JobId,
                                              PBPFileDisplayName = queue.PBPFileDisplayName,
                                              PBPPlanAreaFileDisplayName = queue.PBPPlanAreaFileDisplayName,
                                              //JobLocation = queue.JobLocation
                                          }).ToList();

                    if (PBPImportQueueList.Any() == false)
                    {
                        ServiceResult ImportQueueStatusResult = UpdateImportQueueStatus(ImportID, domain.entities.Enums.ProcessStatusMasterCode.Queued);
                        //Add to Hangfire Queue
                        PBPImportEnqueue PBPImportEnqueue = new PBPImportEnqueue(_hangFireJobManager);
                        PBPImportEnqueue.Enqueue(new PBPImportQueueInfo { QueueId = ImportID, UserId = createdBy, FeatureId = ImportID.ToString(), Name = "PBP Import for PBPImportQueueID: " + ImportID.ToString(), AssemblyName = "tmg.equinox.applicationservices", ClassName = "PBPImportCustomQueue" });
                        Result.Result = ServiceResultStatus.Success;
                    }
                    else
                    {
                        PBPImportQueueViewModel viewmodel = PBPImportQueueList.FirstOrDefault();
                        Result.Result = ServiceResultStatus.Failure;
                        List<ServiceResultItem> items = new List<ServiceResultItem>();
                        string Msg = "PBP import for '" + viewmodel.DataBaseName 
                                    + "' is already in '" + viewmodel.StatusText
                                    + "' status, please queue once it's completed.";
                        items.Add(new ServiceResultItem { Messages = new string[] { Msg } });
                        Result.Items = items;
                    }
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
                    itemToUpdateList.UpdatedBy = "TMG Super User";
                    itemToUpdateList.UpdatedDate = DateTime.Now;
                    using (var scope = new TransactionScope())
                    {
                        this._unitOfWork.RepositoryAsync<PBPImportQueue>().Update(itemToUpdateList, true);
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
            DataTable ExportDataTable = null;

            PBPImportQueueList = (from queue in this._unitOfWork.RepositoryAsync<PBPImportQueue>().Get()
                                  join db in this._unitOfWork.RepositoryAsync<PBPDatabase>().Get()
                                  on queue.PBPDatabase1Up equals db.PBPDatabase1Up
                                  join status in this._unitOfWork.RepositoryAsync<ProcessStatusMaster>().Get()
                                        on queue.Status equals status.ProcessStatus1Up
                                  select new PBPImportQueueViewModel
                                  {
                                      PBPImportQueueID = queue.PBPImportQueueID,
                                      Description = queue.Description,
                                      PBPFileName = queue.PBPFileDisplayName,
                                      PBPPlanAreaFileName = queue.PBPPlanAreaFileDisplayName,
                                      Year = queue.Year,
                                      ImportStartDate = queue.ImportStartDate,
                                      CreatedBy = queue.CreatedBy,
                                      Status = queue.Status,
                                      StatusCode = queue.Status,
                                      StatusText = status.ProcessStatusName,
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
                    row["PBPFileName"] = item.PBPFileName;
                    row["PBPPlanAreaFileName"] = item.PBPPlanAreaFileName;
                    //row["Year"] = item.Year;
                    row["CreatedDate"] = item.CreatedDate.ToString();
                    row["CreatedBy"] = item.CreatedBy;
                    row["Status"] = item.StatusText;
                    ExportDataTable.Rows.Add(row);

                }
            }

            return ExportDataTable;
        }

        public DataTable GetExportPreviewGridDataTable(int pbpImportId, int PlanType)
        {
            DataTable ExportDataTable = null;
            ExportDataTable = new DataTable();
            ExportDataTable.Columns.Add("Index");
            ExportDataTable.Columns.Add("PBPPlanNumber");
            ExportDataTable.Columns.Add("PBPPlanName");
            if (tmg.equinox.config.Config.GetApplicationName().ToLower() == "ebenefitsync")
            {
                ExportDataTable.Columns.Add("eBsPlanNumber");
                ExportDataTable.Columns.Add("ebsPlanName");
            }
            else
            {
                ExportDataTable.Columns.Add("eMsPlanNumber");
                ExportDataTable.Columns.Add("eMsPlanName");
            }            
            //ExportDataTable.Columns.Add("Year");
            ExportDataTable.Columns.Add("UserAction");
            ExportDataTable.Columns.Add("FolderVersion");
            PlanConfigurationViewModel ExportRecord = CreateMatchAndMisMatchPlanLists(pbpImportId);
            int Index = 0;
            List<UserActionViewModel> UserActionList = GetUserActionList().ToList();

            if (PlanType == 3)
            {
                foreach (var missMatchPlan in ExportRecord.MisMatchPlanList)
                {
                    Index = Index + 1;
                    DataRow row = ExportDataTable.NewRow();
                    row["Index"] = Index;
                    row["PBPPlanNumber"] = missMatchPlan.PlanNumber;
                    row["PBPPlanName"] = missMatchPlan.PlanName;
                    if (tmg.equinox.config.Config.GetApplicationName().ToLower() == "ebenefitsync")
                    {
                        row["eBsPlanNumber"] = missMatchPlan.eBsPlanNumber;
                        row["ebsPlanName"] = missMatchPlan.ebsPlanName;
                    }
                    else
                    {
                        row["eMsPlanNumber"] = missMatchPlan.eBsPlanNumber;
                        row["eMsPlanName"] = missMatchPlan.ebsPlanName;
                    }                   
                    row["UserAction"] = UserActionList.Where(s => s.PBPUserActionID.Equals(missMatchPlan.UserAction))
                                                      .Select(s => s.UserAction).FirstOrDefault();
                    row["FolderVersion"] = missMatchPlan.FolderVersion == null ? missMatchPlan.ProductDetails : missMatchPlan.FolderVersion;
                    ExportDataTable.Rows.Add(row);
                }
                foreach (var MatchPlan in ExportRecord.MatchPlanList)
                {
                    Index = Index + 1;
                    DataRow row = ExportDataTable.NewRow();
                    row["Index"] = Index;
                    row["PBPPlanNumber"] = MatchPlan.PlanNumber;
                    row["PBPPlanName"] = MatchPlan.PlanName;
                    if (tmg.equinox.config.Config.GetApplicationName().ToLower() == "ebenefitsync")
                    {
                        row["eBsPlanNumber"] = MatchPlan.eBsPlanNumber;
                        row["ebsPlanName"] = MatchPlan.ebsPlanName;
                    }
                    else
                    {
                        row["eMsPlanNumber"] = MatchPlan.eBsPlanNumber;
                        row["eMsPlanName"] = MatchPlan.ebsPlanName;
                    }                    
                    row["UserAction"] = "Update Plan";
                    row["FolderVersion"] = MatchPlan.FolderVersion;
                    ExportDataTable.Rows.Add(row);
                }
            }
            else if (PlanType == 2)
            {
                foreach (var MatchPlan in ExportRecord.MatchPlanList)
                {
                    Index = Index + 1;
                    DataRow row = ExportDataTable.NewRow();
                    row["Index"] = Index;
                    row["PBPPlanNumber"] = MatchPlan.PlanNumber;
                    row["PBPPlanName"] = MatchPlan.PlanName;
                    if (tmg.equinox.config.Config.GetApplicationName().ToLower() == "ebenefitsync")
                    {
                        row["eBsPlanNumber"] = MatchPlan.eBsPlanNumber;
                        row["ebsPlanName"] = MatchPlan.ebsPlanName;
                    }
                    else
                    {
                        row["eMsPlanNumber"] = MatchPlan.eBsPlanNumber;
                        row["eMsPlanName"] = MatchPlan.ebsPlanName;
                    }                   
                    row["UserAction"] = "Update Plan";
                    row["FolderVersion"] = MatchPlan.FolderVersion;
                    ExportDataTable.Rows.Add(row);
                }
            }
            else if (PlanType == 1)
            {
                foreach (var missMatchPlan in ExportRecord.MisMatchPlanList)
                {
                    Index = Index + 1;
                    DataRow row = ExportDataTable.NewRow();
                    row["Index"] = Index;
                    row["PBPPlanNumber"] = missMatchPlan.PlanNumber;
                    row["PBPPlanName"] = missMatchPlan.PlanName;
                    if (tmg.equinox.config.Config.GetApplicationName().ToLower() == "ebenefitsync")
                    {
                        row["eBsPlanNumber"] = missMatchPlan.eBsPlanNumber;
                        row["ebsPlanName"] = missMatchPlan.ebsPlanName;
                    }
                    else
                    {
                        row["eMsPlanNumber"] = missMatchPlan.eBsPlanNumber;
                        row["eMsPlanName"] = missMatchPlan.ebsPlanName;
                    }                    
                    row["UserAction"] = UserActionList.Where(s => s.PBPUserActionID.Equals(missMatchPlan.UserAction))
                                                      .Select(s => s.UserAction).FirstOrDefault();
                    row["FolderVersion"] = missMatchPlan.FolderVersion == null ? missMatchPlan.ProductDetails : missMatchPlan.FolderVersion;
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

        public PBPImportQueueViewModel GetQueuedOrProcessingPBPImport()
        {
            PBPImportQueueViewModel model = null;
            try
            {
                var PBPImportQueueList = (from queue in this._unitOfWork.RepositoryAsync<PBPImportQueue>().Get()
                                          join db in this._unitOfWork.RepositoryAsync<PBPDatabase>().Get()
                                          on queue.PBPDatabase1Up equals db.PBPDatabase1Up
                                          where queue.Status < 3
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
                                          });
                if (PBPImportQueueList != null && PBPImportQueueList.Count() > 0)
                {
                    model = PBPImportQueueList.First();
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

        public List<PBPPlanConfigViewModel> GetPBPPlanDetailsForProcess(int pBPImportQueueID)
        {
            List<PBPPlanConfigViewModel> PlanDetailList = null;
            PlanDetailList = (from queue in this._unitOfWork.RepositoryAsync<PBPMatchConfig>().Get()
                              join importlist in this._unitOfWork.RepositoryAsync<PBPImportQueue>().Get()
                              on queue.PBPImportQueueID equals importlist.PBPImportQueueID
                              join pbpdatabselist in this._unitOfWork.RepositoryAsync<PBPDatabase>().Get()
                              on importlist.PBPDatabase1Up equals pbpdatabselist.PBPDatabase1Up
                              select new PBPPlanConfigViewModel
                              {
                                  PBPMatchConfig1Up = queue.PBPMatchConfig1Up,
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
                                  PBPDataBase1Up = pbpdatabselist.PBPDatabase1Up,
                                  PBPDataBaseName = pbpdatabselist.DataBaseName,
                                  IsIncludeInEbs = queue.IsIncludeInEbs,
                                  IsProxyUsed = queue.IsProxyUsed,
                                  CreatedBy=queue.CreatedBy,
                                  IsEGWPPlan=queue.IsEGWPPlan
                              }).Where(s => s.PBPImportQueueID.Equals(pBPImportQueueID)
                              && s.UserAction != (int)PBPUserActionList.NoActionRequired
                              ).ToList();
            return PlanDetailList;
        }

        public ServiceResult UpdateImportQueue(int PBPImportQueueID, PBPImportQueueViewModel model)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                PBPImportQueue itemToUpdate = this._unitOfWork.RepositoryAsync<PBPImportQueue>().Get()
                                                .Where(s => s.PBPImportQueueID.Equals(PBPImportQueueID)
                                                ).FirstOrDefault();

                if (itemToUpdate != null)
                {
                    itemToUpdate.ImportStatus = model.ImportStatus;
                    itemToUpdate.JobId = model.JobId;
                    itemToUpdate.ErrorMessage = model.ErrorMessage;
                    this._unitOfWork.RepositoryAsync<PBPImportQueue>().Update(itemToUpdate);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                //bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                _logger.ErrorException("Error in UpdateImportQueue : ", ex);
                throw ex;
            }
            return result;
        }

        public ServiceResult UpdatePBPDetailAfterPerformUserAction(PBPPlanConfigViewModel ViewModel)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                PBPImportDetails itemToUpdate = this._unitOfWork.RepositoryAsync<PBPImportDetails>().Get()
                                                .Where(s => s.PBPImportQueueID.Equals(ViewModel.PBPImportQueueID)
                                                && s.FolderId == ViewModel.FolderId
                                                && s.QID.Equals(ViewModel.QID)
                                                ).FirstOrDefault();

                if (itemToUpdate != null)
                {
                    itemToUpdate.IsActive = false;
                    this._unitOfWork.RepositoryAsync<PBPImportDetails>().Update(itemToUpdate);
                    this._unitOfWork.Save();
                    result.Result = ServiceResultStatus.Success;
                }



                if (itemToUpdate != null)
                {
                    itemToUpdate.FolderVersionId = ViewModel.FolderVersionId;
                    itemToUpdate.FormInstanceID = ViewModel.FormInstanceId;
                    itemToUpdate.DocId = ViewModel.DocumentId;
                    this._unitOfWork.RepositoryAsync<PBPImportDetails>().Update(itemToUpdate);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                //bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                _logger.ErrorException("Error in UpdateImportQueue : ", ex);
                throw ex;
            }
            return result;
        }

        public ServiceResult DiscardImportChanges(int pbpImportId, string UpdateBy)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                result = DeletePBPConfig(pbpImportId, UpdateBy);
                if (result.Result == ServiceResultStatus.Success)
                {
                    result = UpdateImportQueueStatus(pbpImportId, domain.entities.Enums.ProcessStatusMasterCode.Cancel);
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

        public List<PBPImportQueueViewModel> GetPBPImportQueueListForEmailNotification()
        {
            List<PBPImportQueueViewModel> PBPImportQueueList = null;
            string ProcessStatusText = string.Empty;
            PBPImportQueueList = (from queue in this._unitOfWork.RepositoryAsync<PBPImportQueue>().Get()
                                  join db in this._unitOfWork.RepositoryAsync<PBPDatabase>().Get()
                                  on queue.PBPDatabase1Up equals db.PBPDatabase1Up
                                  join status in this._unitOfWork.RepositoryAsync<ProcessStatusMaster>().Get()
                                  on queue.Status equals status.ProcessStatus1Up
                                  where (
                                         queue.Status != (int)ProcessStatusMasterCode.Complete
                                         &&
                                         queue.Status != (int)ProcessStatusMasterCode.Errored
                                         )
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
                                      StatusText = status.ProcessStatusName,
                                      CreatedDate = queue.CreatedDate,
                                      PBPDatabase1Up = queue.PBPDatabase1Up,
                                      DataBaseName = db.DataBaseName,
                                      JobId = queue.JobId,
                                      PBPFileDisplayName = queue.PBPFileDisplayName,
                                      PBPPlanAreaFileDisplayName = queue.PBPPlanAreaFileDisplayName,
                                      //JobLocation = queue.JobLocation
                                      UdateDateTime = queue.UpdatedDate
                                  }).ToList();
            return PBPImportQueueList;
        }

        private PBPDatabaseViewModel GetPBPDatabaseDetails(int PBPImportQueueID)
        {
            PBPImportQueue viewModel = this._unitOfWork.RepositoryAsync<PBPImportQueue>().Get()
                                                   .Where(s => s.PBPImportQueueID.Equals(PBPImportQueueID))
                                                   .FirstOrDefault();

            PBPDatabaseViewModel ViewModel = (from databaselist in this._unitOfWork.RepositoryAsync<PBPDatabase>().Get()
                                              .Where(s => s.PBPDatabase1Up.Equals(viewModel.PBPDatabase1Up))
                                              select new PBPDatabaseViewModel
                                              {
                                                  PBPDatabase1Up = databaselist.PBPDatabase1Up,
                                                  DataBaseName = databaselist.DataBaseName
                                              }).FirstOrDefault();
            return ViewModel;
        }

        public ServiceResult UpdatePBPPlanDetails(PBPPlanConfigViewModel ViewModel, int userAction)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                if (userAction.Equals((int)PBPUserActionList.AddPlanIneBS))
                {
                    PBPImportDetails AddToImport = new PBPImportDetails();
                    AddToImport.PBPDatabase1Up = ViewModel.PBPDataBase1Up;
                    AddToImport.PBPImportQueueID = ViewModel.PBPImportQueueID;
                    AddToImport.PlanName = ViewModel.PlanName;
                    AddToImport.PlanNumber = ViewModel.PlanNumber;
                    AddToImport.ebsPlanName = ViewModel.ebsPlanName;
                    AddToImport.ebsPlanNumber = ViewModel.eBsPlanNumber;
                    AddToImport.QID = ViewModel.QID;
                    AddToImport.FolderId = ViewModel.FolderId;
                    AddToImport.FolderVersionId = ViewModel.FolderVersionId;
                    AddToImport.FormInstanceID = ViewModel.FormInstanceId;
                    AddToImport.DocId = ViewModel.DocumentId;
                    AddToImport.CreatedDate = DateTime.Now;
                    AddToImport.UpdatedBy = "TMG Super User";
                    AddToImport.IsActive = true;
                    AddToImport.Status = (int)ProcessStatusMasterCode.Complete;
                    this._unitOfWork.RepositoryAsync<PBPImportDetails>().Insert(AddToImport);
                    this._unitOfWork.Save();
                    result.Result = ServiceResultStatus.Success;

                }
                else if (userAction.Equals((int)PBPUserActionList.MapItWithAnothereBSPlan) || userAction.Equals((int)PBPUserActionList.UpdatePlan))
                {
                    SqlParameter QID = new SqlParameter("@QID", ViewModel.QID);
                    SqlParameter ebsPlanName = new SqlParameter("@ebsPlanName", ViewModel.ebsPlanName);
                    SqlParameter ebsPlanNumber = new SqlParameter("@ebsPlanNumber", ViewModel.eBsPlanNumber);
                    SqlParameter PlanName = new SqlParameter("@PlanName", ViewModel.PlanName);
                    SqlParameter PlanNumber = new SqlParameter("@PlanNumber", ViewModel.PlanNumber);
                    SqlParameter PBPDatabase1Up = new SqlParameter("@PBPDatabase1Up", ViewModel.PBPDataBase1Up);
                    SqlParameter IsActive = new SqlParameter("@IsActive", 1);
                    SqlParameter Status = new SqlParameter("@Status", (int)ProcessStatusMasterCode.Complete);
                    SqlParameter FormInstanceId = new SqlParameter("@FormInstanceId", ViewModel.FormInstanceId);
                    var log = this._unitOfWork.Repository<PBPImportDetails>()
                          .ExecuteSql("exec [dbo].[UpdatePBPImportDetails] @QID,@ebsPlanName,@ebsPlanNumber,@PlanName,@PlanNumber,@PBPDatabase1Up,@IsActive,@Status,@FormInstanceId",
                          QID, ebsPlanName, ebsPlanNumber, PlanName, PlanNumber, PBPDatabase1Up, IsActive, Status, FormInstanceId).ToList().FirstOrDefault();
                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                IPBPImportActivityLogServices Obj = new PBPImportActivityLogServices(this._unitOfWork);
                Obj.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "", null, null, ViewModel.QID, ex);
            }
            return result;
        }

        public ServiceResult UpdateAccountProductMap(PBPPlanConfigViewModel ViewModel)
        {
            ServiceResult Result = new ServiceResult();
            try
            {

                SqlParameter IsUpdate = new SqlParameter("@IsUpdate", 1);
                SqlParameter ProductID = new SqlParameter("@ProductID", ViewModel.PlanNumber);
                SqlParameter ProductType = new SqlParameter("@ProductType", ViewModel.PlanName);
                SqlParameter FormInstance = new SqlParameter("@FormInstanceId", ViewModel.FormInstanceId);
                SqlParameter FolderId = new SqlParameter("@FolderId", ViewModel.FolderId);
                SqlParameter FolderVaersionId = new SqlParameter("@FolderVersionID", ViewModel.FolderVersionId);
                var ResultService = this._unitOfWork.Repository<AccountProductMap>()
                    .ExecuteSql("exec [dbo].[UpdateAccountProductMap] @ProductID,@ProductType,@FormInstanceId,@FolderId,@FolderVersionID,@IsUpdate",
                                ProductID, ProductType, FormInstance, FolderId, FolderVaersionId, IsUpdate).ToList().FirstOrDefault();

                if (ResultService == null)
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
                IPBPImportActivityLogServices Obj = new PBPImportActivityLogServices(this._unitOfWork);
                Obj.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "", null, null, ViewModel.QID, ex);
            }
            return Result;
        }

        public void SaveMissMatchQIdInAccessFile(List<PBPTableViewModel> PBPTableData, int PBPDataBase1Up, int pbpimportId, string CreatedBy)
        {

            List<PBPPlanViewModel> AccessPlanList= PBPTableData.
                                                   Select(s => s.PBPPlanList)
                                                   .FirstOrDefault();

            IList<PBPPlanConfigViewModel> EbsPlanList = GetPBPPlanDetailsList().ToList()
                                                    .Where(s => s.PBPDataBase1Up.Equals(PBPDataBase1Up))
                                                    .ToList();
            foreach (var item in EbsPlanList)
            {
                if (AccessPlanList.Where(s => s.QID == item.QID).Count()==0)
                {
                    PBPPlanViewModel PBPPlanModel = (from t in EbsPlanList.Where(s => s.QID.Equals(item.QID))
                                                     select new PBPPlanViewModel
                                                     {
                                                         QID = t.QID,
                                                         PBPPlanName = t.PlanName,
                                                         PBPPlanNumber = t.PlanNumber,
                                                         ebsPlanName = t.ebsPlanName,
                                                         ebsPlanNumber = t.eBsPlanNumber,
                                                         FolderId = t.FolderId,
                                                         FolderVersionId = t.FolderVersionId,
                                                         FormInstanceId = t.FormInstanceId,
                                                         DocId = t.DocumentId,
                                                         IsTerminateVisiable = true
                                                     }).FirstOrDefault();
                    if (PBPPlanModel != null)
                    {
                        int Result = SavePBPConfig(pbpimportId, PBPPlanModel, CreatedBy, false);
                    }
                }
            }
        }

        public ServiceResult UpdatePlanConfig(int tenantId, IList<PBPMatchConfigViewModel> PBPMatchConfigList, string UpdateBy)
        {
            ServiceResult Result = new ServiceResult();
            int PBPImportQueueID = 0;
            List<PBPMatchConfig> itemToUpdateList = new List<PBPMatchConfig>();
            try
            {
                if (PBPMatchConfigList != null)
                {
                    PBPMatchConfigList = PBPMatchConfigList.Where(s => s.UserAction != (int)PBPUserActionList.UpdatePlan).ToList();
                    PBPImportQueueID = PBPMatchConfigList.Where(s => s.PBPImportQueueID > 0)
                                       .Select(t => t.PBPImportQueueID)
                                       .FirstOrDefault();
                    itemToUpdateList = this._unitOfWork.RepositoryAsync<PBPMatchConfig>().Get()
                                      .Where(s => s.PBPImportQueueID.Equals(PBPImportQueueID)&& s.UserAction!=(int)PBPUserActionList.UpdatePlan
                                      && s.IsActive == true)
                                      .ToList();

                    //var DeletedPlan =
                    List<int> query = itemToUpdateList.Select(s=>s.PBPMatchConfig1Up)
                     .Where(s => !PBPMatchConfigList.Select(t=>t.PBPMatchConfig1Up)
                     .Contains(s)).ToList();

                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            PBPMatchConfig itemToUpdate = itemToUpdateList.
                                                      Where(s => s.PBPMatchConfig1Up.Equals(item))
                                                      .FirstOrDefault();
                            if (itemToUpdate != null)
                            {
                                itemToUpdate.UpdatedDate = DateTime.Now;
                                itemToUpdate.UpdatedBy = UpdateBy;
                                itemToUpdate.IsActive = false;
                                this._unitOfWork.Repository<PBPMatchConfig>().Update(itemToUpdate);
                                this._unitOfWork.Save();
                            }
                        }
                    }

                    foreach (var item in PBPMatchConfigList)
                    {
                        if (item.PBPMatchConfig1Up == 0)
                        {
                            //to do insert login
                            SaveEGWPPlan(item, PBPImportQueueID, UpdateBy, false);
                        }
                        PBPMatchConfig itemToUpdate = itemToUpdateList.
                                                      Where(s => s.PBPMatchConfig1Up.Equals(item.PBPMatchConfig1Up))
                                                      .FirstOrDefault();
                        if (itemToUpdate != null)
                        {
                            if (item.UserAction.Equals((int)PBPUserActionList.AddPlanIneBS))
                            {
                                itemToUpdate.ebsPlanName = itemToUpdate.PlanName;
                                itemToUpdate.ebsPlanNumber = itemToUpdate.PlanNumber;
                            }

                            if (item.UserAction.Equals((int)PBPUserActionList.MapItWithAnothereBSPlan))
                            {
                                itemToUpdate.ebsPlanName = item.ebsPlanName;
                                itemToUpdate.ebsPlanNumber = item.eBsPlanNumber;
                            }

                            if (item.UserAction.Equals((int)PBPUserActionList.NoActionRequired))
                            {
                                itemToUpdate.IsProxyUsed = item.IsProxyUsed;
                            }

                            //using (var scope = new TransactionScope())
                            //{
                            if (item.UserAction.Equals((int)PBPUserActionList.TerminatePlanFromeBS))
                            {
                                itemToUpdate.UserAction = item.UserAction;
                            }
                            else
                            {
                                itemToUpdate.FolderId = item.FolderId;
                                itemToUpdate.FolderVersionId = item.FolderVersionId;
                                itemToUpdate.FormInstanceID = item.FormInstanceId;
                                itemToUpdate.UserAction = item.UserAction;
                                itemToUpdate.DocId = item.DocumentId;
                                itemToUpdate.UpdatedDate = DateTime.Now;
                                itemToUpdate.UpdatedBy = UpdateBy;
                            }
                            itemToUpdate.UpdatedDate = DateTime.Now;
                            itemToUpdate.UpdatedBy = UpdateBy;
                            this._unitOfWork.Repository<PBPMatchConfig>().Update(itemToUpdate);
                            this._unitOfWork.Save();
                            //scope.Complete();
                            //}
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

                            //using (var scope = new TransactionScope())
                            //{
                                itemToUpdateList.DocId = item.DocumentId;
                                itemToUpdateList.IsIncludeInEbs = item.IsIncludeInEbs;
                                itemToUpdateList.UserAction = (int)PBPUserActionList.UpdatePlan;
                                itemToUpdateList.UpdatedDate = DateTime.Now;
                                itemToUpdateList.UpdatedBy = UpdateBy;
                                this._unitOfWork.Repository<PBPMatchConfig>().Update(itemToUpdateList);
                                this._unitOfWork.Save();
                                //scope.Complete();
                            //}
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

        private void ProxyPlanListForTerminatePlanfromeBS(int pBPImportId, int pBPDatabase1Up, string CreatedBy)
        {
            List<PortfolioFoldersDocumentViewModel> documentList = null;
            List<PBPMatchConfig> planModelList = new List<PBPMatchConfig>();
            PBPMatchConfig ViewModel;
            ServiceResult Result = new ServiceResult();
            int FormID = this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                 .Where(s => s.FormName.Equals("Medicare")).Select(s => s.FormID).FirstOrDefault();
            try
            {
                List<PBPMatchConfig> PBPDetailsList = this._unitOfWork.RepositoryAsync<PBPMatchConfig>().Get()
                                                      .Where(s=>s.PBPImportQueueID.Equals(pBPImportId)
                                                      && s.UserAction==(int)PBPUserActionList.UpdatePlan
                                                      )
                                                      .ToList();
                documentList = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                join pbpdetail  in this._unitOfWork.RepositoryAsync<PBPImportDetails>().Get()
                                on fi.FormInstanceID equals pbpdetail.FormInstanceID 
                                join fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get() on fi.FolderVersionID equals fv.FolderVersionID
                                join fvs in this._unitOfWork.RepositoryAsync<tmg.equinox.domain.entities.Models.FolderVersionState>().Get() on fv.FolderVersionStateID equals fvs.FolderVersionStateID
                                join fldr in this._unitOfWork.RepositoryAsync<Folder>().Get() on fv.FolderID equals fldr.FolderID
                                join accprodmap in this._unitOfWork.RepositoryAsync<AccountProductMap>().Get()
                               on fi.FormInstanceID equals accprodmap.FormInstanceID
                                join cat in this._unitOfWork.RepositoryAsync<FolderVersionCategory>().Get()
                                on fv.CategoryID equals cat.FolderVersionCategoryID
                                join cons in this._unitOfWork.RepositoryAsync<Consortium>().Get()
                                on fv.ConsortiumID equals cons.ConsortiumID
                               
                                into wt1
                                from wt in wt1.DefaultIfEmpty()
                                where (
                                     (fldr.IsPortfolio == true)
                                      && fldr.Name != "Master List"
                                      // && fi.FormDesignID == 3
                                      && fvs.FolderVersionStateID == 1
                                      && fi.FormDesignID == FormID
                                      && fi.IsActive == true
                                      && fv.IsActive==true                                      
                                      && (pbpdetail.PBPDatabase1Up==0 || pbpdetail.PBPDatabase1Up.Equals(pBPDatabase1Up))
                                     && pbpdetail.IsActive==true
                                      )
                                select new PortfolioFoldersDocumentViewModel
                                {
                                    FolderID = fldr.FolderID,
                                    FolderName = fldr.Name,
                                    ConsortiumName = wt.ConsortiumName,
                                    FolderVersionID = fv.FolderVersionID,
                                    FolderVersionStateName = fvs.FolderVersionStateName,
                                    VersionNumber = fv.FolderVersionNumber,
                                    EffectiveDate = fv.EffectiveDate,
                                    FormInstanceID = fi.FormInstanceID,
                                    FormInstanceName = fi.Name,
                                    DocumentId = fi.DocID,
                                    eBsPlanName = accprodmap.ProductType,
                                    eBsPlanNumber = accprodmap.ProductID
                                })
                                .ToList();
                List<PortfolioFoldersDocumentViewModel> documentList1 = documentList
                                                                        .Where(item => !PBPDetailsList
                                                                        .Any(item2 => item2.QID == item.FormInstanceName))
                                                                        .OrderBy(s => s.FolderName)
                                                                        .ToList();

                foreach (var item in documentList1)
                {
                    ViewModel = new PBPMatchConfig
                    {
                        PBPImportQueueID = pBPImportId,
                        FolderId = (int)item.FolderID,
                        FolderVersionId = (int)item.FolderVersionID,
                        FormInstanceID = (int)item.FormInstanceID,
                        DocId = (int)item.DocumentId,
                        ebsPlanName = item.eBsPlanName,
                        ebsPlanNumber = item.eBsPlanNumber,
                        IsTerminateVisible = true,
                        UserAction = (int)PBPUserActionList.NoActionRequired,
                        UpdatedDate = DateTime.Now,
                        UpdatedBy = CreatedBy,
                        CreatedDate = DateTime.Now.Date,
                        IsActive = true,
                        CreatedBy = CreatedBy,
                    };
                    planModelList.Add(ViewModel);
                }
               Result= SavePBPConfigList(planModelList, false);
            }
            catch (Exception ex)
            {
                Result = ex.ExceptionMessages();
                Result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
        }

        public ServiceResult SavePBPConfigList(List<PBPMatchConfig> PBPPlanModelList, bool IsmatchPlan)
        {
            ServiceResult Result = new ServiceResult();
            try
            {
                if (IsmatchPlan == false)
                {
                    this._unitOfWork.RepositoryAsync<PBPMatchConfig>().InsertRange(PBPPlanModelList);
                    this._unitOfWork.Save();
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

        private void SaveEGWPPlan(PBPMatchConfigViewModel PBPPlanModel, int PBPImportQueueID, string CreateBy, bool IsmatchPlan)
        {
            if (IsmatchPlan == false)
            {
                PBPMatchConfig AddToImport = new PBPMatchConfig();
                AddToImport.QID = PBPPlanModel.PlanNumber;
                AddToImport.PlanName = PBPPlanModel.PlanName;
                AddToImport.PlanNumber = PBPPlanModel.PlanNumber;
                AddToImport.ebsPlanName = PBPPlanModel.ebsPlanName;
                AddToImport.ebsPlanNumber = PBPPlanModel.eBsPlanNumber;
                AddToImport.PBPImportQueueID = PBPImportQueueID;
                AddToImport.CreatedDate = DateTime.Now.Date;
                AddToImport.IsActive = true;
                AddToImport.CreatedBy = CreateBy;
                AddToImport.UserAction = PBPPlanModel.UserAction;
                AddToImport.FolderId = PBPPlanModel.FolderId;
                AddToImport.FolderVersionId = PBPPlanModel.FolderVersionId;
                AddToImport.FormInstanceID = PBPPlanModel.FormInstanceId;
                AddToImport.DocId = PBPPlanModel.DocumentId;
                AddToImport.IsTerminateVisible = PBPPlanModel.IsTerminateVisiable;
                AddToImport.UpdatedDate = DateTime.Now;
                AddToImport.UpdatedBy = CreateBy;
                AddToImport.Year = PBPPlanModel.Year;
                AddToImport.IsEGWPPlan = PBPPlanModel.IsEGWPPlan;
                this._unitOfWork.RepositoryAsync<PBPMatchConfig>().Insert(AddToImport);
                this._unitOfWork.Save();
            }
        }

        private void DeleteEGWPPlan()
        {

        }

        private List<PBPPlanConfigViewModel> GetAllMatchingEGWPPlanDetail(int pbpDatabase1Up, string qId)
        {
           List<PBPPlanConfigViewModel> PlanDetails = new List< PBPPlanConfigViewModel>();

            if (!string.IsNullOrEmpty(qId))
            {
                PlanDetails = (from queue in this._unitOfWork.RepositoryAsync<PBPImportDetails>().Get()
                               join flder in this._unitOfWork.RepositoryAsync<Folder>().Get()
                               on queue.FolderId equals flder.FolderID
                               join fldrVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                               on flder.FolderID equals fldrVersion.FolderID
                               join forminst in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                               on queue.FormInstanceID equals forminst.FormInstanceID
                               where (queue.IsActive == true
                               && queue.PlanNumber.Equals(qId)
                               && queue.PBPDatabase1Up.Equals(pbpDatabase1Up)
                               && flder.IsPortfolio == true
                               && fldrVersion.IsActive == true
                               && forminst.IsActive == true
                               && forminst.FormDesignID.Equals(2359)
                               )
                               select new PBPPlanConfigViewModel
                               {
                                   QID = queue.QID,
                                   PlanName = queue.PlanName,
                                   PlanNumber = queue.PlanNumber,
                                   ebsPlanName = queue.ebsPlanName,
                                   eBsPlanNumber = queue.ebsPlanNumber,
                                   FormInstanceId = queue.FormInstanceID,
                                   FolderId = queue.FolderId,
                                   Year = queue.Year,
                                   UserAction = (int)PBPUserActionList.UpdatePlan,
                                   PBPDataBase1Up = queue.PBPDatabase1Up,
                                   FolderVersionId = fldrVersion.FolderVersionID,
                                   DocumentId = forminst.DocID,
                                   ProductDetails = flder.Name + "_" + fldrVersion.FolderVersionNumber,
                                   FolderVersion = flder.Name + "_" + fldrVersion.FolderVersionNumber,
                               })
                               .ToList();
            }

            List<int> LatestFolderVersionIdList = (from e in PlanDetails
                                                  group e by e.FolderId into dptgrp
                                                  let topFolderVersion = dptgrp.Max(x => x.FolderVersionId)
                                                  select topFolderVersion).ToList();

            List<PBPPlanConfigViewModel> LatestPlanDetails = PlanDetails.
                                Where(s => LatestFolderVersionIdList
                                .Contains(s.FolderVersionId))
                                .ToList();
            return LatestPlanDetails;

        }

        private void SaveAllMatchingEGWPPlanDetail(int pbpDatabase1Up, string qId,int pbpimportqueueId,string createdBy, PBPPlanViewModel pbpPlanItem)
        {
            List<PBPPlanConfigViewModel> PlanList=GetAllMatchingEGWPPlanDetail(pbpDatabase1Up, qId);
            foreach (var MatchingPlan in PlanList)
            {
                pbpPlanItem.FolderId = MatchingPlan.FolderId;
                pbpPlanItem.FolderVersionId = MatchingPlan.FolderVersionId;
                pbpPlanItem.FormInstanceId = MatchingPlan.FormInstanceId;
                pbpPlanItem.DocId = MatchingPlan.DocumentId;
                pbpPlanItem.UserAction = (int)PBPUserActionList.UpdatePlan;
                pbpPlanItem.ebsPlanName = MatchingPlan.ebsPlanName;
                pbpPlanItem.ebsPlanNumber = MatchingPlan.eBsPlanNumber;
                //save matching plan details
                MatchingPlan.PBPMatchConfig1Up = SavePBPConfig(pbpimportqueueId, pbpPlanItem, createdBy, true);
            }

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
                    AccessDBTableService Service = new AccessDBTableService(path + "\\" + PBPImportQueueList.PBPFileName);
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
                AccessDBTableService ServiceObj1 = new AccessDBTableService(path + "\\" + pbpFileName);
                IsValidPBPTableScheme = ServiceObj1.IsTableExist(PBPTABLENAME);
                ServiceObj1 = new AccessDBTableService(path + "\\" + pBPAreaPlanFileName);
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
            PBPTableViewModel File_3_PBPTableList = new PBPTableViewModel();
            try
            {
                AccessDBTableService ServiceObj1 = new AccessDBTableService(path + "\\" + fileName1);
                File_1_PBPTableList = new PBPTableViewModel
                {
                    FileName = fileName1,
                    PBPPlanList = ServiceObj1.ReadPBPTableData()
                };

                AccessDBTableService ServiceObj2 = new AccessDBTableService(path + "\\" + fileName2);
                File_2_PBPTableList = new PBPTableViewModel
                {
                    FileName = fileName2,
                    PBPPlanList = ServiceObj2.ReadPBPPlanAreaTableData()
                };

                AccessDBTableService ServiceObj3 = new AccessDBTableService(path + "\\" + fileName2);
                File_3_PBPTableList = new PBPTableViewModel
                {
                    FileName = fileName2,
                    PBPPlanList = ServiceObj2.ReadPBPRegionsTableData()
                };

                if (File_3_PBPTableList.PBPPlanList != null)
                {
                    if (File_3_PBPTableList.PBPPlanList.Count() > 0)
                    {
                        File_2_PBPTableList.PBPPlanList = File_2_PBPTableList.PBPPlanList
                                                        .Union(File_3_PBPTableList.PBPPlanList)
                                                        .ToList();
                    }
                }
                 
                bool IsMisMatch = File_1_PBPTableList.PBPPlanList.Where(item => !File_2_PBPTableList.PBPPlanList.Any(item2 => item2.QID == item.QID) && item.IsEGWP != "1").Any();

                if (IsMisMatch)
                {
                    var MisMatchQIDList = File_1_PBPTableList.PBPPlanList.Where(item => !File_2_PBPTableList.PBPPlanList.Any(item2 => item2.QID == item.QID) && item.IsEGWP != "1").Select(s => s.QID);
                    List<ServiceResultItem> items = new List<ServiceResultItem>();
                    foreach (var item in MisMatchQIDList)
                    {
                        items.Add(new ServiceResultItem { Messages = new string[] { item } });
                        Result.Items = items;
                    }
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

        public int GetContractYear(string fileName1)
        {
            int Year = 0;
            AccessDBTableService ServiceObj1 = new AccessDBTableService(IMPORTFILEPATH + "\\" + fileName1);
           return Year = ServiceObj1.GetPlanYear();
        }

        #endregion Validate AccessFile,File Data

        #region PBPImportHelper
        private string ProductDetailsHelper(IList<Folder> FolderList, int folderid, IList<FolderVersion> FolderVersionList, int folderversionid, IList<FormInstance> FormInstanceList, int forminstanceid)
        {
            string Str = string.Empty;
            string foldername = string.Empty;
            string fromInstName = string.Empty;
            if (folderid > 0)
            {
                foldername = FolderList.Where(s => s.FolderID.Equals(folderid)).Select(s => s.Name).FirstOrDefault();
                Str = foldername != null ? foldername + "_" : null;
            }
            if (folderid > 0)
            {
                var FolderVersionDetails = FolderVersionList.Where(s => s.FolderID.Equals(folderid))
                                      .OrderByDescending(s => s.FolderVersionID).FirstOrDefault();
                Str += FolderVersionDetails.FolderVersionNumber != null ? FolderVersionDetails.FolderVersionNumber : null;
                //Str += FolderVersionDetails.FolderVersionNumber != null ? FolderVersionDetails.FolderVersionNumber + "_" : null;
            }

            //if (forminstanceid > 0)
            //{
            //    fromInstName = FormInstanceList.Where(s => s.FormInstanceID.Equals(forminstanceid)).Select(s => s.Name).FirstOrDefault();
            //    Str += fromInstName != null ? fromInstName : null;
            //}
            return Str;
        }

        private IList<PBPPlanConfigViewModel> PlanListHelper(IList<PBPPlanConfigViewModel> PlanList, int PBPImportQueueID)
        {
            IList<FormInstance> FormInstanceList = this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                                    .Where(s => s.IsActive == true && s.FormDesignID.Equals(2359))
                                                    .ToList();
            var currentFolders = from s in "st"
                                 select new
                                 {
                                     FolderID = 0,
                                     FolderVersionID = 0,
                                     VersionCount = 0
                                 };

            currentFolders = from s in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                             join fl in this._unitOfWork.RepositoryAsync<Folder>().Get()
                             .Where(fld => fld.IsPortfolio == true)
                             on s.FolderID equals fl.FolderID
                             group s by s.FolderID into g
                             select new { FolderID = g.Key, FolderVersionID = g.Max(s => s.FolderVersionID)
                             , VersionCount = g.Where(x => x.FolderVersionStateID != (int)FolderVersionState.BASELINED).Distinct().Count() };

            List<PortfolioViewModel> portfolioDetailsList = (from fldv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                                             join c in currentFolders on new { fldv.FolderID, fldv.FolderVersionID } equals new { c.FolderID, c.FolderVersionID }
                                                             join fol in this._unitOfWork.RepositoryAsync<Folder>().Query().Get()
                                                             on fldv.FolderID equals fol.FolderID
                                                                 into obj
                                                             from p in obj.DefaultIfEmpty()
                                                             select new PortfolioViewModel
                                                             {
                                                                 FolderName = p.Name,
                                                                 FolderID = p.FolderID,
                                                                 FolderVersionID = fldv.FolderVersionID,
                                                                 VersionNumber = fldv.FolderVersionNumber,
                                                                 EffectiveDate = fldv.EffectiveDate,
                                                             }).ToList();

            foreach (var item in PlanList.ToList())
            {
                if (item.FolderId > 0)
                {
                    var FolderDetail = portfolioDetailsList.Where(s => s.FolderID.Equals(item.FolderId)).FirstOrDefault();
                    item.ProductDetails = string.Concat(FolderDetail.FolderName + "_" + FolderDetail.VersionNumber);
                    item.FolderVersion = item.ProductDetails;
                    if (item.UserAction.Equals((int)PBPUserActionList.UpdatePlan))
                    {
                        var Temp = FormInstanceList.Where(s => s.FormInstanceID.Equals(item.FormInstanceId)).FirstOrDefault();
                        if (Temp != null)
                        {
                            item.DocumentId = Temp.DocID;
                        }
                    }
                    if (FolderDetail != null)
                    {
                        if (Convert.ToInt32(FolderDetail.EffectiveDate.Value.Year).Equals(item.Year))
                        {
                            item.IsDisableIsIncludeIneBsFlag = true;
                            item.IsIncludeInEbs = true;
                        }
                        else
                        {
                            item.IsDisableIsIncludeIneBsFlag = false;
                            item.IsIncludeInEbs = false;
                        }
                    }
                }
            }
            return PlanList;
        }

        private class FolderDetailsViewModel
        {
            public List<Folder> FolderList { get; set; }
            public List<FolderVersion> FolderVersionList { get; set; }
            public List<FormInstance> FormInstanceList { get; set; }
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

        private string StatusTextHelper(int statusCode)
        {
            string cellvalue = string.Empty;
            switch (statusCode)
            {

                case 1:
                    // If status is Queued then display status text in back color 
                    cellvalue = "<span style='color:blue'> Queued </span>";
                    //cellvalue = "Queued";
                    break;
                case 2:
                    // If status is Queued then display status text in back color 
                    cellvalue = "<span style='color:blue'> In Progress </span>";
                    //cellvalue = "In Progress";
                    break;
                case 3:
                    // If status is Queued then display status text in back color 
                    cellvalue = "<span style='color:blue'> Errored </span>";
                    //cellvalue = "Errored";

                    break;
                case 4:
                    // If status is Queued then display status text in back color 
                    cellvalue = "<span style='color:blue'> Complete </span>";
                    //cellvalue = "Complete";

                    break;
                case 5:
                    // If status is Queued then display status text in back color 
                    cellvalue = "<span style='color:blue'> Finalized </span>";
                    cellvalue = "Finalized";

                    break;
                case 6:
                    // If status is Queued then display status text in back color 
                    cellvalue = "<span style='color:blue'> Not Scheduled </span>";
                    //cellvalue = "Not Scheduled";

                    break;
                case 7:
                    // If status is Queued then display status text in back color 
                    cellvalue = "<span style='color:blue'> Scheduled </span>";
                    //cellvalue = "Scheduled";

                    break;
                case 8:
                    // If status is Queued then display status text in back color 
                    cellvalue = "<span style='color:blue'> In Review </span>";
                    //cellvalue = "In Review";

                    break;
                case 9:
                    // If status is Queued then display status text in back color 
                    cellvalue = "<span style='color:blue'> Cancel </span>";
                    //cellvalue = "Cancel";
                    break;
            }
            return cellvalue;
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
                                                     Year= queue.Year
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

        #endregion PBPDataBase

        #region Process Status

        public Dictionary<int, string> ProcessStatusKeyValuePair = new Dictionary<int, string>()
        {
            { 1,"Queued" },
            { 2,"In Progress" },
            { 3,"Errored" },
            { 4,"Complete" },
            { 5,"Finalized" },
            { 6,"Not Scheduled" },
            { 7,"Scheduled" },
            { 8,"In Review" },
            {9,"Cancel" }
        };
        #endregion

        private string GetActivityLog(int PBPImportQueueID)
        {
            string ErrorLogMsg = string.Empty;

            List<PBPImportActivityLogViewModel> ViewModelList = (from queue in this._unitOfWork.RepositoryAsync<PBPImportActivityLog>().Get()
                                                 .Where(s => s.PBPImportQueueID.Equals(PBPImportQueueID))
                                                                 select new PBPImportActivityLogViewModel
                                                                 {
                                                                     PBPImportQueueID = queue.PBPImportQueueID,
                                                                     UserErrorMessage = queue.UserErrorMessage,
                                                                     PBPImportActivityLog1Up = queue.PBPImportActivityLog1Up
                                                                 }).OrderByDescending(s => s.PBPImportActivityLog1Up).ToList();

            if (ViewModelList.Count > 0)
            {
                ErrorLogMsg = "Error while Processing below QID(s)";
                foreach (var item in ViewModelList)
                {
                    if (item.UserErrorMessage != null)
                    {
                        ErrorLogMsg += item.UserErrorMessage + "\n";
                    }
                }
            }

            return ErrorLogMsg;
        }


        #region PBPImportEmail
        public void SendPBPImportEmail()
        {
            DateTime After2Hours = DateTime.Now.AddMinutes(-2);
            int Hours = After2Hours.Hour;
            List<PBPImportQueueViewModel> QueueList = GetPBPImportQueueListForEmailNotification()
                                                      .ToList();
            try
            {
                foreach (var item in QueueList)
                {
                    item.UpdateHours = item.UdateDateTime.Hour;
                }
                QueueList = QueueList.Where(s => s.UpdateHours <= Hours).ToList();

                string ImportInfoMsg = "<table><tr><th>PBP Import ID</th><th>PBP Import Status</th><th>Imported By</th><th>Base Name</th></tr>";
                if (QueueList.Count() > 0)
                {
                    foreach (var item in QueueList)
                    {
                        ImportInfoMsg += "<tr>";
                        ImportInfoMsg += "<td>" + item.PBPImportQueueID + "</td>";
                        ImportInfoMsg += "<td>" + item.StatusText + "</td>";
                        ImportInfoMsg += "<td>" + item.AddedBy + "</td>";
                        ImportInfoMsg += "<td>" + item.DataBaseName + "</td>";
                        ImportInfoMsg += "</tr>";
                    }
                    ImportInfoMsg += "</table>";

                    //var _emailService = new EmailNotificationService(this._unitOfWork);
                    var _emailService = new EmailNotificationService(this._unitOfWork, _coreunitOfWork);
                    List<string> sendMailToList = GetEmailIDList();
                    if (sendMailToList.Count > 0)
                    {
                        EmailTemplateInfo templateInfo = _emailService.GetEmailTemplateInfo<EmailTemplateInfo>(EmailTemplateTypes.PBPImportQueueNotification);
                        templateInfo.SetValue("#PBPInfoTable#", ImportInfoMsg);
                        EmailNotificationInfo emailNotificationInfo = new EmailNotificationInfo();
                        emailNotificationInfo.TemplateInfo = templateInfo;
                        emailNotificationInfo.ToBeSendDateTime = DateTime.Now;
                        emailNotificationInfo.ToAddresses = sendMailToList;
                        _emailService.SendEmail(emailNotificationInfo);
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }

        List<string> GetEmailIDList()
        {
            List<string> EmailList = new List<string>();
            List<PBPImportEmailNotificationViewModel> ViewModelList = (from list in this._unitOfWork.RepositoryAsync<PBPImportEmailNotification>().Get()
                                          .Where(s => s.IsActive.Equals(true))
                                                                       select new PBPImportEmailNotificationViewModel
                                                                       {
                                                                           PBPImportEmailNotification1Up = list.PBPImportEmailNotification1Up,
                                                                           Email = list.Email,
                                                                           IsActive = list.IsActive
                                                                       }
                                          ).ToList();

            foreach (var item in ViewModelList)
            {
                if (!String.IsNullOrEmpty(item.Email))
                {
                    EmailList.Add(item.Email);
                }
            }
            return EmailList;
        }
        #endregion PBPImportEmail
    }
}
