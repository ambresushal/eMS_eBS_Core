using FluentValidation.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using tmg.equinox.identitymanagement.Provider;
using tmg.equinox.services.api.Hanlders;
using tmg.equinox.services.api.Services;

namespace tmg.equinox.services.api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Filters.Add(new ValidateModelStateFilter());
            config.MessageHandlers.Add(new ResponseWrapperHandler());
            if (AuthProviderFactory.IsADAuthentication() == true)
                config.MessageHandlers.Add(new AuthRequestHandler());


            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "folderVersions",
                routeTemplate: "api/v1/Folders/{folderId}/Versions",
                defaults: new { controller = "FolderVersions", action = "Versions" }
            );

            config.Routes.MapHttpRoute(
                name: "accountFolders",
                routeTemplate: "api/v1/Accounts/{accountId}/{controller}",
                defaults: new { controller = "Folders", action = "Folders" }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/v1/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Services.Replace(typeof(IExceptionHandler), new GenericExceptionHandler());
            config.Services.Add(typeof(IExceptionLogger), new Log4NetExceptionLogger());

            //Register Fluent Validation Provider
            FluentValidationModelValidatorProvider.Configure(config);


        }
    }
}
