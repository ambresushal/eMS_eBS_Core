using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using tmg.equinox.identitymanagement.Provider;

namespace tmg.equinox.services.api.Hanlders
{
    public class AuthRequestHandler : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {

           
                var context = ((HttpContextBase)request.Properties["MS_HttpContext"]);
                var username = context.User.Identity.Name;
                var provider = AuthProviderFactory.Get(true);
                provider.SignInAsync(username, "");
            
            // Call the inner handler.
            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}