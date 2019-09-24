using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;
using System.Transactions;
using tmg.equinox.domain.entities.Utility;
using tmg.equinox.domain.entities.Enums;
using FolderVersionState = tmg.equinox.domain.entities.Enums.FolderVersionState;
using VersionType = tmg.equinox.domain.entities.Enums.VersionType;
using tmg.equinox.applicationservices.FolderVersionDetail;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.applicationservices.viewmodels.EmailNotitication;
using tmg.equinox.emailnotification;
using tmg.equinox.emailnotification.model;
using tmg.equinox.emailnotification.Model;
using System.Net.Mail;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.applicationservices.viewmodels.DashBoard;
using tmg.equinox.notification;

namespace tmg.equinox.applicationservices
{
    public partial class WorkFlowStateServices : IWorkFlowStateServices
    {
        #region Private Memebers

        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private IFolderVersionServices _folderVersionService { get; set; }
        private EmailSetting emailSettings = new EmailSetting();
        private EmailLogger emailLoggerElements;
        private List<string> sendMailToList;
        private SendGridEmailNotification sendGridInvocation;
        private SmtpEmailNotification smtpInvocation;
        private EmailResponseData emailAcknowledgement;
        private IWorkFlowVersionStatesAccessService _workFlowStateAccessService { get; set; }
        private IReportQueueServices _reportQueueService { get; set; }
        INotificationService _notificationService;
        private IPlanTaskUserMappingService _planTaskUserMappingService;
        Func<string, IUnitOfWorkAsync> _coreunitOfWork;

        #endregion Private Members

        #region Public Properties

        // public string NewFolderVersionId { get; set; }

        #endregion Public Properties

        #region Constructor
        public WorkFlowStateServices(IUnitOfWorkAsync unitOfWork, IFolderVersionServices folderVersionService, IWorkFlowVersionStatesAccessService workFlowStateAccessService, IReportQueueServices reportQueueService, IPlanTaskUserMappingService planTaskUserMappingService, INotificationService notificationService, Func<string, IUnitOfWorkAsync> coreunitOfWork)
        {
            this._unitOfWork = unitOfWork;
            this._folderVersionService = folderVersionService;
            this._workFlowStateAccessService = workFlowStateAccessService;
            this._reportQueueService = reportQueueService;
            _planTaskUserMappingService = planTaskUserMappingService;
            this._notificationService = notificationService;
            _coreunitOfWork = coreunitOfWork;
        }
        #endregion Constructor

        #region Public Methods

        public int GetFolderVersionWorkFlowId(int tenantId, int folderVersionId)
        {
            try
            {
                var WFStateID = (from c in this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>()
                                                .Query()
                                                .Filter(c => c.TenantID == tenantId && c.IsActive == true
                                                          && c.FolderVersionID == folderVersionId)
                                                .Get()
                                 select new FolderVersionWorkFlowViewModel
                                 {
                                     FolderVersionWorkFlowStateID = c.WFStateID,
                                 }).ToList().OrderByDescending(d => d.FolderVersionWorkFlowStateID);
                if (WFStateID.Count() > 0)
                {
                    return (int)WFStateID.FirstOrDefault().FolderVersionWorkFlowStateID;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return 1;
        }

        public string GetFolderVersionWorkFlowName(int tenantId, int folderVersionId)
        {
            string WFStateName = "";
            try
            {
                //WFStateName= (from c in this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>()
                //                               .Query().Include(c => c.WorkFlowVersionState).Include(c=>c.WorkFlowVersionState.WorkFlowState)
                //                               .Filter(c => c.TenantID == tenantId && c.IsActive == true
                //                                         && c.FolderVersionID == folderVersionId)
                //                               .Get().OrderByDescending(c => c.FVWFStateID).Select(c=>c.WorkFlowVersionState.WorkFlowState.WFStateName).FirstOrDefault();   

                WFStateName = this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Query()
                    .Include(c => c.WorkFlowVersionState).Include(c => c.WorkFlowVersionState.WorkFlowState)
                    .Filter(x => x.FolderVersionID == folderVersionId && (x.ApprovalStatusID == (int)ApprovalStatus.APPROVED || x.ApprovalStatusID == (int)ApprovalStatus.COMPLETED))
                                                   .Get().OrderByDescending(c => c.FVWFStateID).Select(c => c.WorkFlowVersionState.WorkFlowState.WFStateName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return WFStateName;
        }
        public string GetAcceleratedConfirmationMsg(int wfversionstateId)
        {
            string accelatedMessage = "";
            try
            {
                var WFStateName = (from wfSat in this._unitOfWork.RepositoryAsync<WFVersionStatesApprovalType>().Query().Filter(c => c.WorkFlowVersionStateID == wfversionstateId && c.WorkFlowStateApprovalTypeID == 4).Get()
                                   join wfAptyAct in this._unitOfWork.RepositoryAsync<WFStatesApprovalTypeAction>().Query().Filter(c => c.ActionID == 2).Get()
                                   on wfSat.WFVersionStatesApprovalTypeID equals wfAptyAct.WFVersionStatesApprovalTypeID
                                   join wft in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get()
                                   on wfAptyAct.ActionResponse equals wft.WorkFlowVersionStateID.ToString()
                                   select new
                                   {
                                       WFStateID = wft.WFStateID,
                                       Sequence = wft.Sequence,
                                       WFStateName = wft.WorkFlowState.WFStateName,
                                       WorkFlowVersionID = wft.WorkFlowVersionID
                                   }).FirstOrDefault();
                var currentState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get().Where(c => c.WorkFlowVersionStateID == wfversionstateId).FirstOrDefault();


                var skippedstatelist = (from a in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Query()
                    .Filter(c => c.WorkFlowVersionID == WFStateName.WorkFlowVersionID && c.Sequence < WFStateName.Sequence && c.Sequence >= currentState.Sequence).Include(c => c.WFStateID).Get()
                                        select new
                                        {
                                            a.WorkFlowState.WFStateName
                                        }).ToList();// c.WorkFlowVersionStateID != wfversionstateId &&
                accelatedMessage = "You have selected that the Accelerated workflow applies to this " + skippedstatelist[0].WFStateName + ". This will skip the ";

                var state = 1;
                while (state < skippedstatelist.Count())
                {

                    if (state > 1 && state != skippedstatelist.Count() - 1)
                    {
                        accelatedMessage = accelatedMessage + "," + skippedstatelist[state].WFStateName;
                    }
                    else if (state == skippedstatelist.Count() - 1)
                    {
                        accelatedMessage = accelatedMessage + " and " + skippedstatelist[state].WFStateName;
                    }
                    else
                    {
                        accelatedMessage = accelatedMessage + skippedstatelist[state].WFStateName;
                    }
                    state++;
                }

                accelatedMessage = accelatedMessage + " steps and will push the product directly to the " + WFStateName.WFStateName + " environment.";

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return accelatedMessage;
        }
        public List<FolderVersionWorkFlowViewModel> GetFolderVersionWorkFlowList(int tenantId, int folderVersionId)
        {
            List<FolderVersionWorkFlowViewModel> folderVersionWorkflowList = null;

            try
            {
                folderVersionWorkflowList = (from c in this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Query().Filter(c => c.TenantID == tenantId && c.IsActive == true && c.FolderVersionID == folderVersionId).Get()
                                             join stateMas in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get()
                                             on c.WFStateID equals stateMas.WorkFlowVersionStateID
                                             select new FolderVersionWorkFlowViewModel
                                             {
                                                 FolderVersionID = c.FolderVersionID,
                                                 WorkflowStateID = c.WFStateID,
                                                 TenantID = c.TenantID,
                                                 ApprovalStatusID = c.ApprovalStatusID,
                                                 WFStateGroupID = c.WorkFlowVersionState.WFStateGroupID
                                             }).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return folderVersionWorkflowList;
        }

        public ServiceResult UpdateWorkflowState(int tenantId, int folderVersionId, int workflowStateId, int approvalStatusId,
                                                    string commenttext, int userId, string userName, string majorFolderVersionNumber, string sendGridUserName, string sendGridPassword)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            Contract.Requires(folderVersionId > 0, "Invalid folderVersionId");
            Contract.Requires(workflowStateId > 0, "Invalid workflowStateId");
            Contract.Requires(approvalStatusId > 0, "Invalid approvalStatusId");
            Contract.Requires(userId > 0, "Invalid userId");
            ServiceResult result = null;
            string workFlowStateName = string.Empty;
            int masterWorkFlowStateId = 0;

            if (workflowStateId == 0)
            {
                workflowStateId = GetFolderVersionWorkFlowId(tenantId, folderVersionId);
            }

            try
            {
                result = new ServiceResult();

                masterWorkFlowStateId = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Query().Get().Where(c => c.WorkFlowVersionStateID == workflowStateId).Select(p => p.WFStateID).FirstOrDefault();

                if (approvalStatusId == Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.APPROVED) ||
                    approvalStatusId == Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.COMPLETED))
                {
                    UpdateApprovalStatus(tenantId, folderVersionId, workflowStateId, approvalStatusId,
                                                    commenttext, userId, userName, majorFolderVersionNumber);
                }
                else if (approvalStatusId == Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.NOTAPPROVED))
                {
                    result = UpdateNotApprovedWorkflowState(tenantId, folderVersionId, workflowStateId, approvalStatusId,
                                                        commenttext, userId, userName);

                    if (result.Result == ServiceResultStatus.Failure)
                    {
                        return result;
                    }
                }

                if (folderVersionId > 0)
                {
                    workFlowStateName = GetFolderVersionWorkFlowName(tenantId, folderVersionId);
                }
                //using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(AppSettings.TransactionTimeOutPeriod)))
                {
                    if (approvalStatusId != Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.NOTAPPROVED))
                    {
                        List<ServiceResultItem> items = new List<ServiceResultItem>();
                        WorkFlowVersionState workflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetReleasedWorkflowState(this._unitOfWork, folderVersionId);
                        if (workflowStateId == workflowState.WorkFlowVersionStateID && (approvalStatusId == Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.APPROVED) || approvalStatusId == Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.COMPLETED)))
                        {
                            UpdateWorkFlowStateUserMapOnStatusUpdate(folderVersionId, tenantId, workflowStateId, userName);
                            items.Add(new ServiceResultItem() { Messages = new string[] { folderVersionId.ToString(), "Release" } });

                        }
                        else if (approvalStatusId == Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.APPROVED) || approvalStatusId == Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.COMPLETED))
                        {
                            items.Add(new ServiceResultItem() { Messages = new string[] { folderVersionId.ToString(), workFlowStateName } });
                        }
                        else
                        {
                            items.Add(new ServiceResultItem() { Messages = new string[] { "false", "" + approvalStatusId + "" } });
                        }
                        result.Items = items;
                    }
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                    this._folderVersionService.UpdateFolderChange(tenantId, userName, null, folderVersionId);
                    //scope.Complete();
                }

