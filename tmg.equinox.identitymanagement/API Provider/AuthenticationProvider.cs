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

namespace tmg.equinox.identitymanagement.APIProvider
{
    public class AuthenticationProvider : BaseAuthorizationServerProvider
    {

        public AuthenticationProvider(string publicClientId)
            : base(publicClientId)
        {

        }

        public override Task<ApplicationUser> Validate(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            return userManager.FindAsync(context.UserName, context.Password);
        }
    }
}