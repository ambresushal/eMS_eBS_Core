using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Description;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.domain.entities;
using tmg.equinox.services.api.Framework;

namespace tmg.equinox.services.api.Controllers
{
    [Authorize]
    public class MDMSyncController : BaseApiController
    {
        IMDMSyncDataService _iMDMSyncDataService;
        public MDMSyncController(IMDMSyncDataService mDMSyncDataService)
        {
            _iMDMSyncDataService = mDMSyncDataService;
        }

        /// <summary>
        /// Get the MDM Sync Status of form Instance
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/MDMSync/MDMStatus")]
        [HttpGet]
        [ResponseType(typeof(string))]
        public HttpResponseMessage GetMDMStatus(int formInstanceID)
        {
            var status = _iMDMSyncDataService.GetDocumentUpdateTrackerStatusByFormInstanceId(formInstanceID);

            if (status != null)
            {
                MSMSyncStatus enumDisplayStatus = (MSMSyncStatus)status.Status;
                return CreateResponse(new { Status = Validators.Constants.Success, Result = enumDisplayStatus.ToString() });
            }
            return CreateResponse(new { Status = Validators.Constants.Failure, Message = Validators.Constants.NotExist });
        }

        /// <summary>
        /// Create Application Task
        /// </summary>
        /// <param name="formInstanceID"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(List<ReponseMessage>))]
        [Route("api/v1/MDMSync/UpdateMDMStatus")]
        public HttpResponseMessage UpdateDocumentUpdateTracker(int formInstanceID, int status)
        {
            try
            {
                _iMDMSyncDataService.UpdateDocumentUpdateTracker(formInstanceID, status);
                return CreateResponse(new ReponseMessage { Status = Validators.Constants.Success, Message = Validators.Constants.UpdateSuccess });

            }
            catch (Exception e)
            {
                return CreateResponse(new { Status = Validators.Constants.Failure, Message = Validators.Constants.Failure });
            }
        }
    }
}