                //On Success sent EmailNotification
                if (result.Result == ServiceResultStatus.Success)
                {

                    List<string> sendMailToList = GetEmailIdListForEmailNotification(workflowStateId, folderVersionId, approvalStatusId);
                    if (sendMailToList.Count > 0)
                        EmailNotificationOnWorkFlowStateUpdate(userId, userName, tenantId, workflowStateId, folderVersionId, approvalStatusId, sendMailToList, sendGridUserName, sendGridPassword);

                    if (masterWorkFlowStateId == (int)WorkFlowStateType.BenchmarkInternalReviewII)
                    {
                        // if Benchmark / Internal Review II state is completed successfully, then trigger "017 2018 Benefits Member Facing_Post_Benchmark 07242017"
                        // report (report id=3)

                        int folderId = this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Get().Where(c => c.FolderVersionID == folderVersionId).Select(p => p.FolderID).FirstOrDefault();

                        int[] folderIdsForReport = new int[] { folderId };
                        int[] folderVersionIdsForReport = new int[] { folderVersionId };

                        this._reportQueueService.AddReportQueue(3, folderIdsForReport, folderVersionIdsForReport, userName, userId, DateTime.UtcNow, "Enqueued");
                    }
                    else if (masterWorkFlowStateId == (int)WorkFlowStateType.PreBenchmarkApproval)
                    {
                        // if Pre- Benchmark Approval state is completed successfully, then trigger "SOT Report Without Member Facing Logic"
                        // report (report id=9)

                        int folderId = this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Get().Where(c => c.FolderVersionID == folderVersionId).Select(p => p.FolderID).FirstOrDefault();

                        int[] folderIdsForReport = new int[] { folderId };
                        int[] folderVersionIdsForReport = new int[] { folderVersionId };

                        this._reportQueueService.AddReportQueue(9, folderIdsForReport, folderVersionIdsForReport, userName, userId, DateTime.UtcNow, "Enqueued");
                    }

                    //send task notifications
                    List<DPFPlanTaskUserMapping> listDPFPlanTaskUserMapping = this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Query().Get().Where(c => c.FolderVersionID == folderVersionId && c.WFStateID == masterWorkFlowStateId).ToList();
                    var folderDetails = (from folder in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                         join version in this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(c => c.FolderVersionID == folderVersionId)
                                         on folder.FolderID equals version.FolderID
                                         join accMap in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get()
                                         on folder.FolderID equals accMap.FolderID into tmp
                                         from accMap in tmp.DefaultIfEmpty()
                                         select new
                                         {
                                             folderName = folder.Name,
                                             accountName = accMap == null ? "" : accMap.Account.AccountName

                                         }
                    ).FirstOrDefault();
                    var workflowStateGroupId = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetWorkFlowStateGroupId(tenantId, workflowStateId);
                    WorkFlowVersionState nextWorkflowState = null;
                    if (workflowStateGroupId != null && (approvalStatusId == Convert.ToInt32(ApprovalStatus.APPROVED) || approvalStatusId == Convert.ToInt32(ApprovalStatus.COMPLETED)))
                    {
                        var notapprovalID = Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.NOTAPPROVED);
                        int currentWorkflowStateIdToAdd = this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>()
                                                      .Query()
                                                      .Include(f => f.WorkFlowVersionState)
                                                      .Filter(
                                                          c =>
                                                          c.FolderVersionID == folderVersionId &&
                                                          c.ApprovalStatusID == notapprovalID
                                                          && c.IsActive == true
                                                          && c.WFStateID != workflowStateId)
                                                      .Get()
                                                      .OrderBy(c => c.WorkFlowVersionState.Sequence)
                                                      .Select(s => s.WFStateID)
                                                      .FirstOrDefault();
                        workFlowStateName = (from stateMaster in this._unitOfWork.RepositoryAsync<WorkFlowStateMaster>().Get()
                                             join version in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get().Where(c => c.WorkFlowVersionStateID == currentWorkflowStateIdToAdd)
                                             on stateMaster.WFStateID equals version.WFStateID
                                             select stateMaster.WFStateName).FirstOrDefault();
                    }
                    else
                    {
                        nextWorkflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetNextWorkflowState(this._unitOfWork, folderVersionId, workflowStateId, approvalStatusId, true);
                        if (nextWorkflowState != null)
                        {
                            workFlowStateName = (from stateMaster in this._unitOfWork.RepositoryAsync<WorkFlowStateMaster>().Get()
                                                 join version in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get().Where(c => c.WorkFlowVersionStateID == nextWorkflowState.WorkFlowVersionStateID)
                                                 on stateMaster.WFStateID equals version.WFStateID
                                                 select stateMaster.WFStateName).FirstOrDefault();
                        }else
                        {
                            workFlowStateName = "Released";
                        }
                    }

                    List<User> managerList = (from user in this._unitOfWork.RepositoryAsync<User>().Get()
                                              join team in this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Get().Where(c => c.IsDeleted == false && c.IsTeamManager == true)
                                              on user.UserID equals team.UserID
                                              select user
                                      ).ToList();

                    List<Paramters> paramater = new List<Paramters>();
                    paramater.Add(new Paramters { key = "user", Value = userName });
                    paramater.Add(new Paramters { key = "WF State", Value = workFlowStateName });
                    paramater.Add(new Paramters { key = "Folder name", Value = folderDetails.folderName });
                    paramater.Add(new Paramters { key = "Accountname", Value = folderDetails.accountName });

