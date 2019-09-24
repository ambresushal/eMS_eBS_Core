using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.domain.entities.Enums;
using System.Data.SqlClient;
using System.Transactions;

namespace tmg.equinox.applicationservices
{
    public class WorkFlowCategoryMappingService : IWorkFlowCategoryMappingService
    {
        #region Private Memebers

        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private IWorkFlowVersionStatesService _workFlowVersionStateServices { get; set; }
        private IWFVersionStatesApprovalTypeService _wFVersionStatesApprovalTypeService { get; set; }
        private IWorkFlowVersionStatesAccessService _workFlowVersionStatesAccessService { get; set; }
        private string UserName { get; set; }

        #endregion Private Members

        #region Constructor

        public WorkFlowCategoryMappingService(IUnitOfWorkAsync unitOfWork, IWorkFlowVersionStatesService workFlowVersionStateServices, IWFVersionStatesApprovalTypeService wFVersionStatesApprovalTypeService, IWorkFlowVersionStatesAccessService workFlowVersionStatesAccessService)
        {
            this._unitOfWork = unitOfWork;
            this._workFlowVersionStateServices = workFlowVersionStateServices;
            this._wFVersionStatesApprovalTypeService = wFVersionStatesApprovalTypeService;
            this._workFlowVersionStatesAccessService = workFlowVersionStatesAccessService;
        }
        #endregion Constructor

        /// <summary>
        /// Gets the WorkFlow Category Mapping List for the specified TenantID.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public List<WorkFlowCategoryMappingViewModel> GetWorkFlowCategoryMappingList(int tenantId)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            List<WorkFlowCategoryMappingViewModel> workFlowCategoryMappingList = null;
            try
            {
                workFlowCategoryMappingList = (from c in this._unitOfWork.RepositoryAsync<WorkFlowCategoryMapping>()
                                                                        .Query()
                                                                        .Include(c => c.FolderVersionCategory)
                                                                        .Filter(c => c.TenantID == tenantId)
                                                                        .Get()
                                               select new WorkFlowCategoryMappingViewModel
                                               {
                                                   WorkFlowVersionID = c.WorkFlowVersionID,
                                                   FolderVersionCategoryID = c.FolderVersionCategoryID,
                                                   AccountType = c.AccountType,
                                                   WorkFlowType = c.WorkFlowType,
                                                   CategoryName = c.FolderVersionCategory.FolderVersionCategoryName,
                                                   IsFinalized = c.IsFinalized,
                                                   AccountTypeName = ((AccountType)c.AccountType).ToString(),//AccountType.(c.AccountType],
                                                   WorkFlowtypeName = ((WorkFlowType)c.WorkFlowType).ToString()

                                               }).ToList();

                if (workFlowCategoryMappingList.Count() == 0)
                    workFlowCategoryMappingList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return workFlowCategoryMappingList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantID"></param>
        /// <param name="workFlowVersionID"></param>
        /// <param name="workFlowType"></param>
        /// <param name="accountType"></param>
        /// <param name="folderVersionCategoryID"></param>
        /// <param name="addedBy"></param>
        /// <returns></returns>
        public ServiceResult AddWorkFlowCategoryMapping(int tenantID, int workFlowType, int accountType, int folderVersionCategoryID, string addedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                WorkFlowCategoryMapping workFlowCategoryMapping = this._unitOfWork.RepositoryAsync<WorkFlowCategoryMapping>()
                                .Query().Filter(a => a.FolderVersionCategoryID == folderVersionCategoryID && a.AccountType == accountType).Get().FirstOrDefault();

                if (workFlowCategoryMapping == null)
                {
                    WorkFlowCategoryMapping workFlowCategoryMappingToAdd = new WorkFlowCategoryMapping();
                    workFlowCategoryMappingToAdd.FolderVersionCategoryID = folderVersionCategoryID;
                    workFlowCategoryMappingToAdd.WorkFlowType = workFlowType;
                    workFlowCategoryMappingToAdd.AccountType = accountType;
                    workFlowCategoryMappingToAdd.TenantID = tenantID;
                    workFlowCategoryMappingToAdd.AddedBy = addedBy;
                    workFlowCategoryMappingToAdd.AddedDate = DateTime.Now;
                    workFlowCategoryMappingToAdd.UpdatedBy = null;
                    workFlowCategoryMappingToAdd.UpdatedDate = null;
                    workFlowCategoryMappingToAdd.WorkFlowVersionID = 1;
                    workFlowCategoryMappingToAdd.EffectiveDate = null;

                    this._unitOfWork.RepositoryAsync<WorkFlowCategoryMapping>().Insert(workFlowCategoryMappingToAdd);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "Category with same name and  account type already exists." } });
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
        /// <param name="tenantID"></param>
        /// <param name="workFlowVersionID"></param>
        /// <param name="workFlowType"></param>
        /// <param name="accountType"></param>
        /// <param name="folderVersionCategoryID"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        public ServiceResult UpdateWorkFlowCategoryMapping(int tenantID, int workFlowVersionID, int workFlowType, int accountType, int folderVersionCategoryID, string updatedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                WorkFlowCategoryMapping existWorkFlowCategoryMapping = this._unitOfWork.RepositoryAsync<WorkFlowCategoryMapping>()
                                .Query().Filter(a => a.WorkFlowVersionID != workFlowVersionID && a.FolderVersionCategoryID == folderVersionCategoryID && a.AccountType == accountType).Get().FirstOrDefault();

