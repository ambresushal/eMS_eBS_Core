using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using tmg.equinox.dependencyresolution;

namespace tmg.equinox.services.genericWebApi
{
    public class genericWebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            if (AuditConfig.EnableEntityFrameworkQueryLog)
            {
                DbInterception.Add(new CommandInterceptor());
            }
            UnityConfig.RegisterComponents();
        }
    }
}
