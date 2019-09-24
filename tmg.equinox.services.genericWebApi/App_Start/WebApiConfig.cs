using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;

namespace tmg.equinox.services.genericWebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            //config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "Accounts",
                routeTemplate: "api/data/v1.0/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "FolderVersion",
                routeTemplate: "api/data/v1.0/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );
            config.Routes.MapHttpRoute(
                name: "AddDeleteAccount",
                routeTemplate: "api/data/v1.0/{controller}/{action}/{AccountName}/{AddedBy}"
                );
            config.Routes.MapHttpRoute(
                name: "Products",
                routeTemplate: "api/data/v1.0/{controller}/{action}/{id}/{sections}",
                defaults: new { id = RouteParameter.Optional }
                );
            config.Routes.MapHttpRoute(
                name: "Repeaters",
                routeTemplate: "api/data/v1.0/{controller}/{action}/{id}/{repeaters}",
                defaults: new { id = RouteParameter.Optional }
                );
        }
    }
}
