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
    public class WFVersionStatesApprovalTypeService : IWFVersionStatesApprovalTypeService
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        #endregion Private Members
        #region Constructor
        public WFVersionStatesApprovalTypeService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        public List<WFVersionStatesApprovalTypeViewModel> GetWFVersionStatesApprovalTypeList(int workFlowVersionStateID)
        {
            List<WFVersionStatesApprovalTypeViewModel> wFVersionStatesApprovalTypeViewModelList = null;
            try
            {
                wFVersionStatesApprovalTypeViewModelList = (from c in this._unitOfWork.RepositoryAsync<WFVersionStatesApprovalType>()
                                                                        .Query()
                                                                        .Filter(c => c.WorkFlowVersionStateID == workFlowVersionStateID)
                                                                        .Get()
                                                            select new WFVersionStatesApprovalTypeViewModel
                                           {
                                               WFVersionStatesApprovalTypeID = c.WFVersionStatesApprovalTypeID,
                                               WorkFlowStateApprovalTypeID = c.WorkFlowStateApprovalTypeID,
                                               WorkFlowVersionStateID = c.WorkFlowVersionStateID,
                                               WorkFlowStateApprovalTypeName = c.WorkFlowStateApprovalTypeMaster.WorkFlowStateApprovalTypeName
                                           }).ToList();

                if (wFVersionStatesApprovalTypeViewModelList.Count() == 0)
                    wFVersionStatesApprovalTypeViewModelList = new List<WFVersionStatesApprovalTypeViewModel>();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return wFVersionStatesApprovalTypeViewModelList;
        }

        public ServiceResult AddWFVersionStatesApprovalType(int workFlowStateApprovalTypeID, int workFlowVersionStateID, string addedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                WFVersionStatesApprovalType workFlowStateApprovalType = this._unitOfWork.RepositoryAsync<WFVersionStatesApprovalType>()
                                .Query().Filter(a => a.WorkFlowStateApprovalTypeID == workFlowStateApprovalTypeID && a.WorkFlowVersionStateID == workFlowVersionStateID).Get().FirstOrDefault();

                if (workFlowStateApprovalType == null)
                {
                    WFVersionStatesApprovalType wFVersionStatesApprovalTypeToAdd = new WFVersionStatesApprovalType();
                    wFVersionStatesApprovalTypeToAdd.WorkFlowStateApprovalTypeID = workFlowStateApprovalTypeID;
                    wFVersionStatesApprovalTypeToAdd.WorkFlowVersionStateID = workFlowVersionStateID;
                    wFVersionStatesApprovalTypeToAdd.AddedBy = addedBy;
                    wFVersionStatesApprovalTypeToAdd.AddedDate = DateTime.Now;
                    wFVersionStatesApprovalTypeToAdd.UpdatedBy = null;
                    wFVersionStatesApprovalTypeToAdd.UpdatedDate = null;

                    this._unitOfWork.RepositoryAsync<WFVersionStatesApprovalType>().Insert(wFVersionStatesApprovalTypeToAdd);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;

                    //Add default actions for the Approval Type
                    WFVersionStatesApprovalType AddedworkFlowStateApprovalType = this._unitOfWork.RepositoryAsync<WFVersionStatesApprovalType>()
                               .Query().Filter(a => a.WorkFlowStateApprovalTypeID == workFlowStateApprovalTypeID && a.WorkFlowVersionStateID == workFlowVersionStateID).Get().FirstOrDefault();
                    string emptyString = String.Empty;
                    AddWFStatesApprovalTypeAction(AddedworkFlowStateApprovalType.WFVersionStatesApprovalTypeID, 1, emptyString, addedBy);
                    AddWFStatesApprovalTypeAction(AddedworkFlowStateApprovalType.WFVersionStatesApprovalTypeID, 2, emptyString, addedBy);
                    AddWFStatesApprovalTypeAction(AddedworkFlowStateApprovalType.WFVersionStatesApprovalTypeID, 3, emptyString, addedBy);
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

        public ServiceResult UpdateWFVersionStatesApprovalType(int wFVersionStatesApprovalTypeID, int workFlowStateApprovalTypeID, string updatedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                WFVersionStatesApprovalType existWorkFlowVersionStates = this._unitOfWork.RepositoryAsync<WFVersionStatesApprovalType>()
                                .Query().Filter(a => a.WFVersionStatesApprovalTypeID == wFVersionStatesApprovalTypeID && a.WorkFlowStateApprovalTypeID == workFlowStateApprovalTypeID).Get().FirstOrDefault();

                if (existWorkFlowVersionStates == null)
                {

                    WFVersionStatesApprovalType wFVersionStatesApprovalType = this._unitOfWork.RepositoryAsync<WFVersionStatesApprovalType>().Find(wFVersionStatesApprovalTypeID);
                    wFVersionStatesApprovalType.WorkFlowStateApprovalTypeID = workFlowStateApprovalTypeID;
                    wFVersionStatesApprovalType.UpdatedBy = updatedBy;
                    wFVersionStatesApprovalType.UpdatedDate = DateTime.Now;
                    this._unitOfWork.RepositoryAsync<WFVersionStatesApprovalType>().Update(wFVersionStatesApprovalType);
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

        public ServiceResult DeleteWFVersionStatesApprovalType(int wFVersionStatesApprovalTypeID)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                WFVersionStatesApprovalType wFVersionStatesApprovalType = this._unitOfWork.RepositoryAsync<WFVersionStatesApprovalType>().Find(wFVersionStatesApprovalTypeID);
                //Delete ApprovalType Actions
                List<WFStatesApprovalTypeAction> wFStatesApprovalTypeActions = this._unitOfWork.RepositoryAsync<WFStatesApprovalTypeAction>()
                                .Query().Filter(a => a.WFVersionStatesApprovalTypeID == wFVersionStatesApprovalType.WFVersionStatesApprovalTypeID).Get().ToList();
                foreach (var item in wFStatesApprovalTypeActions)
                {
                    DeleteWFStatesApprovalTypeAction(item.WFStatesApprovalTypeActionID);
                }

                this._unitOfWork.Repository<WFVersionStatesApprovalType>().Delete(wFVersionStatesApprovalType);
                this._unitOfWork.Save();
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


        public List<WFStatesApprovalTypeActionViewModel> GetWFStatesApprovalTypeActionList(int wFVersionStatesApprovalTypeID)
        {
            List<WFStatesApprovalTypeActionViewModel> wFStatesApprovalTypeActionViewModelList = null;
            try
            {
                wFStatesApprovalTypeActionViewModelList = (from c in this._unitOfWork.RepositoryAsync<WFStatesApprovalTypeAction>()
                                                                        .Query()
                                                                        .Filter(c => c.WFVersionStatesApprovalTypeID == wFVersionStatesApprovalTypeID)
                                                                        .Get()
                                                           select new WFStatesApprovalTypeActionViewModel
                                                            {
                                                                WFVersionStatesApprovalTypeID = c.WFVersionStatesApprovalTypeID,
                                                                WFStatesApprovalTypeActionID = c.WFStatesApprovalTypeActionID,
                                                                ActionID = c.ActionID,
                                                                ActionName = c.WorkFlowAction.ActionName,
                                                                ActionResponse = c.ActionResponse,
                                                                WFVersionStatesApprovalTypeName = c.WFVersionStatesApprovalType.WorkFlowStateApprovalTypeMaster.WorkFlowStateApprovalTypeName,
                                                            }).ToList();

                foreach (var item in wFStatesApprovalTypeActionViewModelList)
                {
                    if ((item.ActionID == 2 || item.ActionID == 3) && !String.IsNullOrEmpty(item.ActionResponse.Trim()))
                    {
                        WorkFlowVersionState workFlowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Find(Convert.ToInt32(item.ActionResponse));
                        item.ActionResponse = GetStateName(workFlowState.WFStateID);
                    }
                }
                if (wFStatesApprovalTypeActionViewModelList.Count() == 0)
                    wFStatesApprovalTypeActionViewModelList = new List<WFStatesApprovalTypeActionViewModel>();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return wFStatesApprovalTypeActionViewModelList;
        }

        private string GetStateName(int stateId)
        {
            string stateName = String.Empty;
            if (stateId == null || stateId == 0)
            {
                return stateName;
            }
            else
            {
                WorkFlowStateMaster workFlowState = this._unitOfWork.RepositoryAsync<WorkFlowStateMaster>().Find(Convert.ToInt32(stateId));
                if (workFlowState != null)
                {
                    stateName = workFlowState.WFStateName;
                }

            }
            return stateName;
        }

        public ServiceResult AddWFStatesApprovalTypeAction(int wFVersionStatesApprovalTypeID, int actionID, string actionResponse, string addedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                WFStatesApprovalTypeAction wFStatesApprovalTypeAction = this._unitOfWork.RepositoryAsync<WFStatesApprovalTypeAction>()
                                .Query().Filter(a => a.WFVersionStatesApprovalTypeID == wFVersionStatesApprovalTypeID && a.ActionID == actionID && a.ActionResponse == actionResponse).Get().FirstOrDefault();

                if (wFStatesApprovalTypeAction == null)
                {
                    WFStatesApprovalTypeAction wFStatesApprovalTypeActionToAdd = new WFStatesApprovalTypeAction();
                    wFStatesApprovalTypeActionToAdd.ActionID = actionID;
                    wFStatesApprovalTypeActionToAdd.WFVersionStatesApprovalTypeID = wFVersionStatesApprovalTypeID;
                    wFStatesApprovalTypeActionToAdd.ActionResponse = actionResponse;
                    wFStatesApprovalTypeActionToAdd.AddedBy = addedBy;
                    wFStatesApprovalTypeActionToAdd.AddedDate = DateTime.Now;
                    wFStatesApprovalTypeActionToAdd.UpdatedBy = null;
                    wFStatesApprovalTypeActionToAdd.UpdatedDate = null;

                    this._unitOfWork.RepositoryAsync<WFStatesApprovalTypeAction>().Insert(wFStatesApprovalTypeActionToAdd);
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

        public ServiceResult UpdateWFStatesApprovalTypeAction(int wFStatesApprovalTypeActionID, string actionResponse, string updatedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                WFStatesApprovalTypeAction wFStatesApprovalTypeAction = this._unitOfWork.RepositoryAsync<WFStatesApprovalTypeAction>().Find(wFStatesApprovalTypeActionID);
                wFStatesApprovalTypeAction.ActionResponse = actionResponse;
                wFStatesApprovalTypeAction.UpdatedBy = updatedBy;
                wFStatesApprovalTypeAction.UpdatedDate = DateTime.Now;
                this._unitOfWork.RepositoryAsync<WFStatesApprovalTypeAction>().Update(wFStatesApprovalTypeAction);
                this._unitOfWork.Save();
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

        public ServiceResult DeleteWFStatesApprovalTypeAction(int wFStatesApprovalTypeActionID)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                WFStatesApprovalTypeAction wFStatesApprovalTypeAction = this._unitOfWork.RepositoryAsync<WFStatesApprovalTypeAction>().Find(wFStatesApprovalTypeActionID);
                this._unitOfWork.Repository<WFStatesApprovalTypeAction>().Delete(wFStatesApprovalTypeAction);
                this._unitOfWork.Save();
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
    }
}
