using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tmg.equinox.applicationservices.viewmodels.Consortium;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.interfaces;
using System.Web.Http.Description;
using tmg.equinox.services.api.Validators;
using tmg.equinox.services.api.Framework;
using tmg.equinox.services.api.Models;
using FluentValidation.Attributes;

namespace tmg.equinox.services.api.Controllers
{
    [Authorize]
    public class ConsortiumController : BaseApiController
    {
        private IConsortiumService _consortiumService;

        public ConsortiumController(IConsortiumService consortiumService)
        {
            _consortiumService = consortiumService;
        }

        /// <summary>
        /// Get the list of Consortiums.
        /// </summary>
        /// <param></param>
        /// <returns>Returns the list of Consortiums.</returns>
        [HttpGet]
        [Route("api/v1/Consortiums")]
        [ResponseType(typeof(Consortium))]
        public HttpResponseMessage Get()
        {
            var consortiumList = this._consortiumService.GetConsortiumForDropdown(TenantId);
            if (consortiumList == null)
            {
                return CreateResponse(new { Status = "Failure", Message = Constants.ConsortiumNotExist });
            }
            var consortiums = (from cL in consortiumList
                                 select new Consortium
                                 {
                                     ConsortiumID = cL.ConsortiumID ?? default(int),
                                     ConsortiumName = cL.ConsortiumName
                                 });
            return CreateResponse(new { Status = "Success", Result = consortiums });
        }

        /// <summary>
        /// Add a new Consortium.
        /// </summary>
        /// <param name="consortium"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/Consortiums")]
        public HttpResponseMessage Add([FromBody] ConsortiumAdd consortium)
        {
            if (String.IsNullOrEmpty(consortium.ConsortiumName))
            {
                return CreateResponse(new { Status = "Failure", Message = Constants.ConsortiumNameEmpty });
            }
            ServiceResult result = this._consortiumService.AddConsortium(consortium.ConsortiumName, TenantId, CurrentUserName);
            if (result.Result == ServiceResultStatus.Success)
                return CreateResponse(new { Status = "Success", Message = "Consortium added Successfully.", ConsortiumID = result.Items.First().Messages.First() });
            else
            {
                if (result.Result == ServiceResultStatus.Failure)
                {
                    if (result.Items != null && result.Items.Count() != 0)
                        return CreateResponse(new { Status = "Failure", Message = result.Items.First().Messages.First() });
                }
            }

            return CreateResponse(result);

        }

        /// <summary>
        /// Update an existing Consortium by id.
        /// </summary>
        /// <param name="consortium"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/v1/Consortiums")]
        public HttpResponseMessage Update([FromBody] Consortium consortium)
        {
            int? consortiumExists = this._consortiumService.GetConsortiumId(consortium.ConsortiumName);
            if (consortiumExists == null)
            {
                return CreateResponse(new { Status = "Failure", Message = Constants.ConsortiumNotExist });
            }
            ServiceResult result = this._consortiumService.UpdateConsortium(consortium.ConsortiumID, consortium.ConsortiumName, TenantId, CurrentUserName);
            if (result.Items != null && result.Items.Count() > 0)
            {
                if (result.Result == ServiceResultStatus.Success)
                    return CreateResponse(new { Status = "Success", Message = result.Items.First().Messages.First() });
                else if (result.Result == ServiceResultStatus.Failure)
                    return CreateResponse(new { Status = "Failure", Message = result.Items.First().Messages.First() });
            }
            return CreateResponse(result);
        }

        /// <summary>
        /// Get a Consortium by FolderVersion.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns the Consortium details for the specified Folder Version</returns>
        [HttpGet]
        [Route("api/v1/Consortium/FolderVersions/{id}")]
        [ResponseType(typeof(Consortium))]
        public HttpResponseMessage GetConsortiumByFolderVersionId(int id)
        {
            ConsortiumViewModel consortium = this._consortiumService.GetConsortium(id);
            if (consortium == null)
            {
                return CreateResponse(new { Status = "Failure", Message = Constants.ConsortiumNotExist });
            }

            Consortium consort = new Consortium();
            consort.ConsortiumID = consortium.ConsortiumID ?? default(int);
            consort.ConsortiumName = consortium.ConsortiumName;

            return CreateResponse(new { Status = "Success", Result = consort });
        }

        /// <summary>
        /// Get Consortium by Name.
        /// </summary>
        /// <param name="consortium"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/Consortiums/ConsortiumName")]
        public HttpResponseMessage GetConsortiumByName([FromBody] ConsortiumAdd consortium)
        {
            if (String.IsNullOrEmpty(consortium.ConsortiumName))
            {
                return CreateResponse(new { Status = "Failure", Message = Constants.ConsortiumNameEmpty });
            }
            int? consortiumExists = this._consortiumService.GetConsortiumId(consortium.ConsortiumName);
            if (consortiumExists == null)
            {
                return CreateResponse(new { Status = "Failure", Message = Constants.ConsortiumNotExist });
            }
            Consortium consort = new Consortium();
            consort.ConsortiumID = consortiumExists ?? default(int);
            consort.ConsortiumName = consortium.ConsortiumName;

            return CreateResponse(new { Status = "Success", Result = consort });

        }
    }
}
