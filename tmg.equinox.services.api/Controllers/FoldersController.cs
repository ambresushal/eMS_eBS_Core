using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;
using tmg.equinox.applicationservices.interfaces;
//using tmg.equinox.applicationservices.viewmodels.Folder;
using tmg.equinox.services.api.Models;
using tmg.equinox.services.api.Validators;
using tmg.equinox.services.api.Framework;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.web.FormInstance;

namespace tmg.equinox.services.api.Controllers
{
    [Authorize]
    public class FoldersController : BaseApiController
    {  
        private IFolderService _folderService;
        private IConsumerAccountService _consumerAccountService;
        private IFolderVersionServices _folderVersionService;

        public FoldersController(IFolderService folderService, IFolderVersionServices folderVersionService, IConsumerAccountService consumerAccountService)
        {
            this._folderService = folderService;
            this._folderVersionService = folderVersionService;
            this._consumerAccountService = consumerAccountService;
        }

        /// <summary>
        /// Get the list of Folders by an account.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Accounts/{id}/Folders")]
        [ResponseType(typeof(FolderViewModel))]
        public HttpResponseMessage GetByAccountId(int id)
        {
            int total = 0;

            var folders = _folderService.GetFoldersByAccount(id);

            var linkBuilder = new PageLinkBuilder(Url, "Folders", null, 1, 10, total);

            var response = Request.CreateResponse(HttpStatusCode.OK, new PagedResult<FolderViewModel>(folders, 1, 10, total));

            response.Headers.Add("Link", string.Join(", ", PageLinkBuilder.GetPageLink(linkBuilder)));

            return response;
        }

        /// <summary>
        /// Get a Folder by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Folders/{id}")]
        [ResponseType(typeof(FolderViewModel))]
        public HttpResponseMessage GetFolderById(int id)
        {
            var folder = _folderService.GetFolderById(id);
            if (folder == null)
            {
                HttpError myCustomError = new HttpError(Constants.FolderNotExist);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, myCustomError);
            }
            return Request.CreateResponse(HttpStatusCode.OK, folder);
        }

        /// <summary>
        /// Get the list of the Folders - Non Portfolio.
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Folders")]
        [ResponseType(typeof(FolderViewModel))]
        public HttpResponseMessage Get(int pageNo = 1, int pageSize = 10)
        {
            int skip = (pageNo - 1) * pageSize;
            int total = 0;

            var folders = _folderService.GetFolderList(skip, pageSize, ref total);

            var linkBuilder = new PageLinkBuilder(Url, "Get", null, pageNo, pageSize, total);

            var response = Request.CreateResponse(HttpStatusCode.OK, new PagedResult<FolderViewModel>(folders, pageNo, pageSize, total));

            response.Headers.Add("Link", string.Join(", ", PageLinkBuilder.GetPageLink(linkBuilder)));

            return response;
        }

        /// <summary>
        /// Get the list of Folders - Portfolio.
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Route("api/v1/Folders/Portfolio")]
        [HttpGet]
        [ResponseType(typeof(FolderViewModel))]
        public HttpResponseMessage GetPortfolio(int pageNo = 1, int pageSize = 10)
        {
            int skip = (pageNo - 1) * pageSize;
            int total = 0;

            var folders = _folderService.GetPortfolioFolders(skip, pageSize, ref total);

            var linkBuilder = new PageLinkBuilder(Url, "Portfolio", null, pageNo, pageSize, total);

            var response = Request.CreateResponse(HttpStatusCode.OK, new PagedResult<FolderViewModel>(folders, pageNo, pageSize, total));

            response.Headers.Add("Link", string.Join(", ", PageLinkBuilder.GetPageLink(linkBuilder)));

            return response;
        }

        /// <summary>
        /// Get a Folder by id - Portfolio 
        /// </summary>
        /// <param name="id"></param> 
        /// <returns></returns>
        [Route("api/v1/Folders/Portfolio/{id}")]
        [HttpGet]
        [ResponseType(typeof(FolderViewModel))]
        public HttpResponseMessage GetPortfolioFoldersById(int id)
        {
            var folders = _folderService.GetFolderById(id);

            if (folders == null)
            {
                HttpError myCustomError = new HttpError(Constants.FolderNotExist);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, myCustomError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, folders);

        }

        ///<summary>
        ///Get Folder by Name.
        ///</summary>
        ///<param name="folderName"></param>
        /// <returns></returns>
        [Route("api/v1/Folders/{folderName}/FolderName")]
        [HttpGet]
        public HttpResponseMessage GetFolderByName(string folderName)
        {
            var folder = _folderService.GetFolderByName(folderName);
            return CreateResponse(folder, Constants.FolderNotExist, HttpStatusCode.NotFound); 
        } 

        /// <summary>
        /// Add a new Folder.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/Folders")]
        public HttpResponseMessage Add([FromBody]Folder folder)
        {
            string resultMessage = string.Empty;
            int marketSegment = 1;
            int category = 0;  
           
            //marketSegment = _consumerAccountService.GetMarketSegmentId(folder.MarketSegment);
            category = _consumerAccountService.GetFolderCategoryId(folder.Category);
            ServiceResult result = _consumerAccountService.AddFolder(base.TenantId,folder.AccountID,folder.FolderName,folder.EffectiveDate,false,
                                    base.CurrentUserId, RequestContext.Principal.Identity.Name, marketSegment, null, category, folder.CategoryId, RequestContext.Principal.Identity.Name);

            if (result.Result == ServiceResultStatus.Success)
            {
                if (result.Items != null)
                {
                    resultMessage = result.Items.First().Messages[2];
                }
            }

            if (result.Result == ServiceResultStatus.Failure)
            {
                if (result.Items != null)
                {
                    resultMessage = result.Items.First().Messages.First();
                }

                HttpError myCustomError = new HttpError(resultMessage);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, myCustomError);
            }
            return Request.CreateResponse(HttpStatusCode.Created, new { id = resultMessage });
        }
        ///<summary>
        ///Update a folder by id
        ///</summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        /// Confirm once
        [HttpPut]
        [Route("api/v1/Folders")]
        public HttpResponseMessage Folders([FromBody] Folder folder)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        /// <summary>
        /// Delete a Folder by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/v1/Folders/{id}")]
        public HttpResponseMessage Delete(int id)
        { 
            ServiceResult result = new ServiceResult();
            result = _folderVersionService.DeleteNonPortfolioBasedFolder(base.TenantId, id);
            return CreateResponse(result);  
        }

    }
}
