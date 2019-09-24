using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using tmg.equinox.identitymanagement.Models;
using tmg.equinox.identitymanagement.Provider;
using tmg.equinox.identitymanagement;
using System.Configuration;

namespace tmg.equinox.services.api
{
    public class AuthenticationProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public AuthenticationProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            var userRole = IdentityManager.GetUserRole(user.Roles.FirstOrDefault().RoleId);
            string userRoleName = ConfigurationManager.AppSettings["WebAPIRoleAccess"];
            if (string.IsNullOrEmpty(userRoleName) == false)
            {
                if(userRole != userRoleName.Trim())
                {
                    context.SetError("invalid_grant", "The user role is incorrect.");
                    return;
                }
            }

            AuthProvider authProvider = new AuthProvider(true);

            if (user != null)
            {
                ClaimsIdentity claimsIdentity = authProvider.GenerateUserIdentity(user);
                authProvider.SetHttpContextCurrentPrincipal(claimsIdentity);
            }

            

            var oAuthIdentity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
            var subClaim = new Claim(ClaimTypes.Name, context.UserName);
            var idClaim = new Claim(ClaimTypes.NameIdentifier, user.Id.ToString());
            var roleClaim = new Claim(ClaimTypes.Role, user.Roles.FirstOrDefault().RoleId.ToString());
            oAuthIdentity.AddClaim(subClaim);
            oAuthIdentity.AddClaim(idClaim);
            oAuthIdentity.AddClaim(roleClaim);

            ClaimsIdentity cookiesIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationType);
            cookiesIdentity.AddClaim(subClaim);
            cookiesIdentity.AddClaim(idClaim);
            cookiesIdentity.AddClaim(roleClaim);

            AuthenticationProperties properties = CreateProperties(user.Id.ToString(), user.UserName);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        private static AuthenticationProperties CreateProperties(string userId, string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userId", userId },
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }

    }
}