using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace tmg.equinox.services.webapi.Framework.Routing
{
    public interface IRouteRegistry
    {
        void RegisterRoutes(HttpRouteCollection Routes);
    }
}