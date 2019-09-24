using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.services.webapi.Framework
{
    public class ElmahExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            ExceptionPolicyWrapper.HandleException(context.Exception, ExceptionPolicies.ExceptionShielding);
        }
    }
}