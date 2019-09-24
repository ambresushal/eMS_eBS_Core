using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository.extensions
{
    public static class WorkFlowVersionStateRepository
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        #endregion Constructor

        #region Public Methods

        public static WorkFlowVersionState GetFirstWorkflowState(this IRepositoryAsync<WorkFlowVersionState> workflowstateRepository, IUnitOfWork unitOfWork, int categoryID, bool isPortfolio)
        {
            WorkFlowVersionState workflowVersionState = null;
            var workFlowVersionId = unitOfWork.Repository<WorkFlowCategoryMapping>()
                                                        .Query()
                                                        .Filter(c => c.FolderVersionCategoryID == categoryID && c.AccountType == (isPortfolio == true ? 1 : 2))
                                                        .Get()
                                                        .Select(c => c.WorkFlowVersionID)
                                                        .FirstOrDefault();
            if (workFlowVersionId > 0)
            {
                workflowVersionState = workflowstateRepository
                                                        .Query()
                                                        .Filter(c => c.WorkFlowVersionID == workFlowVersionId)
                                                        .Get()
                                                        .OrderBy(c => c.Sequence)
                                                        .FirstOrDefault();
            }
            return workflowVersionState;

        }

        public static WorkFlowVersionState GetNextWorkflowState(this IRepositoryAsync<WorkFlowVersionState> workflowstateRepository, IUnitOfWork unitOfWork, int folderVersionId, int workflowStateId, int workFlowStateApprovalTypeID, bool success = true)
        {
            WorkFlowVersionState workFlowVersionState = null;
            int nextWorkFlowStateId = 0;
            try
            {
                var folderVersion = unitOfWork.Repository<FolderVersion>().Query().Filter(c => c.FolderVersionID == folderVersionId).Get().FirstOrDefault();
                if (folderVersion.CategoryID != null && folderVersion.CategoryID > 0)
                {
                    WorkFlowVersionState currentWorkFlowVersionState = null;
                    if (workflowStateId > 0)
                    {
                        currentWorkFlowVersionState = unitOfWork.Repository<WorkFlowVersionState>().Query().Filter(c => c.WorkFlowVersionStateID == workflowStateId).Get().FirstOrDefault();
                    }
                    else
                        currentWorkFlowVersionState = unitOfWork.Repository<WorkFlowVersionState>().Query().Filter(c => c.WorkFlowVersionStateID == folderVersion.WFStateID).Get().FirstOrDefault();
                    var currentWorkFlowVersionStateApprovalType = unitOfWork.Repository<WFVersionStatesApprovalType>().Query().Include(c => c.WFStatesApprovalTypeActions).Filter(c => c.WorkFlowVersionStateID == currentWorkFlowVersionState.WorkFlowVersionStateID && c.WorkFlowStateApprovalTypeID == workFlowStateApprovalTypeID).Get().FirstOrDefault();
                    //Get Approvaltypes for currentWorkFlowVersionState
                    var approvalTypeActions = currentWorkFlowVersionStateApprovalType.WFStatesApprovalTypeActions;//unitOfWork.Repository<WFStatesApprovalTypeAction>().Query().Filter(c => c.WFVersionStatesApprovalTypeID == currentWorkFlowVersionStateApprovalType.wf.WFStatesApprovalTypeActionID).Get();
                    //Get Actions for the Approval Type
                    if (success)
                    {
                        var approvalTypeAction = approvalTypeActions.Where(row => row.ActionID == 2).FirstOrDefault();
                        if (approvalTypeAction != null)
                        {
                            nextWorkFlowStateId = Convert.ToInt32(approvalTypeAction.ActionResponse);
                        }
                    }
                    else
                    {
                        var approvalTypeAction = approvalTypeActions.Where(row => row.ActionID == 3).FirstOrDefault();
                        if (approvalTypeAction != null)
                        {
                            nextWorkFlowStateId = Convert.ToInt32(approvalTypeAction.ActionResponse);
                        }
                    }
                }
                if (nextWorkFlowStateId != 0)
                {
                    workFlowVersionState = unitOfWork.Repository<WorkFlowVersionState>()
                                                                        .Query()
                                                                        .Filter(c => c.WorkFlowVersionStateID == nextWorkFlowStateId)
                                                                        .Get().FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                // bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                //if (reThrow) throw ex;
            }
            return workFlowVersionState;

        }

        //public static WorkFlowVersionState GetNotApprovedWorkflowState(this IRepositoryAsync<WorkFlowVersionState> workflowstateRepository, IUnitOfWork unitOfWork, int tenantId, int workflowStateId)
        //{
        //    int? wfStateID;
        //    wfStateID = unitOfWork.Repository<WorkFlowVersionState>()
        //                                                .Query()
        //                                                //.Filter(c => c.WFStateID == workflowStateId && c.TenantID == tenantId && c.IsActive == true)
        //                                                .Get()
        //                                                //.Select(c => c.NotApprovedWFStateID)
        //                                                .FirstOrDefault();

        //    return workflowstateRepository
        //            .Query()
        //            .Filter(c => c.WFStateID == wfStateID)
        //            .Get()
        //            .FirstOrDefault();

        //}

        /// <summary>
        /// SH Updated the method to retrieve Released Workflow state.
        /// </summary>
        /// <param name="workflowstateRepository"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static WorkFlowVersionState GetReleasedWorkflowState(this IRepositoryAsync<WorkFlowVersionState> workflowstateRepository, IUnitOfWork unitOfWork, int folderVersionId)
        {
            var folderVersion = unitOfWork.Repository<FolderVersion>()
                                                        .Query().Include(c => c.Folder)
                                                        .Filter(c => c.FolderVersionID == folderVersionId && c.IsActive == true)
                                                        .Get()
                                                        .FirstOrDefault();
            WorkFlowVersionState workflowVersionState = null;
            var workFlowVersionId = unitOfWork.Repository<WorkFlowCategoryMapping>()
                                                        .Query()
                                                        .Filter(c => c.FolderVersionCategoryID == folderVersion.CategoryID && c.AccountType == (folderVersion.Folder.IsPortfolio == true ? 1 : 2))
                                                        .Get()
                                                        .Select(c => c.WorkFlowVersionID)
                                                        .FirstOrDefault();
            if (workFlowVersionId > 0)
            {
                workflowVersionState = workflowstateRepository
                                                        .Query()
                                                        .Filter(c => c.WorkFlowVersionID == workFlowVersionId)
                                                        .Get()
                                                        .OrderByDescending(c => c.Sequence)
                                                        .FirstOrDefault();
            }
            return workflowVersionState;

        }

        public static int? GetWorkFlowStateGroupId(this IRepositoryAsync<WorkFlowVersionState> workflowstateRepository, int tenantId, int workflowStateId)
        {
            //For now the GroupID coulmn is not used so returning as null, when we implement the parallel workflows we may need this.
            return workflowstateRepository
                    .Query()
                    .Filter(c => c.WorkFlowVersionStateID == workflowStateId)
                    .Get()
                    .Select(g => g.WFStateGroupID)
                    .FirstOrDefault();
        }

        public static int GetWorkflowStatesCount(this IRepositoryAsync<WorkFlowVersionState> workflowstateRepository, IUnitOfWork unitOfWork, int folderVersionId)
        {
            var folderVersion = unitOfWork.Repository<FolderVersion>()
                                                        .Query().Include(c => c.Folder)
                                                        .Filter(c => c.FolderVersionID == folderVersionId && c.IsActive == true)
                                                        .Get()
                                                        .FirstOrDefault();
            int workflowVersionStateCount = 0;
            var workFlowVersionId = unitOfWork.Repository<WorkFlowCategoryMapping>()
                                                        .Query()
                                                        .Filter(c => c.FolderVersionCategoryID == folderVersion.CategoryID && c.AccountType == (folderVersion.Folder.IsPortfolio == true ? 1 : 2))
                                                        .Get()
                                                        .Select(c => c.WorkFlowVersionID)
                                                        .FirstOrDefault();
            if (workFlowVersionId > 0)
            {
                workflowVersionStateCount = workflowstateRepository
                                                        .Query()
                                                        .Filter(c => c.WorkFlowVersionID == workFlowVersionId)
                                                        .Get()
                                                        .ToList()
                   .Count();
            }
            return workflowVersionStateCount;
        }

        public static WorkFlowVersionState GetNextStateAfterParallelWorkflowState(this IRepositoryAsync<WorkFlowVersionState> workflowstateRepository, IUnitOfWorkAsync unitOfWork, int tenantId, int workflowStateGroupId, int workFlowStateID)
        {
            int sequence;
            var currentWorkFlowState = unitOfWork.RepositoryAsync<WorkFlowVersionState>().Query().Filter(c => c.WorkFlowVersionStateID == workFlowStateID).Get().FirstOrDefault();
            sequence = unitOfWork.RepositoryAsync<WorkFlowVersionState>()
                                                        .Query()
                                                        .Filter(c => c.WFStateGroupID == workflowStateGroupId && c.WorkFlowVersionID == currentWorkFlowState.WorkFlowVersionID)
                                                        .Get()
                                                        .Select(c => c.Sequence)
                                                        .Max();
            sequence = sequence + 1;
            return workflowstateRepository
                    .Query()
                     .Filter(c => c.Sequence == sequence && c.WorkFlowVersionID == currentWorkFlowState.WorkFlowVersionID)
                    .Get()
                    .FirstOrDefault();
        }

        /// <summary>
        /// This method will return all the states related to the Category. (Each category is associated with workflow)
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public static List<WorkFlowVersionState> GetAllStatesForFolderVersion(this IRepositoryAsync<WorkFlowVersionState> workflowstateRepository, IUnitOfWorkAsync unitOfWork, int categoryID, bool isPortfolio)
        {
            int accountType = isPortfolio == true ? 1 : 2;
            var workFlowVersion = unitOfWork.Repository<WorkFlowCategoryMapping>().Query().Filter(c => c.FolderVersionCategoryID == categoryID && c.AccountType == accountType).Get().FirstOrDefault();

            return workflowstateRepository
                .Query()
                .Include(c => c.WorkFlowState)
                .Filter(c => c.WorkFlowVersionID == workFlowVersion.WorkFlowVersionID)
                .Get()
                .OrderBy(c => c.Sequence)
                .ToList();
        }

        public static WorkFlowVersionState GetNextWorkflowStateBySequence(this IRepositoryAsync<WorkFlowVersionState> workflowstateRepository, IUnitOfWork unitOfWork, int folderVersionId, int workflowStateId, int workFlowStateApprovalTypeID, bool success = true)
        {
            WorkFlowVersionState workFlowVersionState = null;

            try
            {
                var folderVersion = unitOfWork.Repository<FolderVersion>().Query().Filter(c => c.FolderVersionID == folderVersionId).Get().FirstOrDefault();
                if (folderVersion.CategoryID != null && folderVersion.CategoryID > 0)
                {
                    WorkFlowVersionState currentWorkFlowVersionState = null;
                    if (workflowStateId > 0)
                    {
                        currentWorkFlowVersionState = unitOfWork.Repository<WorkFlowVersionState>().Query().Filter(c => c.WorkFlowVersionStateID == workflowStateId).Get().FirstOrDefault();
                    }
                    else
                        currentWorkFlowVersionState = unitOfWork.Repository<WorkFlowVersionState>().Query().Filter(c => c.WorkFlowVersionStateID == folderVersion.WFStateID).Get().FirstOrDefault();

                    workFlowVersionState = unitOfWork.Repository<WorkFlowVersionState>()
                                                                        .Query()
                                                                        .Filter(c => c.WorkFlowVersionID == currentWorkFlowVersionState.WorkFlowVersionID && c.WorkFlowVersionStateID > currentWorkFlowVersionState.WorkFlowVersionStateID)
                                                                       .Get().FirstOrDefault();
                }
            }
            catch (Exception ex)
            {

            }
            return workFlowVersionState;

        }


        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
