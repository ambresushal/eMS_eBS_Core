using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Net;

namespace tmg.equinox.services.webapi.Framework
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            Exception ex = context.Exception;

            if (ex != null)
            {
                if (ex is ItemNotFoundException)
                {
                    context.Response = context.Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
                }
                else if (ex is InvalidFormInstanceIDException)
                {
                    context.Response = context.Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
                }
                else if (ex is Exception)
                {
                    context.Response = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
            }

            base.OnException(context);
        }
    }
}