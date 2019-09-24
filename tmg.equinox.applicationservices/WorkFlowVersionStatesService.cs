using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public class WorkFlowVersionStatesService : IWorkFlowVersionStatesService
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        #endregion Private Members

        #region Constructor
        public WorkFlowVersionStatesService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workFlowVersionStatesID"></param>
        /// <returns></returns>
        public List<WorkFlowVersionStatesViewModel> GetWorkFlowVersionStatesList(int workFlowVersionID)
        {
            List<WorkFlowVersionStatesViewModel> workFlowVersionStatesList = null;
            try
            {
                workFlowVersionStatesList = (from c in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>()
                                                                        .Query()
                                                                        .Include(c => c.WorkFlowState)
                                                                        .Filter(c => c.WorkFlowVersionID == workFlowVersionID)
                                                                        .Get()
                                             select new WorkFlowVersionStatesViewModel
                                               {
                                                   WorkFlowVersionID = c.WorkFlowVersionID,
                                                   WorkFlowVersionStateID = c.WorkFlowVersionStateID,
                                                   WFStateID = c.WFStateID,
                                                   WFStateName = c.WorkFlowState.WFStateName,
                                                   Sequence = c.Sequence,
                                                   WFStateGroupID = c.WFStateGroupID
                                               }).ToList();

                if (workFlowVersionStatesList.Count() == 0)
                    workFlowVersionStatesList = new List<WorkFlowVersionStatesViewModel>();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return workFlowVersionStatesList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantID"></param>
        /// <param name="workFlowVersionID"></param>
        /// <param name="wFStateID"></param>
        /// <param name="sequence"></param>
        /// <param name="addedBy"></param>
        /// <returns></returns>
        public ServiceResult AddWorkFlowVersionStates(int tenantID, int workFlowVersionID, int wFStateID, int sequence, string addedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                WorkFlowVersionState workFlowVersionStates = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>()
                                .Query().Filter(a => a.WorkFlowVersionID == workFlowVersionID && a.WFStateID == wFStateID).Get().FirstOrDefault();
                //Get max Sequence Order
                var maxOrderState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Query().Filter(a => a.WorkFlowVersionID == workFlowVersionID).Get().ToList().OrderByDescending(a => a.Sequence).FirstOrDefault();
                int stateSequence = 1;
                if (maxOrderState != null)
                {
                    stateSequence = maxOrderState.Sequence + 1;
                }
                if (workFlowVersionStates == null)
                {
                    WorkFlowVersionState workFlowVersionStatesToAdd = new WorkFlowVersionState();
                    workFlowVersionStatesToAdd.WorkFlowVersionID = workFlowVersionID;
                    workFlowVersionStatesToAdd.WFStateID = wFStateID;
                    workFlowVersionStatesToAdd.AddedBy = addedBy;
                    workFlowVersionStatesToAdd.AddedDate = DateTime.Now;
                    workFlowVersionStatesToAdd.UpdatedBy = null;
                    workFlowVersionStatesToAdd.UpdatedDate = null;
                    workFlowVersionStatesToAdd.Sequence = stateSequence;
                    workFlowVersionStatesToAdd.WFStateGroupID = null;
                    this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Insert(workFlowVersionStatesToAdd);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workFlowVersionStatesID"></param>
        /// <param name="wFStateID"></param>
        /// <param name="sequence"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        public ServiceResult UpdateWorkFlowVersionStates(int workFlowVersionStatesID, int wFStateID, int sequence, string updatedBy, int? wFStateGroupID)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                WorkFlowVersionState existworkFlowVersionState = (this._unitOfWork.RepositoryAsync<WorkFlowVersionState>()
                            .Query().Filter(a => a.WorkFlowVersionStateID == workFlowVersionStatesID).Get().FirstOrDefault());

                if (existworkFlowVersionState != null)
                {
                    existworkFlowVersionState.Sequence = sequence;
                    existworkFlowVersionState.WFStateID = wFStateID;
                    if (updatedBy != null) //null comes when its internal update for the WFStateGroupID
                    {
                        existworkFlowVersionState.UpdatedBy = updatedBy;
                        existworkFlowVersionState.UpdatedDate = DateTime.Now;
                    }
                    if (wFStateGroupID != null) //null comes when its internal update for the WFStateGroupID
                    {
                        existworkFlowVersionState.WFStateGroupID = wFStateGroupID;
                    }
                    this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Update(existworkFlowVersionState);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workFlowVersionStatesID"></param>
        /// <returns></returns>
        public ServiceResult DeleteWorkFlowVersionStates(int workFlowVersionStatesID)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                WorkFlowVersionState workFlowVersionStates = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Find(workFlowVersionStatesID);
                List<WFVersionStatesApprovalType> wFVersionStatesApprovalTypeList = this._unitOfWork.RepositoryAsync<WFVersionStatesApprovalType>()
                                                                                                .Query()
                                                                                                .Filter(c => c.WorkFlowVersionStateID == workFlowVersionStatesID)
                                                                                                .Get()
                                                                                                .ToList();
                List<WorkFlowVersionStatesAccess> workFlowVersionStatesAccessList = this._unitOfWork.RepositoryAsync<WorkFlowVersionStatesAccess>()
                                                                                                .Query()
                                                                                                .Filter(c => c.WorkFlowVersionStateID == workFlowVersionStatesID)
                                                                                                .Get()
                                                                                                .ToList();
                //delete all State Approval Types
                if (wFVersionStatesApprovalTypeList.Any())
                {
                    foreach (var item in wFVersionStatesApprovalTypeList)
                    {
                        //Delete ApprovalType Actions
                        List<WFStatesApprovalTypeAction> wFStatesApprovalTypeActionList = this._unitOfWork.RepositoryAsync<WFStatesApprovalTypeAction>()
                                                                                                .Query()
                                                                                                .Filter(c => c.WFVersionStatesApprovalTypeID == item.WFVersionStatesApprovalTypeID)
                                                                                                .Get()
                                                                                                .ToList();
                        foreach (var wFStatesApprovalTypeAction in wFStatesApprovalTypeActionList)
                        {
                            this._unitOfWork.Repository<WFStatesApprovalTypeAction>().Delete(wFStatesApprovalTypeAction);
                        }
                        this._unitOfWork.Repository<WFVersionStatesApprovalType>().Delete(item);
                    }
                }

                //delete all State User Access Types
                if (workFlowVersionStatesAccessList.Any())
                {
                    foreach (var item in workFlowVersionStatesAccessList)
                    {
                        this._unitOfWork.Repository<WorkFlowVersionStatesAccess>().Delete(item);
                    }
                }
                //delete email log
                List<EmailLog> emailLogList = this._unitOfWork.RepositoryAsync<EmailLog>()
                                                                                                .Query()
                                                                                                .Filter(c => c.CurrentWorkFlowStateID == workFlowVersionStatesID || c.ApprovedWorkFlowStateID == workFlowVersionStatesID)
                                                                                                .Get()
                                                                                                .ToList();
                if (emailLogList.Any())
                {
                    foreach (var item in emailLogList)
                    {
                        this._unitOfWork.Repository<EmailLog>().Delete(item);
                    }
                }

                //Delete the WorkFlowVersionStates
                this._unitOfWork.Repository<WorkFlowVersionState>().Delete(workFlowVersionStates);

                using (var scope = new TransactionScope())
                {
                    this._unitOfWork.Save();
                    result.Result = ServiceResultStatus.Success;
                    scope.Complete();
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


        public int GetMaxWFStateGroupID()
        {
            int? maxWFStateGroupId = 0;
            try
            {
                maxWFStateGroupId = (this._unitOfWork.RepositoryAsync<WorkFlowVersionState>()
                            .Get().Max(c => c.WFStateGroupID));
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return maxWFStateGroupId == null ? 0 : (int)maxWFStateGroupId;
        }
    }
}
