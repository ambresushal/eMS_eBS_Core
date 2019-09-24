using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using tmg.equinox.applicationservices.interfaces;
using System.Web.Http.Description;
using tmg.equinox.identitymanagement.Models;
using Microsoft.AspNet.Identity.Owin;
using tmg.equinox.services.api.Models;


namespace tmg.equinox.services.api.Framework
{
    public class BaseApiController : ApiController
    {
        private ApplicationUserManager _AppUserManager = null;
        private ApplicationRoleManager _AppRoleManager = null;
        private ModelFactory _modelFactory;
        public int TenantId
        {
            get
            {
                Int16 tenantId = 1;
                return tenantId;
            }
        }

        public int CurrentUserId
        {
            get
            {
                string userId = RequestContext.Principal.Identity.GetUserId();
                return Convert.ToInt32(userId);
            }
        }

        public string CurrentUserName
        {
            get
            {
                return RequestContext.Principal.Identity.Name;
            }
        }
        protected ModelFactory TheModelFactory
        {
            get
            {
                if (_modelFactory == null)
                {
                    _modelFactory = new ModelFactory(this.Request, this.AppUserManager);
                }
                return _modelFactory;
            }
        }
        protected ApplicationUserManager AppUserManager
        {
            get
            { 
                return _AppUserManager ??  Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        protected ApplicationRoleManager AppRoleManager
        {
            get
            {
                return _AppRoleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage CreateResponse(ServiceResult result, bool partialSuccess)
        {
            string resultMessage = string.Empty;
            if (result.Items != null && result.Items.Count() != 0)
            {
                //always first item will be success or failore
                resultMessage = result.Items.First().Messages.First();
                HttpError myCustomError = new HttpError();
                if (result.Result == ServiceResultStatus.Failure) //if main havinf error then return error response
                {
                    myCustomError = new HttpError(resultMessage);
                }
              //  int i = 0;

                string sucessMsg = "";
                object warningMsg = null;
                foreach (var item in result.Items)
                {
                    if (item.Status==ServiceResultStatus.Success)
                    {
                        sucessMsg = item.Messages[0];
                    }
                    if (item.Status == ServiceResultStatus.Warning)
                    {
                        if(item.Messages == null)
                        {
                            ServiceResultResponse serviceResultResponse = (ServiceResultResponse)item;
                            if (serviceResultResponse != null)
                                warningMsg = serviceResultResponse.Messages;
                        }
                        else
                            warningMsg = item.Messages[0]; 
                    }

                    //i++;
                    //myCustomError.Add(i.ToString(), item.Messages);
                }
                
                myCustomError.Add(ServiceResultStatus.Success.ToString(), sucessMsg);
                myCustomError.Add(ServiceResultStatus.Warning.ToString(), warningMsg);

                if (result.Result == ServiceResultStatus.Failure)//if main havinf error then return error response
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, myCustomError);
                }

                return Request.CreateErrorResponse(HttpStatusCode.OK, myCustomError);

            }
            return Request.CreateResponse(HttpStatusCode.OK, resultMessage);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage CreateResponseTemp(ServiceResult result, bool partialSuccess)
        {
            string resultMessage = string.Empty;
            if (result.Items != null && result.Items.Count() != 0)
            {
                //always first item will be success or failore
                resultMessage = result.Items.First().Messages.First();
                HttpError myCustomError = new HttpError();
                if (result.Result == ServiceResultStatus.Failure) //if main havinf error then return error response
                {
                    myCustomError = new HttpError(resultMessage);
                }

                //  int i = 0;

                //string sucessMsg = "";
                //string warningMsg = "";
                //foreach (var item in result.Items)
                //{
                //    if (item.Status == ServiceResultStatus.Success)
                //    {
                //        sucessMsg += item.Messages[0];
                //    }
                //    if (item.Status == ServiceResultStatus.Warning)
                //    {
                //        warningMsg += item.Messages[0];
                //    }

                //    //i++;
                //    //myCustomError.Add(i.ToString(), item.Messages);
                //}

                myCustomError.Add(ServiceResultStatus.Success.ToString(), result.Items);
                myCustomError.Add(ServiceResultStatus.Warning.ToString(), result.Items);

                if (result.Result == ServiceResultStatus.Failure)//if main havinf error then return error response
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, myCustomError);
                }

                return Request.CreateErrorResponse(HttpStatusCode.OK, myCustomError);

            }
            return Request.CreateResponse(HttpStatusCode.OK, resultMessage);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage CreateResponse<T>(T doc)
        {
            
            return Request.CreateResponse(HttpStatusCode.OK, doc);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage CreateResponse<T>(T doc, string constantKey, HttpStatusCode httpStatusCode)
        {
            if (doc == null)
            {
                HttpError myCustomError = new HttpError(constantKey);
                return Request.CreateErrorResponse(httpStatusCode, myCustomError);
            }
            return CreateResponse(doc);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage CreateResponse(ServiceResult result)
        {
            string resultMessage = string.Empty;
            if (result.Items != null && result.Items.Count() > 0)
            {
                //always first item will be success or failore
                resultMessage = result.Items.First().Messages.First();
            }

            if (result.Result == ServiceResultStatus.Failure)
            {
                HttpError myCustomError = new HttpError(resultMessage);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, myCustomError);
            }
            if (!string.IsNullOrEmpty(resultMessage))
                return Request.CreateResponse(HttpStatusCode.OK, resultMessage);
            else
                return Request.CreateResponse(HttpStatusCode.OK);
        }

    }
    public class ReponseMessage
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
