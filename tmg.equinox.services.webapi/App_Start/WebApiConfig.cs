using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.dependencyresolution;
using tmg.equinox.services.webapi.Framework;
using tmg.equinox.services.webapi.Framework.ExceptionHandling;
using tmg.equinox.services.webapi.Framework.Routing;
using tmg.equinox.services.webapi.Models;

namespace tmg.equinox.services.webapi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            IRouteRegistry routeRegistry = new RouteRegistry();
            if (routeRegistry != null)
            {
                routeRegistry.RegisterRoutes(config.Routes);
            }

            config.Routes.MapHttpRoute(
                name: "Error404",
                routeTemplate: "{*url}",
                defaults: new { controller = "Error", action = "Error404" }
            );


            // Web API routes
            config.MapHttpAttributeRoutes();

            //register ServiceRequestModelBinderProvider
            var provider = new SimpleModelBinderProvider(typeof(ServiceRequestModel), new ServiceRequestModelBinderProvider());
            config.Services.Insert(typeof(ModelBinderProvider), 0, provider);

            //register GlobalExceptionHanlder for handling exceptions
            ///config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());

            //register ElmahExceptionLogger for logging exceptions
            config.Services.Replace(typeof(IExceptionLogger), new ElmahExceptionLogger());

            config.Services.Replace(typeof(IHttpControllerSelector), new HttpNotFoundAwareDefaultHttpControllerSelector(config));
            config.Services.Replace(typeof(IHttpActionSelector), new HttpNotFoundAwareControllerActionSelector());
        }
    }
}
