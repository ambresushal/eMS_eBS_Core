using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.DashBoard;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;
using tmg.equinox.domain.entities;
using tmg.equinox.repository.extensions;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.applicationservices.viewmodels;
using System.Diagnostics.Contracts;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using System.Data;
using System.Text;
using tmg.equinox.emailnotification.model;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.applicationservices.viewmodels.EmailNotitication;
using tmg.equinox.emailnotification;
using tmg.equinox.emailnotification.Model;
using System.Configuration;
using System.Net.Mail;
using System.Data.Entity.Core.Objects;
using System.Reflection;
using Newtonsoft.Json;
using tmg.equinox.applicationservices.viewmodels.DPF;
using System.Linq.Expressions;
using System.Threading;
using System.Runtime.InteropServices.ComTypes;

namespace tmg.equinox.applicationservices
{
    public partial class DashboardService : IDashboardService
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private ILoggingService _loggingService { get; set; }
        private IDomainModelService _domainModelService { get; set; }
        private const string InvalidTenantId = "Invalid tenantId";

        private EmailSetting emailSettings = new EmailSetting();
        private EmailLogger emailLoggerElements;
        private SendGridEmailNotification sendGridInvocation;
        private SmtpEmailNotification smtpInvocation;
        private EmailResponseData emailAcknowledgement;
        #endregion Private Members

        #region Constructor
        public DashboardService(IUnitOfWorkAsync unitOfWork, ILoggingService loggingService, IDomainModelService domainModelService)
        {
            this._unitOfWork = unitOfWork;
            this._loggingService = loggingService;
            this._domainModelService = domainModelService;
        }
        #endregion

        public IQueryable<FormUpdatesViewModel> GetFormUpdatesList(int tenantId)
        {
            if (tenantId == 0) throw new Exception(InvalidTenantId);
            IQueryable<FormUpdatesViewModel> formUpdatesList = null;
            try
            {
                var formUpdatesListQuery = (from form in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                            join formdesignversion in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                                            on form.FormID equals formdesignversion.FormDesignID
                                            join latestVersion in
                                                (from lv in _unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                                                 group lv by new { lv.FormDesignID, lv.StatusID }
                                                     into g
                                                 select new
                                                 {
                                                     groupFormDesignId = g.Key.FormDesignID,
                                                     groupStatusId = g.Key.StatusID,
                                                     groupFormDesignVersionID = g.Max(x => x.FormDesignVersionID)
                                                 }).AsQueryable()
                                                 on new
                                                 {
                                                     formIdInner = form.FormID,
                                                     formdesignversionIdInner = formdesignversion.FormDesignVersionID,
                                                     statusIdInner = formdesignversion.StatusID
                                                 }

                                                 equals new
                                                 {
                                                     formIdInner = latestVersion.groupFormDesignId.Value,
                                                     formdesignversionIdInner = latestVersion.groupFormDesignVersionID,
                                                     statusIdInner = latestVersion.groupStatusId
                                                 }
                                            where form.TenantID == tenantId && formdesignversion.StatusID == (int)domain.entities.Status.Finalized
                                            orderby formdesignversion.FormDesignVersionID descending
                                            select new
                                            {
                                                latestForm = form,
                                                latestFormDesignVersion = formdesignversion
                                            }).AsQueryable();

                var latestFormUpdatesList = from res in formUpdatesListQuery.ToList()
                                            select new FormUpdatesViewModel
                                            {
                                                FormName = res.latestForm.FormName,
                                                Comments = res.latestFormDesignVersion.Comments,
                                                VersionNumber = res.latestFormDesignVersion.VersionNumber,
                                                EffectiveDate = res.latestFormDesignVersion.EffectiveDate,
                                                ReleaseDate = res.latestFormDesignVersion.UpdatedDate.HasValue
                                                          ? res.latestFormDesignVersion.UpdatedDate.Value
                                                          : res.latestFormDesignVersion.AddedDate,
                                            };
                formUpdatesList = latestFormUpdatesList.AsQueryable();
            }

            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return formUpdatesList;
        }

