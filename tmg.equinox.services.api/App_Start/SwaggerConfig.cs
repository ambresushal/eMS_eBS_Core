using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Generic;
using System.Web.Http.Description;
using System.Web.Http.Routing.Constraints;
using WebActivatorEx;
using tmg.equinox.services.api;
using Swashbuckle.Application;
using tmg.equinox.services.api.Helper;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace tmg.equinox.services.api
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "tmg.equinox.services.api");
                        c.IncludeXmlComments(GetXmlCommentsPath());
                        c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                        c.OperationFilter(() => new AddRequiredHeaderParameter());
                        c.DocumentFilter<AuthTokenOperation>();
                        
                    })
                .EnableSwaggerUi(c =>
                    {
                        c.InjectJavaScript(typeof(SwaggerConfig).Assembly, "tmg.equinox.services.api.SwaggerSetting.onComplete.js");
                    });
            
        }

        private static string GetXmlCommentsPath()
        {
            var path = String.Format(@"{0}App_Data\Documentation.XML", AppDomain.CurrentDomain.BaseDirectory);
            return path;
        }
    }
}
