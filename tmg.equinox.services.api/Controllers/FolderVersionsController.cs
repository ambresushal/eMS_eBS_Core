using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.services.api.Models;
using tmg.equinox.services.api.Validators;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using tmg.equinox.domain.entities;
using Newtonsoft.Json;
using tmg.equinox.applicationservices.FolderVersionDetail;
using FolderVersionState = tmg.equinox.domain.entities.Enums.FolderVersionState;
using tmg.equinox.applicationservices.viewmodels.DashBoard;
using System.Configuration;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using tmg.equinox.services.api.Framework;
using tmg.equinox.services.api.Services;
namespace tmg.equinox.services.api.Controllers
{
    [Authorize]
    public class FolderVersionsController : BaseApiController
    {
        private IFolderVersionServices _folderVersionService;
        private IDashboardService _dashboardService;
        private IConsumerAccountService _consumerAccountService;
        private string sendGridUserName = string.Empty;
        private string sendGridPassword = string.Empty;
        private string smtpUserName = string.Empty;
        private string smtpPassword = string.Empty;
        private string smtpPort = string.Empty;
        private string smtpHostServerName = string.Empty;
        private IWorkFlowStateServices _workFlowStateService;
        public FolderVersionsController(IFolderVersionServices folderVersionServices, IDashboardService dashboardservice, IWorkFlowStateServices workFlowStateService, IConsumerAccountService consumerAccountService)
        {
            this._folderVersionService = folderVersionServices;
            this._dashboardService = dashboardservice;
            this._workFlowStateService = workFlowStateService;
            this._consumerAccountService = consumerAccountService;
        }

        /// <summary>
        /// Get a Folder Version by FolderID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a Folder Version</returns>
        [HttpGet]
        [Route("api/v1/FolderVersions/{id}")]
        [ResponseType(typeof(FolderVersionModel))]
        public HttpResponseMessage GetVersionById(int id)
        {
            var folderVersion = _folderVersionService.GetVersionById(id);
            return CreateResponse(folderVersion, tmg.equinox.services.api.Validators.Constants.FolderVersionNotExist, HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Get the list of Folder Versions by a folder.
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Folders/{folderId}/FolderVersions")]
        [ResponseType(typeof(FolderVersionModel))]
        public HttpResponseMessage GetVersionsByFolder(int folderId)
        {
            var versions = _folderVersionService.GetVersionsByFolder(folderId);
            if (versions == null || versions.Count() == 0)
            {
                HttpError myCustomError = new HttpError(tmg.equinox.services.api.Validators.Constants.FolderVersionsDoNotExist);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, myCustomError);
            }
            return Request.CreateResponse(HttpStatusCode.OK, versions);
        }

        /// <summary>
        /// Add a Folder Version to a folder.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="folderVersion"></param>
        /// <returns>Returns newly added Folder Version id</returns>
        [HttpPost]
        [Route("api/v1/FolderVersions")]
        public HttpResponseMessage Add([FromBody]FolderVersion folderVersion)
        {
            ServiceResult result;
            result = new ServiceResult();
            _folderVersionService.AddFolderVersion(1, folderVersion.EffectiveDate, CurrentUserName, folderVersion.folderId, null, null, Convert.ToInt32(folderVersion.CategoryId), folderVersion.CategoryId, CurrentUserId);
            //var AddFolderVersionService = new AddFolderVersionService(_folderVersionService, _workFlowStateService, _consumerAccountService, this);
            //result = AddFolderVersionService.AddFolderVersion(folderVersion.folderId,folderVersion.EffectiveDate,folderVersion.Category,folderVersion.CategoryId,base.TenantId,CurrentUserId,CurrentUserName);
            return CreateResponse(result);
        }

        ///<summary>
        ///Update Folder Version.
        ///</summary>
        /// <param name="folderVersion"></param>
        [HttpPut]
        [Route("api/v1/FolderVersions/Update")]
        public HttpResponseMessage Update([FromBody]FolderVersionToUpdate folderVersion)
        {
            string resultMessage = string.Empty;
            ServiceResult result;
            result = new ServiceResult();
            FolderVersionViewModel folderVersiondetails = _folderVersionService.GetLatestFolderVersion(base.TenantId, folderVersion.folderId);
            if (folderVersiondetails != null && folderVersiondetails.FolderVersionStateID == 3)
            {
                result.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Requested folder State is Released folder version hence could not be update as minor folder version!" } });
                return Request.CreateResponse(result);
            }
            else if (folderVersiondetails != null && folderVersiondetails.FolderVersionStateID == 5)
            {
                result.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Requested folder State is Cancelled folder version hence could not be update as minor folder version!" } });
                return Request.CreateResponse(result);
            }
            else if (folderVersiondetails != null)
            {
                tmg.equinox.applicationservices.FolderVersionDetail.VersionNumberBuilder builder = new tmg.equinox.applicationservices.FolderVersionDetail.VersionNumberBuilder();
                string newFolderVersionNumber = builder.GetNextMinorVersionNumber(folderVersiondetails.FolderVersionNumber, folderVersiondetails.EffectiveDate);

                result = _folderVersionService.BaseLineFolder(base.TenantId, 0, folderVersion.folderId, folderVersiondetails.FolderVersionId, CurrentUserId,
                CurrentUserName, newFolderVersionNumber, folderVersion.comments, 0, folderVersiondetails.EffectiveDate, false, isNotApproved: false, isNewVersion: false);

                if (result.Result == ServiceResultStatus.Success)
                {
                    string newFolderVersionId = result.Items.First().Messages.First();
                    resultMessage = "Folder is baselined and new folderVersionId is  " + newFolderVersionId + "";
                }
            }
            else
            {
                result.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Please check the FolderId entered. There are no details associated to it. " } });
                return Request.CreateResponse(result);
            }
            return Request.CreateResponse(resultMessage);
        }

        /// <summary>
        /// Delete a Folder Version.
        /// </summary>
        /// <param name="folderVersiontodelete"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/v1/FolderVersions")]
        public HttpResponseMessage Delete([FromBody]FolderVersionToDelete folderVersiontodelete)
        {
            string currentUserName = RequestContext.Principal.Identity.Name;
            string resultMessage = string.Empty;
            //TODO: Finalized version should not be allowed to delete.
            //if (!_folderVersionService.IsFolderVersionExist(folderVersiontodelete.folderversionId))
            {
                HttpError myCustomError = new HttpError(tmg.equinox.services.api.Validators.Constants.FolderVersionNotExist);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, myCustomError);
            }
            ServiceResult result = _folderVersionService.DeleteFolderVersion(base.TenantId, folderVersiontodelete.folderId, folderVersiontodelete.folderversionId, folderVersiontodelete.versionType, currentUserName);

            if (result.Items != null)
            {
                resultMessage = result.Items.First().Messages.First();
            }

            if (result.Result == ServiceResultStatus.Failure)
            {
                HttpError myCustomError = new HttpError(resultMessage);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, myCustomError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, resultMessage);
        }

    }
}
