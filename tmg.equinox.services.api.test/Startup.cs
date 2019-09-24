using Microsoft.Owin;
using Microsoft.Owin.Builder;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using tmg.equinox.services.api;
using Unity.WebApi;

namespace tmg.equinox.services.api.test
{
    public class Startup 
    {
        public virtual void Configuration(IAppBuilder builder)
        {
            // if you want to move all your web api initialization 
            // and configuration to here then you can. 
            // Otherwise just leave this method empty

            var startup = new tmg.equinox.services.api.Startup();
             startup.ConfigureAuth(builder);

            HttpConfiguration config = new HttpConfiguration();

            WebApiConfig.Register(config);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
           // RouteConfig.RegisterRoutes(RouteTable.Routes);
           // BundleConfig.RegisterBundles(BundleTable.Bundles);
          
        //  UnityConfig.RegisterComponents();
        /*    config.MessageHandlers.Insert(0, new CurrentRequestHandler());

            UnityConfig.container.RegisterType<CurrentRequest>(
            new HierarchicalLifetimeManager());

            UnityConfig.container.RegisterType<IOwinContext>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(c => c.Resolve<CurrentRequest>().Value.GetOwinContext()));
                */

            config.DependencyResolver = new UnityDependencyResolver(UnityConfig.container);

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };
          
    

            builder.UseWebApi(config);

          
        }
    }

    public class CurrentRequest
    {
        public HttpRequestMessage Value { get; set; }
    }

    public class CurrentRequestHandler : DelegatingHandler
    {
        protected async override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var scope = request.GetDependencyScope();
            var currentRequest = (CurrentRequest)scope.GetService(typeof(CurrentRequest));
            currentRequest.Value = request;
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
