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
  public class TaskListService: ITaskListService
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        #endregion Private Members

        #region Constructor
        public TaskListService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        public List<TaskListViewModel> GetTaskList(int tenantID)
        {
            List<TaskListViewModel> TaskList = null;
            try
            {
                TaskList = (from c in this._unitOfWork.RepositoryAsync<TaskList>()
                                                                        .Query()
                                                                        .Filter(c => c.TenantID == tenantID && c.IsActive == true)
                                                                        .Get()
                                           select new TaskListViewModel
                                           {
                                               TaskID = c.TaskID,
                                               TaskDescription = c.TaskDescription,
                                               IsStandardTask=c.IsStandardTask,
                                               IsActive =  c.IsActive
                                           }).OrderByDescending(O => O.TaskID).ToList();

                if (TaskList.Count() == 0)
                    TaskList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return TaskList;
        }

        public ServiceResult AddTask(int tenantID, string TaskDescription, string addedBy,Boolean IsStandardCheckbox)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                TaskList Task = this._unitOfWork.RepositoryAsync<TaskList>()
                                .Query().Filter(a => a.TaskDescription == TaskDescription && a.TenantID == tenantID && a.IsActive == true).Get().FirstOrDefault();

                if (Task == null)
                {
                    TaskList TaskToAdd = new TaskList();
                    TaskToAdd.TaskDescription = TaskDescription;
                    TaskToAdd.TenantID = tenantID;
                    TaskToAdd.AddedBy = addedBy;
                    TaskToAdd.AddedDate = DateTime.Now;
                    TaskToAdd.UpdatedBy = null;
                    TaskToAdd.UpdatedDate = null;
                    TaskToAdd.IsActive = true;
                    TaskToAdd.IsStandardTask = IsStandardCheckbox;
                    this._unitOfWork.RepositoryAsync<TaskList>().Insert(TaskToAdd);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "Task already exists" } });
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

        public ServiceResult UpdateTask(int TaskID, string TaskDescription, string updatedBy, bool IsStandardCheckbox)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {

                var finalizedTask = (this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>()
                                                                      .Query()
                                                                      .Filter(c => c.TaskID == TaskID && (c.Status == "Assigned" || c.Status == "Completed" || c.Status == "In Progress"))
                                                                      .Get()
                                    ).ToList();
                if (finalizedTask != null && finalizedTask.Count() > 0)
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "Task can not be edited as it is used in finalized workflow" } });
                    result.Items = items;
                    result.Result = ServiceResultStatus.Failure;
                }

                else
                {
                        TaskList existtask = this._unitOfWork.RepositoryAsync<TaskList>()
                                        .Query().Filter(a => a.TaskDescription == TaskDescription && a.IsActive == true && a.IsStandardTask==IsStandardCheckbox).Get().FirstOrDefault();
                    

                    if (existtask == null)
                    {

                        TaskList TaskToupdate = this._unitOfWork.RepositoryAsync<TaskList>().Find(TaskID);
                        TaskToupdate.TaskDescription = TaskDescription;
                        TaskToupdate.UpdatedBy = updatedBy;
                        TaskToupdate.UpdatedDate = DateTime.Now;
                        TaskToupdate.IsStandardTask = IsStandardCheckbox;
                        this._unitOfWork.RepositoryAsync<TaskList>().Update(TaskToupdate);
                        this._unitOfWork.Save();
                        result.Result = ServiceResultStatus.Success;
                    }
                    else
                    {
                        items.Add(new ServiceResultItem() { Messages = new string[] { "Task already exists" } });
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

        public ServiceResult DeleteTask(int TaskID, string updatedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {

                var finalizedTask = (this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>()
                                                                      .Query()
                                                                      .Filter(c => c.TaskID == TaskID && (c.Status == "Assigned" || c.Status == "Completed" || c.Status == "In Progress"))
                                                                      .Get()
                                    ).ToList();
                if (finalizedTask != null && finalizedTask.Count() > 0)
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "Task can not be deleted as it is used in finalized workflow" } });
                    result.Items = items;
                    result.Result = ServiceResultStatus.Failure;
                }
                else
                {
                    TaskList TaskTodelete = this._unitOfWork.RepositoryAsync<TaskList>().Find(TaskID);
                    TaskTodelete.IsActive = false;
                    TaskTodelete.UpdatedBy = updatedBy;
                    TaskTodelete.UpdatedDate = DateTime.Now;
                    this._unitOfWork.RepositoryAsync<TaskList>().Update(TaskTodelete);
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

        public IQueryable<TaskListViewModel> GetDPFTasksMasterList(int tenantId, int? userId, int wfStateId)
        {
            IQueryable<TaskListViewModel> DPFTasksList = null;
            try
            {
                DPFTasksList = (from dpftask in this._unitOfWork.RepositoryAsync<TaskList>().Get().Where(b => b.IsActive == true)
                                join dpfWFStateTaskMap in this._unitOfWork.RepositoryAsync<WorkflowTaskMap>().Get().Where(a => a.IsActive == true
                                 && a.WFStateID == wfStateId)
                                                    on dpftask.TaskID equals dpfWFStateTaskMap.TaskID
                                select new TaskListViewModel
                                {
                                    TaskID = dpftask.TaskID,
                                    TaskDescription = dpftask.TaskDescription
                                }).AsQueryable();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return DPFTasksList;
        }
    }
}
