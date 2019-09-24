using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.DPF;
using tmg.equinox.services.api.Framework;

namespace tmg.equinox.services.api.Controllers
{
    [Authorize]
    public class PlanTaskUserMappingController : BaseApiController
    {
        private IDashboardService _dashboardService;
        private IPlanTaskUserMappingService _planTaskUserMappingService;

        public PlanTaskUserMappingController(IDashboardService dashboardService, IPlanTaskUserMappingService planTaskUserMappingService)
        {
            this._dashboardService = dashboardService;
            _planTaskUserMappingService = planTaskUserMappingService;
        }


        /// <summary>
        /// Create Plan Task User Mapping.
        /// </summary>
        /// <param name="planTaskModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/PlanTaskUserMapping")]
        public HttpResponseMessage Add([FromBody]DPFPlanTaskUserMappingViewModel planTaskModel)
        {
            planTaskModel.AddedBy = base.CurrentUserName;
            planTaskModel.UpdatedBy = base.CurrentUserName;
            planTaskModel.ManagerUserName = base.CurrentUserName;
            var result = this._planTaskUserMappingService.SavePlanTaskUserMapping(planTaskModel);
            return CreateResponse(result);
        }


        /// <summary>
        /// Update Plan Task User Mapping.
        /// </summary>
        /// <param name="planTaskModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/v1/PlanTaskUserMapping")]
        public HttpResponseMessage Update([FromBody]DPFPlanTaskUserMappingViewModel planTaskModel)
        {
            planTaskModel.UpdatedDate = DateTime.Now;
            planTaskModel.UpdatedBy = base.CurrentUserName;
            planTaskModel.ManagerUserName = base.CurrentUserName;
            var result = this._planTaskUserMappingService.UpdatePlanTaskUserMapping(planTaskModel);
            return CreateResponse(result);
        }

        /// <summary>
        /// Get list of Tasks for a folder.
        /// </summary>
        /// <param name="folderid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/PlanTaskUserMapping")]
        public HttpResponseMessage GetTasksForFolder(int folderid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get list of Work Queue Tasks.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/PlanTaskUserMapping/WorkQueue")]
        public HttpResponseMessage GetWorkQueueTasks(int userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get list of Work Queue Open Tasks.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/PlanTaskUserMapping/WorkQueue/OpenTasks")]
        public HttpResponseMessage GetWorkQueueOpenTasks(int userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get list of Watch List Tasks.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/PlanTaskUserMapping/WatchList")]
        public HttpResponseMessage GetWatchListTasks(int userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update Task status.
        /// </summary>
        /// <param name="taskId"></param>
        ///<param name="status"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/v1/PlanTaskUserMapping/TaskStatus")]
        public HttpResponseMessage UpdatetaskStatus(int taskId, string status)
        {
            throw new NotImplementedException();
        }
    }
}