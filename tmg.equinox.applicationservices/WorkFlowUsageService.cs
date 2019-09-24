using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.DashBoard;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public class WorkFlowUsageService : IWorkFlowUsageService
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        #endregion Private Members

        #region Constructor
        public WorkFlowUsageService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        /// <summary>
        /// This method will return all the states related to the Category. (Each category is associated with workflow)
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        //public List<WorkFlowVersionStatesViewModel> GetAllStatesForFolderVersion(int categoryID, bool isPortfolio)
        //{
        //    List<WorkFlowVersionStatesViewModel> workFlowVersionStatesList = null;
        //    try
        //    {
        //        int accountType = isPortfolio == true ? 1 : 2;
        //        var workFlowVersion = this._unitOfWork.Repository<WorkFlowCategoryMapping>().Query().Filter(c => c.FolderVersionCategoryID == categoryID && c.AccountType == accountType).Get().FirstOrDefault();
        //        if (workFlowVersion != null)
        //        {
        //            workFlowVersionStatesList = (from c in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>()
        //                                                                    .Query()
        //                                                                    .Include(c => c.WorkFlowState)
        //                                                                    .Filter(c => c.WorkFlowVersionID == workFlowVersion.WorkFlowVersionID)
        //                                                                    .Get().OrderBy(c => c.Sequence)
        //                                         select new WorkFlowVersionStatesViewModel
        //                                         {
        //                                             WorkFlowVersionID = c.WorkFlowVersionID,
        //                                             WorkFlowVersionStateID = c.WorkFlowVersionStateID,
        //                                             WFStateID = c.WFStateID,
        //                                             WFStateGroupID = c.WFStateGroupID,
        //                                             WFStateName = c.WorkFlowState.WFStateName
        //                                         }).ToList();
        //        }
        //        if (workFlowVersionStatesList.Count() == 0)
        //            workFlowVersionStatesList = new List<WorkFlowVersionStatesViewModel>();
        //    }
        //    catch (Exception ex)
        //    {
        //        bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
        //        if (reThrow) throw ex;
        //    }
        //    return workFlowVersionStatesList;
        //}

        /// <summary>
        /// Gets the workFlowVersionStateID from Fldr.FolderVersion
        /// </summary>
        /// <param name="folderVersionID"></param>
        /// <returns></returns>
        //public WorkFlowVersionStatesViewModel GetCurrentWorkFlowState(int folderVersionID)
        //{
        //    WorkFlowVersionStatesViewModel workFlowVersionState = null;
        //    try
        //    {
        //        var folderVersion = this._unitOfWork.Repository<FolderVersion>().Query().Filter(c => c.FolderVersionID == folderVersionID).Get().FirstOrDefault();

        //        if (folderVersion.CategoryID != null && folderVersion.CategoryID > 0)
        //        {
        //            workFlowVersionState = (from c in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>()
        //                                                                        .Query()
        //                                                                        .Include(c => c.WorkFlowState)
        //                                                                        .Filter(c => c.WorkFlowVersionStateID == folderVersion.WFStateID)
        //                                                                        .Get()
        //                                    select new WorkFlowVersionStatesViewModel
        //                                    {
        //                                        WorkFlowVersionID = c.WorkFlowVersionID,
        //                                        WorkFlowVersionStateID = c.WorkFlowVersionStateID,
        //                                        WFStateID = c.WFStateID,
        //                                        Sequence = c.Sequence,
        //                                        WFStateName = c.WorkFlowState.WFStateName
        //                                    }).FirstOrDefault();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
        //        if (reThrow) throw ex;
        //    }
        //    return workFlowVersionState;
        //}

        /// <summary>
        /// This method will update the WorkFlowState in the table fldr.FolderVersion
        /// </summary>
        /// <param name="nextWorkFlowVersionStateID"></param>
        //public ServiceResult MoveToWorkFlowState(int folderVersionID, int nextWorkFlowVersionStateID, string updatedBy, string commentText)
        //{
        //    ServiceResult result = new ServiceResult();
        //    var folderVersion = this._unitOfWork.Repository<FolderVersion>().Query().Filter(c => c.FolderVersionID == folderVersionID).Get().FirstOrDefault();
        //    folderVersion.WFStateID = nextWorkFlowVersionStateID;
        //    folderVersion.UpdatedBy = updatedBy;
        //    folderVersion.UpdatedDate = DateTime.Now;
        //    folderVersion.Comments = commentText;
        //    this._unitOfWork.RepositoryAsync<FolderVersion>().Update(folderVersion);
        //    this._unitOfWork.Save();
        //    result.Result = ServiceResultStatus.Success;
        //    return result;
        //}

        /// <summary>
        /// Get current state of the folder version from Fldr.FolderVersion
        /// Get CategoryID from FolderversionID
        /// Get WorkFlowVersionID from CategoryID 
        /// Get the Approval type
        /// based on the success or failure this will return the Next State ID/ID’s
        /// </summary>
        /// <param name="folderVersionID"></param>
        /// <param name="success"></param>
        /// <param name="isAccelerated"></param>
        /// <param name="approvalType"></param>
        /// <returns></returns>
        //public WorkFlowVersionStatesViewModel GetNextWorkFlowState(int folderVersionID, bool success, int workFlowStateApprovalTypeID)
        //{

        //    WorkFlowVersionStatesViewModel workFlowVersionState = null;
        //    int nextWorkFlowStateId = 0;
        //    try
        //    {
        //        var folderVersion = this._unitOfWork.Repository<FolderVersion>().Query().Filter(c => c.FolderVersionID == folderVersionID).Get().FirstOrDefault();
        //        if (folderVersion.CategoryID != null && folderVersion.CategoryID > 0)
        //        {
        //            var currentWorkFlowVersionState = this._unitOfWork.Repository<WorkFlowVersionState>().Query().Filter(c => c.WorkFlowVersionStateID == folderVersion.WFStateID).Get().FirstOrDefault();
        //            var currentWorkFlowVersionStateApprovalType = this._unitOfWork.Repository<WFVersionStatesApprovalType>().Query().Include(c => c.WFStatesApprovalTypeActions).Filter(c => c.WorkFlowVersionStateID == folderVersion.WFStateID && c.WorkFlowStateApprovalTypeID == workFlowStateApprovalTypeID).Get().FirstOrDefault();
        //            //Get Approvaltypes for currentWorkFlowVersionState
        //            var approvalTypeActions = currentWorkFlowVersionStateApprovalType.WFStatesApprovalTypeActions;//unitOfWork.Repository<WFStatesApprovalTypeAction>().Query().Filter(c => c.WFVersionStatesApprovalTypeID == currentWorkFlowVersionStateApprovalType.wf.WFStatesApprovalTypeActionID).Get();
        //            //Get Actions for the Approval Type
        //            if (success)
        //            {
        //                var approvalTypeAction = approvalTypeActions.Where(row => row.ActionID == 2).FirstOrDefault();
        //                if (approvalTypeAction != null)
        //                {
        //                    nextWorkFlowStateId = Convert.ToInt32(approvalTypeAction.ActionResponse);
        //                }
        //            }
        //            else
        //            {
        //                var approvalTypeAction = approvalTypeActions.Where(row => row.ActionID == 3).FirstOrDefault();
        //                if (approvalTypeAction != null)
        //                {
        //                    nextWorkFlowStateId = Convert.ToInt32(approvalTypeAction.ActionResponse);
        //                }
        //            }
        //        }
        //        if (nextWorkFlowStateId != 0)
        //        {
        //            workFlowVersionState = (from c in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>()
        //                                                                .Query()
        //                                                                .Filter(c => c.WorkFlowVersionStateID == nextWorkFlowStateId)
        //                                                                .Get()
        //                                    select new WorkFlowVersionStatesViewModel
        //                                    {
        //                                        WorkFlowVersionID = c.WorkFlowVersionID,
        //                                        WorkFlowVersionStateID = c.WorkFlowVersionStateID,
        //                                        WFStateID = c.WFStateID,
        //                                        Sequence = c.Sequence,
        //                                        WFStateName = c.WorkFlowState.WFStateName
        //                                    }).FirstOrDefault();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
        //        if (reThrow) throw ex;
        //    }
        //    return workFlowVersionState;
        //}

        /// <summary>
        /// /Return List of assigned user roles for WorkflowVersionStateID
        /// </summary>
        /// <param name="workFlowVersionStateID"></param>
        /// <returns></returns>
        //public List<WorkFlowVersionStatesAccessViewModel> GetWorkFlowStateUserRoles(int workFlowVersionStateID)
        //{
        //    List<WorkFlowVersionStatesAccessViewModel> workFlowVersionStatesAccessViewModelList = null;
        //    try
        //    {
        //        workFlowVersionStatesAccessViewModelList = (from c in this._unitOfWork.RepositoryAsync<WorkFlowVersionStatesAccess>()
        //                                                                .Query()
        //                                                                .Filter(c => c.WorkFlowVersionStateID == workFlowVersionStateID)
        //                                                                .Get()
        //                                                    select new WorkFlowVersionStatesAccessViewModel
        //                                                    {
        //                                                        WorkFlowVersionStatesAccessID = c.WorkFlowVersionStatesAccessID,
        //                                                        WorkFlowVersionStateID = c.WorkFlowVersionStateID,
        //                                                        RoleID = c.RoleID,
        //                                                        RoleName = c.UserRole.Name
        //                                                    }).ToList();

        //        if (workFlowVersionStatesAccessViewModelList.Count() == 0)
        //            workFlowVersionStatesAccessViewModelList = new List<WorkFlowVersionStatesAccessViewModel>();
        //    }
        //    catch (Exception ex)
        //    {
        //        bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
        //        if (reThrow) throw ex;
        //    }
        //    return workFlowVersionStatesAccessViewModelList;
        //}

        //public void GetWorkFlowNotificationDetails(int workFlowApprovalTypeID)
        //{
        //    //Returns the Email notification details for a workFlowApprovalTypeID (what we would be doing here is say a State is “Approved” then an email notification will be sent to the email ID/s mentioned in the configuration that – 
        //    //For XXX Account and XXX Folder and XXX Folderversion, the status has been “Approved” and moved from “Current” 
        //    //“Approved” is the workFlowApprovalTypeID
        //    //“Current” is the workFlowVersionStateID that the folder version is currently in
        //    List<WFStatesApprovalTypeActionViewModel> wFStatesApprovalTypeActionList = null;
        //    try
        //    {
        //        wFStatesApprovalTypeActionList = (from c in this._unitOfWork.RepositoryAsync<WorkFlowVersionStatesAccess>()
        //                                                                .Query()
        //                                                                .Filter(c => c.WorkFlowVersionStateID == workFlowApprovalTypeID)
        //                                                                .Get()
        //                                          select new WFStatesApprovalTypeActionViewModel
        //                                          {
        //                                              //WorkFlowVersionStatesAccessID = c.WorkFlowVersionStatesAccessID,
        //                                              //WorkFlowVersionStateID = c.WorkFlowVersionStateID,
        //                                              //RoleID = c.RoleID,
        //                                              //RoleName = c.UserRole.Name
        //                                          }).ToList();

        //        if (wFStatesApprovalTypeActionList.Count() == 0)
        //            wFStatesApprovalTypeActionList = new List<WFStatesApprovalTypeActionViewModel>();
        //    }
        //    catch (Exception ex)
        //    {
        //        bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
        //        if (reThrow) throw ex;
        //    }
        //    //return wFStatesApprovalTypeActionList;
        //}

        /// <summary>
        /// Returns the Approval types for that workFlowVersionStateID
        /// </summary>
        /// <param name="workFlowVersionStateID"></param>
        /// <returns></returns>
        //public List<WFVersionStatesApprovalTypeViewModel> GetWorkFlowStateApprovalTypes(int workFlowVersionStateID)
        //{
        //    List<WFVersionStatesApprovalTypeViewModel> wFVersionStatesApprovalTypeViewModelList = null;
        //    try
        //    {
        //        wFVersionStatesApprovalTypeViewModelList = (from c in this._unitOfWork.RepositoryAsync<WFVersionStatesApprovalType>()
        //                                                                .Query()
        //                                                                .Filter(c => c.WorkFlowVersionStateID == workFlowVersionStateID)
        //                                                                .Get()
        //                                                    select new WFVersionStatesApprovalTypeViewModel
        //                                                    {
        //                                                        WFVersionStatesApprovalTypeID = c.WFVersionStatesApprovalTypeID,
        //                                                        WorkFlowStateApprovalTypeID = c.WorkFlowStateApprovalTypeID,
        //                                                        WorkFlowVersionStateID = c.WorkFlowVersionStateID,
        //                                                        WorkFlowStateApprovalTypeName = c.WorkFlowStateApprovalTypeMaster.WorkFlowStateApprovalTypeName
        //                                                    }).ToList();

        //        if (wFVersionStatesApprovalTypeViewModelList.Count() == 0)
        //            wFVersionStatesApprovalTypeViewModelList = new List<WFVersionStatesApprovalTypeViewModel>();
        //    }
        //    catch (Exception ex)
        //    {
        //        bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
        //        if (reThrow) throw ex;
        //    }
        //    return wFVersionStatesApprovalTypeViewModelList;
        //}
    }
}
