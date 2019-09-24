using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.services.api.Framework;
using tmg.equinox.services.api.Services;
//using tmg.equinox.web.Validator;

namespace tmg.equinox.services.api.Controllers
{
    [Authorize]
    public class WorkFlowsController : BaseApiController
    {  
        private IWorkFlowStateServices _workFlowStateService;
        private IFolderVersionServices _folderVersionServices;
        private IFormDesignService _formDesignServices;
        private IMasterListService _masterListServices;

        public WorkFlowsController(IWorkFlowStateServices workFlowStateService, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, IMasterListService masterListServices)
        {
            this._workFlowStateService = workFlowStateService;
            this._formDesignServices = formDesignServices;
            this._folderVersionServices = folderVersionServices;
            this._masterListServices = masterListServices;
        } 

        ///<summary>
        /// Gets the current Workflow State.
        /// </summary> 
        /// <param name="id"></param>
        /// <returns>Return the current Workflow State of the Folder Version.</returns>
        [HttpGet]
        [Route("api/v1/WorkFlows/{id}")]
        public HttpResponseMessage GetWorkFlowState(int id)
        {
            var wFState = _workFlowStateService.GetWorkFlowState(base.TenantId, id);
            return CreateResponse(wFState, tmg.equinox.services.api.Validators.Constants.FolderVersionNotExist, HttpStatusCode.NotFound);           
        }

        ///<summary>
        ///Update the Workflow State.
        ///</summary>
        ///<param name="id"></param>   
        [HttpPut]
        [Route("api/v1/WorkFlows/{id}")]
        public HttpResponseMessage UpdateWorkFlowState(int id)
        {
            throw new NotImplementedException();
            //var workFlowFolderVersionStatusChanger = new WorkFlowFolderVersionStatusChanger(_workFlowStateService,
            //    _folderVersionServices, _formDesignServices, _masterListServices, this);
            
            //return CreateResponse(workFlowFolderVersionStatusChanger.UpdateWorkFlowState(id)); 
        }
    }
}
