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
    public class WorkFlowVersionStatesAccessService : IWorkFlowVersionStatesAccessService
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        #endregion Private Members
        #region Constructor
        public WorkFlowVersionStatesAccessService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        public List<WorkFlowVersionStatesAccessViewModel> GetWorkFlowVersionStatesAccessList(int workFlowVersionStateID)
        {
            List<WorkFlowVersionStatesAccessViewModel> workFlowVersionStatesAccessViewModelList = null;
            try
            {
                workFlowVersionStatesAccessViewModelList = (from c in this._unitOfWork.RepositoryAsync<WorkFlowVersionStatesAccess>()
                                                                        .Query()
                                                                        .Filter(c => c.WorkFlowVersionStateID == workFlowVersionStateID)
                                                                        .Get()
                                                            select new WorkFlowVersionStatesAccessViewModel
                                                            {
                                                                WorkFlowVersionStatesAccessID = c.WorkFlowVersionStatesAccessID,
                                                                WorkFlowVersionStateID = c.WorkFlowVersionStateID,
                                                                RoleID = c.RoleID,
                                                                RoleName = c.UserRole.Name
                                                            }).ToList();

                if (workFlowVersionStatesAccessViewModelList.Count() == 0)
                    workFlowVersionStatesAccessViewModelList = new List<WorkFlowVersionStatesAccessViewModel>();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return workFlowVersionStatesAccessViewModelList;
        }

        public ServiceResult AddWorkFlowVersionStatesAccess(int workFlowVersionStateID, int roleID, string addedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                WorkFlowVersionStatesAccess workFlowVersionStatesAccess = this._unitOfWork.RepositoryAsync<WorkFlowVersionStatesAccess>()
                                .Query().Filter(a => a.WorkFlowVersionStateID == workFlowVersionStateID && a.RoleID == roleID).Get().FirstOrDefault();

                if (workFlowVersionStatesAccess == null)
                {
                    WorkFlowVersionStatesAccess workFlowVersionStatesAccessToAdd = new WorkFlowVersionStatesAccess();
                    workFlowVersionStatesAccessToAdd.WorkFlowVersionStateID = workFlowVersionStateID;
                    workFlowVersionStatesAccessToAdd.RoleID = roleID;
                    //workFlowVersionStatesAccessToAdd.EditPermission = editPermission;
                    //workFlowVersionStatesAccessToAdd.IsOwner = isOwner;
                    workFlowVersionStatesAccessToAdd.AddedBy = addedBy;
                    workFlowVersionStatesAccessToAdd.AddedDate = DateTime.Now;
                    workFlowVersionStatesAccessToAdd.UpdatedBy = null;
                    workFlowVersionStatesAccessToAdd.UpdatedDate = null;

                    this._unitOfWork.RepositoryAsync<WorkFlowVersionStatesAccess>().Insert(workFlowVersionStatesAccessToAdd);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "User role already exists." } });
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

        public ServiceResult UpdateWorkFlowVersionStatesAccess(int workFlowVersionStatesAccessID, int roleID, string updatedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                WorkFlowVersionStatesAccess existWorkFlowVersionStatesAccess = this._unitOfWork.RepositoryAsync<WorkFlowVersionStatesAccess>()
                                .Query().Filter(a => a.WorkFlowVersionStatesAccessID == workFlowVersionStatesAccessID && a.RoleID == roleID).Get().FirstOrDefault();

                if (existWorkFlowVersionStatesAccess == null)
                {

                    WorkFlowVersionStatesAccess workFlowVersionStatesAccess = this._unitOfWork.RepositoryAsync<WorkFlowVersionStatesAccess>().Find(workFlowVersionStatesAccessID);
                    workFlowVersionStatesAccess.RoleID = roleID;
                    //workFlowVersionStatesAccess.EditPermission = editPermission;
                    //workFlowVersionStatesAccess.IsOwner = isOwner;
                    workFlowVersionStatesAccess.UpdatedBy = updatedBy;
                    workFlowVersionStatesAccess.UpdatedDate = DateTime.Now;
                    this._unitOfWork.RepositoryAsync<WorkFlowVersionStatesAccess>().Update(workFlowVersionStatesAccess);
                    this._unitOfWork.Save();
                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "Workflow state approval type with same permissions already exists" } });
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

        public ServiceResult DeleteWorkFlowVersionStatesAccess(int workFlowVersionStatesAccessID)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                WorkFlowVersionStatesAccess workFlowVersionStatesAccess = this._unitOfWork.RepositoryAsync<WorkFlowVersionStatesAccess>().Find(workFlowVersionStatesAccessID);
                this._unitOfWork.Repository<WorkFlowVersionStatesAccess>().Delete(workFlowVersionStatesAccess);
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
