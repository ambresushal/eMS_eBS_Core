using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public class WorkFlowMasterService : IWorkFlowMasterService
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        #endregion Private Members

        #region Constructor
        public WorkFlowMasterService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        public List<WorkFlowStateMasterViewModel> GetWorkFlowStateMasterList(int tenantID)
        {
            List<WorkFlowStateMasterViewModel> workFlowStateMasterList = null;
            try
            {
                workFlowStateMasterList = (from c in this._unitOfWork.RepositoryAsync<WorkFlowStateMaster>()
                                                                        .Query()
                                                                        .Filter(c => c.TenantID == tenantID && c.IsActive == true)
                                                                        .Get()
                                           select new WorkFlowStateMasterViewModel
                                           {
                                               WFStateID = c.WFStateID,
                                               WFStateName = c.WFStateName
                                           }).ToList();

                if (workFlowStateMasterList.Count() == 0)
                    workFlowStateMasterList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return workFlowStateMasterList;
        }

        public ServiceResult AddWorkFlowStateMaster(int tenantID, string wFStateName, string addedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                WorkFlowStateMaster workFlowStateMaster = this._unitOfWork.RepositoryAsync<WorkFlowStateMaster>()
                                .Query().Filter(a => a.WFStateName == wFStateName && a.TenantID == tenantID && a.IsActive == true).Get().FirstOrDefault();

                if (workFlowStateMaster == null)
                {
                    WorkFlowStateMaster workFlowStateMasterToAdd = new WorkFlowStateMaster();
                    workFlowStateMasterToAdd.WFStateName = wFStateName;
                    workFlowStateMasterToAdd.TenantID = tenantID;
                    workFlowStateMasterToAdd.AddedBy = addedBy;
                    workFlowStateMasterToAdd.AddedDate = DateTime.Now;
                    workFlowStateMasterToAdd.UpdatedBy = null;
                    workFlowStateMasterToAdd.UpdatedDate = null;
                    workFlowStateMasterToAdd.IsActive = true;

                    this._unitOfWork.RepositoryAsync<WorkFlowStateMaster>().Insert(workFlowStateMasterToAdd);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "Workflow state already exists" } });
                    result.Items = items;
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

        public ServiceResult UpdateWorkFlowStateMaster(int wFStateID, string wFStateName, string updatedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                var finalizedWorkflowsUsingState = from c in this._unitOfWork.RepositoryAsync<WorkFlowCategoryMapping>()
                                                                        .Query()
                                                                        .Include(c => c.WorkFlowVersionStates)
                                                                        .Filter(c => c.WorkFlowVersionStates.Any(d => d.WFStateID == wFStateID) && c.IsFinalized == true)
                                                                        .Get()
                                                   select c;
                if (finalizedWorkflowsUsingState != null && finalizedWorkflowsUsingState.Count() > 0)
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "Workflow State can not be edited as it is used in finalized workflow" } });
                    result.Items = items;
                    result.Result = ServiceResultStatus.Failure;
                }
                else
                {
                    WorkFlowStateMaster existWorkFlowVersionStates = this._unitOfWork.RepositoryAsync<WorkFlowStateMaster>()
                                    .Query().Filter(a => a.WFStateName == wFStateName && a.IsActive == true).Get().FirstOrDefault();

                    if (existWorkFlowVersionStates == null)
                    {

                        WorkFlowStateMaster workFlowStateMaster = this._unitOfWork.RepositoryAsync<WorkFlowStateMaster>().Find(wFStateID);
                        workFlowStateMaster.WFStateName = wFStateName;
                        workFlowStateMaster.UpdatedBy = updatedBy;
                        workFlowStateMaster.UpdatedDate = DateTime.Now;
                        this._unitOfWork.RepositoryAsync<WorkFlowStateMaster>().Update(workFlowStateMaster);
                        this._unitOfWork.Save();
                        result.Result = ServiceResultStatus.Success;
                    }
                    else
                    {
                        items.Add(new ServiceResultItem() { Messages = new string[] { "Workflow state already exists" } });
                        result.Items = items;
                        result.Result = ServiceResultStatus.Failure;
                    }
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

        public ServiceResult DeleteWorkFlowStateMaster(int wFStateID, string updatedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                var finalizedWorkflowsUsingState = from c in this._unitOfWork.RepositoryAsync<WorkFlowCategoryMapping>()
                                                                        .Query()
                                                                        .Include(c => c.WorkFlowVersionStates)
                                                                        .Filter(c => c.WorkFlowVersionStates.Any(d => d.WFStateID == wFStateID) && c.IsFinalized == true)
                                                                        .Get()
                                                   select c;
                if (finalizedWorkflowsUsingState != null && finalizedWorkflowsUsingState.Count() > 0)
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "Workflow State can not be deleted as it is used in finalized workflow" } });
                    result.Items = items;
                    result.Result = ServiceResultStatus.Failure;
                }
                else
                {
                    List<WorkFlowVersionState> workFlowVersionStates = this._unitOfWork.Repository<WorkFlowVersionState>()
                                    .Query().Filter(a => a.WFStateID == wFStateID).Include(e => e.WorkFlowVersionStatesAccess).Include(e => e.WFVersionStatesApprovalType).Include(e => e.WFVersionStatesApprovalType.Select(s => s.WFStatesApprovalTypeActions)).Get().ToList();
                    if (workFlowVersionStates != null && workFlowVersionStates.Count() > 0)
                    {
                        foreach (var state in workFlowVersionStates)
                        {
                            foreach (var userAccess in state.WorkFlowVersionStatesAccess.ToList())
                            {
                                this._unitOfWork.Repository<WorkFlowVersionStatesAccess>().Delete(userAccess);
                            }
                            foreach (var approvalType in state.WFVersionStatesApprovalType.ToList())
                            {
                                foreach (var approvalTypeActions in approvalType.WFStatesApprovalTypeActions.ToList())
                                {
                                    this._unitOfWork.Repository<WFStatesApprovalTypeAction>().Delete(approvalTypeActions);
                                }
                                this._unitOfWork.Repository<WFVersionStatesApprovalType>().Delete(approvalType);
                            }
                            this._unitOfWork.Repository<WorkFlowVersionState>().Delete(state);
                        }
                    }
                    //--
                    WorkFlowStateMaster workFlowStateMaster = this._unitOfWork.RepositoryAsync<WorkFlowStateMaster>().Find(wFStateID);
                    workFlowStateMaster.IsActive = false;
                    workFlowStateMaster.UpdatedBy = updatedBy;
                    workFlowStateMaster.UpdatedDate = DateTime.Now;
                    this._unitOfWork.RepositoryAsync<WorkFlowStateMaster>().Update(workFlowStateMaster);
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

        public List<WorkFlowStateApprovalTypeMasterViewModel> GetWorkFlowStateApprovalTypeMasterList(int tenantID)
        {
            List<WorkFlowStateApprovalTypeMasterViewModel> workFlowStateApprovalTypeMasterList = null;
            try
            {
                workFlowStateApprovalTypeMasterList = (from c in this._unitOfWork.RepositoryAsync<WorkFlowStateApprovalTypeMaster>()
                                                                        .Query()
                                                                        .Filter(c => c.TenantID == tenantID && c.IsActive == true)
                                                                        .Get()
                                                       select new WorkFlowStateApprovalTypeMasterViewModel
                                                       {
                                                           WorkFlowStateApprovalTypeID = c.WorkFlowStateApprovalTypeID,
                                                           WorkFlowStateApprovalTypeName = c.WorkFlowStateApprovalTypeName
                                                       }).ToList();

                if (workFlowStateApprovalTypeMasterList.Count() == 0)
                    workFlowStateApprovalTypeMasterList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return workFlowStateApprovalTypeMasterList;
        }

        public ServiceResult AddWorkFlowStateApprovalTypeMaster(int tenantID, string workFlowStateApprovalTypeName, string addedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                WorkFlowStateApprovalTypeMaster workFlowStateApprovalTypeMaster = this._unitOfWork.RepositoryAsync<WorkFlowStateApprovalTypeMaster>()
                                .Query().Filter(a => a.TenantID == tenantID && a.WorkFlowStateApprovalTypeName == workFlowStateApprovalTypeName && a.IsActive == true).Get().FirstOrDefault();

                if (workFlowStateApprovalTypeMaster == null)
                {
                    WorkFlowStateApprovalTypeMaster workFlowStateApprovalTypeMasterToAdd = new WorkFlowStateApprovalTypeMaster();
                    workFlowStateApprovalTypeMasterToAdd.WorkFlowStateApprovalTypeName = workFlowStateApprovalTypeName;
                    workFlowStateApprovalTypeMasterToAdd.TenantID = tenantID;
                    workFlowStateApprovalTypeMasterToAdd.AddedBy = addedBy;
                    workFlowStateApprovalTypeMasterToAdd.AddedDate = DateTime.Now;
                    workFlowStateApprovalTypeMasterToAdd.UpdatedBy = null;
                    workFlowStateApprovalTypeMasterToAdd.UpdatedDate = null;
                    workFlowStateApprovalTypeMasterToAdd.IsActive = true;

                    this._unitOfWork.RepositoryAsync<WorkFlowStateApprovalTypeMaster>().Insert(workFlowStateApprovalTypeMasterToAdd);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "Approval type already exists" } });
                    result.Items = items;
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

        public ServiceResult UpdateWorkFlowStateApprovalTypeMaster(int workFlowStateApprovalTypeID, string workFlowStateApprovalTypeName, string updatedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                WorkFlowStateApprovalTypeMaster existWorkFlowStateApprovalTypeMaster = this._unitOfWork.RepositoryAsync<WorkFlowStateApprovalTypeMaster>()
                                .Query().Filter(a => a.WorkFlowStateApprovalTypeName == workFlowStateApprovalTypeName && a.IsActive == true).Get().FirstOrDefault();
                //---------------
                var finalizedWorkflowsUsingState = from c in this._unitOfWork.RepositoryAsync<WorkFlowCategoryMapping>()
                                                                        .Query()
                                                                        .Include(c => c.WorkFlowVersionStates)
                                                                        .Filter(c => c.WorkFlowVersionStates.Any(d => d.WFVersionStatesApprovalType.Any(e => e.WorkFlowStateApprovalTypeID == workFlowStateApprovalTypeID) && c.IsFinalized == true))
                                                                        .Get()
                                                   select c;
                if (finalizedWorkflowsUsingState != null && finalizedWorkflowsUsingState.Count() > 0)
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "Workflow State Approval Type can not be edited as it is used in finalized workflow" } });
                    result.Items = items;
                    result.Result = ServiceResultStatus.Failure;
                }
                else
                {
                    if (existWorkFlowStateApprovalTypeMaster == null)
                    {

                        WorkFlowStateApprovalTypeMaster workFlowStateApprovalTypeMaster = this._unitOfWork.RepositoryAsync<WorkFlowStateApprovalTypeMaster>().Find(workFlowStateApprovalTypeID);
                        workFlowStateApprovalTypeMaster.WorkFlowStateApprovalTypeName = workFlowStateApprovalTypeName;
                        workFlowStateApprovalTypeMaster.UpdatedBy = updatedBy;
                        workFlowStateApprovalTypeMaster.UpdatedDate = DateTime.Now;
                        this._unitOfWork.RepositoryAsync<WorkFlowStateApprovalTypeMaster>().Update(workFlowStateApprovalTypeMaster);
                        this._unitOfWork.Save();
                        result.Result = ServiceResultStatus.Success;
                    }
                    else
                    {
                        items.Add(new ServiceResultItem() { Messages = new string[] { "Approval type already exists" } });
                        result.Items = items;
                        result.Result = ServiceResultStatus.Failure;
                    }
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

        public ServiceResult DeleteWorkFlowStateApprovalTypeMaster(int workFlowStateApprovalTypeID, string updatedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                var finalizedWorkflowsUsingState = from c in this._unitOfWork.RepositoryAsync<WorkFlowCategoryMapping>()
                                                                        .Query()
                                                                        .Include(c => c.WorkFlowVersionStates)
                                                                        .Filter(c => c.WorkFlowVersionStates.Any(d => d.WFVersionStatesApprovalType.Any(e => e.WorkFlowStateApprovalTypeID == workFlowStateApprovalTypeID) && c.IsFinalized == true))
                                                                        .Get()
                                                   select c;
                if (finalizedWorkflowsUsingState != null && finalizedWorkflowsUsingState.Count() > 0)
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "Workflow State Approval Type can not be deleted as it is used in finalized workflow" } });
                    result.Items = items;
                    result.Result = ServiceResultStatus.Failure;
                }
                else
                {
                    List<WFVersionStatesApprovalType> workFlowVersionStatesApprovalTypes = this._unitOfWork.Repository<WFVersionStatesApprovalType>()
                                    .Query().Filter(a => a.WorkFlowStateApprovalTypeID == workFlowStateApprovalTypeID).Include(e => e.WFStatesApprovalTypeActions).Get().ToList();
                    if (workFlowVersionStatesApprovalTypes != null && workFlowVersionStatesApprovalTypes.Count() > 0)
                    {
                        foreach (var approvalType in workFlowVersionStatesApprovalTypes)
                        {
                            foreach (var approvalTypeActions in approvalType.WFStatesApprovalTypeActions.ToList())
                            {
                                this._unitOfWork.Repository<WFStatesApprovalTypeAction>().Delete(approvalTypeActions);
                            }
                            this._unitOfWork.Repository<WFVersionStatesApprovalType>().Delete(approvalType);
                        }
                    }
                    
                    WorkFlowStateApprovalTypeMaster workFlowStateApprovalTypeMaster = this._unitOfWork.RepositoryAsync<WorkFlowStateApprovalTypeMaster>().Find(workFlowStateApprovalTypeID);
                    workFlowStateApprovalTypeMaster.IsActive = false;
                    workFlowStateApprovalTypeMaster.UpdatedBy = updatedBy;
                    workFlowStateApprovalTypeMaster.UpdatedDate = DateTime.Now;
                    this._unitOfWork.RepositoryAsync<WorkFlowStateApprovalTypeMaster>().Update(workFlowStateApprovalTypeMaster);
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

        public List<WorkFlowStateMasterViewModel> GetWorkFlowStateListGreaterThanSelected(int tenantID, int wfStateId, int folderVersionId = 0)
        {
            List<WorkFlowStateMasterViewModel> workFlowStateMasterList = null;
            try
            {
                int categoryId = 0;
                var folderVersionRec = from c in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                                           .Query()
                                                                           .Filter(c => c.FolderVersionID == folderVersionId)
                                                                           .Get()
                                    select c;
                if(folderVersionRec != null && folderVersionRec.Count() > 0)
                {
                    categoryId = (Int32)folderVersionRec.FirstOrDefault().CategoryID;
                }
                                    
            
                WorkFlowVersionState worlkFlowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>()
                               .Query().Filter(a => a.WorkFlowVersionStateID == wfStateId).Get().FirstOrDefault();
                if (worlkFlowState != null)
                {
                    //var worlkFlowStateId = 
                    workFlowStateMasterList = (from c in this._unitOfWork.RepositoryAsync<WorkFlowStateMaster>()
                                                                            .Query()
                                                                            .Filter(c => c.TenantID == tenantID && c.IsActive == true) // && c.WFStateID >= worlkFlowState.WFStateID)
                                                                            .Get()
                                               select new WorkFlowStateMasterViewModel
                                               {
                                                   WFStateID = c.WFStateID,
                                                   WFStateName = c.WFStateName
                                               }).ToList();
                }
                var selectedworkFlowState = (from c in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>()
                               .Query().Filter(a => a.WorkFlowCategoryMapping.FolderVersionCategoryID == categoryId && a.WorkFlowVersionStateID >= worlkFlowState.WorkFlowVersionStateID).Get()
                                             select c.WFStateID).ToList();

                workFlowStateMasterList = workFlowStateMasterList.Where(row => selectedworkFlowState.Contains(row.WFStateID)).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return workFlowStateMasterList;
        }
    }
}
