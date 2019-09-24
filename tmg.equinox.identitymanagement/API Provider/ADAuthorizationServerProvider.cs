using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using tmg.equinox.identitymanagement.Models;
using System.Configuration;

namespace tmg.equinox.identitymanagement.Provider
{
    public class ADAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            string domainName = ConfigurationManager.AppSettings["DomainName"].ToString();
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domainName))
            {
                // validate the credentials
                bool isValid = pc.ValidateCredentials(context.UserName, context.Password);

                if (!isValid)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                } 
            }

            //var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            //identity.AddClaim(new Claim(ClaimTypes.Role, "user"));

            //context.Validated(identity);
             ApplicationUser user = await userManager.FindByNameAsync("superuser");
             ClaimIdentity.SetClaims(context, user);
        }
    }
}