                if (existWorkFlowCategoryMapping == null)
                {

                    WorkFlowCategoryMapping workFlowCategoryMapping = this._unitOfWork.RepositoryAsync<WorkFlowCategoryMapping>().Find(workFlowVersionID);
                    if (workFlowCategoryMapping.WorkFlowType != workFlowType)
                    {
                        //update sequence of all states to "0" and  Change State to On Success and Change State to On Failure should be reset to the BLANK ("")
                        List<WorkFlowVersionState> workFlowVersionStates = this._unitOfWork.Repository<WorkFlowVersionState>()
                                    .Query().Filter(a => a.WorkFlowVersionID == workFlowVersionID).Include(e => e.WFVersionStatesApprovalType).Include(e => e.WFVersionStatesApprovalType.Select(s => s.WFStatesApprovalTypeActions)).Get().ToList();
                        if (workFlowVersionStates != null && workFlowVersionStates.Count() > 0)
                        {
                            foreach (var state in workFlowVersionStates)
                            {
                                foreach (var approvalType in state.WFVersionStatesApprovalType.ToList())
                                {
                                    foreach (var approvalTypeAction in approvalType.WFStatesApprovalTypeActions.ToList())
                                    {
                                        if (approvalTypeAction.ActionID == 2 || approvalTypeAction.ActionID == 3)
                                        {
                                            approvalTypeAction.ActionResponse = String.Empty;
                                            this._unitOfWork.RepositoryAsync<WFStatesApprovalTypeAction>().Update(approvalTypeAction);
                                        }
                                    }
                                }
                                state.Sequence = 0;
                                this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Update(state);
                            }
                        }
                    }
                    workFlowCategoryMapping.WorkFlowType = workFlowType;
                    workFlowCategoryMapping.AccountType = accountType;
                    workFlowCategoryMapping.UpdatedBy = updatedBy;
                    workFlowCategoryMapping.UpdatedDate = DateTime.Now;
                    this._unitOfWork.RepositoryAsync<WorkFlowCategoryMapping>().Update(workFlowCategoryMapping);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "Category with same name and  account type already exists." } });
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
        /// <param name="tenantID"></param>
        /// <param name="workFlowVersionID"></param>
        /// <returns></returns>
        public ServiceResult DeleteWorkFlowCategoryMapping(int tenantID, int workFlowVersionID)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                int folderVersionsCount = 0;
                WorkFlowCategoryMapping workFlowCategoryMapping = this._unitOfWork.RepositoryAsync<WorkFlowCategoryMapping>().Find(workFlowVersionID);
                if (workFlowCategoryMapping != null)
                {
                    int folderVersionCategoryID = this._unitOfWork.Repository<WorkFlowCategoryMapping>().Find(workFlowVersionID).FolderVersionCategoryID;
                    List<int> workFlowStatesIds = (from s in this._unitOfWork.Repository<WorkFlowVersionState>().Get() where s.WorkFlowVersionID == workFlowVersionID select s.WorkFlowVersionStateID).ToList();
                    folderVersionsCount = this._unitOfWork.Repository<FolderVersion>()
                        .Query().Include(e => e.Folder).Filter(a => a.CategoryID == folderVersionCategoryID && workFlowStatesIds.Contains(a.WFStateID == null ? 0 : (int)a.WFStateID)).Get().Count();
                }
                if (folderVersionsCount > 0)
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "WorkFlow associated Category is used with Folder versions, so can not be deleted." } });
                    result.Items = items;
                    result.Result = ServiceResultStatus.Failure;
                }
                else
                {
                    //Delete States associated with Work Flow Version
                    List<WorkFlowVersionState> workFlowVersionStates = this._unitOfWork.Repository<WorkFlowVersionState>()
                                    .Query().Filter(a => a.WorkFlowVersionID == workFlowVersionID).Include(e => e.WorkFlowVersionStatesAccess).Include(e => e.WFVersionStatesApprovalType).Include(e => e.WFVersionStatesApprovalType.Select(s => s.WFStatesApprovalTypeActions)).Get().ToList();
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
                            //delete email log
                            List<EmailLog> emailLogList = this._unitOfWork.RepositoryAsync<EmailLog>()
                                                                                                            .Query()
                                                                                                            .Filter(c => c.CurrentWorkFlowStateID == state.WorkFlowVersionStateID || c.ApprovedWorkFlowStateID == state.WorkFlowVersionStateID)
                                                                                                            .Get()
                                                                                                            .ToList();
                            if (emailLogList.Any())
                            {
                                foreach (var item in emailLogList)
                                {
                                    this._unitOfWork.Repository<EmailLog>().Delete(item);
                                }
                            }
                            this._unitOfWork.Repository<WorkFlowVersionState>().Delete(state);
                        }
                    }
                    workFlowCategoryMapping = this._unitOfWork.Repository<WorkFlowCategoryMapping>().Find(workFlowVersionID);
                    this._unitOfWork.Repository<WorkFlowCategoryMapping>().Delete(workFlowCategoryMapping);
                    using (var scope = new TransactionScope())
                    {
                        this._unitOfWork.Save();
                        items.Add(new ServiceResultItem() { Messages = new string[] { "WorkFlow mappings deleted successfully." } });
                        result.Items = items;
                        result.Result = ServiceResultStatus.Success;
                        scope.Complete();
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

        /// <summary>
        /// Finalize the work flow - validate before finalized.
        /// </summary>
        /// <param name="workFlowVersionID"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        public ServiceResult FinalizeWorkFlowVersion(int workFlowVersionID, string updatedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                //Validate the WorkFlowVersion It needs to have at least one state, State Approval type, Sequence for the states, State Actions to be defined
                List<WorkFlowVersionStatesViewModel> workFlowVersionStates = _workFlowVersionStateServices.GetWorkFlowVersionStatesList(workFlowVersionID);
                if (workFlowVersionStates != null && workFlowVersionStates.Count() > 0)
                {
                    foreach (var item in workFlowVersionStates)
                    {
                        if (item.Sequence == 0)
                        {
                            items.Add(new ServiceResultItem() { Messages = new string[] { "Please make sure every WorkFlow state have sequence greater than 0." } });
                            result.Items = items;
                            result.Result = ServiceResultStatus.Failure;
                            return result;
                        }
                        List<WFVersionStatesApprovalTypeViewModel> wFVersionStatesApprovalTypeList = _wFVersionStatesApprovalTypeService.GetWFVersionStatesApprovalTypeList(item.WorkFlowVersionStateID);
                        if (wFVersionStatesApprovalTypeList != null && wFVersionStatesApprovalTypeList.Count() > 0)
                        {
                            //Validate Approval Types
                            foreach (var wFVersionStatesApprovalType in wFVersionStatesApprovalTypeList)
                            {
                                //Validate Approval Types Actions
                                List<WFStatesApprovalTypeActionViewModel> wFVersionStatesApprovalTypeActionList = _wFVersionStatesApprovalTypeService.GetWFStatesApprovalTypeActionList(wFVersionStatesApprovalType.WFVersionStatesApprovalTypeID);
                                if (wFVersionStatesApprovalTypeActionList != null && wFVersionStatesApprovalTypeActionList.Count() > 0)
                                {
                                    if (wFVersionStatesApprovalTypeActionList.All(a => String.IsNullOrEmpty(a.ActionResponse.Trim())))
                                    {
                                        items.Add(new ServiceResultItem() { Messages = new string[] { "Please set Action for Approval type - " + item.WFStateName + "->" + wFVersionStatesApprovalType.WorkFlowStateApprovalTypeName } });
                                        result.Items = items;
                                        result.Result = ServiceResultStatus.Failure;
                                        return result;
                                    }     
                                }
                            }
                        }
                        else
                        {
                            items.Add(new ServiceResultItem() { Messages = new string[] { "Please add at least one Approval Type for state - " + item.WFStateName } });
                            result.Items = items;
                            result.Result = ServiceResultStatus.Failure;
                            return result;
                        }

                        List<WorkFlowVersionStatesAccessViewModel> workFlowVersionStatesAccessList = _workFlowVersionStatesAccessService.GetWorkFlowVersionStatesAccessList(item.WorkFlowVersionStateID);
                        if (workFlowVersionStatesAccessList == null || workFlowVersionStatesAccessList.Count() == 0)
                        {
                            items.Add(new ServiceResultItem() { Messages = new string[] { "Please add at least one Access Permission type for state - " + item.WFStateName } });
                            result.Items = items;
                            result.Result = ServiceResultStatus.Failure;
                            return result;
                        }
                    }
                }
                else
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "Please add at least one State to the workflow." } });
                    result.Items = items;
                    result.Result = ServiceResultStatus.Failure;
                    return result;
                }
                //Update Finalize attibute for the workflow
                WorkFlowCategoryMapping workFlowCategoryMapping = this._unitOfWork.RepositoryAsync<WorkFlowCategoryMapping>().Find(workFlowVersionID);
                if (workFlowCategoryMapping.WorkFlowType == 2 && workFlowVersionStates.Count() > 1)
                {
                    var satetsWithSameSequence = from s in workFlowVersionStates
                                                 group s by s.Sequence into g
                                                 where g.Count() > 1
                                                 select new { Id = g.Key, Count = g.Count() };

                    if (satetsWithSameSequence != null && satetsWithSameSequence.Count() < 1)
                    {
                        items.Add(new ServiceResultItem() { Messages = new string[] { "If the WorkFlow type is HYBRID, then at least 2 states should have the same order." } });
                        result.Items = items;
                        result.Result = ServiceResultStatus.Failure;
                        return result;
                    }
                }
                if (workFlowCategoryMapping != null)
                {

                    workFlowCategoryMapping.IsFinalized = true;
                    workFlowCategoryMapping.UpdatedBy = updatedBy;
                    workFlowCategoryMapping.UpdatedDate = DateTime.Now;
                    this._unitOfWork.RepositoryAsync<WorkFlowCategoryMapping>().Update(workFlowCategoryMapping);
                    this._unitOfWork.Save();
                    result.Result = ServiceResultStatus.Success;
                    //Update the WFGroupID
                    if (workFlowCategoryMapping.WorkFlowType == (int)WorkFlowType.HYBRID && workFlowVersionStates.Count() > 1)
                    {
                        var satetsWithSameSequence = from s in workFlowVersionStates
                                                     group s by s.Sequence into g
                                                      where g.Count() > 1
                                                     select new { Id = g.Key, Count = g.Count() };

                        if (satetsWithSameSequence != null && satetsWithSameSequence.Count() > 0)
                        {
                            List<int> sameSequenceStateIds = (from s in satetsWithSameSequence select s.Id).ToList();
                            var distinctSequenceIds = (from s in workFlowVersionStates where sameSequenceStateIds.Contains(s.Sequence) select s.Sequence).Distinct().ToList();
                            foreach (var sequence in distinctSequenceIds)
                            {
                                var sequenceStates = (from s in workFlowVersionStates where s.Sequence == sequence select s).ToList();
                                int? maxWFStateGroupId = 0;
                                var WGGroupIdStates = (from s in sequenceStates where s.WFStateGroupID != null && s.WFStateGroupID > 0 select s).ToList();
                                maxWFStateGroupId = _workFlowVersionStateServices.GetMaxWFStateGroupID();
                                if ((WGGroupIdStates == null && WGGroupIdStates.Count() < 2) || WGGroupIdStates.Count() == 0)
                                {
                                    maxWFStateGroupId = maxWFStateGroupId + 1;
                                }
                                else
                                {
                                    maxWFStateGroupId = (from s in WGGroupIdStates select s.WFStateGroupID).ToList().Max();
                                }
                                foreach (var item in sequenceStates)
                                {
                                    _workFlowVersionStateServices.UpdateWorkFlowVersionStates(item.WorkFlowVersionStateID, item.WFStateID, item.Sequence, null, maxWFStateGroupId);
                                }
                            }
                        }
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

        public ServiceResult CopyWorkFlowCategorymapping(int folderVersionCategoryID, int workFlowVersionID, string updatedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                WorkFlowCategoryMapping workFlowCategoryMapping = this._unitOfWork.Repository<WorkFlowCategoryMapping>()
                               .Query().Filter(a => a.WorkFlowVersionID == workFlowVersionID).Get().FirstOrDefault();
                WorkFlowCategoryMapping workFlowCategoryMappingExists = this._unitOfWork.Repository<WorkFlowCategoryMapping>()
                                .Query().Filter(a => a.FolderVersionCategoryID == folderVersionCategoryID && a.AccountType == workFlowCategoryMapping.AccountType).Get().FirstOrDefault();

                if (workFlowCategoryMappingExists == null)
                {
                    SqlParameter folderVersionCategoryId = new SqlParameter("@FolderVersionCategoryID", folderVersionCategoryID);
                    SqlParameter workFlowVersionId = new SqlParameter("@WorkFlowVersionID", workFlowVersionID);
                    SqlParameter paramUserName = new SqlParameter("@UserName", updatedBy);

                    var status = this._unitOfWork.Repository<WorkFlowCategoryMapping>().ExecuteSql("exec [dbo].[USP_CopyWorkFlowCategorryMappings]  @WorkFlowVersionID,@FolderVersionCategoryID, @UserName",
                        workFlowVersionId, folderVersionCategoryId, paramUserName).ToList().FirstOrDefault();

                    if (status != null)
                    {
                        items.Add(new ServiceResultItem() { Messages = new string[] { "WorkFlow has been copied successfully." } });
                        result.Items = items;
                        result.Result = ServiceResultStatus.Success;
                    }
                    else
                    {
                        items.Add(new ServiceResultItem() { Messages = new string[] { "Some issue while copying the WorkFlow settings." } });
                        result.Items = items;
                        result.Result = ServiceResultStatus.Failure;
                    }
                }
                else
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "Category with same name and  account type already exists." } });
                    result.Items = items;
                    result.Result = ServiceResultStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(new Exception("WorkFlow - " + ex.Message), ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }
    }
}
