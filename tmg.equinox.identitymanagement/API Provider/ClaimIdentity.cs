using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using tmg.equinox.identitymanagement.Models;
using tmg.equinox.identitymanagement.Provider;

namespace tmg.equinox.identitymanagement.APIProvider
{
    public class ClaimIdentity
    {
        public static void SetClaims(OAuthGrantResourceOwnerCredentialsContext context, ApplicationUser user)
        {

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