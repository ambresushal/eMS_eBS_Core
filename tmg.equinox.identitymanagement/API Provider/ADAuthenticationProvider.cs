using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using tmg.equinox.identitymanagement.Models;
using System.Configuration;
using System.DirectoryServices;
using tmg.equinox.identitymanagement.Provider;

namespace tmg.equinox.identitymanagement.APIProvider
{
    public class ADAuthenticationProvider : BaseAuthorizationServerProvider
    {
        public ADAuthenticationProvider(string publicClientId)
            : base(publicClientId)
        {

        }
        public override Task<ApplicationUser> Validate(OAuthGrantResourceOwnerCredentialsContext context)
        {
           context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
           IADAuthProvider  authProvider = AuthProviderFactory.Get();
           return Task.Run(() => authProvider.GetUser(context.UserName, context.Password));           
        }
     
    }
}