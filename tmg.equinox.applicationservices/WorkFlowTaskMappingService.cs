using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.repository.interfaces;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.infrastructure.exceptionhandling;
using System.Transactions;
using tmg.equinox.applicationservices.viewmodels.Settings;

namespace tmg.equinox.applicationservices
{
    public class WorkFlowTaskMappingService : IWorkFlowTaskMappingService
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        #endregion Private Members

        #region Constructor
        public WorkFlowTaskMappingService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion

        public IList<KeyValue> GetWorkFlowList(int tenantID)
        {
            IList<KeyValue> list = new List<KeyValue>();

            list = (from Workflowstate in this._unitOfWork.RepositoryAsync<WorkFlowStateMaster>()
                                                .Query()
                                                .Filter(c => c.TenantID == tenantID && c.IsActive == true)
                                                .Get()
                    select new KeyValue
                    {
                        Key = Workflowstate.WFStateID,
                        Value = Workflowstate.WFStateName
                    }).ToList();

            return list;
        }

        public IEnumerable<KeyValue> GetNonSelectedTaskList(int tenantId)
        {
            IList<KeyValue> getnonselectedtaskList = new List<KeyValue>();
            try
            {
                getnonselectedtaskList = (from c in this._unitOfWork.RepositoryAsync<TaskList>()
                                                                .Query()
                                                                .Get().Where(c => c.IsActive == true)
                                          select new KeyValue
                                          {
                                              Key = c.TaskID,
                                              Value = c.TaskDescription
                                          }).ToList();

                if (getnonselectedtaskList.Count() == 0)
                    getnonselectedtaskList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return getnonselectedtaskList;
        }


        public IEnumerable<KeyValue> GetApplicableWfTaskList(int tenantId, int WfstateId)
        {
            IList<KeyValue> getApplicableWftaskList = new List<KeyValue>();
            ServiceResult result = new ServiceResult();
            try
            {
                getApplicableWftaskList = (from applicableWftaskMap in this._unitOfWork.RepositoryAsync<WorkflowTaskMap>().Get().Where(c => c.WFStateID == WfstateId && c.IsActive == true)
                                           join task in this._unitOfWork.RepositoryAsync<TaskList>().Get()
                                           on applicableWftaskMap.TaskID equals task.TaskID
                                           where task.IsActive == true
                                           select new KeyValue
                                           {
                                               Key = task.TaskID,
                                               Value = task.TaskDescription
                                           }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
            }
            return getApplicableWftaskList;
        }


        public ServiceResult UpdateApplicableWFTaskMap(int tenantId, int WfstateId, List<ApplicableTaskMapModel> applicableTaskMapModel, string userName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var applicableWfTaskMapList = (from applicableWorkflowTaskMap in this._unitOfWork.RepositoryAsync<WorkflowTaskMap>()
                               .Query()
                               .Filter(c => c.WFStateID == WfstateId && c.IsActive == true)
                               .Get()
                                               select applicableWorkflowTaskMap).ToList();
                if (applicableWfTaskMapList.Count > 0)
                {
                    {
                        foreach (var data in applicableWfTaskMapList)
                        {

                            data.IsActive = false;
                            data.UpdatedBy = userName;
                            data.UpdatedDate = DateTime.Now;
                            this._unitOfWork.RepositoryAsync<WorkflowTaskMap>().Update(data);
                        }
                        foreach (var mapData in applicableTaskMapModel)
                        {
                            WorkflowTaskMap userMapToAdd = new WorkflowTaskMap();
                            userMapToAdd.WFStateID = WfstateId;
                            userMapToAdd.TaskID = mapData.TaskID;
                            userMapToAdd.AddedBy = userName;
                            userMapToAdd.AddedDate = DateTime.Now;
                            userMapToAdd.IsActive = true;
                            this._unitOfWork.RepositoryAsync<WorkflowTaskMap>().Insert(userMapToAdd);
                            this._unitOfWork.Save();
                        }
                        result.Result = ServiceResultStatus.Success;
                        //scope.Complete();
                    }

                }
                else
                {
                    foreach (var mapData in applicableTaskMapModel)
                    {
                        WorkflowTaskMap userMapToAdd = new WorkflowTaskMap();
                        userMapToAdd.WFStateID = WfstateId;
                        userMapToAdd.TaskID = mapData.TaskID;
                        userMapToAdd.AddedBy = userName;
                        userMapToAdd.AddedDate = DateTime.Now;
                        userMapToAdd.IsActive = true;
                        this._unitOfWork.RepositoryAsync<WorkflowTaskMap>().Insert(userMapToAdd);
                        this._unitOfWork.Save();
                    }
                    result.Result = ServiceResultStatus.Success;
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

        public ServiceResult SaveDPFTaskAndWorkflowMappings(int wfStateId, string taskDescription, DateTime addedDate, string addedBy, int tenantId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                TaskList objNewTaskList = new TaskList();
                objNewTaskList.TaskDescription = taskDescription;
                objNewTaskList.AddedDate = addedDate;
                objNewTaskList.AddedBy = addedBy;
                objNewTaskList.UpdatedDate = addedDate;
                objNewTaskList.UpdatedBy = addedBy;
                objNewTaskList.TenantID = tenantId;
                objNewTaskList.IsActive = true;

                this._unitOfWork.RepositoryAsync<TaskList>().Insert(objNewTaskList);
                this._unitOfWork.Save();

                WorkflowTaskMap objNewWorkflowTaskMap = new WorkflowTaskMap();
                objNewWorkflowTaskMap.TaskID = objNewTaskList.TaskID;
                objNewWorkflowTaskMap.WFStateID = wfStateId;
                objNewWorkflowTaskMap.IsActive = true;
                objNewWorkflowTaskMap.AddedDate = addedDate;
                objNewWorkflowTaskMap.AddedBy = addedBy;
                objNewWorkflowTaskMap.UpdatedDate = addedDate;
                objNewWorkflowTaskMap.UpdatedBy = addedBy;
                objNewWorkflowTaskMap.TenantID = tenantId;

                this._unitOfWork.RepositoryAsync<WorkflowTaskMap>().Insert(objNewWorkflowTaskMap);
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
    }
}

