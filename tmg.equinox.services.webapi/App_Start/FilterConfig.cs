using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using tmg.equinox.services.webapi.Framework;

namespace tmg.equinox.services.webapi
{
    public static class FilterConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Filters.Add(new ValidateModelAttribute());
            config.Filters.Add(new ApiExceptionFilterAttribute());
        }
    }
}