        public IQueryable<WorkQueueViewModel> GetWorkQueueList(int tenantId, int? userId)
        {

            if (userId == null) throw new ArgumentNullException();
            if (tenantId == 0) throw new Exception(InvalidTenantId);

            IQueryable<WorkQueueViewModel> workQueueList = null;
            try
            {
                workQueueList = (from folder in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                 join folderVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                     on folder.FolderID equals folderVersion.FolderID
                                 join folderVersionWorkFlowState in this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Get()
                                     on folderVersion.FolderVersionID equals folderVersionWorkFlowState.FolderVersionID
                                 select new WorkQueueViewModel
                                 {
                                     TenantID = folder.TenantID,
                                     Folder = folder.Name,
                                     EffectiveDate = folderVersion.EffectiveDate,
                                     Status = folderVersionWorkFlowState.WorkFlowVersionState.WorkFlowState.WFStateName,
                                     StatusDate =
                                         folderVersionWorkFlowState.UpdatedDate.HasValue
                                             ? folderVersionWorkFlowState.UpdatedDate.Value
                                             : folderVersionWorkFlowState.AddedDate,
                                     FolderId = folder.FolderID,
                                     FolderVersionId = folderVersion.FolderVersionID
                                 }).AsQueryable();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return workQueueList;
        }

        /// <summary>
        /// Only the work status which are queued are to be shown to user.
        /// WorkFlow Status Release and Approved won't be displayed.
        /// Status Date (the latest date that the status was changed). 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IQueryable<WorkQueueViewModel> GetWorkQueueListNotReleasedAndApproved(int tenantId, int? userId)
        {
            if (userId == null) throw new ArgumentNullException();
            if (tenantId == 0) throw new Exception(InvalidTenantId);
            int inProgressStateId = (int)tmg.equinox.domain.entities.Enums.FolderVersionState.INPROGRESS;

            IList<WorkQueueViewModel> workQueueListNotReleasedAndApproved = new List<WorkQueueViewModel>();
            try
            {
                //var teamAsoc = from teamAsoc in this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Get().Where(t => t.ApplicableTeamUserMapID == )
                var folderVersionIds = (from wfStateUserMap in this._unitOfWork.RepositoryAsync<WorkFlowStateUserMap>().Get().Where(x => x.UserID == userId && x.IsActive == true)
                                        join folderVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(x => x.FolderVersionStateID == inProgressStateId && x.Folder.Name != GlobalVariables.MASTERLIST)
                                        on wfStateUserMap.FolderVersionID equals folderVersion.FolderVersionID
                                        select wfStateUserMap.FolderVersionID
                                      ).Distinct().ToList();

                foreach (var folderVersionId in folderVersionIds)
                {
                    WorkQueueViewModel workQueueViewModel = new WorkQueueViewModel();

                    workQueueViewModel = (from folderVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(x => x.FolderVersionID == folderVersionId)
                                          join folder in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                          on folderVersion.FolderID equals folder.FolderID
                                          join wf in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get()
                                          on folderVersion.WFStateID equals wf.WorkFlowVersionStateID
                                          where (folderVersion.FolderVersionID == folderVersionId)
                                          join accFolderMap in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get().Where(c => c.Account.IsActive == true)
                                          on folderVersion.FolderID equals accFolderMap.FolderID
                                          into tmp
                                          from accFolderMap in tmp.DefaultIfEmpty()
                                          select new WorkQueueViewModel
                                          {
                                              FolderVersionId = folderVersion.FolderVersionID,
                                              EffectiveDate = folderVersion.EffectiveDate,
                                              Folder = folder.Name,
                                              FolderVersionNumber = folderVersion.FolderVersionNumber,
                                              Status = wf.WorkFlowState.WFStateName,
                                              StatusDate = folderVersion.UpdatedDate.HasValue ? folderVersion.UpdatedDate.Value : folderVersion.AddedDate,
                                              FolderId = folder.FolderID,
                                              TenantID = folderVersion.TenantID,
                                              Account = accFolderMap == null ? "NA" : accFolderMap.Account.AccountName,
                                              IsActive = accFolderMap == null ? false : accFolderMap.Account.IsActive
                                          }).FirstOrDefault();

                    if (workQueueViewModel != null)
                    {
                        workQueueListNotReleasedAndApproved.Add(workQueueViewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return workQueueListNotReleasedAndApproved.AsQueryable();

        }

        public GridPagingResponse<ViewModelForProofingTasks> GetWorkQueueListForProofingTasks(int tenantId, int? userId, string UserName, string viewMode, int taskFolderVersionId, bool isDownload, GridPagingRequest gridPagingRequest)
        {
            if (userId == null) throw new ArgumentNullException();
            if (tenantId == 0) throw new Exception(InvalidTenantId);
            int inProgressStateId = (int)tmg.equinox.domain.entities.Enums.FolderVersionState.INPROGRESS;
            SearchCriteria criteria = new SearchCriteria();
            int count = 0;
            criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

            IList<ViewModelForProofingTasks> viewModelForProofingTasks = new List<ViewModelForProofingTasks>();
            try
            {

                List<int> folderVersionIds = new List<int>();
                if (taskFolderVersionId == 0)
                {
                    folderVersionIds = (from folderVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(x => x.FolderVersionStateID == inProgressStateId && x.Folder.Name != GlobalVariables.MASTERLIST)
                                        select folderVersion.FolderVersionID
                                      ).Distinct().ToList();

                }
                else
                {
                    folderVersionIds.Add(taskFolderVersionId);
                }
                string[] statuses = new string[] { "Completed", "Assigned", "InProgress", "Late", "Completed - Fail", "Completed - Pass" };
                if (viewMode != "Completed")
                    statuses = statuses.Where(x => x != statuses[0] && x != statuses[4] && x != statuses[5]).ToArray();
                else
                    statuses = statuses.Where(x => x == statuses[0] || x == statuses[4] || x == statuses[5]).ToArray();

                viewModelForProofingTasks = (from planUserTask in this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Get().Where(x => x.AssignedUserName.Contains(UserName) && statuses.Contains(x.Status))
                                             join task in _unitOfWork.RepositoryAsync<TaskList>().Get() on planUserTask.TaskID equals task.TaskID
                                             //join forminstance in _unitOfWork.RepositoryAsync<FormInstance>().Get() on planUserTask.FormInstanceId equals forminstance.FormInstanceID
                                             join folderVersion in _unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(y => folderVersionIds.Contains(y.FolderVersionID)) on planUserTask.FolderVersionID equals folderVersion.FolderVersionID
                                             join folder in _unitOfWork.RepositoryAsync<Folder>().Get() on folderVersion.FolderID equals folder.FolderID
                                             join wf in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get()
                                             on folderVersion.WFStateID equals wf.WorkFlowVersionStateID
                                             join accFolderMap in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get().Where(c => c.Account.IsActive == true)
                                             on folderVersion.FolderID equals accFolderMap.FolderID into tmp
                                             from accFolderMap in tmp.DefaultIfEmpty()
                                             orderby planUserTask.ID descending
                                             select new ViewModelForProofingTasks
                                             {
                                                 MappingRowID = planUserTask.ID,
                                                 Account = accFolderMap == null ? "NA" : accFolderMap.Account.AccountName + "|" + accFolderMap.Account.AccountID,
                                                 TenantID = tenantId,
                                                 FolderId = folderVersion.FolderID,
                                                 FolderVersionId = folderVersion.FolderVersionID,
                                                 TaskId = planUserTask.TaskID,
                                                 Folder = folder.Name,
                                                 FolderVersion = folderVersion.FolderVersionNumber,
                                                 EffectiveDate = folderVersion.EffectiveDate,
                                                 Workflow = wf.WorkFlowState.WFStateName,
                                                 Task = task.TaskDescription,
                                                 Assignment = planUserTask.AssignedUserName,
                                                 Status = planUserTask.Status,
                                                 StartDate = planUserTask.StartDate,
                                                 DueDate = planUserTask.DueDate,
                                                 Completed = planUserTask.CompletedDate,
                                                 Order = planUserTask.Order,
                                                 PlanTaskUserMappingDetails = planUserTask.PlanTaskUserMappingDetails,
                                                 EstimatedTime = planUserTask.EstimatedTime,
                                                 ActualTime = planUserTask.ActualTime,
                                                 FolderVersionWFStateID = wf.WorkFlowState.WFStateID,
                                                 TaskWFStateID = planUserTask.WFStateID
                                             }).OrderByDescending(O => O.MappingRowID).ToList();

                viewModelForProofingTasks = GetPlanTaskMappingDetails(viewModelForProofingTasks, isDownload);

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            if (gridPagingRequest.filters != null || gridPagingRequest.sidx != null)
                viewModelForProofingTasks = (viewModelForProofingTasks.AsQueryable()).ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                       .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count).ToList();
            return new GridPagingResponse<ViewModelForProofingTasks>(gridPagingRequest.page, count, gridPagingRequest.rows, viewModelForProofingTasks);
            //return viewModelForProofingTasks.AsQueryable();

        }

        public GridPagingResponse<ViewModelForProofingTasks> GetWatchListForProofingTasks(int tenantId, int? userId, string UserName, bool isViewInterested, string viewMode, GridPagingRequest gridPagingRequest, int taskFolderVersionId, bool isDownload)
        {
            if (userId == null) throw new ArgumentNullException();
            if (tenantId == 0) throw new Exception(InvalidTenantId);
            int inProgressStateId = (int)tmg.equinox.domain.entities.Enums.FolderVersionState.INPROGRESS;
            SearchCriteria criteria = new SearchCriteria();
            int count = 0;
            criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

            IList<ViewModelForProofingTasks> viewModelForProofingTasks = new List<ViewModelForProofingTasks>();
            try
            {
                List<int> folderVersionIds = new List<int>();
                if (taskFolderVersionId == 0)
                {
                    folderVersionIds = (from folderVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(x => x.FolderVersionStateID == inProgressStateId && x.Folder.Name != GlobalVariables.MASTERLIST)
                                        select folderVersion.FolderVersionID
                                      ).Distinct().ToList();

                }
                else
                {
                    folderVersionIds.Add(taskFolderVersionId);
                }

                string[] statuses = new string[] { "Completed", "Assigned", "InProgress", "Late", "Completed - Fail", "Completed - Pass" };
                if (viewMode != "Completed")
                    statuses = statuses.Where(x => x != statuses[0] && x != statuses[4] && x != statuses[5]).ToArray();
                else
                    statuses = statuses.Where(x => x == statuses[0] || x == statuses[4] || x == statuses[5]).ToArray();

                if (CheckIsManager(userId))
                {
                    viewModelForProofingTasks = GetProofingTasksListForAllManagers(tenantId, isViewInterested, folderVersionIds, statuses);
                    viewModelForProofingTasks = GetPlanTaskMappingDetails(viewModelForProofingTasks, isDownload);
                }

                if (gridPagingRequest.filters != null || gridPagingRequest.sidx != null)
                    viewModelForProofingTasks = (viewModelForProofingTasks.AsQueryable()).ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                           .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count).ToList();

                return new GridPagingResponse<ViewModelForProofingTasks>(gridPagingRequest.page, count, gridPagingRequest.rows, viewModelForProofingTasks);

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return new GridPagingResponse<ViewModelForProofingTasks>(gridPagingRequest.page, count, gridPagingRequest.rows, viewModelForProofingTasks);
        }

        private IList<ViewModelForProofingTasks> GetProofingTasksListForAllManagers(int tenantId, bool isViewInterested, List<int> folderVersionIds, string[] statuses)
        {
            var planTasks =  (from planUserTask in this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Get().Where(x => (isViewInterested ? x.MarkInterested == isViewInterested : 1 == 1) && statuses.Contains(x.Status))
                    join task in _unitOfWork.RepositoryAsync<TaskList>().Get() on planUserTask.TaskID equals task.TaskID
                    //join forminstance in _unitOfWork.RepositoryAsync<FormInstance>().Get() on planUserTask.FormInstanceId equals forminstance.FormInstanceID
                    join folderVersion in _unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(y => folderVersionIds.Contains(y.FolderVersionID)) on planUserTask.FolderVersionID equals folderVersion.FolderVersionID
                    join folder in _unitOfWork.RepositoryAsync<Folder>().Get() on folderVersion.FolderID equals folder.FolderID
                    join wf in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get()
                    on folderVersion.WFStateID equals wf.WorkFlowVersionStateID
                    join accFolderMap in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get().Where(c => c.Account.IsActive == true)
                    on folderVersion.FolderID equals accFolderMap.FolderID into tmp
                    from accFolderMap in tmp.DefaultIfEmpty()
                    orderby planUserTask.ID descending
                    select new ViewModelForProofingTasks
                    {
                        MappingRowID = planUserTask.ID,
                        Account = accFolderMap == null ? "NA" : accFolderMap.Account.AccountName + "|" + accFolderMap.Account.AccountID,
                        TenantID = tenantId,
                        FolderId = folderVersion.FolderID,
                        FolderVersionId = folderVersion.FolderVersionID,
                        //FormInstanceId = planUserTask.FormInstanceId,
                        TaskId = planUserTask.TaskID,
                        Folder = folder.Name,
                        FolderVersion = folderVersion.FolderVersionNumber,
                        EffectiveDate = folderVersion.EffectiveDate,
                        Workflow = wf.WorkFlowState.WFStateName,
                     
                        Order = planUserTask.Order,
                        Task = task.TaskDescription,
                        Assignment = planUserTask.AssignedUserName,
                        Status = planUserTask.Status,
                        StartDate = planUserTask.StartDate,
                        DueDate = planUserTask.DueDate,
                        Completed = planUserTask.CompletedDate,
                        MarkInterested = planUserTask.MarkInterested,
                        PlanTaskUserMappingDetails = planUserTask.PlanTaskUserMappingDetails,
                        EstimatedTime = planUserTask.EstimatedTime,
                        ActualTime = planUserTask.ActualTime,
                        FolderVersionWFStateID = wf.WorkFlowState.WFStateID,
                        TaskWFStateID = planUserTask.WFStateID
                    }).OrderByDescending(O => O.MappingRowID).ToList();

            foreach(var task in planTasks)
            {
                task.Priority = getPriority(task.Order);
            }
            return planTasks;

        }

        private string getPriority(int? order)
        {
            string prioityName = WatchTaskPriority.Critical.ToString();
            switch (order)
            {
                case (int)WatchTaskPriority.Critical:
                    prioityName = WatchTaskPriority.Critical.ToString();
                    break;
                case (int)WatchTaskPriority.High:
                    prioityName = WatchTaskPriority.High.ToString();
                    break;
                case (int)WatchTaskPriority.Medium:
                    prioityName = WatchTaskPriority.Medium.ToString();
                    break;
                case (int)WatchTaskPriority.Low:
                    prioityName = WatchTaskPriority.Low.ToString();
                    break;
            }
            return prioityName;
        }
        private string getComments(int planUserTaskId, bool isDownload)
        {
            var allComments = "";
            var commets = this._unitOfWork.RepositoryAsync<TaskComments>().Get().Where(x => x.TaskID == planUserTaskId).OrderByDescending(m => m.ID).ToList();
            foreach (var comment in commets)
            {
                string commFormat = string.Empty;
                if (isDownload)
                    commFormat = string.Format("({0}-{1}): {2}", comment.AddedBy, comment.AddedDate.ToString("MM/dd/yyyy hh:mm tt"), comment.Comments);
                else
                    commFormat = !String.IsNullOrEmpty(comment.Attachment) ? "Attachment" : "";

                allComments = string.Format("{0} {1}", allComments, commFormat);
            }
            return allComments;
        }
        /// <summary>
        /// FolderId and FolderVersionId are set here to get the user for edit and view.
        /// Status Date (the latest date that the status was changed). m
        /// Only the folders on which the user is a primaryContact will be displayed on dashboard.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public GridPagingResponse<WatchListViewModel> GetWatchList(int tenantId, int? userId, GridPagingRequest gridPagingRequest, bool isViewInterested, int? RoleID)
        {
            if (userId == null) throw new ArgumentNullException();

            if (tenantId == 0) throw new Exception(InvalidTenantId);
            IList<WatchListViewModel> watchList = new List<WatchListViewModel>();
            SearchCriteria criteria = new SearchCriteria();
            criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);
            int count = 0;
            int inProgressStateId = (int)tmg.equinox.domain.entities.Enums.FolderVersionState.INPROGRESS;

            // WorkFlowState workflowState = this._unitOfWork.RepositoryAsync<WorkFlowState>().GetReleasedWorkflowState(tenantId);
            // int releaseStatus = workflowState.WFStateID;

            try
            {
                IList<WatchListViewModel> watchListNew = new List<WatchListViewModel>();
                IList<WatchListViewModel> watchListViewModelList = new List<WatchListViewModel>();
                // find list of folders(folder versions) the user(Primary Content) has created.
                List<int> folderVersionIds = new List<int>();
                if (CheckIsManager(userId))
                {
                    folderVersionIds = (from folder in this._unitOfWork.RepositoryAsync<Folder>().Get().Where(c => c.Name != GlobalVariables.MASTERLIST)
                                        join folderversion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(fv => fv.FolderVersionStateID == inProgressStateId)
                                        on folder.FolderID equals folderversion.FolderID
                                        join formInstance in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                        on folderversion.FolderVersionID equals formInstance.FolderVersionID
                                        join formDesign in this._unitOfWork.RepositoryAsync<FormDesign>().Get().Where(row => row.IsMasterList == false)
                                        on formInstance.FormDesignID equals formDesign.FormID
                                        select folderversion.FolderVersionID).Distinct().ToList();
                }
                else
                {
                    folderVersionIds = (from folder in this._unitOfWork.RepositoryAsync<Folder>().Get().Where(c => c.PrimaryContentID == userId && c.Name != GlobalVariables.MASTERLIST)
                                        join folderversion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(fv => fv.FolderVersionStateID == inProgressStateId)
                                        on folder.FolderID equals folderversion.FolderID
                                        join formInstance in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                        on folderversion.FolderVersionID equals formInstance.FolderVersionID
                                        join formDesign in this._unitOfWork.RepositoryAsync<FormDesign>().Get().Where(row => row.IsMasterList == false)
                                        on formInstance.FormDesignID equals formDesign.FormID
                                        select folderversion.FolderVersionID).Distinct().ToList();

                }
                var teamManagersFolderVersionIds = (from appTeamUserMap in this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Get().Where(c => c.UserID == userId)
                                                    join wfStateFolderversion in this._unitOfWork.RepositoryAsync<WorkFlowStateFolderVersionMap>().Get()
                                                    on appTeamUserMap.ApplicableTeamID equals wfStateFolderversion.ApplicableTeamID
                                                    join folderversion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(fv => fv.FolderVersionStateID == inProgressStateId && fv.Folder.Name != GlobalVariables.MASTERLIST)
                                                    on wfStateFolderversion.FolderVersionID equals folderversion.FolderVersionID
                                                    where appTeamUserMap.IsTeamManager == true && appTeamUserMap.IsDeleted == false
                                                    select wfStateFolderversion.FolderVersionID).ToList();

                folderVersionIds.AddRange(teamManagersFolderVersionIds);
                folderVersionIds = folderVersionIds.Distinct().ToList();
                var accountList = (from folderVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                   join accFolderMap in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get()
                                   on folderVersion.FolderID equals accFolderMap.FolderID
                                   where folderVersionIds.Contains(folderVersion.FolderVersionID)
                                   select new { folderVersion.FolderVersionID, accFolderMap.Account }
                                    ).ToList();

                //var accountList = accountListAll.Where(row => row.Account.IsActive == true).ToList();
                if (isViewInterested)
                {
                    watchListViewModelList = (from folderVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Include(c => c.WorkFlowStateUserMaps).Get()
                                              join folder in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                              on folderVersion.FolderID equals folder.FolderID
                                              join interestedfolder in this._unitOfWork.RepositoryAsync<InterestedFolderVersion>().Get()
                                               on folderVersion.FolderVersionID equals interestedfolder.FolderVersionID
                                              where folderVersionIds.Contains(folderVersion.FolderVersionID) && interestedfolder.UserID == userId
                                              select new WatchListViewModel
                                              {
                                                  FolderVersionId = folderVersion.FolderVersionID,
                                                  EffectiveDate = folderVersion.EffectiveDate,
                                                  Folder = folder.Name,
                                                  FolderVersionNumber = folderVersion.FolderVersionNumber,
                                                  StatusDate = folderVersion.UpdatedDate.HasValue ? folderVersion.UpdatedDate.Value : folderVersion.AddedDate,
                                                  FolderId = folder.FolderID,
                                                  Owner = folderVersion.AddedBy,
                                                  TenantID = folderVersion.TenantID,
                                                  // Comments = folderVersion.Comments,
                                                  CategoryId = folderVersion.CategoryID == null ? 0 : folderVersion.CategoryID.Value,
                                                  MarkInterested = interestedfolder.FolderVersionID == folderVersion.FolderVersionID ? true : false,
                                                  CategoryName = folderVersion.CategoryID != null ? folderVersion.FolderVersionCategory.FolderVersionCategoryName : "",
                                                  AssignedUserCount = folderVersion.WorkFlowStateUserMaps.Where(row => row.IsActive == true).Count()
                                              }).ToList();
                }
                else
                {

                    watchListViewModelList = (from folderVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Query()
                                              .Include(c => c.WorkFlowStateUserMaps)
                                             .Get() // .Include(c => c.InterstedFolderVersions)
                                              join folder in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                              on folderVersion.FolderID equals folder.FolderID
                                              where folderVersionIds.Contains(folderVersion.FolderVersionID)
                                              select new WatchListViewModel
                                              {
                                                  FolderVersionId = folderVersion.FolderVersionID,
                                                  EffectiveDate = folderVersion.EffectiveDate,
                                                  Folder = folder.Name,
                                                  FolderVersionNumber = folderVersion.FolderVersionNumber,
                                                  StatusDate = folderVersion.UpdatedDate.HasValue ? folderVersion.UpdatedDate.Value : folderVersion.AddedDate,
                                                  FolderId = folder.FolderID,
                                                  Owner = folderVersion.AddedBy,
                                                  TenantID = folderVersion.TenantID,
                                                  //Comments = folderVersion.Comments,
                                                  CategoryId = folderVersion.CategoryID == null ? 0 : folderVersion.CategoryID.Value,
                                                  MarkInterested = false,
                                                  //MarkInterested = folderVersion.InterstedFolderVersions.Where(ro => ro.FolderVersionID == folderVersion.FolderVersionID).Select(c => c.InterstedFolderVersionID).Any(),
                                                  CategoryName = folderVersion.CategoryID != null ? folderVersion.FolderVersionCategory.FolderVersionCategoryName : "",
                                                  AssignedUserCount = folderVersion.WorkFlowStateUserMaps.Where(row => row.IsActive == true).Count()
                                              }).ToList();
                    foreach (var item in watchListViewModelList)
                    {
                        var interestedfolderversionId = this._unitOfWork.RepositoryAsync<InterestedFolderVersion>().Query().Filter(c => c.FolderVersionID == item.FolderVersionId && c.UserID == userId).Get().Select(c => c.InterstedFolderVersionID).Any();
                        if (interestedfolderversionId)
                        {
                            item.MarkInterested = true;
                        }

                    }
                }
                //foreach( var item in watchListViewModelList){
                //      item.AssignedUserCount = this._unitOfWork.RepositoryAsync<WorkFlowStateUserMap>().Query().Filter(c => c.FolderVersionID == item.FolderVersionId && c.IsActive == true).Get().Select(c=>c.UserID).Distinct().ToList().Count();
                //}

                foreach (var folderVersionId in folderVersionIds)
                {
                    int workFlowStateID = 0;


                    WatchListViewModel watchListViewModel = new WatchListViewModel();
                    var account = accountList.Where(x => x.FolderVersionID == folderVersionId).Select(x => x.Account).FirstOrDefault();

                    if (account == null && (RoleID == 22 || RoleID == 21))
                        continue;

                    //if (account != null && account.IsActive)
                    {
                        //for TPA Analyst show only JAA folders
                        //if (RoleID == 22)
                        //{
                        //    watchListViewModel = watchListViewModelList.Where(x => x.FolderVersionId == folderVersionId && (x.CategoryName == "New Group - JAA" || x.CategoryName == "Change Request - JAA")).FirstOrDefault();
                        //}
                        //else
                        //{
                            watchListViewModel = watchListViewModelList.Where(x => x.FolderVersionId == folderVersionId).FirstOrDefault();
                        //}
                        if (watchListViewModel != null && (account == null || account.IsActive))
                        {
                            string currentWorkFlowState = GetFolderVersionWorkFlowState(folderVersionId, ref workFlowStateID);
                            watchListViewModel.Owner = string.IsNullOrEmpty(watchListViewModel.Owner) ? GetUserNameByUserId((int)userId) : watchListViewModel.Owner;
                            watchListViewModel.IsActive = account == null ? true : account.IsActive;
                            watchListViewModel.Status = currentWorkFlowState;
                            watchListViewModel.Account = account == null ? "NA" : account.AccountName;

                            watchList.Add(watchListViewModel);
                        }
                    }
                }
                watchList = (watchList.AsQueryable()).ApplySearchCriteria(criteria).OrderByDescending(x => x.EffectiveDate).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                       .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return new GridPagingResponse<WatchListViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, watchList);
        }

        //
        public DataTable GetWatchListListDataTable(int tenantId, int? userId)
        {
            if (userId == null) throw new ArgumentNullException();

            if (tenantId == 0) throw new Exception(InvalidTenantId);
            IList<WatchListViewModel> watchList = new List<WatchListViewModel>();
            int count = 0;
            int inProgressStateId = (int)tmg.equinox.domain.entities.Enums.FolderVersionState.INPROGRESS;
            DataTable dt = new DataTable();
            try
            {
                IList<WatchListViewModel> watchListNew = new List<WatchListViewModel>();
                // find list of folders(folder versions) the user(Primary Content) has created.
                List<int> folderVersionIds = (from folder in this._unitOfWork.RepositoryAsync<Folder>().Get().Where(c => c.PrimaryContentID == userId && c.Name != GlobalVariables.MASTERLIST)
                                              join folderversion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(fv => fv.FolderVersionStateID == inProgressStateId)
                                              on folder.FolderID equals folderversion.FolderID
                                              select folderversion.FolderVersionID).Distinct().ToList();


                var teamManagersFolderVersionIds = (from appTeamUserMap in this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Get().Where(c => c.UserID == userId)
                                                    join wfStateFolderversion in this._unitOfWork.RepositoryAsync<WorkFlowStateFolderVersionMap>().Get()
                                                    on appTeamUserMap.ApplicableTeamID equals wfStateFolderversion.ApplicableTeamID
                                                    join folderversion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(fv => fv.FolderVersionStateID == inProgressStateId && fv.Folder.Name != GlobalVariables.MASTERLIST)
                                                    on wfStateFolderversion.FolderVersionID equals folderversion.FolderVersionID
                                                    where appTeamUserMap.IsTeamManager == true && appTeamUserMap.IsDeleted == false
                                                    select wfStateFolderversion.FolderVersionID).ToList();

                folderVersionIds.AddRange(teamManagersFolderVersionIds);
                folderVersionIds = folderVersionIds.Distinct().ToList();
                var accountList = (from folderVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                   join accFolderMap in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get()
                                   on folderVersion.FolderID equals accFolderMap.FolderID
                                   where folderVersionIds.Contains(folderVersion.FolderVersionID)
                                   select new { folderVersion.FolderVersionID, accFolderMap.Account }
                                    ).ToList();

                List<WatchListViewModel> watchListViewModelList = (from folderVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                                                   join folder in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                                                   on folderVersion.FolderID equals folder.FolderID
                                                                   where folderVersionIds.Contains(folderVersion.FolderVersionID)
                                                                   select new WatchListViewModel
                                                                   {
                                                                       FolderVersionId = folderVersion.FolderVersionID,
                                                                       EffectiveDate = folderVersion.EffectiveDate,
                                                                       Folder = folder.Name,
                                                                       FolderVersionNumber = folderVersion.FolderVersionNumber,
                                                                       StatusDate = folderVersion.UpdatedDate.HasValue ? folderVersion.UpdatedDate.Value : folderVersion.AddedDate,
                                                                       FolderId = folder.FolderID,
                                                                       Owner = folderVersion.AddedBy,
                                                                       TenantID = folderVersion.TenantID,
                                                                       //Comments = folderVersion.Comments,
                                                                       CategoryId = folderVersion.CategoryID == null ? 0 : folderVersion.CategoryID.Value,
                                                                       CategoryName = folderVersion.CategoryID != null ? folderVersion.FolderVersionCategory.FolderVersionCategoryName : "",
                                                                       AssignedUserCount = folderVersion.WorkFlowStateUserMaps.Where(row => row.IsActive == true).Count(),
                                                                       MarkInterested = false
                                                                   }).ToList();
                foreach (var folderVersionId in folderVersionIds)
                {
                    int workFlowStateID = 0;
                    string currentWorkFlowState = GetFolderVersionWorkFlowState(folderVersionId, ref workFlowStateID);

                    WatchListViewModel watchListViewModel = new WatchListViewModel();
                    var account = accountList.Where(x => x.FolderVersionID == folderVersionId).Select(x => x.Account).FirstOrDefault();
                    if (account != null && account.IsActive)
                    {
                        watchListViewModel = watchListViewModelList.Where(x => x.FolderVersionId == folderVersionId).FirstOrDefault();
                        if (watchListViewModel != null)
                        {
                            watchListViewModel.Owner = string.IsNullOrEmpty(watchListViewModel.Owner) ? GetUserNameByUserId((int)userId) : watchListViewModel.Owner;
                            watchListViewModel.IsActive = account == null ? true : account.IsActive;
                            watchListViewModel.Status = currentWorkFlowState;
                            watchListViewModel.Account = account == null ? "NA" : account.AccountName;

                            var interestedfolderversionId = this._unitOfWork.RepositoryAsync<InterestedFolderVersion>().Query().Filter(c => c.FolderVersionID == watchListViewModel.FolderVersionId && c.UserID == userId).Get().Select(c => c.InterstedFolderVersionID).Any();
                            if (interestedfolderversionId)
                            {
                                watchListViewModel.MarkInterested = true;
                            }

                            watchList.Add(watchListViewModel);
                        }
                    }
                }
                watchList = (watchList.AsQueryable()).OrderByDescending(x => x.FolderVersionId).ToList();

                dt.Columns.Add("Account");
                dt.Columns.Add("Folder");
                dt.Columns.Add("FolderVersionNumber");
                dt.Columns.Add("EffectiveDate");
                dt.Columns.Add("Status");
                dt.Columns.Add("StatusDate");
                dt.Columns.Add("Comments");
                dt.Columns.Add("CategoryName");
                dt.Columns.Add("AssignedUserCount");
                dt.Columns.Add("MarkInterested");

                foreach (var watchListitem in watchList)
                {
                    DataRow row = dt.NewRow();
                    row["Account"] = watchListitem.Account;
                    row["Folder"] = watchListitem.Folder;
                    row["FolderVersionNumber"] = watchListitem.FolderVersionNumber;
                    row["EffectiveDate"] = watchListitem.EffectiveDate.ToString();
                    row["Status"] = watchListitem.Status;
                    row["StatusDate"] = watchListitem.StatusDate.ToString();
                    row["Comments"] = watchListitem.Comments;
                    row["CategoryName"] = watchListitem.CategoryName;
                    row["AssignedUserCount"] = watchListitem.AssignedUserCount.ToString();
                    row["MarkInterested"] = watchListitem.MarkInterested == true ? "Yes" : "No";

                    dt.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return dt;
        }
        //TODO: Need get appropriate UserID and UserName

        public DataTable GetWatchListListForProofingTasksDataTable(int tenantId, int? userId, string UserName, bool isViewInterested, string viewMode, int taskFolderVersionId)
        {
            GridPagingRequest gridPagingRequest = new GridPagingRequest();
            GridPagingResponse<ViewModelForProofingTasks> viewModelForProofingTasks = GetWatchListForProofingTasks(tenantId, userId, UserName, isViewInterested, viewMode, gridPagingRequest, taskFolderVersionId, true);
            return ToDataTable<ViewModelForProofingTasks>(viewModelForProofingTasks.rows.ToList());
        }
        public DataTable GetWorkQueueListListForProofingTasksDataTable(int tenantId, int? userId, string UserName, string viewMode, int taskFolderVersionId)
        {
            GridPagingRequest gridPagingRequest = new GridPagingRequest();
            var viewModelForProofingTasks = GetWorkQueueListForProofingTasks(tenantId, userId, UserName, viewMode, taskFolderVersionId, true, gridPagingRequest);
            return ToDataTable<ViewModelForProofingTasks>(viewModelForProofingTasks.rows.ToList());
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                if (type.Name == "DateTime" || type.Name == "Boolean")
                    dataTable.Columns.Add(prop.Name, typeof(string));
                else
                    dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public GridPagingResponse<UserRoleAssignmentViewModel> GetUserRoleAssignment(int folderVersionID, int? currentUserId, string userAssignmentDialogState, GridPagingRequest gridPagingRequest)
        {
            IList<UserRoleAssignmentViewModel> userRoleAssignment = new List<UserRoleAssignmentViewModel>(); ;
            SearchCriteria criteria = new SearchCriteria();
            criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);
            int count = 0;
            Nullable<int> catID = null;
            Nullable<bool> IsPortfolioFolder = null;
            try
            {
                if (folderVersionID > 0)
                {
                    //get folderversion category
                    catID = (from fldrVer in this._unitOfWork.RepositoryAsync<FolderVersion>().Query()
                                     .Filter(c => c.FolderVersionID == folderVersionID && c.IsActive == true)
                                     .Get()
                             select fldrVer.CategoryID).FirstOrDefault();
                    // Get Folder is Portfolio or not
                    IsPortfolioFolder = (from fldrVer in this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(x => x.FolderVersionID == folderVersionID)
                                         join folder in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                         on fldrVer.FolderID equals folder.FolderID
                                         select folder.IsPortfolio).FirstOrDefault();

                    if (userAssignmentDialogState != "add" && userAssignmentDialogState != "delete")
                    {
                        userRoleAssignment = (from userMap in this._unitOfWork.RepositoryAsync<WorkFlowStateUserMap>().Query().Filter(c => c.FolderVersionID == folderVersionID && c.IsActive == true).Get()
                                              join user in this._unitOfWork.RepositoryAsync<User>().Get()
                                              on userMap.UserID equals user.UserID

                                              join userRoleAsco in this._unitOfWork.RepositoryAsync<UserRoleAssoc>().Get()
                                              on userMap.UserID equals userRoleAsco.UserId

                                              join userRole in this._unitOfWork.RepositoryAsync<UserRole>().Get()
                                              on userRoleAsco.RoleId equals userRole.RoleID

                                              join teamAsoc in this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Get()
                                              on userMap.ApplicableTeamID equals teamAsoc.ApplicableTeamID
                                              where teamAsoc.IsDeleted == false && teamAsoc.IsTeamManager == false

                                              join team in this._unitOfWork.RepositoryAsync<ApplicableTeam>().Get()
                                              on userMap.ApplicableTeamID equals team.ApplicableTeamID
                                              select new UserRoleAssignmentViewModel
                                              {
                                                  UserRoleID = userRoleAsco.RoleId,
                                                  UserRoleName = userRole.Name,
                                                  UserID = userMap.UserID,
                                                  UserName = user.UserName,
                                                  ApplicableTeamID = userMap.ApplicableTeamID,
                                                  ApplicableTeamName = team.ApplicableTeamName
                                              }).Distinct().ToList();
                    }
                    else if (userAssignmentDialogState == "add")
                    {
                        List<int> teamIds = (from teamMap in this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Get().Where(t => t.UserID == currentUserId && t.IsDeleted == false && t.IsTeamManager == true)
                                             select teamMap.ApplicableTeamID).Distinct().ToList();

                        userRoleAssignment = (from teamAsoc in this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Get()
                                              where teamIds.Contains(teamAsoc.ApplicableTeamID) && teamAsoc.IsDeleted == false

                                              join team in this._unitOfWork.RepositoryAsync<ApplicableTeam>().Get()
                                              on teamAsoc.ApplicableTeamID equals team.ApplicableTeamID

                                              join user in this._unitOfWork.RepositoryAsync<User>().Get()
                                              on teamAsoc.UserID equals user.UserID

                                              join userRoleAsco in this._unitOfWork.RepositoryAsync<UserRoleAssoc>().Get()
                                             on user.UserID equals userRoleAsco.UserId

                                              join userRole in this._unitOfWork.RepositoryAsync<UserRole>().Get()
                                              on userRoleAsco.RoleId equals userRole.RoleID


                                              select new UserRoleAssignmentViewModel
                                              {
                                                  UserRoleID = userRoleAsco.RoleId,
                                                  UserRoleName = userRole.Name,
                                                  UserID = user.UserID,
                                                  UserName = user.UserName,
                                                  ApplicableTeamID = teamAsoc.ApplicableTeamID,
                                                  ApplicableTeamName = team.ApplicableTeamName,
                                                  ApplicableTeamUserMapID = teamAsoc.ApplicableTeamUserMapID
                                              }).Distinct().ToList();

                    }
                    else if (userAssignmentDialogState == "delete")
                    {
                        List<int> teamIds = (from teamMap in this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Get().Where(t => t.UserID == currentUserId && t.IsDeleted == false && t.IsTeamManager == true)
                                             select teamMap.ApplicableTeamID).Distinct().ToList();

                        userRoleAssignment = (from userMap in this._unitOfWork.RepositoryAsync<WorkFlowStateUserMap>().Get()
                                              where (userMap.FolderVersionID == folderVersionID && teamIds.Contains(userMap.ApplicableTeamID) && userMap.IsActive == true)

                                              join team in this._unitOfWork.RepositoryAsync<ApplicableTeam>().Get()
                                              on userMap.ApplicableTeamID equals team.ApplicableTeamID

                                              join user in this._unitOfWork.RepositoryAsync<User>().Get()
                                              on userMap.UserID equals user.UserID

                                              join userRoleAsco in this._unitOfWork.RepositoryAsync<UserRoleAssoc>().Get()
                                             on user.UserID equals userRoleAsco.UserId

                                              join userRole in this._unitOfWork.RepositoryAsync<UserRole>().Get()
                                              on userRoleAsco.RoleId equals userRole.RoleID

                                              select new UserRoleAssignmentViewModel
                                              {
                                                  UserRoleID = userRoleAsco.RoleId,
                                                  UserRoleName = userRole.Name,
                                                  UserID = userMap.UserID,
                                                  UserName = user.UserName,
                                                  ApplicableTeamID = userMap.ApplicableTeamID,
                                                  ApplicableTeamName = team.ApplicableTeamName,
                                              }).Distinct().ToList();
                    }
                }
                //Restricting folders to be assigned to TPA Analyst when category is not JAA, and If the folder is Portfolio then also to restrict portfolio folder assigned to TPA Analyst
                //if ((catID != Convert.ToInt32(Category.JAANewGroup) && catID != Convert.ToInt32(Category.JAAChangeRequest)) || (IsPortfolioFolder != null && IsPortfolioFolder == true))
                //    userRoleAssignment = userRoleAssignment.ToList().Where(c => c.UserRoleID != Convert.ToInt32(21)).ToList(); //Role.TPAANALYST
                //if ((IsPortfolioFolder != null && IsPortfolioFolder == true))
                //    userRoleAssignment = userRoleAssignment.ToList().Where(c => c.UserRoleID != 21).ToList(); //Convert.ToInt32(Role.EBAANALYST)
                //Restricting folders to be assigned to Viewer
                //userRoleAssignment = userRoleAssignment.ToList().Where(c => c.UserRoleID != Convert.ToInt32(21)).ToList(); //Role.VIEWER

                if (userRoleAssignment.Count > 0)
                    userRoleAssignment = (userRoleAssignment.AsQueryable()).ApplySearchCriteria(criteria).OrderBy(x => x.UserID).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                          .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return new GridPagingResponse<UserRoleAssignmentViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, userRoleAssignment);
        }

        public IList<KeyValue> GetTeamsOfFolderVersion(int tenantId, int folderVersionID)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            IList<KeyValue> list = new List<KeyValue>();
            if (folderVersionID > 0)
            {
                list = (from workflowStateFolderVersionMap in this._unitOfWork.RepositoryAsync<WorkFlowStateFolderVersionMap>().Get()
                        join applicableTeam in this._unitOfWork.RepositoryAsync<ApplicableTeam>().Get()
                        on workflowStateFolderVersionMap.ApplicableTeamID equals applicableTeam.ApplicableTeamID
                        where (workflowStateFolderVersionMap.FolderVersionID == folderVersionID && workflowStateFolderVersionMap.Folder.TenantID == tenantId)
                        select new KeyValue
                        {
                            Key = (int)applicableTeam.ApplicableTeamID,
                            Value = applicableTeam.ApplicableTeamName
                        }).Distinct().ToList();
            }
            return list;
        }

        #region Private Methods
        string GetUserNameByUserId(int userId)
        {
            string userName = string.Empty;
            userName = (from user in this._unitOfWork.RepositoryAsync<User>()
                           .Query()
                           .Filter(c => c.UserID == userId)
                           .Get()
                        select user.UserName).FirstOrDefault();


            return userName;
        }
        string GetFolderVersionWorkFlowState(int folderVersionId, ref int workFlowStateID)
        {
            var workFlowStateName = string.Empty;
            var workFlow = (from fldrVer in this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(x => x.FolderVersionID == folderVersionId)
                            join workflow in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get()
                            on fldrVer.WFStateID equals workflow.WorkFlowVersionStateID

                            select new
                            {
                                workflow.WorkFlowVersionStateID,
                                workflow.Sequence,
                                workflow.WorkFlowVersionID,
                                workflow.WorkFlowState
                            }).FirstOrDefault();


            if (workFlow != null)
            {
                var workflowStatesbyseq = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get().Where(c => c.WorkFlowVersionID == workFlow.WorkFlowVersionID && c.Sequence == workFlow.Sequence).Select(c => c.WorkFlowState).ToList();
                if (workflowStatesbyseq != null)
                {
                    foreach (var nm in workflowStatesbyseq)
                    {
                        if (workFlowStateName == string.Empty)
                        {
                            workFlowStateName = nm.WFStateName;
                        }
                        else
                        {
                            workFlowStateName = workFlowStateName + '|' + nm.WFStateName;
                        }
                    }
                }
                workFlowStateID = workFlow.WorkFlowVersionStateID;
            }

            return workFlowStateName;
            // as ObjectQuery<WorkFlowVersionState>).Include(c => c.WorkFlowState).FirstOrDefault(); //.FirstOrDefault() as WorkFlowVersionState).;          
            //return workFlow.WFStateName;
        }
        #endregion


        #region EmailNotification To Member

        private EmailLogger GetWorkFlowStateFolderVersionDetails(int folderVersionID, int workFlowStateID, int? userID, int tenantID, string userName, List<string> sendMailToList, int approvalStatusId)
        {
            var firstWorkflowStateID = (from wfVersion in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get().Where(c => c.WorkFlowVersionStateID == workFlowStateID)
                                        join workMast in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get()
                                        on wfVersion.WorkFlowVersionID equals workMast.WorkFlowVersionID
                                        select wfVersion.WorkFlowVersionStateID
                                       ).FirstOrDefault();

            EmailLogger emailLoggerElements = (from folderVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                               join folder in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                               on folderVersion.FolderID equals folder.FolderID
                                               //join accountFolderMap in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get()
                                               //on folderVersion.FolderID equals accountFolderMap.FolderID
                                               //join account in this._unitOfWork.RepositoryAsync<Account>().Get()
                                               //on accountFolderMap.AccountID equals account.AccountID
                                               join folderVersionWFState in this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Get()
                                               on folderVersion.FolderVersionID equals folderVersionWFState.FolderVersionID
                                               join approveStatusType in this._unitOfWork.RepositoryAsync<WorkFlowStateApprovalTypeMaster>().Get()
                                               on folderVersionWFState.ApprovalStatusID equals approveStatusType.WorkFlowStateApprovalTypeID
                                               join workState in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get()
                                               on folderVersionWFState.WFStateID equals workState.WorkFlowVersionStateID
                                               where folderVersion.FolderVersionID == folderVersionID && folderVersionWFState.WFStateID == workFlowStateID
                                               join accountFolderMap in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get()
                                               on folder.FolderID equals accountFolderMap.FolderID
                                               into tmp
                                               from accountFolderMap in tmp.DefaultIfEmpty()
                                               join account in this._unitOfWork.RepositoryAsync<Account>().Get()
                                                on accountFolderMap.AccountID equals account.AccountID
                                                into tmpAcc
                                               from account in tmpAcc.DefaultIfEmpty()
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
            emailLoggerElements.UserId = (int)userID;
            emailLoggerElements.TenantID = tenantID;
            emailLoggerElements.AddedBy = userName;
            emailLoggerElements.ApprovedWorkFlowStateID = workFlowStateID == firstWorkflowStateID ? workFlowStateID : firstWorkflowStateID;
            emailLoggerElements.ApprovedWorkFlowStateName = approvalStatusId == (int)ApprovalStatus.APPROVED ? this._unitOfWork.RepositoryAsync<WorkFlowVersionState>()
                                                                   .Query().Filter(x => x.WorkFlowVersionStateID == emailLoggerElements.ApprovedWorkFlowStateID)
                                                                   .Get().Select(sel => sel.WorkFlowState.WFStateName).FirstOrDefault() : emailLoggerElements.WorkFlowState;
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
                //if (emailSettings.SendGridFrom == null || string.IsNullOrEmpty(emailSettings.SendGridPassword))
                //{
                //    items.Add(new ServiceResultItem() { Messages = new string[] { EmailNotificationConstants.EmailNotificationErrorMessage } });
                //}
                if (emailSettings.SmtpFrom == null || string.IsNullOrEmpty(emailSettings.SmtpServerHostName))
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
                                                                 "Regards,<br />Core Support Team.</p>" +
                                                                 "<p style='font-style:italic;font-family:Calibri;font-size:15px;'>This is Autogenerated Mail. Please do not reply.</p>";

            public const string EmptyEmailSentToListMessage = "Email Sent To list is Empty.Hence unable delivered an email.Please contact support team.";
        }


        #endregion EmailNotification To Member


        public ServiceResult SaveInterestedWatchList(int folderVersionId, int? currentUserId)
        {
            if (currentUserId == null) throw new ArgumentNullException();
            ServiceResult result = new ServiceResult();
            try
            {

                var UserName = this._unitOfWork.RepositoryAsync<User>().Query()
                     .Filter(c => c.UserID == currentUserId).Get().Select(c => c.UserName).FirstOrDefault();

                //var IsManager = this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Query()
                //     .Filter(c => c.UserID == currentUserId).Get().Select(c => c.IsTeamManager).FirstOrDefault();

                //if (IsManager)
                //{
                var watchlistmodel = new InterestedFolderVersion();
                watchlistmodel.UserID = Convert.ToInt32(currentUserId);
                watchlistmodel.FolderVersionID = folderVersionId;
                watchlistmodel.AddedBy = UserName;
                watchlistmodel.AddedDate = DateTime.Now;
                this._unitOfWork.RepositoryAsync<InterestedFolderVersion>().Insert(watchlistmodel);
                this._unitOfWork.Save();
                //}
                result.Result = ServiceResultStatus.Success;
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

        public ServiceResult SaveInterestedWatchList(int TaskMappingID, bool markInterested)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                DPFPlanTaskUserMapping dpfPlanTaskUserMapping = new DPFPlanTaskUserMapping();
                dpfPlanTaskUserMapping = _unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Get().Where(x => x.ID == TaskMappingID).FirstOrDefault();
                if (dpfPlanTaskUserMapping != null)
                {
                    dpfPlanTaskUserMapping.MarkInterested = markInterested;
                    _unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Update(dpfPlanTaskUserMapping);
                    _unitOfWork.Save();
                }
                result.Result = ServiceResultStatus.Success;
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

        public ServiceResult DeleteInterestedWatchList(int folderVersionId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var deleterow = this._unitOfWork.RepositoryAsync<InterestedFolderVersion>().Query().Get().Where(c => c.FolderVersionID == folderVersionId).FirstOrDefault();
                this._unitOfWork.RepositoryAsync<InterestedFolderVersion>().Delete(deleterow);
                this._unitOfWork.Save();
                result.Result = ServiceResultStatus.Success;
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

        public ServiceResult SaveInterestedAllWatchList(int[] taskMappingIDs, bool value)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                foreach (int taskId in taskMappingIDs)
                {
                    DPFPlanTaskUserMapping dpfPlanTaskUserMapping = new DPFPlanTaskUserMapping();
                    dpfPlanTaskUserMapping = _unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Get().Where(x => x.ID == taskId).FirstOrDefault();
                    if (dpfPlanTaskUserMapping != null)
                    {
                        dpfPlanTaskUserMapping.MarkInterested = value;
                        _unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Update(dpfPlanTaskUserMapping);
                        _unitOfWork.Save();
                    }
                }
                result.Result = ServiceResultStatus.Success;
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

        public bool CheckIsManager(int? currentUserId)
        {

            var IsManager = this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Query()
                     .Filter(c => c.UserID == currentUserId && c.IsDeleted == false && c.IsTeamManager == true).Get().FirstOrDefault();

            return IsManager == null ? false : true;
        }
        public string GetLastUserActivity(string UserName)
        {
            var actCount = this._unitOfWork.RepositoryAsync<UserActivity>().Query()
                .Filter(c => c.UserName == UserName).Get().ToList();
            if (actCount.Count > 0)
            {
                if (actCount.Count == 1)
                {
                    var act = (from a in actCount
                               where a.UserName == UserName
                               orderby a.TimeUtc descending
                               select new
                               {
                                   a.TimeUtc
                               }).FirstOrDefault().TimeUtc;
                    return act.Value.ToLongDateString() + ", " + act.Value.ToShortTimeString();
                }
                else
                {
                    var act = (from a in actCount
                               where a.UserName == UserName
                               orderby a.TimeUtc descending
                               select new
                               {
                                   a.TimeUtc
                               })
                .Skip(1).Take(1)
                .FirstOrDefault().TimeUtc;
                    return act.Value.ToLongDateString() + ", " + act.Value.ToShortTimeString();
                }
            }
            return null;
        }

        public IList<UserActivity> GetActivity(string UserName)
        {
            UserActivity objUserActivity = new UserActivity();
            List<UserActivity> lstUserActivity = new List<UserActivity>();

            //Login Activity log
            var logCount = this._unitOfWork.RepositoryAsync<UserActivity>().Query()
                .Filter(c => c.UserName == UserName && c.Event == "LogOn").Get().ToList();
            if (logCount.Count > 0)
            {
                var logActivity = this._unitOfWork.RepositoryAsync<UserActivity>().Query()
                   .Filter(c => c.UserName == UserName && c.Event == "LogOn").Get().OrderByDescending(c => c.TimeUtc)
                   .Take(1)
                   .FirstOrDefault()
                   .TimeUtc;
                objUserActivity.Event = "Today " + logActivity.Value.ToShortTimeString();
                string appName = config.Config.GetApplicationName();
                if(appName.ToLower() == "emedicaresync")
                {
                    objUserActivity.Message = "Logged in eMS";
                }else
                {
                    objUserActivity.Message = "Logged in eBS";
                }
                lstUserActivity.Add(objUserActivity);
            }
            //Docuement update Activity log
            UserName = "<b>" + UserName + "</b>";
            var docActivity = (from fial in this._unitOfWork.RepositoryAsync<FormInstanceActivityLog>().Get()
                               group fial by new { fial.FormInstanceID } into gr
                               let firstFolderGroup = gr.OrderByDescending(s => s.UpdatedLast).FirstOrDefault()
                               join fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                               on firstFolderGroup.FormInstanceID equals fi.FormInstanceID
                               where firstFolderGroup.UpdatedBy == UserName && fi.IsActive == true
                               orderby firstFolderGroup.UpdatedLast descending
                               select new
                               {
                                   firstFolderGroup.UpdatedLast,
                                   fi.Name,
                               }).ToList().Take(6);
            foreach (var d in docActivity)
            {
                objUserActivity = new UserActivity();
                objUserActivity.Event = (d.UpdatedLast.ToShortDateString() == DateTime.Today.ToShortDateString() ? "Today " : (d.UpdatedLast.ToString("ddd") + " " + d.UpdatedLast.Day) + ", ") + d.UpdatedLast.ToShortTimeString();
                objUserActivity.Message = "Updated a Doc - " + d.Name;
                lstUserActivity.Add(objUserActivity);
            }
            return lstUserActivity;
        }

        public int GetUserID(string username)
        {
            User user = this._unitOfWork.RepositoryAsync<User>().Query().Filter(x => x.UserName == username).Get().FirstOrDefault();
            if (user == null)
                return 0;

            return user.UserID;
        }

        public int GetNotificationCount(int userid)
        {
            try
            {
                List<Notificationstatus> notificationDataList = this._unitOfWork.RepositoryAsync<Notificationstatus>().Query()
                                           .Filter(x => x.Userid == userid && x.IsRead == false).Get().ToList();

                return notificationDataList.Count();
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        public GridPagingResponse<NotificationstatusViewModel> GetNotificationstatusDataList(bool viewMode, int? CurrentUserId, GridPagingRequest gridPagingRequest)
        {
            List<NotificationstatusViewModel> notificationDataList = null;
            SearchCriteria criteria = new SearchCriteria();
            criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);
            int count = 0;
            notificationDataList = Get(CurrentUserId, viewMode).ToList();
            notificationDataList = notificationDataList.ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                  .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);

            return new GridPagingResponse<NotificationstatusViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, notificationDataList);
        }

        private List<NotificationstatusViewModel> Get(int? currentUserId, bool isActive)
        {
            List<NotificationstatusViewModel> notificationDataList;

            notificationDataList = (from i in _unitOfWork.RepositoryAsync<Notificationstatus>().Query()
            .Filter(m => m.Userid == currentUserId && m.IsRead == isActive).Get().ToList()
                                    orderby i.ID descending
                                    select new NotificationstatusViewModel
                                    {
                                        ID = i.ID,
                                        Message = (i.AddedDate == null ? i.Message : string.Concat(i.Message, " -- ", i.AddedDate.ToString("MM/dd/yyyy hh:mm tt")))
                                    }).ToList();
            return notificationDataList;
        }

        public IList<NotificationstatusViewModel> GetNotificationisreadClrData(bool viewMode, int? CurrentUserId)
        {
            List<NotificationstatusViewModel> notificationClrDataList = null;

            var unReadNotificationdata = _unitOfWork.RepositoryAsync<Notificationstatus>().Query().Filter
                (x => x.Userid == CurrentUserId && x.IsRead == viewMode).Get().ToList();

            foreach (var item in unReadNotificationdata)
            {
                item.IsRead = true;
                this._unitOfWork.RepositoryAsync<Notificationstatus>().Update(item);
                this._unitOfWork.Save();
            }

            return notificationClrDataList;
        }

        public Boolean SaveNotificationDataList(string Message, int SendToUserId, string loggedinUserName)
        {

            Notificationstatus notificationDataObj;
            try
            {
                notificationDataObj = new Notificationstatus();
                notificationDataObj.Message = Message;
                notificationDataObj.Userid = Convert.ToInt32(SendToUserId);
                notificationDataObj.IsRead = false;
                notificationDataObj.IsActive = true;
                notificationDataObj.AddedBy = loggedinUserName;// GetUserNameByUserId(Convert.ToInt32(CurrentUserId));
                notificationDataObj.AddedDate = DateTime.Now;
                notificationDataObj.UpdatedBy = loggedinUserName;// GetUserNameByUserId(Convert.ToInt32(CurrentUserId));
                notificationDataObj.UpdatedDate = DateTime.Now;
                this._unitOfWork.RepositoryAsync<Notificationstatus>().Insert(notificationDataObj);
                _unitOfWork.Save();

            }

            catch (Exception ex)
            {

                throw;
            }

            return true;
        }

        public IQueryable<ViewModelForProofingTasks> GetSubworkQueueList(int FolderVersionID, string viewMode)
        {
            IList<ViewModelForProofingTasks> viewModelForProofingTasks = new List<ViewModelForProofingTasks>();

            try
            {

                string[] statuses = new string[] { "Completed", "Assigned", "InProgress", "Late", "Completed - Fail", "Completed - Pass" };
                if (viewMode != "Completed")
                    statuses = statuses.Where(x => x != statuses[0] && x != statuses[4] && x != statuses[5]).ToArray();
                else
                    statuses = statuses.Where(x => x == statuses[0] || x == statuses[4] || x == statuses[5]).ToArray();

                viewModelForProofingTasks = (from planUserTask in this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Get().Where(x => statuses.Contains(x.Status))
                                             join task in _unitOfWork.RepositoryAsync<TaskList>().Get() on planUserTask.TaskID equals task.TaskID
                                             //join forminstance in _unitOfWork.RepositoryAsync<FormInstance>().Get() on planUserTask.FormInstanceId equals forminstance.FormInstanceID
                                             join folderVersion in _unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(c => c.FolderVersionID == FolderVersionID) on planUserTask.FolderVersionID equals folderVersion.FolderVersionID
                                             orderby planUserTask.Order descending
                                             select new ViewModelForProofingTasks
                                             {
                                                 // Plan = forminstance.Name,
                                                 //View = planUserTask.ViewID,
                                                 //Section = planUserTask.SectionID,
                                                 Order = planUserTask.Order,
                                                 ID = planUserTask.ID,
                                                 Task = task.TaskDescription,
                                                 Assignment = planUserTask.AssignedUserName,
                                                 Status = planUserTask.Status,
                                                 //Attachments = planUserTask.Attachment,
                                                 StartDate = planUserTask.StartDate,
                                                 DueDate = planUserTask.DueDate,
                                                 Completed = planUserTask.CompletedDate,
                                                 PlanTaskUserMappingDetails = planUserTask.PlanTaskUserMappingDetails,
                                                 EstimatedTime = planUserTask.EstimatedTime,
                                                 ActualTime = planUserTask.ActualTime
                                             }).ToList();
                //.Where(x => x.Status == "Assigned" || x.Status == "InProgress" || x.Status == "Late").
                // viewModelForProofingTasks = GetPlanTaskMappingDetails(viewModelForProofingTasks);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return viewModelForProofingTasks.AsQueryable();
        }

        public List<CommentViewModel> GetComment(int ID, bool isAttachmentsOnly)
        {
            if (ID == 0)
            {
                return new List<CommentViewModel>();
            }


            List<CommentViewModel> taskcommentList = new List<CommentViewModel>();
            if (isAttachmentsOnly)
            {
                taskcommentList = (from comment in this._unitOfWork.RepositoryAsync<TaskComments>().Get().Where(c => !String.IsNullOrEmpty(c.Attachment))
                                   //join status in this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Get()
                                   //on comment.TaskID equals status.ID
                                   join folderVersion in _unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                   on comment.FolderVersionID equals folderVersion.FolderVersionID
                                   where comment.TaskID == ID
                                   orderby comment.AddedDate descending
                                   //.Query.Filter(x => x.TaskID == ID).Get()
                                   select new CommentViewModel
                                   {
                                       TaskID = comment.TaskID,
                                       Comment = comment.Comments,
                                       Datetimestamp = comment.AddedDate,
                                       Status = comment.PlanTaskUserMappingState,
                                       FolderVersionID = comment.FolderVersionID,
                                       Attachment = comment.Attachment,
                                       FolderVersionNumber = folderVersion.FolderVersionNumber,
                                       filename = comment.filename,
                                       AddedBy = comment.AddedBy
                                   }).ToList();
            }
            else
            {
                taskcommentList = (from comment in this._unitOfWork.RepositoryAsync<TaskComments>().Get()
                                   //join status in this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Get()
                                   //on comment.TaskID equals status.ID
                                   join folderVersion in _unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                   on comment.FolderVersionID equals folderVersion.FolderVersionID
                                   where comment.TaskID == ID
                                   orderby comment.AddedDate descending
                                   //.Query.Filter(x => x.TaskID == ID).Get()
                                   select new CommentViewModel
                                   {
                                       TaskID = comment.TaskID,
                                       Comment = comment.Comments,
                                       Datetimestamp = comment.AddedDate,
                                       Status = comment.PlanTaskUserMappingState,
                                       FolderVersionID = comment.FolderVersionID,
                                       Attachment = comment.Attachment,
                                       FolderVersionNumber = folderVersion.FolderVersionNumber,
                                       filename = comment.filename,
                                       AddedBy = comment.AddedBy
                                   }).ToList();
            }
            //taskcommentList = taskcommentList.OrderByDescending(x => x.AddedDate).ToList();
            return taskcommentList;
        }

        public IQueryable<ViewModelForProofingTasks> GetSubwatchQueueList(int FolderVersionID, string viewMode)
        {
            string[] statuses = new string[] { "Completed", "Assigned", "InProgress", "Late", "Completed - Fail", "Completed - Pass" };
            if (viewMode != "Completed")
                statuses = statuses.Where(x => x != statuses[0] && x != statuses[4] && x != statuses[5]).ToArray();
            else
                statuses = statuses.Where(x => x == statuses[0] || x == statuses[4] || x == statuses[5]).ToArray();

            IList<ViewModelForProofingTasks> viewModelForProofingTasks = new List<ViewModelForProofingTasks>();
            try
            {
                viewModelForProofingTasks = (from planUserTask in this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Get().Where(x => statuses.Contains(x.Status))
                                             join task in _unitOfWork.RepositoryAsync<TaskList>().Get() on planUserTask.TaskID equals task.TaskID
                                             //join forminstance in _unitOfWork.RepositoryAsync<FormInstance>().Get() on planUserTask.FormInstanceId equals forminstance.FormInstanceID
                                             join folderVersion in _unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(c => c.FolderVersionID == FolderVersionID) on planUserTask.FolderVersionID equals folderVersion.FolderVersionID
                                             orderby planUserTask.Order descending
                                             select new ViewModelForProofingTasks
                                             {

                                                 //Plan = forminstance.Name,
                                                 //View = planUserTask.ViewID,
                                                 //Section = planUserTask.SectionID,
                                                 Order = planUserTask.Order,
                                                 ID = planUserTask.ID,
                                                 Task = task.TaskDescription,
                                                 Assignment = planUserTask.AssignedUserName,
                                                 Status = planUserTask.Status,
                                                 //Attachments = planUserTask.Attachment,
                                                 StartDate = planUserTask.StartDate,
                                                 DueDate = planUserTask.DueDate,
                                                 Completed = planUserTask.CompletedDate,
                                                 MarkInterested = planUserTask.MarkInterested,
                                                 PlanTaskUserMappingDetails = planUserTask.PlanTaskUserMappingDetails,
                                                 EstimatedTime = planUserTask.EstimatedTime,
                                                 ActualTime = planUserTask.ActualTime
                                             }).ToList();

                //viewModelForProofingTasks = GetPlanTaskMappingDetails(viewModelForProofingTasks);

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return viewModelForProofingTasks.AsQueryable();
        }

        IList<ViewModelForProofingTasks> GetPlanTaskMappingDetails(IList<ViewModelForProofingTasks> viewModelForProofingTasks, bool isDownload)
        {
            try
            {
                if (viewModelForProofingTasks.Count > 0)
                {
                    foreach (ViewModelForProofingTasks task in viewModelForProofingTasks)
                    {
                        if (task.PlanTaskUserMappingDetails != null)
                        {
                            var designDetails = JsonConvert.DeserializeObject<PlanTaskUserMappingDetails>(task.PlanTaskUserMappingDetails);
                            task.Section = designDetails.SectionLabel;
                            task.View = designDetails.FormDesignVersionLabel;
                            task.Plan = designDetails.FormInstanceLabel;
                            task.Comments = getComments(task.MappingRowID, isDownload);
                        }
                        task.Priority = getPriority(task.Order);
                    }
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return viewModelForProofingTasks;
        }
        //public string GetAttchmentNameByName(int attachment)
        //{
        //    var filePath = (from file in this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Get()
        //                    where file.att == mappingRowID
        //                    select file.Attachment).FirstOrDefault();
        //    return filePath;
        //}

    }
}