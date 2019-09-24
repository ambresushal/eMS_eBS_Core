using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace tmg.equinox.services.api.Services
{
    public class GenericExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            context.Result = new InternalServerErrorCustomResult(context);
        }
    }

    public class InternalServerErrorCustomResult : IHttpActionResult
    {
     //   private Exception _exception;
      //  private HttpRequestMessage _request;
        private ExceptionHandlerContext _context;

        public InternalServerErrorCustomResult(ExceptionHandlerContext context)
        {
            //  this._exception = exception;
            //   this._request = request;
            _context = context;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            /*  HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
              response.Content = new StringContent("Oops! Sorry! Something went wrong. Please contact support@themostgroup.com so we can try to fix it.");
              response.RequestMessage = _context.Request;
              response.ReasonPhrase = "Internal Error!!!";
              return response;*/
            var message = string.Format("Oops! Sorry! Something went wrong. Please contact {0} so we can try to fix it.", WebConfigurationManager.AppSettings["ErrorPageMailID"]); 


            HttpError error;
            if (WebConfigurationManager.AppSettings["EnableShowError"] == "true")
            {
                error = new HttpError(_context.Exception, true);
            }
            else
            {
                error = new HttpError(_context.Exception.Message);
            }
           
            error.Message = string.Format("{0}-{1}", message, error.Message);

            string errorMessage = "Oops! Sorry! Something went wrong.";
            var response = _context.Request.CreateResponse(HttpStatusCode.InternalServerError,
                           error, JsonMediaTypeFormatter.DefaultMediaType
                         );
            response.Headers.Add("X-Error", errorMessage);
            return response;            
        }

    }
}