                    foreach (DPFPlanTaskUserMapping task in listDPFPlanTaskUserMapping)
                    {
                        string[] assignedUsers = new string[] { };
                        if (task.AssignedUserName.IndexOf(',') >= 0)
                            assignedUsers = task.AssignedUserName.Split(',');
                        else
                            assignedUsers = new string[] { task.AssignedUserName };

                        foreach (string user in assignedUsers)
                        {
                            _notificationService.SendNotification(
                                new NotificationInfo
                                {
                                    SentTo = user,
                                    MessageKey = MessageKey.TASK_WORKFLOWSTATE_UPDATE,
                                    ParamterValues = paramater,
                                    loggedInUserName = userName,
                                });
                        }
                    }
                    foreach (User user in managerList)
                    {
                        _notificationService.SendNotification(
                            new NotificationInfo
                            {
                                SentTo = user.UserName,
                                MessageKey = MessageKey.TASK_WORKFLOWSTATE_UPDATE,
                                ParamterValues = paramater,
                                loggedInUserName = userName,
                            });
                    }
                }
                //  UpdateWorkFlowStateOnStatusUpdate(folderVersionId, tenantId, userName, workflowStateId);
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

        public ServiceResult AddApplicableTeams(List<int> applicableTeamsIDList, int folderId, int folderVersionId, string addedBy)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();
                if (applicableTeamsIDList != null)
                {
                    foreach (var applicableTeamId in applicableTeamsIDList)
                    {
                        WorkFlowStateFolderVersionMap applicableTeamToAdd = new WorkFlowStateFolderVersionMap();
                        applicableTeamToAdd.ApplicableTeamID = applicableTeamId;
                        applicableTeamToAdd.FolderID = folderId;
                        applicableTeamToAdd.FolderVersionID = folderVersionId;
                        applicableTeamToAdd.AddedBy = addedBy;
                        applicableTeamToAdd.AddedDate = DateTime.Now;
                        this._unitOfWork.RepositoryAsync<WorkFlowStateFolderVersionMap>().Insert(applicableTeamToAdd);
                        this._unitOfWork.Save();
                    }
                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
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

        public bool CheckMilestoneChecklistSection(int folderVersionId, string sectionName)
        {
            bool result = false;
            try
            {
                int WFStateFolderVersionMapID = this._unitOfWork.RepositoryAsync<WorkFlowStateFolderVersionMap>()
                                                .Query()
                                                .Filter(c => c.FolderVersionID == folderVersionId && c.ApplicableTeam.ApplicableTeamName == sectionName)
                                                .Get()
                                                .Select(c => c.WFStateFolderVersionMapID)
                                                .FirstOrDefault();
                if (WFStateFolderVersionMapID > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return result;
        }

        public List<WorkFlowStateFolderVersionMapViewModel> GetApplicableTeams(int folderVersionId)
        {
            List<WorkFlowStateFolderVersionMapViewModel> applicableTeamList = null;

            try
            {
                applicableTeamList = (from c in this._unitOfWork.RepositoryAsync<WorkFlowStateFolderVersionMap>()
                                              .Query()
                                              .Filter(c => c.FolderVersionID == folderVersionId)
                                              .Get()

                                      select new WorkFlowStateFolderVersionMapViewModel
                                      {
                                          ApplicableTeamName = c.ApplicableTeam.ApplicableTeamName
                                      }).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return applicableTeamList;
        }

        public bool isFolderVersionAccelarated(int tenantId, int folderVersionId)
        {
            bool isAccelerated = false;
            int acceleratedApprovalStateId = 0;
            try
            {
                var accelaratedApprovalWFStates = (from c in this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>()
                                                .Query()
                                                .Filter(c => c.TenantID == tenantId && c.IsActive == true
                                                          && c.FolderVersionID == folderVersionId && c.ApprovalStatusID == acceleratedApprovalStateId)
                                                .Get()

                                                   select new FolderVersionWorkFlowViewModel
                                                   {
                                                       FolderVersionWorkFlowStateID = c.WFStateID,
                                                   }).ToList();

                if (accelaratedApprovalWFStates.Count > 0)
                {
                    isAccelerated = true;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return isAccelerated;
        }

        public int GetWorkFlowStateByProductID(string productId)
        {
            int workFlowStateId = 0;
            try
            {

                var workflowStateId = (from fi in this._unitOfWork.Repository<FormInstance>()
                                       .Get()
                                       join fv in this._unitOfWork.Repository<FolderVersion>()
                                       .Get()
                                       on fi.FolderVersionID equals fv.FolderVersionID
                                       join prodMap in this._unitOfWork.Repository<AccountProductMap>()
                                       .Get()
                                       on fi.FormInstanceID equals prodMap.FormInstanceID
                                       where fv.FolderVersionStateID != (int)FolderVersionState.INPROGRESS_BLOCKED && prodMap.ProductID == productId
                                       orderby fi.FormInstanceID descending
                                       select fv.WFStateID).FirstOrDefault();

                workFlowStateId = Convert.ToInt32(workflowStateId);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return workFlowStateId;
        }

        public IList<KeyValue> GetWorkFlowTeamMembers(int tenantId, int folderId, int? currentUserId)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            IList<KeyValue> list = new List<KeyValue>();
            if (folderId > 0 && currentUserId > 0)
            {
                list = (from workflowStateFolderVersionMap in this._unitOfWork.RepositoryAsync<WorkFlowStateFolderVersionMap>().Get()
                        join applicableTeamUserMap in this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Get()
                        on workflowStateFolderVersionMap.ApplicableTeamID equals applicableTeamUserMap.ApplicableTeamID
                        join user in this._unitOfWork.RepositoryAsync<User>().Get()
                        on applicableTeamUserMap.UserID equals user.UserID
                        where user.IsActive == true
                        where (workflowStateFolderVersionMap.FolderID == folderId && workflowStateFolderVersionMap.Folder.TenantID == tenantId && applicableTeamUserMap.IsDeleted == false)// && user.UserID != currentUserId) Make login user visible 
                        select new KeyValue
                        {
                            Key = (int)applicableTeamUserMap.UserID,
                            Value = user.UserName
                        }).Distinct().ToList();

            }
            return list;
        }

        public int GetApprovedWFStateId(int folderVersionId)
        {
            var approvalID = Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.APPROVED);
            int? wfStateID = 1;
            try
            {

                wfStateID = this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>()
                                                            .Query()
                                                            .Filter(c => c.FolderVersionID == folderVersionId && c.IsActive == true && c.ApprovalStatusID == approvalID)
                                                            .Get()
                                                            .OrderByDescending(p => p.FVWFStateID)
                                                            .Select(c => c.WFStateID)
                                                            .FirstOrDefault();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return wfStateID.Value;
        }

        public string GetApprovedWFStateName(int folderVersionId)
        {
            var approvalID = Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.APPROVED);
            string wfStateName = string.Empty;
            try
            {

                wfStateName = this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>()
                                                            .Query().Include(c => c.WorkFlowVersionState).Include(c => c.WorkFlowVersionState.WorkFlowState)
                                                            .Filter(c => c.FolderVersionID == folderVersionId && c.IsActive == true && c.ApprovalStatusID == approvalID)
                                                            .Get()
                                                            .OrderByDescending(p => p.FVWFStateID)
                                                            .Select(c => c.WorkFlowVersionState.WorkFlowState.WFStateName)
                                                            .FirstOrDefault();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return wfStateName;
        }

        /// <summary>
        /// This method is used to save workflow state folder members in 'WorkFlowStateUserMap' table.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="folderId"></param>
        /// <param name="folderVersionId"></param>
        /// <param name="workflowStateId"></param>
        /// <param name="userIdList"></param>
        /// <param name="currentUserName"></param>
        /// <returns></returns>
        public ServiceResult UpdateWorkflowStateFolderMember(int tenantId, int folderId, int folderVersionId, IList<int> userIdList, string currentUserName, int accountId, int currentUserId, string sendGridUserName, string sendGridPassword)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            ServiceResult result = new ServiceResult();
            int approvedWorkflowStateId = 0;
            int currentWorkFlowStateId = 0;
            if (userIdList.Count == 0)
            {
                result.Result = ServiceResultStatus.Failure;
                List<ServiceResultItem> items = new List<ServiceResultItem>();
                items.Add(new ServiceResultItem() { Messages = new string[] { "Please select at least one folder team member." } });
                result.Items = items;
                return result;
            }

            try
            {
                var workFlowStateUserMapList = this._unitOfWork.RepositoryAsync<WorkFlowStateUserMap>()
                                               .Query()
                                               .Filter(c => c.FolderVersionID == folderVersionId && c.IsActive == true)
                                               .Get()
                                               .ToList();

                // get approved workflowStateId to show approved status in dashboard
                int? maxApprovedID = this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Query().Filter(x => x.FolderVersionID == folderVersionId && (x.ApprovalStatusID == (int)ApprovalStatus.APPROVED || x.ApprovalStatusID == (int)ApprovalStatus.COMPLETED))
                                          .Get().Select(m => (int?)m.WFStateID).Max();

                approvedWorkflowStateId = maxApprovedID.HasValue ? (int)maxApprovedID : (from wfVersion in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get().Where(c => c.WorkFlowVersionStateID == maxApprovedID)
                                                                                         join workMast in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get()
                                                                                         on wfVersion.WorkFlowVersionID equals workMast.WorkFlowVersionID
                                                                                         select wfVersion.WorkFlowVersionStateID
                                                                                         ).FirstOrDefault();

                currentWorkFlowStateId = this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Query().Filter(x => x.FolderVersionID == folderVersionId).Get().Select(m => m.WFStateID).Max();



                var workflowStateUserMapToAdd = new WorkFlowStateUserMap();
                foreach (var userId in userIdList)
                {
                    workflowStateUserMapToAdd.WFStateID = currentWorkFlowStateId;
                    workflowStateUserMapToAdd.UserID = userId;
                    workflowStateUserMapToAdd.FolderID = folderId;
                    workflowStateUserMapToAdd.FolderVersionID = folderVersionId;
                    workflowStateUserMapToAdd.AddedBy = currentUserName;
                    workflowStateUserMapToAdd.AddedDate = DateTime.Now;
                    workflowStateUserMapToAdd.IsActive = true;
                    workflowStateUserMapToAdd.TenantID = 1;
                    workflowStateUserMapToAdd.ApprovedWFStateID = approvedWorkflowStateId;
                    this._unitOfWork.RepositoryAsync<WorkFlowStateUserMap>().Insert(workflowStateUserMapToAdd);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;

                    this._folderVersionService.UpdateFolderChange(tenantId, currentUserName, folderId, folderVersionId);
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

            //On Success sent EmailNotification
            if (result.Result == ServiceResultStatus.Success)
            {
                //EmailNotificationToAssignedMembers(currentUserId, currentUserName, tenantId, folderVersionId, currentWorkFlowStateId, approvedWorkflowStateId, userIdList, sendGridUserName, sendGridPassword);
            }
            return result;
        }

        /// <summary>
        /// This method is used to save workflow state folder members in 'WorkFlowStateUserMap' table
        /// </summary>
        /// <param name="folderVersionId"></param>
        /// <param name="userID"></param>
        /// <param name="userRoleID"></param>
        /// <param name="currentUserName"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public ServiceResult UpdateFolderVersionWorkflowStateUser(int tenantId, List<UserRoleAssignmentViewModel> assignedUserList, int folderVersionId, string currentUserName, int? currentUserId, string sendGridUserName, string sendGridPassword, string smtpUserName, string smtpPort, string smtpHostServerName)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            int approvedWorkflowStateId = 0;
            int currentWorkFlowStateId = 0;
            var folderversion = this._unitOfWork.RepositoryAsync<FolderVersion>()
                                               .Query()
                                               .Filter(c => c.FolderVersionID == folderVersionId && c.IsActive == true)
                                               .Get().FirstOrDefault();
            try
            {

                foreach (var assignedUser in assignedUserList)
                {
                    currentWorkFlowStateId = (int)folderversion.WFStateID;

                    var workFlowStateUserMaps = this._unitOfWork.RepositoryAsync<WorkFlowStateUserMap>()
                                               .Query()
                                               .Filter(c => c.UserID == assignedUser.UserID && c.FolderVersionID == folderVersionId && c.IsActive == true)
                                               .Get()
                                               .ToList();
                    if (workFlowStateUserMaps == null || workFlowStateUserMaps.Count() == 0)
                    {
                        var workflowStateUserMapToAdd = new WorkFlowStateUserMap();
                        workflowStateUserMapToAdd.WFStateID = (int)currentWorkFlowStateId;
                        workflowStateUserMapToAdd.UserID = assignedUser.UserID;
                        workflowStateUserMapToAdd.FolderID = folderversion.FolderID;
                        workflowStateUserMapToAdd.FolderVersionID = folderVersionId;
                        workflowStateUserMapToAdd.AddedBy = currentUserName;
                        workflowStateUserMapToAdd.AddedDate = DateTime.Now;
                        workflowStateUserMapToAdd.IsActive = true;
                        workflowStateUserMapToAdd.TenantID = tenantId;
                        workflowStateUserMapToAdd.ApprovedWFStateID = approvedWorkflowStateId;
                        workflowStateUserMapToAdd.ApplicableTeamID = assignedUser.ApplicableTeamID;
                        this._unitOfWork.RepositoryAsync<WorkFlowStateUserMap>().Insert(workflowStateUserMapToAdd);
                        this._unitOfWork.Save();
                    }
                }
                result.Result = ServiceResultStatus.Success;

                if (result.Result == ServiceResultStatus.Success)
                {
                    List<int> userIdList = assignedUserList.Select(a => a.UserID).ToList();
                    if (userIdList.Count > 0)
                        EmailNotificationToAssignedMembers(currentUserId, currentUserName, tenantId, folderVersionId, currentWorkFlowStateId, approvedWorkflowStateId, userIdList, sendGridUserName, sendGridPassword);
                }

                return result;
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

        public ServiceResult ValidateWorkflowStateUser(List<UserRoleAssignmentViewModel> assignedUserList, int folderVersionId)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();

            StringBuilder userNamelist = new StringBuilder();
            if (folderVersionId > 0)
            {
                //List<int> userIdList = assignedUserList.Select(a => a.UserID).ToList();
                var anyDuplicateUserId = assignedUserList.GroupBy(x => x.UserID).Any(g => g.Count() > 1);

                if (anyDuplicateUserId)
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "User(s) cannot be duplicate." } });
                    result.Items = items;
                    result.Result = ServiceResultStatus.Failure;
                }
                else
                {
                    foreach (var assignedUser in assignedUserList)
                    {
                        var workFlowStateUserMaps = this._unitOfWork.RepositoryAsync<WorkFlowStateUserMap>()
                                                   .Query()
                                                   .Filter(c => c.UserID == assignedUser.UserID && c.FolderVersionID == folderVersionId && c.IsActive == true)
                                                   .Get()
                                                   .FirstOrDefault();
                        if (workFlowStateUserMaps != null)
                        {
                            string userName = this._unitOfWork.RepositoryAsync<User>().Get().Where(u => u.UserID == workFlowStateUserMaps.UserID).Select(u => u.UserName).FirstOrDefault();
                            userNamelist.Append(userName).Append(" ,");
                        }
                    }
                    items.Add(new ServiceResultItem() { Messages = new string[] { userNamelist.ToString() } });
                    result.Items = items;
                    result.Result = ServiceResultStatus.Failure;
                }


            }
            return result;
        }

        public ServiceResult DeleteWorkFlowVersionStatesUser(List<UserRoleAssignmentViewModel> assignedUserList, int folderVersionId, string currentUserName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                foreach (var assignedUser in assignedUserList)
                {
                    WorkFlowStateUserMap workFlowStateUserMap = this._unitOfWork.RepositoryAsync<WorkFlowStateUserMap>().Query().Get()
                        .Where(u => u.FolderVersionID == folderVersionId && u.ApplicableTeamID == assignedUser.ApplicableTeamID && u.UserID == assignedUser.UserID && u.IsActive == true)
                        .FirstOrDefault();
                    workFlowStateUserMap.IsActive = false;
                    workFlowStateUserMap.UpdatedBy = currentUserName;
                    workFlowStateUserMap.UpdatedDate = DateTime.Now;
                    this._unitOfWork.Repository<WorkFlowStateUserMap>().Update(workFlowStateUserMap);
                    this._unitOfWork.Save();
                }
                result.Result = ServiceResultStatus.Success;
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


        #endregion Public Methods

        #region Private Methods

        private ServiceResult EmailNotificationToAssignedMembers(int? userID, string userName, int tenantID, int folderVersionId, int currentWorkFlowStateId, int approvedWorkFlowStateId, List<int> userList, string sendGridUserName, string sendGridPassword, string smtpUserName, string smtpPort, string smtpHostServerName)
        {
            ServiceResult result = new ServiceResult();
            sendGridInvocation = new SendGridEmailNotification();
            // smtpInvocation = new SmtpEmailNotification();
            try
            {
                var sendMailToList = this._unitOfWork.RepositoryAsync<User>().Get().Where(c => userList.Contains(c.UserID)).Select(sel => sel.Email).ToList();
                emailSettings.To = sendMailToList;
                emailSettings.SendGridUserName = sendGridUserName;
                emailSettings.SendGridPassword = sendGridPassword;
                emailSettings.SendGridFrom = ((!string.IsNullOrEmpty(sendGridUserName)) && (!string.IsNullOrEmpty(emailSettings.DisplayName))) ? new MailAddress(sendGridUserName, emailSettings.DisplayName) : null;
                emailSettings.SmtpUserName = smtpUserName;
                emailSettings.SmtpFrom = ((!string.IsNullOrEmpty(smtpUserName)) && (!string.IsNullOrEmpty(emailSettings.DisplayName))) ? new MailAddress(smtpUserName, emailSettings.DisplayName) : null;
                emailSettings.SmtpServerHostName = smtpHostServerName;
                emailSettings.SmtpPort = Int32.Parse(smtpPort);
                emailLoggerElements = GetWorkFlowStateFolderVersionDetails(folderVersionId, currentWorkFlowStateId, userID, tenantID, userName, sendMailToList, (int)ApprovalStatus.APPROVED);

                ServiceResult emailValidationResult = ValidateEmailSettings(emailSettings);
                if (emailValidationResult.Result == ServiceResultStatus.Success)
                {
                    emailSettings.SubjectLine = string.Format(EmailNotificationConstants.SubjectLineFormatAssignMembers, emailLoggerElements.AccountName, emailLoggerElements.FolderName, emailLoggerElements.FolderEffectiveDate.ToString("MM/dd/yyyy"));
                    emailSettings.Text = string.Format(EmailNotificationConstants.AssignedMemberEmailContentHtml, emailLoggerElements.FolderVersionNumber, emailLoggerElements.WorkFlowState);
                    emailLoggerElements.EmailContent = emailSettings.Text;
                    emailAcknowledgement = sendGridInvocation.SendMessage(emailSettings);//smtpInvocation.SendMessage(emailSettings);
                    emailLoggerElements.Comment = emailAcknowledgement.Message;

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                    emailLoggerElements.Comment = string.Join(", ", emailValidationResult.Items.Select(x => x.Messages.FirstOrDefault().ToString()).ToArray());
                }
            }

            catch (Exception ex)
            {
                //Return Failure result
                emailLoggerElements.Comment = EmailNotificationConstants.UnknownExceptionErrorMessage;
                result = ex.ExceptionMessages();
            }
            UpdateEmailLog(emailLoggerElements);
            return result;
        }

        /// <summary>
        /// 1. Updates currently active WorkflowState = selected workflowstate(From Dropdown) in 'FolderVersion' table
        /// 2. Updates isActive = False in 'FolderVersionWorkFlowState' table if record found for selected workflowstate and isActive = True
        /// 3. Inserts new record with isActive = true for selected workflowstate in 'FolderVersionWorkFlowState' table 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="folderVersionId"></param>
        /// <param name="workflowStateId"></param>
        /// <param name="approvalStatusId"></param>
        /// <param name="commenttext"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        private void UpdateApprovalStatus(int tenantId, int folderVersionId, int workflowStateId, int approvalStatusId,
                                                 string commenttext, int userId, string userName, string majorFolderVersionNumber)
        {

            int currentWorkflowStateId = 0;
            WorkFlowVersionState nextWorkflowState = null;
            int? nextWorkflowStateGroupId = null;
            var folderversionToUpdate = this._unitOfWork.RepositoryAsync<FolderVersion>().FindById(folderVersionId);
            var notapprovalID = Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.NOTAPPROVED);
            var workflowStateGroupId = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetWorkFlowStateGroupId(tenantId, workflowStateId);
            WorkFlowVersionState releasedWorkflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetReleasedWorkflowState(this._unitOfWork, folderVersionId);
            // Find next workflowstate and groupId until reached to release state
            if (workflowStateId != releasedWorkflowState.WorkFlowVersionStateID)
            {
                nextWorkflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetNextWorkflowState(this._unitOfWork, folderVersionId, workflowStateId, approvalStatusId, true);
                nextWorkflowStateGroupId = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetWorkFlowStateGroupId(tenantId, nextWorkflowState.WorkFlowVersionStateID);
            }
            if (folderversionToUpdate != null)
            {
                // Crud operation for sequential workflow state
                if (workflowStateGroupId == null)
                {
                    UpdateSequentialWorkflowState(tenantId, folderVersionId, workflowStateId, approvalStatusId,
                        commenttext, userId, userName, majorFolderVersionNumber, releasedWorkflowState, nextWorkflowState,
                        folderversionToUpdate, notapprovalID, nextWorkflowStateGroupId);
                }
                // Crud operation for Parallel workflow state
                else
                {
                    UpdateParallelWorkflowState(tenantId, folderVersionId, workflowStateId, approvalStatusId,
                        commenttext, userId, userName, majorFolderVersionNumber, currentWorkflowStateId,
                        nextWorkflowState, workflowStateGroupId, notapprovalID, folderversionToUpdate, releasedWorkflowState);
                }
                if (nextWorkflowStateGroupId != null)
                {
                    var parallelWorkFlowList = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>()
                                                   .Query()
                                                   .Filter(c => c.WFStateGroupID == nextWorkflowStateGroupId.Value)
                                                   .Get()
                                                   .Select(c => c.WorkFlowVersionStateID)
                                                   .ToList();

                    for (var i = 0; i < parallelWorkFlowList.Count(); i++)
                    {
                        int workflowStateIdToAdd = parallelWorkFlowList[i];
                        //AddWorkflowStateAfterStatusUpdate(tenantId, folderVersionId, workflowStateIdToAdd, userName, false, true, userId);
                        if(workflowStateIdToAdd > 0)
                        _planTaskUserMappingService.ResetStartDateDueDateOnFolderVersionWorkflowStateChange(folderVersionId, workflowStateIdToAdd);
                    }
                }else
                {
                    if(nextWorkflowState != null)
                    _planTaskUserMappingService.ResetStartDateDueDateOnFolderVersionWorkflowStateChange(folderVersionId, nextWorkflowState.WorkFlowVersionStateID);
                }
                    

            }
            else
            {
                throw new Exception("FolderVersion Does Not exists");
            }

        }

        private void UpdateSequentialWorkflowState(int tenantId, int folderVersionId, int workflowStateId, int approvalStatusId,
                                                   string commenttext, int userId, string userName,
                                                   string majorFolderVersionNumber, WorkFlowVersionState releasedWorkflowState,
                                                   WorkFlowVersionState nextWorkflowState, FolderVersion folderversionToUpdate,
                                                   int notapprovalID, int? nextWorkflowStateGroupId)
        {
            int currentWorkflowStateId;
            int nextWorkflowStateId;
            currentWorkflowStateId = workflowStateId;
            int taskWorkflowStateId;

            if (currentWorkflowStateId != releasedWorkflowState.WorkFlowVersionStateID)
            {
                nextWorkflowStateId = nextWorkflowState.WorkFlowVersionStateID;
                taskWorkflowStateId = nextWorkflowStateId;

                if (nextWorkflowStateId != releasedWorkflowState.WorkFlowVersionStateID)
                {
                    // If nextWorkflowState is other then release then pass ApprovalStatus as selected in dropdown 
                    UpdateFolderVersion(tenantId, folderVersionId, nextWorkflowStateId, approvalStatusId, commenttext, userName,
                                        majorFolderVersionNumber, folderversionToUpdate);
                }
                else
                {
                    // If nextWorkflowState is release then pass ApprovalStatus as NotApproved 
                    UpdateFolderVersion(tenantId, folderVersionId, nextWorkflowStateId, notapprovalID, commenttext, userName,
                                        majorFolderVersionNumber, folderversionToUpdate);
                }

                // Update FolderVersionWorkflowState for currentWorkflowState's Approval Status as selected in dropdown
                UpdateFolderVersionWorkflowState(folderVersionId, currentWorkflowStateId, userName, approvalStatusId, commenttext, userId);

                // Add nextWorkflowState as Not-Approved into FolderVersionWorkflowState Table
                AddFolderVersionWorkflowState(tenantId, folderVersionId, commenttext, userId,
                                              userName, nextWorkflowStateId, nextWorkflowStateGroupId);
            }
            else
            {
                taskWorkflowStateId = currentWorkflowStateId;
                // If release workflow is selected from dropdown then folder version current state is set to release 
                // and in FolderVersionWorkflowState table release state is set to approved
                // Here no need to add next state in FolderVersionWorkflowState
                UpdateFolderVersion(tenantId, folderVersionId, currentWorkflowStateId, approvalStatusId, commenttext, userName,
                                    majorFolderVersionNumber, folderversionToUpdate);
                //Update FolderVersionWorkflowState release state as approved.
                UpdateFolderVersionWorkflowState(folderVersionId, currentWorkflowStateId, userName, approvalStatusId, commenttext, userId);

                _folderVersionService.UpdateRetroChangesWhenReleased(tenantId, folderVersionId, userName, folderversionToUpdate.FolderID);
            }

            //send task notifications
            List<DPFPlanTaskUserMapping> listDPFPlanTaskUserMapping = this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Query().Get().Where(c => c.FolderVersionID == folderVersionId && c.WFStateID == taskWorkflowStateId).ToList();
            var folderDetails = (from folder in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                 join version in this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(c => c.FolderVersionID == folderVersionId)
                                 on folder.FolderID equals version.FolderID
                                 join accMap in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get()
                                 on folder.FolderID equals accMap.FolderID into tmp
                                         from accMap in tmp.DefaultIfEmpty()
                                 select new
                                 {
                                     folderName = folder.Name,
                                     accountName = accMap == null ? "" : accMap.Account.AccountName

                                 }
                   ).FirstOrDefault();
            //string workFlowStateName = (from stateMaster in this._unitOfWork.RepositoryAsync<WorkFlowStateMaster>().Get()
            //                            join version in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get().Where(c => c.WorkFlowVersionStateID == taskWorkflowStateId)
            //                            on stateMaster.WFStateID equals version.WFStateID
            //                            select stateMaster.WFStateName).FirstOrDefault();

            //List<User> managerList = (from user in this._unitOfWork.RepositoryAsync<User>().Get()
            //                          join team in this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Get().Where(c => c.IsDeleted == false && c.IsTeamManager == true)
            //                          on user.UserID equals team.UserID
            //                          select user
            //                          ).ToList();

            //List<Paramters> paramater = new List<Paramters>();
            //paramater.Add(new Paramters { key = "user", Value = userName });
            //paramater.Add(new Paramters { key = "WF State", Value = workFlowStateName });
            //paramater.Add(new Paramters { key = "Folder name", Value = folderDetails.folderName });
            //paramater.Add(new Paramters { key = "Accountname", Value = folderDetails.accountName });

            //foreach (DPFPlanTaskUserMapping task in listDPFPlanTaskUserMapping)
            //{
            //    string[] assignedUsers = new string[] { };
            //    if (task.AssignedUserName.IndexOf(',') >= 0)
            //        assignedUsers = task.AssignedUserName.Split(',');
            //    else
            //        assignedUsers = new string[] { task.AssignedUserName };

            //    foreach (string user in assignedUsers)
            //    {
            //        _notificationService.SendNotification(
            //            new NotificationInfo
            //            {
            //                SentTo = user,
            //                MessageKey = MessageKey.TASK_WORKFLOWSTATE_UPDATE,
            //                ParamterValues = paramater,
            //                loggedInUserName = userName,
            //            });
            //    }
            //}
            //foreach (User user in managerList)
            //{
            //    _notificationService.SendNotification(
            //        new NotificationInfo
            //        {
            //            SentTo = user.UserName,
            //            MessageKey = MessageKey.TASK_WORKFLOWSTATE_UPDATE,
            //            ParamterValues = paramater,
            //            loggedInUserName = userName,
            //        });
            //}
        }

        private List<WorkFlowVersionState> GetWorkflowStateListAfterCurrentWorkflowState(int currentWorkflowStateId, int workFlowVersionId)
        {

            //List<WorkFlowVersionState> worflowStateList = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get().Where(c => c.WorkFlowVersionStateID >= currentWorkflowStateId && c.WorkFlowVersionID == workFlowVersionId).ToList();
            List<WorkFlowVersionState> worflowStateList = (from c in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>()
                                                               .Query()
                                                               .Include(c => c.WorkFlowState)
                                                               .Filter(c => c.WorkFlowVersionStateID >= currentWorkflowStateId && c.WorkFlowVersionID == workFlowVersionId).Get()
                                                           select c).ToList();
            return worflowStateList;
        }

        private void UpdateParallelWorkflowState(int tenantId, int folderVersionId, int workflowStateId, int approvalStatusId,
                                                 string commenttext, int userId, string userName,
                                                 string majorFolderVersionNumber, int currentWorkflowStateId,
                                                 WorkFlowVersionState nextWorkflowState, int? workflowStateGroupId, int notapprovalID,
                                                 FolderVersion folderversionToUpdate, WorkFlowVersionState releasedWorkflowState)
        {

            List<FolderVersionWorkFlowState> folderVersionWorkflowList;
            currentWorkflowStateId = workflowStateId;
            // Get List of NotApproved Parallel WorkflowStates by workflowStateGroupId
            folderVersionWorkflowList = GetNotApprovedParallelWorkflowState(folderVersionId, workflowStateGroupId.Value);

            if (folderVersionWorkflowList.Count() > 1)
            {
                // If NotApproved Parallel WorkflowStates List count is > 1 then 
                // find out which stae is set as current state to approve
                // Here smallest sequence numbered state is set as current 
                // state excluding selected workflowstate from dropdown
                int currentWorkflowStateIdToAdd = this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>()
                                                      .Query()
                                                      .Include(f => f.WorkFlowVersionState)
                                                      .Filter(
                                                          c =>
                                                          c.FolderVersionID == folderVersionId &&
                                                          c.ApprovalStatusID == notapprovalID
                                                          && c.IsActive == true
                                                          //&& c.WorkFlowState.WorkFlowState.WFStateGroupID == workflowStateGroupId.Value
                                                          && c.WFStateID != currentWorkflowStateId)
                                                      .Get()
                                                      .OrderBy(c => c.WorkFlowVersionState.Sequence)
                                                      .Select(s => s.WFStateID)
                                                      .FirstOrDefault();

                // Update FolderVersion current state to samllest sequence associated workflow form Parallel workflow states
                UpdateFolderVersion(tenantId, folderVersionId, currentWorkflowStateIdToAdd, approvalStatusId, commenttext,
                                    userName, majorFolderVersionNumber, folderversionToUpdate);
                //Update FolderVersionWorkflowState as approved for selected workflow from dropdown
                UpdateFolderVersionWorkflowState(folderVersionId, currentWorkflowStateId, userName, approvalStatusId, commenttext, userId);
            }
            // This check is for Last parallel state to be approved from group
            // Next state may be release state or other then release
            else if (folderVersionWorkflowList.Count() == 1)
            {
                // Find list count of all workflow states 
                int totalWorkflowStates = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetWorkflowStatesCount(this._unitOfWork, folderVersionId);
                // Find count of approved and not applicable states
                var approvedAndnotApplicableWorkflowList =
                    this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>()
                        .GetAllApprovedAndNotApplicableStates(tenantId, folderVersionId);

                // This condition is true for next state as release state
                if ((totalWorkflowStates - approvedAndnotApplicableWorkflowList.Count()) == 2)
                {
                    // Find next state to add after parallel state.
                    WorkFlowVersionState nextWorkflowStateToAdd =
                        this._unitOfWork.RepositoryAsync<WorkFlowVersionState>()
                            .GetNextStateAfterParallelWorkflowState(this._unitOfWork, tenantId, workflowStateGroupId.Value, currentWorkflowStateId);

                    //Update FolderVersion current state as release state and pass Approval Status as NotApproved.
                    UpdateFolderVersion(tenantId, folderVersionId, nextWorkflowStateToAdd.WorkFlowVersionStateID, notapprovalID, commenttext,
                                        userName, majorFolderVersionNumber, folderversionToUpdate);

                    // Update FolderVersionWorkflowState as Approved for selected workflow from dropdown
                    UpdateFolderVersionWorkflowState(folderVersionId, currentWorkflowStateId, userName, approvalStatusId, commenttext, userId);

                    // Add release state as Not-Approved into FolderVersionWorkflowState Table
                    AddWorkflowStateAfterStatusUpdate(tenantId, folderVersionId, nextWorkflowStateToAdd.WorkFlowVersionStateID, userName, false, false, userId);
                }
                else
                {
                    currentWorkflowStateId = workflowStateId;
                    // Find next state to add after parallel state.
                    // If next state is other then release then follow previous steps
                    WorkFlowVersionState nextWorkflowStateToAdd =
                        this._unitOfWork.RepositoryAsync<WorkFlowVersionState>()
                            .GetNextStateAfterParallelWorkflowState(this._unitOfWork, tenantId, workflowStateGroupId.Value, currentWorkflowStateId);
                    var nextWorkflowStateGroupIdToAdd =
                        this._unitOfWork.RepositoryAsync<WorkFlowVersionState>()
                            .GetWorkFlowStateGroupId(tenantId, nextWorkflowStateToAdd.WorkFlowVersionStateID);

                    UpdateFolderVersion(tenantId, folderVersionId, nextWorkflowStateToAdd.WorkFlowVersionStateID, approvalStatusId,
                                        commenttext, userName, majorFolderVersionNumber, folderversionToUpdate);

                    UpdateFolderVersionWorkflowState(folderVersionId, currentWorkflowStateId, userName, approvalStatusId, commenttext, userId);

                    if (currentWorkflowStateId != releasedWorkflowState.WorkFlowVersionStateID)
                    {
                        // Add nextWorkflowState as Not-Approved into FolderVersionWorkflowState Table
                        AddFolderVersionWorkflowState(tenantId, folderVersionId, commenttext, userId,
                                                      userName, nextWorkflowStateToAdd.WorkFlowVersionStateID,
                                                      nextWorkflowStateGroupIdToAdd);
                    }
                }
            }
        }


        private void AddFolderVersionWorkflowState(int tenantId, int folderVersionId, string commenttext, int userId, string userName, int nextWorkflowStateId, int? nextWorkflowStateGroupId)
        {
            if (nextWorkflowStateGroupId != null)
            {
                // Add all parallel states of same group simultaneously as not-approved status into FolderVersionWorkflowState
                var parallelWorkFlowList = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>()
                                                    .Query()
                                                    .Filter(c => c.WFStateGroupID == nextWorkflowStateGroupId.Value)
                                                    .Get()
                                                    .Select(c => c.WorkFlowVersionStateID)
                                                    .ToList();

                for (var i = 0; i < parallelWorkFlowList.Count(); i++)
                {
                    int workflowStateIdToAdd = parallelWorkFlowList[i];
                    AddWorkflowStateAfterStatusUpdate(tenantId, folderVersionId, workflowStateIdToAdd, userName, false, true, userId);
                }
            }
            else
            {
                AddWorkflowStateAfterStatusUpdate(tenantId, folderVersionId, nextWorkflowStateId, userName, false, false, userId);
            }
        }

        public void UpdateFolderVersionWorkflowState(int folderVersionId, int workflowStateId, string userName, int approvalStatusId, string commenttext, int userId)
        {
            int notapprovalID = Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.NOTAPPROVED);
            if (this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().IsWorkflowStateExist(folderVersionId, workflowStateId))
            {

                var workflowStateToUpdateList = this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>()
                                                .Query()
                                                .Filter(c => c.FolderVersionID == folderVersionId &&
                                                             c.WFStateID == workflowStateId
                                                             && c.IsActive == true
                                                             && c.ApprovalStatusID == notapprovalID)
                                                .Get()
                                                .ToList();


                if (workflowStateToUpdateList.Count() > 0)
                {

                    foreach (var item in workflowStateToUpdateList)
                    {
                        item.UpdatedBy = userName;
                        item.UpdatedDate = DateTime.Now;
                        item.Comments = commenttext;
                        item.UserID = userId;
                        item.ApprovalStatusID = approvalStatusId;
                        this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Update(item);
                        this._unitOfWork.Save();
                    }
                }
                else
                {
                    throw new Exception("FolderVersionWorkFlowState Does Not exists");
                }
            }
        }

        private void UpdateFolderVersion(int tenantId, int folderVersionId, int workflowStateId, int approvalStatusId,
                                         string commenttext, string userName, string majorFolderVersionNumber,
                                         FolderVersion folderversionToUpdate)
        {
            WorkFlowVersionState releasedWorkflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetReleasedWorkflowState(this._unitOfWork, folderVersionId);
            var workflowSateGroupId = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetWorkFlowStateGroupId(tenantId, workflowStateId);
            int notapprovalID = Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.NOTAPPROVED);
            WorkFlowVersionState releasedWFstate = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetReleasedWorkflowState(this._unitOfWork, folderVersionId);
            if (workflowStateId == releasedWorkflowState.WorkFlowVersionStateID && (approvalStatusId == Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.APPROVED) ||
                (approvalStatusId == Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.COMPLETED))
                ))
            {
                if (string.IsNullOrEmpty(majorFolderVersionNumber))
                {
                    VersionNumberBuilder builder = new VersionNumberBuilder();
                    var folderVersion = this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(c => c.FolderVersionID == folderVersionId).FirstOrDefault();
                    string updatedVersionNumber = builder.GetNextMajorVersionNumber(folderVersion.FolderVersionNumber, folderVersion.EffectiveDate);
                    folderversionToUpdate.FolderVersionNumber = updatedVersionNumber;
                    folderversionToUpdate.FolderVersionStateID = Convert.ToInt32(FolderVersionState.RELEASED);
                    folderversionToUpdate.WFStateID = releasedWFstate.WorkFlowVersionStateID;
                    folderversionToUpdate.Comments = commenttext;
                }
                else
                {
                    folderversionToUpdate.FolderVersionNumber = majorFolderVersionNumber;
                    folderversionToUpdate.FolderVersionStateID = Convert.ToInt32(FolderVersionState.RELEASED);
                    folderversionToUpdate.WFStateID = releasedWFstate.WorkFlowVersionStateID;
                    folderversionToUpdate.Comments = commenttext;
                }

            }
            else
            {
                folderversionToUpdate.WFStateID = workflowStateId;
            }

            folderversionToUpdate.UpdatedBy = userName;
            folderversionToUpdate.UpdatedDate = DateTime.Now;
            this._unitOfWork.RepositoryAsync<FolderVersion>().Update(folderversionToUpdate);
        }

        private FolderVersionWorkFlowState GetFolderVersionWorkflowState(int folderVersionId, int workflowStateId)
        {
            return this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>()
                                                .Query()
                                                .Filter(c => c.WFStateID == workflowStateId && c.FolderVersionID == folderVersionId && c.IsActive == true)
                                                .Get()
                                                .FirstOrDefault();
        }
        /// <summary>
        /// 1. Updates currently active WorkflowState = SetUp in 'FolderVersion' table
        /// 2. Updates isActive = False and approvalStatus = Not Approved in 'FolderVersionWorkFlowState' table if record found for selected workflowstate and isActive = true,
        ///    Otherwise Insert a new record with isActive = True and approvalStatus = Not Approved for selected workflowstate
        /// 3. Retrieve all workflowstate list with isActive = True for a specific FolderVersion from 'FolderVersionWorkFlowState' table and set isActive = False, approvalStatus remain same
        /// 4. Inserts new record with workflowState = SetUp and isActive = true in 'FolderVersionWorkFlowState' table 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="folderVersionId"></param>
        /// <param name="workflowStateId"></param>
        /// <param name="approvalStatusId"></param>
        /// <param name="commenttext"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        private ServiceResult UpdateNotApprovedWorkflowState(int tenantId, int folderVersionId, int workflowStateId, int approvalStatusId,
                                                    string commenttext, int userId, string userName)
        {
            ServiceResult result = null;
            VersionNumberBuilder builder = null;


            try
            {
                builder = new VersionNumberBuilder();
                result = new ServiceResult();

                FolderVersionViewModel folderVersionModel = (from c in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                  .Query()
                                                  .Filter(c => c.TenantID == tenantId &&
                                                    c.FolderVersionID == folderVersionId)
                                                  .Get()
                                                             select new FolderVersionViewModel
                                                             {
                                                                 FolderVersionNumber = c.FolderVersionNumber,
                                                                 EffectiveDate = c.EffectiveDate,
                                                                 FolderId = c.FolderID,
                                                                 FolderVersionStateID = c.FolderVersionStateID
                                                             }).FirstOrDefault();

                var isValid = this._unitOfWork.RepositoryAsync<FolderVersion>().IsValidFolderVersionNumber(folderVersionModel.FolderVersionNumber);

                if (isValid)
                {
                    if (folderVersionModel.FolderVersionStateID == 1)
                    {
                        UpdateNotApprovedWorkflowStatus(tenantId, folderVersionId, workflowStateId, approvalStatusId, commenttext, userId, userName);

                        var updatedVersionNumber = builder.GetNextMinorVersionNumber(folderVersionModel.FolderVersionNumber, folderVersionModel.EffectiveDate);

                        WorkFlowVersionState notApprovedWorkflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetNextWorkflowState(this._unitOfWork, folderVersionId, workflowStateId, approvalStatusId, true);

                        result = this._folderVersionService.BaseLineFolder(tenantId, notApprovedWorkflowState.WorkFlowVersionStateID, folderVersionModel.FolderId,
                            folderVersionId, userId, userName, updatedVersionNumber, null, 0,
                            folderVersionModel.EffectiveDate, isRelease: false, isNotApproved: true, isNewVersion: false);
                    }
                    else
                    {
                        throw new ArgumentException("Only In-Progress versions can be Baselined");
                    }
                }
                else
                {
                    throw new ArgumentException("FolderVersion number is not in proper format");
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

        /// <summary>
        /// This method is used to update workflow state when there ia any translation or transmission error.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="folderVersionId"></param>
        /// <param name="workflowStateId"></param>
        /// <param name="approvalStatusId"></param>
        /// <param name="commenttext"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        private ServiceResult UpdateErrorWorkflowState(int tenantId, int folderVersionId, int workflowStateId, int approvalStatusId,
                                                   string commenttext, int userId, string userName)
        {
            ServiceResult result = null;
            VersionNumberBuilder builder = null;


            try
            {
                builder = new VersionNumberBuilder();
                result = new ServiceResult();

                FolderVersionViewModel folderVersionModel = (from c in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                  .Query()
                                                  .Filter(c => c.TenantID == tenantId &&
                                                    c.FolderVersionID == folderVersionId)
                                                  .Get()
                                                             select new FolderVersionViewModel
                                                             {
                                                                 FolderVersionNumber = c.FolderVersionNumber,
                                                                 EffectiveDate = c.EffectiveDate,
                                                                 FolderId = c.FolderID,
                                                             }).FirstOrDefault();

                var isValid = this._unitOfWork.RepositoryAsync<FolderVersion>().IsValidFolderVersionNumber(folderVersionModel.FolderVersionNumber);

                if (isValid)
                {

                    UpdateNotApprovedWorkflowStatus(tenantId, folderVersionId, workflowStateId, Convert.ToInt32(ApprovalStatus.NOTAPPROVED), commenttext, userId, userName);

                    var updatedVersionNumber = builder.GetNextMinorVersionNumber(folderVersionModel.FolderVersionNumber, folderVersionModel.EffectiveDate);
                    // WorkFlowVersionState errorWorkflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetErrorWorkflowState(this._unitOfWork, tenantId, workflowStateId);
                    WorkFlowVersionState errorWorkflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetNextWorkflowState(this._unitOfWork, folderVersionId, workflowStateId, approvalStatusId, false);
                    result = this._folderVersionService.BaseLineFolder(tenantId, errorWorkflowState.WorkFlowVersionStateID, folderVersionModel.FolderId,
                        folderVersionId, userId, userName, updatedVersionNumber, null, 0,
                        folderVersionModel.EffectiveDate, isRelease: false, isNotApproved: true, isNewVersion: false);
                }
                else
                {
                    throw new ArgumentException("FolderVersion number is not in proper format");
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
        private List<FolderVersionWorkFlowState> GetNotApprovedParallelWorkflowState(int folderVersionId, int workflowSateGroupId)
        {
            var notapprovalID = Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.NOTAPPROVED);
            return this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>()
                                                        .Query()
                                                        .Include(f => f.WorkFlowVersionState)
                                                        .Filter(c => c.FolderVersionID == folderVersionId && c.ApprovalStatusID == notapprovalID
                                                                        && c.IsActive == true)// && c.WorkFlowState.WFStateGroupID == workflowSateGroupId)
                                                        .Get()
                                                        .ToList();
        }

        private void AddWorkflowStateAfterStatusUpdate(int tenantId, int folderVersionId, int workflowStateId,
            string userName, bool isSetupState, bool isParallel, int userId)
        {
            var folderversion = this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Include(c => c.Folder).Filter(c => c.FolderVersionID == folderVersionId).Get().FirstOrDefault();
            WorkFlowVersionState firstWorkflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetFirstWorkflowState(this._unitOfWork, (int)folderversion.CategoryID, folderversion.Folder.IsPortfolio);
            var workflowStateToAdd = new FolderVersionWorkFlowState();
            workflowStateToAdd.TenantID = tenantId;
            workflowStateToAdd.FolderVersionID = folderVersionId;

            // After update any of workflow status to not approved then an entry of setup state is done
            if (isSetupState)
            {
                workflowStateToAdd.WFStateID = firstWorkflowState.WorkFlowVersionStateID;
            }
            else
            {
                workflowStateToAdd.WFStateID = workflowStateId;
            }

            workflowStateToAdd.ApprovalStatusID = Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.NOTAPPROVED);
            workflowStateToAdd.AddedBy = userName;
            workflowStateToAdd.AddedDate = DateTime.Now;
            workflowStateToAdd.IsActive = true;
            workflowStateToAdd.UserID = userId;
            this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Insert(workflowStateToAdd);
        }

        private void UpdateNotApprovedWorkflowStatus(int tenantId, int folderVersionId, int workflowStateId, int approvalStatusId, string commenttext, int userId, string userName)
        {
            var toUpdateFolderVersionWorkflowState = this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>()
                                          .Query()
                                          .Filter(c => c.TenantID == tenantId
                                              && c.FolderVersionID == folderVersionId
                                              && c.WFStateID == workflowStateId
                                              && c.IsActive == true
                                              && c.ApprovalStatusID == approvalStatusId)
                                          .Get()
                                          .FirstOrDefault();
            toUpdateFolderVersionWorkflowState.UpdatedBy = userName;
            toUpdateFolderVersionWorkflowState.UpdatedDate = DateTime.Now;
            toUpdateFolderVersionWorkflowState.Comments = commenttext;
            toUpdateFolderVersionWorkflowState.UserID = userId;
            this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Update(toUpdateFolderVersionWorkflowState);
        }

        /// <summary>
        /// SH This method is used to update WorkflowStateUserMap table's IsActive flag to populate active entry in Dashboard WorkQueue.
        /// </summary>
        /// <param name="folderVersionId"></param>
        /// <param name="TenantId"></param>
        /// <param name="WFStateId"></param>
        /// <param name="userName"></param>
        private void UpdateWorkFlowStateUserMapOnStatusUpdate(int folderVersionId, int tenantId, int WFStateId, string userName)
        {
            var toUpdateWorkFlowStateUserMapList = this._unitOfWork.RepositoryAsync<WorkFlowStateUserMap>()
                                                .Query()
                                                .Include(c => c.Folder)
                                                .Include(c => c.FolderVersion)
                                                .Filter(c => c.FolderVersionID == folderVersionId && c.TenantID == tenantId && c.IsActive == true)
                                                .Get()
                                                .ToList();



            if (toUpdateWorkFlowStateUserMapList.Count > 0)
            {
                foreach (var toUpdateWorkFlowStateUserMap in toUpdateWorkFlowStateUserMapList)
                {
                    if (toUpdateWorkFlowStateUserMap.IsActive)
                    {
                        toUpdateWorkFlowStateUserMap.IsActive = false;
                        toUpdateWorkFlowStateUserMap.UpdatedDate = DateTime.Now;
                        toUpdateWorkFlowStateUserMap.UpdatedBy = userName;
                        this._unitOfWork.RepositoryAsync<WorkFlowStateUserMap>().Update(toUpdateWorkFlowStateUserMap);
                        this._unitOfWork.Save();
                    }
                }

            }

        }

        /// <summary>
        /// This method is used to update WFStateId on 'Status Update' to show latest approved state in WorkQueue.
        /// </summary>
        /// <param name="folderVersionId"></param>
        /// <param name="tenantId"></param>
        /// <param name="userName"></param>
        /// <param name="WFStateId"></param>


        private bool isTeamManagerExist(int applicableTeamID)
        {
            int isTeamManagerExist = this._unitOfWork.Repository<ApplicableTeamUserMap>()
                                                                    .Query()
                                                                    .Filter(c => c.ApplicableTeamID == applicableTeamID && c.IsTeamManager == true && c.IsDeleted == false)
                                                                    .Get()
                                                                    .Select(c => c.UserID)
                                                                    .ToList().Count();

            if (isTeamManagerExist > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #region WorkFlow State Email Notification

        public void SendMailForFoldersWithUnchangedWorkFlowState()
        {
            int psotPreparation = Convert.ToInt32(WorkFlowStateType.PSoTPreparation);
            int notApproved = Convert.ToInt32(ApprovalStatus.NOTAPPROVED);
            var _emailService = new EmailNotificationService(this._unitOfWork, _coreunitOfWork);

            IEnumerable<FolderVersionViewModel> folderList = null;

            folderList = _folderVersionService.GetAllFoldersList();

            if (folderList != null)
            {

                List<FolderVersionViewModel> latestFolderVersions = new List<FolderVersionViewModel>();

                foreach (FolderVersionViewModel folder in folderList)
                {
                    latestFolderVersions.Add(_folderVersionService.GetLatestFolderVersion(1, folder.FolderId));
                }
                var psotPreparationFolderVersions = this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Get().Where(f => f.WorkFlowVersionState.WorkFlowState.WFStateID == psotPreparation
                                                                && f.ApprovalStatusID == notApproved && f.IsActive == true);
                foreach (var psotPreparationFolderVersion in psotPreparationFolderVersions.ToList())
                {
                    if (latestFolderVersions.Any(a => a.FolderVersionId == psotPreparationFolderVersion.FolderVersionID))
                    {
                        if (DateTime.Now.Subtract(psotPreparationFolderVersion.AddedDate).TotalDays >= 7)
                        {
                            List<string> sendMailToList = GetEmailIdListForEmailNotification(0, psotPreparationFolderVersion.FolderVersionID, 0);
                            if (sendMailToList.Count > 0)
                            {
                                emailLoggerElements = GetWorkFlowStateFolderVersionDetails(psotPreparationFolderVersion.FolderVersionID, psotPreparationFolderVersion.WFStateID, 0, 1, string.Empty);

                                EmailTemplateInfo templateInfo = _emailService.GetEmailTemplateInfo<EmailTemplateInfo>(EmailTemplateTypes.PSoTPreparationNotification);
                                templateInfo.SetValue("#FolderName#", emailLoggerElements.FolderName);
                                templateInfo.SetValue("#WorkFlowState#", emailLoggerElements.WorkFlowState);
                                templateInfo.SetValue("#FolderVersionNumber#", emailLoggerElements.FolderVersionNumber);

                                EmailNotificationInfo emailNotificationInfo = new EmailNotificationInfo();
                                emailNotificationInfo.TemplateInfo = templateInfo;
                                emailNotificationInfo.ToBeSendDateTime = DateTime.Now;
                                emailNotificationInfo.ToAddresses = sendMailToList;

                                _emailService.SendEmail(emailNotificationInfo);
                            }
                        }
                    }
                }
            }
        }

        private void EmailNotificationOnWorkFlowStateUpdate(int? userID, string userName, int tenantID, int workFlowStateID, int folderVersionID, int approvalStatusId, List<string> sendMailToList, string sendGridUserName, string sendGridPassword)
        {
            emailLoggerElements = GetWorkFlowStateFolderVersionDetails(folderVersionID, workFlowStateID, userID, tenantID, userName, sendMailToList, approvalStatusId);
            var _emailService = new EmailNotificationService(this._unitOfWork, _coreunitOfWork);
            EmailTemplateInfo templateInfo = _emailService.GetEmailTemplateInfo<EmailTemplateInfo>(EmailTemplateTypes.WorkFlowStateChangeNotification);
            templateInfo.SetValue("#FolderName#", emailLoggerElements.FolderName);
            templateInfo.SetValue("#FolderEffectiveDate#", emailLoggerElements.FolderEffectiveDate.ToString("MM/dd/yyyy"));
            templateInfo.SetValue("#WorkFlowStatus#", emailLoggerElements.WorkFlowStatus);
            templateInfo.SetValue("#WorkFlowState#", emailLoggerElements.WorkFlowState);
            templateInfo.SetValue("#FolderVersionNumber#", emailLoggerElements.FolderVersionNumber);

            EmailNotificationInfo emailNotificationInfo = new EmailNotificationInfo();
            emailNotificationInfo.TemplateInfo = templateInfo;
            emailNotificationInfo.ToBeSendDateTime = DateTime.Now;
            emailNotificationInfo.ToAddresses = sendMailToList;

            _emailService.SendEmail(emailNotificationInfo);

            //_emailService.Execute();
        }

        private List<string> GetEmailIdListForEmailNotification(int workFlowStateID, int folderVersionID, int approvalStatusId)
        {
            List<string> sendMailToList = new List<string>();

            var applicableTeamMemberList = (from workFlowStateUserMap in this._unitOfWork.RepositoryAsync<WorkFlowStateUserMap>().Get()
                                            join appTeamUserMap in this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Get()
                                            on workFlowStateUserMap.UserID equals appTeamUserMap.UserID
                                            join user in this._unitOfWork.RepositoryAsync<User>().Get()
                                            on appTeamUserMap.UserID equals user.UserID
                                            where workFlowStateUserMap.FolderVersionID == folderVersionID && appTeamUserMap.IsDeleted == false
                                            select user.Email).ToList();
            var teamManagersEmailIdList = (from wFlowStateFvMap in this._unitOfWork.RepositoryAsync<WorkFlowStateFolderVersionMap>().Get()
                                           join appTeamUserMap in this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Get()
                                           on wFlowStateFvMap.ApplicableTeamID equals appTeamUserMap.ApplicableTeamID
                                           join users in this._unitOfWork.RepositoryAsync<User>().Get()
                                           on appTeamUserMap.UserID equals users.UserID
                                           where wFlowStateFvMap.FolderVersionID == folderVersionID && appTeamUserMap.IsDeleted == false && appTeamUserMap.IsTeamManager == true
                                           select users.Email).ToList();

            var primaryContactEmail = (from folder in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                       join folderVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(x => x.FolderVersionID == folderVersionID)
                                       on folder.FolderID equals folderVersion.FolderID
                                       join users in this._unitOfWork.RepositoryAsync<User>().Get()
                                       on folder.PrimaryContentID equals users.UserID
                                       select users.Email
                                     ).FirstOrDefault();

            //var sendMailToActionAproval = (from wf2 in this._unitOfWork.RepositoryAsync<WFVersionStatesApprovalType>().Get()
            //                               join wf3 in this._unitOfWork.RepositoryAsync<WFStatesApprovalTypeAction>().Get()
            //                               on wf2.WFVersionStatesApprovalTypeID equals wf3.WFVersionStatesApprovalTypeID
            //                               where wf2.WorkFlowVersionStateID == workFlowStateID && wf3.ActionID == 1 && wf2.WorkFlowStateApprovalTypeID == approvalStatusId
            //                               select wf3.ActionResponse
            //                        ).FirstOrDefault();

            if (applicableTeamMemberList != null && applicableTeamMemberList.Count() > 0)
            {
                sendMailToList = applicableTeamMemberList.ToList();
            }
            if (primaryContactEmail != null && primaryContactEmail.Count() > 0)
            {
                sendMailToList.Add(primaryContactEmail);
            }
            if (teamManagersEmailIdList != null && teamManagersEmailIdList.Count > 0)
            {
                foreach (var teamManagerMailId in teamManagersEmailIdList)
                {
                    sendMailToList.Add(teamManagerMailId);
                }
            }
            //if (sendMailToActionAproval != null)
            //{
            //    var ismoremails = sendMailToActionAproval.Contains(',');

            //    if (ismoremails)
            //    {
            //        var sendMailToList1 = sendMailToActionAproval.Trim().Split(',');
            //        if (sendMailToList1 != null && sendMailToList1.Count() > 0)
            //        {
            //            foreach (var mailid in sendMailToList1)
            //            {
            //                sendMailToList.Add(mailid);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        sendMailToList.Add(sendMailToActionAproval);
            //    }
            //}

            sendMailToList = sendMailToList.Distinct().ToList();
            return sendMailToList;
        }

        private void UpdateEmailLog(EmailLogger emailContentElement)
        {
            EmailLog emailLog = new EmailLog();
            emailLog.UserID = emailContentElement.UserId;
            emailLog.FolderID = emailContentElement.FolderID;
            emailLog.FolderVersionID = emailContentElement.FolderVersionID;
            emailLog.FolderVersionStateID = emailContentElement.FolderVersionStateID;
            emailLog.To = emailContentElement.To;
            emailLog.Bcc = emailContentElement.Bcc;
            emailLog.Cc = emailContentElement.Cc;
            emailLog.EmailContent = string.IsNullOrEmpty(emailContentElement.EmailContent) ? string.Empty : emailContentElement.EmailContent;
            emailLog.AccountID = emailContentElement.AccountID;
            emailLog.FolderEffectiveDate = emailContentElement.FolderEffectiveDate;
            emailLog.ApprovedWorkFlowStateID = emailContentElement.ApprovedWorkFlowStateID;
            emailLog.CurrentWorkFlowStateID = emailContentElement.CurrentWorkFlowStateID;
            emailLog.Comments = emailContentElement.Comment;
            emailLog.TenantID = emailContentElement.TenantID;
            emailLog.AddedBy = emailContentElement.AddedBy;
            emailLog.EmailSentDateTime = DateTime.Now;

            this._unitOfWork.RepositoryAsync<EmailLog>().Insert(emailLog);
            this._unitOfWork.Save();
        }

        private EmailLogger GetWorkFlowStateFolderVersionDetails(int folderVersionID, int workFlowStateID, int? userID, int tenantID, string userName)
        {
            var folderid = this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(c => c.FolderVersionID == folderVersionID).Select(c => c.FolderID).FirstOrDefault();
            var isportFolioFolder = this._unitOfWork.RepositoryAsync<Folder>().Get().Where(c => c.FolderID == folderid).Select(c => c.IsPortfolio).FirstOrDefault();
            var masterStateName = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>()
                 .Query().Filter(c => c.WorkFlowVersionStateID == workFlowStateID).Include(c => c.WorkFlowState)
                 .Get().Select(c => c.WorkFlowState.WFStateName).FirstOrDefault();

            EmailLogger emailLoggerElements = null;
            if (Convert.ToBoolean(isportFolioFolder))
            {
                emailLoggerElements = (from folderVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                       join folder in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                       on folderVersion.FolderID equals folder.FolderID
                                       join folderVersionWFState in this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Get()
                                       on folderVersion.FolderVersionID equals folderVersionWFState.FolderVersionID
                                       join approveStatusType in this._unitOfWork.RepositoryAsync<WorkFlowStateApprovalTypeMaster>().Get()
                                       on folderVersionWFState.ApprovalStatusID equals approveStatusType.WorkFlowStateApprovalTypeID
                                       join workState in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get()
                                       on folderVersionWFState.WFStateID equals workState.WorkFlowVersionStateID
                                       where folderVersion.FolderVersionID == folderVersionID && folderVersionWFState.WFStateID == workFlowStateID
                                       select new EmailLogger
                                       {
                                           AccountName = null,
                                           AccountID = null,
                                           FolderVersionID = folderVersion.FolderVersionID,
                                           FolderVersionStateID = folderVersion.FolderVersionStateID,
                                           FolderEffectiveDate = folderVersion.EffectiveDate,
                                           FolderName = folder.Name,
                                           FolderID = folder.FolderID,
                                           FolderVersionNumber = folderVersion.FolderVersionNumber,
                                           WorkFlowState = workState.WorkFlowState.WFStateName,
                                           WorkFlowStatus = approveStatusType.WorkFlowStateApprovalTypeName,
                                           CurrentWorkFlowStateID = workFlowStateID,
                                       }).FirstOrDefault();
            }
            else
            {
                emailLoggerElements = (from folderVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                       join folder in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                       on folderVersion.FolderID equals folder.FolderID
                                       join accountFolderMap in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get()
                                       on folderVersion.FolderID equals accountFolderMap.FolderID
                                       join account in this._unitOfWork.RepositoryAsync<Account>().Get()
                                       on accountFolderMap.AccountID equals account.AccountID
                                       join folderVersionWFState in this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Get()
                                       on folderVersion.FolderVersionID equals folderVersionWFState.FolderVersionID
                                       join approveStatusType in this._unitOfWork.RepositoryAsync<WorkFlowStateApprovalTypeMaster>().Get()
                                       on folderVersionWFState.ApprovalStatusID equals approveStatusType.WorkFlowStateApprovalTypeID
                                       join workState in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get()
                                       on folderVersionWFState.WFStateID equals workState.WorkFlowVersionStateID
                                       where folderVersion.FolderVersionID == folderVersionID && folderVersionWFState.WFStateID == workFlowStateID
                                       select new EmailLogger
                                       {
                                           AccountName = account.AccountName,
                                           AccountID = account.AccountID,
                                           FolderVersionID = folderVersion.FolderVersionID,
                                           FolderVersionStateID = folderVersion.FolderVersionStateID,
                                           FolderEffectiveDate = folderVersion.EffectiveDate,
                                           FolderName = folder.Name,
                                           FolderID = folder.FolderID,
                                           FolderVersionNumber = folderVersion.FolderVersionNumber,
                                           WorkFlowState = workState.WorkFlowState.WFStateName,
                                           WorkFlowStatus = approveStatusType.WorkFlowStateApprovalTypeName,
                                           CurrentWorkFlowStateID = workFlowStateID,
                                       }).FirstOrDefault();
            }

            emailLoggerElements.UserId = (int)userID;
            emailLoggerElements.TenantID = tenantID;
            emailLoggerElements.AddedBy = userName;

            return emailLoggerElements;
        }


        private EmailLogger GetWorkFlowStateFolderVersionDetails(int folderVersionID, int workFlowStateID, int? userID, int tenantID, string userName, List<string> sendMailToList, int approvalStatusId)
        {
            var folderid = this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(c => c.FolderVersionID == folderVersionID).Select(c => c.FolderID).FirstOrDefault();
            var isportFolioFolder = this._unitOfWork.RepositoryAsync<Folder>().Get().Where(c => c.FolderID == folderid).Select(c => c.IsPortfolio).FirstOrDefault();
            var masterStateName = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>()
                 .Query().Filter(c => c.WorkFlowVersionStateID == workFlowStateID).Include(c => c.WorkFlowState)
                 .Get().Select(c => c.WorkFlowState.WFStateName).FirstOrDefault();

            EmailLogger emailLoggerElements = null;
            if (Convert.ToBoolean(isportFolioFolder))
            {
                emailLoggerElements = (from folderVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                       join folder in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                       on folderVersion.FolderID equals folder.FolderID
                                       join folderVersionWFState in this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Get()
                                       on folderVersion.FolderVersionID equals folderVersionWFState.FolderVersionID
                                       join approveStatusType in this._unitOfWork.RepositoryAsync<WorkFlowStateApprovalTypeMaster>().Get()
                                       on folderVersionWFState.ApprovalStatusID equals approveStatusType.WorkFlowStateApprovalTypeID
                                       join workState in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get()
                                       on folderVersionWFState.WFStateID equals workState.WorkFlowVersionStateID
                                       where folderVersion.FolderVersionID == folderVersionID && folderVersionWFState.WFStateID == workFlowStateID
                                       select new EmailLogger
                                       {
                                           AccountName = null,
                                           AccountID = null,
                                           FolderVersionID = folderVersion.FolderVersionID,
                                           FolderVersionStateID = folderVersion.FolderVersionStateID,
                                           FolderEffectiveDate = folderVersion.EffectiveDate,
                                           FolderName = folder.Name,
                                           FolderID = folder.FolderID,
                                           FolderVersionNumber = folderVersion.FolderVersionNumber,
                                           WorkFlowState = workState.WorkFlowState.WFStateName,
                                           WorkFlowStatus = approveStatusType.WorkFlowStateApprovalTypeName,
                                           CurrentWorkFlowStateID = workFlowStateID,
                                       }).FirstOrDefault();
            }
            else
            {
                emailLoggerElements = (from folderVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                       join folder in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                       on folderVersion.FolderID equals folder.FolderID
                                       join accountFolderMap in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get()
                                       on folderVersion.FolderID equals accountFolderMap.FolderID
                                       join account in this._unitOfWork.RepositoryAsync<Account>().Get()
                                       on accountFolderMap.AccountID equals account.AccountID
                                       join folderVersionWFState in this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Get()
                                       on folderVersion.FolderVersionID equals folderVersionWFState.FolderVersionID
                                       join approveStatusType in this._unitOfWork.RepositoryAsync<WorkFlowStateApprovalTypeMaster>().Get()
                                       on folderVersionWFState.ApprovalStatusID equals approveStatusType.WorkFlowStateApprovalTypeID
                                       join workState in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get()
                                       on folderVersionWFState.WFStateID equals workState.WorkFlowVersionStateID
                                       where folderVersion.FolderVersionID == folderVersionID && folderVersionWFState.WFStateID == workFlowStateID
                                       select new EmailLogger
                                       {
                                           AccountName = account.AccountName,
                                           AccountID = account.AccountID,
                                           FolderVersionID = folderVersion.FolderVersionID,
                                           FolderVersionStateID = folderVersion.FolderVersionStateID,
                                           FolderEffectiveDate = folderVersion.EffectiveDate,
                                           FolderName = folder.Name,
                                           FolderID = folder.FolderID,
                                           FolderVersionNumber = folderVersion.FolderVersionNumber,
                                           WorkFlowState = workState.WorkFlowState.WFStateName,
                                           WorkFlowStatus = approveStatusType.WorkFlowStateApprovalTypeName,
                                           CurrentWorkFlowStateID = workFlowStateID,
                                       }).FirstOrDefault();
            }

            emailLoggerElements.UserId = (int)userID;
            emailLoggerElements.TenantID = tenantID;
            emailLoggerElements.AddedBy = userName;

            //int notApprovedState = Convert.ToInt32(ApprovalStatus.NOTAPPROVED);

            //emailLoggerElements.ApprovedWorkFlowStateID = masterStateName.Trim() == "Product Analysis" ? workFlowStateID : this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>()
            //                                             .Query().Filter(x => x.FolderVersionID == folderVersionID && x.ApprovalStatusID != notApprovedState)
            //                                             .Get().Select(sel => sel.WFStateID).Max();
            //emailLoggerElements.ApprovedWorkFlowStateName = approvalStatusId == (int)ApprovalStatus.APPROVED ? this._unitOfWork.RepositoryAsync<WorkFlowVersionState>()
            //                                                       .Query().Filter(x => x.WorkFlowVersionStateID == emailLoggerElements.ApprovedWorkFlowStateID)
            //                                                       .Get().Select(sel => sel.WorkFlowState.WFStateName).FirstOrDefault() : emailLoggerElements.WorkFlowState;
            emailLoggerElements.To = (sendMailToList != null && sendMailToList.Count() > 0) ? string.Join(",", sendMailToList) : string.Empty;
            emailLoggerElements.Cc = string.Empty;
            emailLoggerElements.Bcc = string.Empty;

            return emailLoggerElements;
        }
        private ServiceResult ValidateEmailSettings(EmailSetting emailSettings)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();

            try
            {
                result.Result = ServiceResultStatus.Success;
                if (emailSettings.SendGridFrom == null || string.IsNullOrEmpty(emailSettings.SendGridPassword))
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { EmailNotificationConstants.EmailNotificationErrorMessage } });
                }
                if (emailSettings.To == null || emailSettings.To.Count() <= 0)
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { EmailNotificationConstants.EmptyEmailSentToListMessage } });
                }

                if (items.Count() > 0)
                {
                    result.Result = ServiceResultStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                items.Add(new ServiceResultItem() { Messages = new string[] { EmailNotificationConstants.UnknownExceptionErrorMessage } });
                result.Result = ServiceResultStatus.Failure;
            }
            result.Items = items;
            return result;
        }
        #endregion WorkFlow State Email Notification

        /// <summary>
        /// get workflow state name by WFStateID
        /// </summary>
        /// <param name="workflowStateId"></param>
        /// <returns></returns>
        public string GetWorkflowStateName(int workflowStateId)
        {
            string workflowStateName = string.Empty;
            workflowStateName = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Query().Include(c => c.WorkFlowState).Get().Where(c => c.WorkFlowVersionStateID == workflowStateId).Select(p => p.WorkFlowState.WFStateName).FirstOrDefault();
            return workflowStateName;
        }

        #endregion Private Methods

        #region EmailNotification To Member

        public void EmailNotificationToAssignedMembers(int? userID, string userName, int tenantID, int folderVersionId, int currentWorkFlowStateId, int approvedWorkFlowStateId, IList<int> userList, string sendGridUserName, string sendGridPassword)
        {
            sendMailToList = this._unitOfWork.RepositoryAsync<User>().Get().Where(c => userList.Contains(c.UserID)).Select(sel => sel.Email).ToList();
            emailLoggerElements = GetWorkFlowStateFolderVersionDetails(folderVersionId, currentWorkFlowStateId, userID, tenantID, userName, sendMailToList, (int)ApprovalStatus.APPROVED);

            var _emailService = new EmailNotificationService(this._unitOfWork, _coreunitOfWork);
            EmailTemplateInfo templateInfo = _emailService.GetEmailTemplateInfo<EmailTemplateInfo>(EmailTemplateTypes.AssignFolderMemberNotification);
            templateInfo.SetValue("#FolderName#", emailLoggerElements.FolderName);
            templateInfo.SetValue("#FolderEffectiveDate#", emailLoggerElements.FolderEffectiveDate.ToString("MM/dd/yyyy"));
            templateInfo.SetValue("#WorkFlowState#", emailLoggerElements.WorkFlowState);
            templateInfo.SetValue("#FolderVersionNumber#", emailLoggerElements.FolderVersionNumber);

            EmailNotificationInfo emailNotificationInfo = new EmailNotificationInfo();
            emailNotificationInfo.TemplateInfo = templateInfo;
            emailNotificationInfo.ToBeSendDateTime = DateTime.Now;
            emailNotificationInfo.ToAddresses = sendMailToList;

            _emailService.SendEmail(emailNotificationInfo);
        }

        #endregion EmailNotification To Member

        #region new implementation
        public List<WorkFlowVersionStatesViewModel> GetWorkFlowStateList(int tenantId, int folderVersionId)
        {
            IEnumerable<WorkFlowVersionStatesViewModel> workflowList = null;

            try
            {
                var folderVersion = this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                .Query()
                                                .Filter(c => c.FolderVersionID == folderVersionId && c.IsActive == true)
                                                .Get()
                                                .FirstOrDefault();

                bool accountType = this._unitOfWork.RepositoryAsync<Folder>()
                                                .Query()
                                                .Filter(c => c.FolderID == folderVersion.FolderID && c.TenantID == tenantId)
                                                .Get()
                                                .Select(c => c.IsPortfolio)
                                                .FirstOrDefault();

                //workflowList = _workFlowUsageService.GetAllStatesForFolderVersion(folderVersion.CategoryID.Value, accountType);
                workflowList = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetAllStatesForFolderVersion(this._unitOfWork, folderVersion.CategoryID.Value, accountType)
                    .Select(view => new WorkFlowVersionStatesViewModel()
                    {
                        Sequence = view.Sequence,
                        WFStateGroupID = view.WFStateGroupID,
                        WFStateID = view.WFStateID,
                        WFStateName = view.WorkFlowState.WFStateName,
                        WorkFlowVersionID = view.WorkFlowVersionID,
                        WorkFlowVersionStateID = view.WorkFlowVersionStateID

                    });

                if (workflowList.Count() == 0)
                    workflowList = new List<WorkFlowVersionStatesViewModel>();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return workflowList.ToList();
        }

        public List<WorkFlowVersionStatesAccessViewModel> GetWorkFlowStateUserRoles(int tenantId, int folderVersionId)
        {
            List<WorkFlowVersionStatesAccessViewModel> stateUserRoleList = null;

            try
            {
                var folderVersion = this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                   .Query()
                                                   .Filter(c => c.FolderVersionID == folderVersionId && c.IsActive == true)
                                                   .Get()
                                                   .FirstOrDefault();
                if (folderVersion.WFStateID != null)
                {
                    stateUserRoleList = _workFlowStateAccessService.GetWorkFlowVersionStatesAccessList((int)folderVersion.WFStateID);
                }
                if (stateUserRoleList == null || stateUserRoleList.Count() == 0)
                    stateUserRoleList = new List<WorkFlowVersionStatesAccessViewModel>();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return stateUserRoleList.ToList();
        }

        public IEnumerable<KeyValue> GetApprovalStatusTypeForFolderVersion(int tenantId, int folderVersionId, int wfVersionStateId)
        {
            IList<KeyValue> approvalStatusTypeList = null;
            var notapprovalID = Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.NOTAPPROVED);
            try
            {
                int currentStateId = 0;
                if (wfVersionStateId == 0)
                {
                    currentStateId = (from fldrWF in this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Get().Where(c => c.FolderVersion.FolderVersionID == folderVersionId && c.ApprovalStatusID == notapprovalID && c.IsActive == true).OrderByDescending(c => c.FVWFStateID)
                                      select fldrWF.WFStateID
                                           ).FirstOrDefault();
                }
                else currentStateId = wfVersionStateId;

                approvalStatusTypeList = (from stateApp in this._unitOfWork.RepositoryAsync<WFVersionStatesApprovalType>()
                                                     .Query()
                                                     .Include(f => f.WorkFlowStateApprovalTypeMaster)
                                                     .Filter(c => c.WorkFlowVersionStateID == currentStateId)
                                                                  .Get()
                                          select new KeyValue
                                          {
                                              Key = stateApp.WorkFlowStateApprovalTypeMaster.WorkFlowStateApprovalTypeID,
                                              Value = stateApp.WorkFlowStateApprovalTypeMaster.WorkFlowStateApprovalTypeName
                                          }).ToList();

                if (approvalStatusTypeList.Count() == 0)
                    approvalStatusTypeList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return approvalStatusTypeList;
        }

        public IEnumerable<KeyValue> GetCurrentWorkFlowState(int tenantId, int folderVersionId)
        {
            IList<KeyValue> workFlowList = null;
            var notapprovalID = Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.NOTAPPROVED);
            try
            {
                var currentStateId = this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Filter(c => c.FolderVersionID == folderVersionId).Get().FirstOrDefault().WFStateID;
                workFlowList = (from wfState1 in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get()
                                join wfState2 in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Query().Include(w => w.WorkFlowState).Get()
                                on new { wfState1.WorkFlowVersionID, wfState1.Sequence } equals new { wfState2.WorkFlowVersionID, wfState2.Sequence }
                                where wfState1.WorkFlowVersionStateID == currentStateId
                                select new KeyValue
                                {
                                    Key = wfState2.WorkFlowVersionStateID,
                                    Value = wfState2.WorkFlowState.WFStateName
                                }).ToList();

                if (workFlowList.Count() == 0)
                    workFlowList = new List<KeyValue>();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return workFlowList;
        }

        public List<KeyValue> GetCurrentWorkFlowStateForStatusUpdate(int tenantId, int folderVersionId)
        {
            IList<KeyValue> workFlowList = null;
            List<KeyValue> notApprovedWorkFlowList = new List<KeyValue>();

            var notapprovalID = Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.NOTAPPROVED);
            try
            {
                var currentStateId = this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Filter(c => c.FolderVersionID == folderVersionId).Get().FirstOrDefault().WFStateID;
                workFlowList = (from wfState1 in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get()
                                join wfState2 in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Query().Include(w => w.WorkFlowState).Get()
                                on new { wfState1.WorkFlowVersionID, wfState1.Sequence } equals new { wfState2.WorkFlowVersionID, wfState2.Sequence }
                                where wfState1.WorkFlowVersionStateID == currentStateId
                                select new KeyValue
                                {
                                    Key = wfState2.WorkFlowVersionStateID,
                                    Value = wfState2.WorkFlowState.WFStateName
                                }).ToList();

                if (workFlowList.Count() > 0)
                {
                    foreach (KeyValue wf in workFlowList)
                    {
                        var notApprovedWFStates = this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Get().Where(w => w.FolderVersionID == folderVersionId
           && w.ApprovalStatusID == notapprovalID && wf.Key == w.WFStateID).ToList();

                        if (notApprovedWFStates != null)
                        {
                            var notApprovedWFState = notApprovedWFStates.FirstOrDefault();
                            if (notApprovedWFState != null)
                            {
                                notApprovedWorkFlowList.Add(new KeyValue() { Key = wf.Key, Value = wf.Value });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return notApprovedWorkFlowList;
        }

        public WorkFlowStateMasterViewModel GetWorkFlowState(int tenantId, int folderVersionId)
        {
            WorkFlowStateMasterViewModel wfVersionState = new WorkFlowStateMasterViewModel();
            try
            {
                wfVersionState = (from fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                  join wfvs in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get()
                                      on fv.WFStateID equals wfvs.WorkFlowVersionStateID
                                  join ws in this._unitOfWork.RepositoryAsync<WorkFlowStateMaster>().Get()
                                      on wfvs.WFStateID equals ws.WFStateID
                                  where fv.FolderVersionID == folderVersionId
                                  select new WorkFlowStateMasterViewModel
                                  {
                                      WFStateID = ws.WFStateID,
                                      WFStateName = ws.WFStateName,
                                  }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return wfVersionState;
        }
        #endregion new implementation
    }
}

internal class EmailNotificationConstants
{
    public const string SubjectLineFormatAssignMembers = "{0};{1};{2};";
    public const string SubjectLineFormatStatusUpdate = "{0};{1};{2};{3}";
    public const string EmailNotificationErrorMessage = "EmailSettings contain an insufficient information, hence unable delivered an email.Please contact support team.";
    public const string WorkFlowStateUpdateEmailHtml = " <p style='font-family:Calibri;font-size:15px;'>Hi,<br />The WorkFlowState : {0} has been moved to {1} Status.The detailed information is as follows:</p>" +
                                                            "<table style='font-family:Calibri;font-size:15px;padding-left:2%'>" +
                                                                "<tbody>" +
                                                                    "<tr><td>Account Name</td><td>  : {2}</td> </tr> <tr> <td>Folder Name</td><td>  : {3}</td></tr>" +
                                                                    "<tr><td>Folder Effective Date</td><td>  : {4}</td></tr><tr><td>Folder Version Number</td><td>  : {5}</td></tr>" +
                                                                    "<tr><td>WorkFlow State</td><td>  : {6}</td></tr>" +
                                                                "</tbody>" +
                                                            "</table>" +
                                                        "<p style='font-family:Calibri;font-size:15px;'>Regards,<br />HSB Support Team.</p><br />" +
                                                        "<p style='font-style:italic;font-family:Calibri;font-size:15px;'>This is Autogenerated Mail. Please do not reply.</p> ";

    public const string UnknownExceptionErrorMessage = "Unbale to process email notification request.Please contact support team.";
    public const string AssignedMemberEmailContentHtml = "<p  style='font-family:Calibri;font-size:15px;'>Hi, <br />" +
                                                         "FolderVersion : {0} for WorkFlowState : {1} has been assigned to you.<br />" +
                                                         "Please complete the assignments mentioned in WorkQueue. <br /><br />" +
                                                         "Regards,<br />HSB Support Team.</p>" +
                                                         "<p style='font-style:italic;font-family:Calibri;font-size:15px;'>This is Autogenerated Mail. Please do not reply.</p>";

    public const string EmptyEmailSentToListMessage = "Email Sent To list is Empty.Hence unable delivered an email.Please contact support team.";
}

internal class SettingConstants
{
    public const string UnknownExceptionErrorMessage = "Unable to save settings request.Please contact support team.";
}

