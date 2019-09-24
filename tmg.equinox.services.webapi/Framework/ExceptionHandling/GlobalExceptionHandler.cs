using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace tmg.equinox.services.webapi.Framework
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        HttpResponseMessage message = new HttpResponseMessage();

        public virtual void HandleCore(ExceptionHandlerContext context)
        {
            Exception ex = context.Exception;

            if (ex is ItemNotFoundException)
            {
                message.StatusCode = HttpStatusCode.NotFound;
                message.ReasonPhrase = "ItemNotFound";
                message.Content = new StringContent(ex.Message);
            }
            else if (ex is InvalidFormInstanceIDException)
            {
                message.StatusCode = HttpStatusCode.NotFound;
                message.ReasonPhrase = "InvalidFormInstanceID";
                message.Content = new StringContent(ex.Message);
            }
            else if (ex is Exception)
            {
                message = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            throw new HttpResponseException(message);
        }

        public virtual Task HandleAsync(ExceptionHandlerContext context,
                                        CancellationToken cancellationToken)
        {
            if (!ShouldHandle(context))
            {
                return Task.FromResult(message);
            }

            return HandleAsyncCore(context, cancellationToken);
        }

        public virtual Task HandleAsyncCore(ExceptionHandlerContext context,
                                           CancellationToken cancellationToken)
        {
            HandleCore(context);
            return Task.FromResult(message);
        }

        public virtual bool ShouldHandle(ExceptionHandlerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            ExceptionContext exceptionContext = context.ExceptionContext;
            ExceptionContextCatchBlock catchBlock = exceptionContext.CatchBlock;
            return true;
        }
    }
}