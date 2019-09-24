using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.DashBoard;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.web.Framework;



namespace tmg.equinox.web.Controllers
{
    public class TaskController : AuthenticatedController
    {

        #region Private Variables
        private ITaskListService taskListService;
        private string sendGridUserName = string.Empty;
        private string sendGridPassword = string.Empty;
        private string smtpUserName = string.Empty;
        private string smtpPassword = string.Empty;
        private string smtpPort = string.Empty;
        private string smtpHostServerName = string.Empty;

        #endregion

        #region Constructor

        public TaskController(ITaskListService taskListService)
        { 
            this.taskListService = taskListService;
        }

        #endregion
        // GET: Task
        public JsonResult GetTaskList(int tenantID, GridPagingRequest gridPagingRequest)
        {
            List<TaskListViewModel> TaskList = this.taskListService.GetTaskList(tenantID);
            return Json(TaskList, JsonRequestBehavior.AllowGet);
        }
        //Add:Task
        public ActionResult AddTask(int tenantId, string TaskDescription,Boolean IsStandardCheckbox)
        {
            ServiceResult result = this.taskListService.AddTask(tenantId, TaskDescription, CurrentUserName, IsStandardCheckbox);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //Update:Task
        public ActionResult UpdateTask(int tenantId, int TaskID, string TaskDescription, bool IsStandardCheckbox)
        {
            ServiceResult result = this.taskListService.UpdateTask(TaskID, TaskDescription, CurrentUserName, IsStandardCheckbox);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //Delete:Task
        public ActionResult DeleteTask(int TaskID)
        {
            ServiceResult result = this.taskListService.DeleteTask(TaskID, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDPFTasksMasterList(int wfStateId)
        {

            IQueryable<TaskListViewModel> objList;
            try
            {
                objList = this.taskListService.GetDPFTasksMasterList(base.TenantID, base.CurrentUserId, wfStateId);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(objList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckTasksAlreadyExists(string strTaskName)
        {

            bool isTasksAlreadyExist = false;
            try
            {
                List<TaskListViewModel> TaskList = this.taskListService.GetTaskList(base.TenantID);
                if(TaskList != null && TaskList.Count > 0)
                {
                    List<TaskListViewModel> countTask = TaskList.Where(a => a.TaskDescription == strTaskName).ToList();
                    if(countTask != null && countTask.Count > 0)
                    {
                        isTasksAlreadyExist = true;
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(isTasksAlreadyExist, JsonRequestBehavior.AllowGet);
        }
    }
}