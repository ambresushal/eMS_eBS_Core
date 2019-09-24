using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using tmg.equinox.applicationservices.interfaces;

namespace tmg.equinox.services.api.Controllers
{
    public class BaseController:ApiController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage CreateResponse(ServiceResult result, bool partialSuccess)
        {
            string resultMessage = string.Empty;
            if (result.Items != null)
            {
                //always first item will be success or failore
                   resultMessage = result.Items.First().Messages.First();
                   HttpError myCustomError = new HttpError();
                    if (result.Result == ServiceResultStatus.Failure) //if main havinf error then return error response
                    {
                        myCustomError = new HttpError(resultMessage);
                    }

                    foreach (var item in result.Items)
                    {
                        myCustomError.Add(item.Status.ToString(), item.Messages);
                    }

                    if (result.Result == ServiceResultStatus.Failure)//if main havinf error then return error response
                    {                       
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, myCustomError);
                    }

                    return Request.CreateErrorResponse(HttpStatusCode.OK, myCustomError);
                
            }
            return Request.CreateResponse(HttpStatusCode.OK, resultMessage);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage CreateResponse(ServiceResult result)
        {
            string resultMessage = string.Empty;
            if (result.Items != null)
            {
                //always first item will be success or failore
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