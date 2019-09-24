using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.services.api.Services
{
    public class Log4NetExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            ExceptionLoggerWrapper.LogException(context.Exception, ExceptionPolicies.ExceptionShielding);
        }

    }
}