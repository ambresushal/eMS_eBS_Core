using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.services.api.Framework;

namespace tmg.equinox.services.api.Controllers
{
    [Authorize]
    public class TaskController : BaseApiController
    {
        private ITaskListService _taskListService;
        private IWorkFlowTaskMappingService _workFlowTaskMappingService;
        public TaskController(ITaskListService taskListService, IWorkFlowTaskMappingService workFlowTaskMappingService)
        {
            this._taskListService = taskListService;
            _workFlowTaskMappingService = workFlowTaskMappingService;
    }

        /// <summary>
        /// Get the list of application tasks
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/Task/tasks")]
        [HttpGet]
        [ResponseType(typeof(List<TaskListViewModel>))]
        public HttpResponseMessage GetTasks()
        {
            var tasks = _taskListService.GetTaskList(1);

            if (tasks != null)
            {
                return CreateResponse(new { Status = Validators.Constants.Success, Result = tasks });
            }
            return CreateResponse(new { Status = Validators.Constants.Failure, Message = Validators.Constants.NotExist });
        }

        /// <summary>
        /// Create Application Task
        /// </summary>
        /// <param name="TaskDescription"></param>
        /// <param name="isStandardTask"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(List<ReponseMessage>))]
        [Route("api/v1/Task/CreateTask")]
        public HttpResponseMessage Create(string TaskDescription, Boolean isStandardTask)
        {
            try
            {
                var result = this._taskListService.AddTask(1, TaskDescription, CurrentUserName, isStandardTask);
                if (result.Result == ServiceResultStatus.Success)
                {
                    return CreateResponse(new ReponseMessage { Status = Validators.Constants.Success, Message = Validators.Constants.CreateSuccess });
                }
                else
                {
                    return CreateResponse(new { Status = Validators.Constants.Failure, Message = Validators.Constants.Failure });
                }
            }
            catch (Exception e)
            {
                return CreateResponse(new { Status = Validators.Constants.Failure, Message = Validators.Constants.Failure });
            }
        }

        /// <summary>
        /// Get the list of tasks associated with WorkFlow state
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/Task/WorkFlowStateTasks")]
        [HttpGet]
        [ResponseType(typeof(List<TaskListViewModel>))]
        public HttpResponseMessage GetWorkFlowStateTasks(int workFlowStateId, int taskId)
        {
            var tasks = _workFlowTaskMappingService.GetApplicableWfTaskList(1, workFlowStateId);
            if (tasks != null)
            {
                return CreateResponse(new { Status = Validators.Constants.Success, Result = tasks });
            }
            return CreateResponse(new { Status = Validators.Constants.Failure, Message = Validators.Constants.NotExist });
        }

        /// <summary>
        /// Add workflow state - task mapping
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/Task/WorkFlowStateTasks")]
        [HttpPut]
        [ResponseType(typeof(List<TaskListViewModel>))]
        public HttpResponseMessage AddWorkFlowStateTasksMapping(int workFlowStateId, int taskId)
        {
            var taskModel = new ApplicableTaskMapModel() { TaskID = taskId };
            var taskModelList = new List<ApplicableTaskMapModel>();
            taskModelList.Add(taskModel);
            var result = _workFlowTaskMappingService.UpdateApplicableWFTaskMap(1, workFlowStateId, taskModelList, CurrentUserName);
            return CreateResponse(result); 
        }


        /// <summary>
        /// Remove workflow state - task mapping
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/Task/WorkFlowStateTasks")]
        [HttpPut]
        [ResponseType(typeof(List<TaskListViewModel>))]
        public HttpResponseMessage RemoveWorkFlowStateTasksMapping(int workFlowStateId, int taskId)
        {
            var taskModel = new ApplicableTaskMapModel() { TaskID = taskId };
            var taskModelList = new List<ApplicableTaskMapModel>();
            taskModelList.Add(taskModel);
            var result = _workFlowTaskMappingService.UpdateApplicableWFTaskMap(1, workFlowStateId, taskModelList, CurrentUserName);
            return CreateResponse(result);
        }
    }
}