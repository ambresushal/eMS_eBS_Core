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
//using System.Web.Http.ExceptionHandling;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.identitymanagement.APIProvider
{
    public class BaseAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        
            #region Auth
        public const string GeneralErrorMessage = "Oops! Sorry! Something went wrong. Please contact support@themostgroup.com so we can try to fix it.";
        public const string InvalidUserOrPass = "The user name or password is incorrect."; 
        #endregion 


        protected readonly string _publicClientId;
        public BaseAuthorizationServerProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public virtual Task<ApplicationUser> Validate(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return null;
        }

       
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                ApplicationUser user = await Validate(context);

                if (user == null)
                {
                    context.SetError("invalid_grant", InvalidUserOrPass);
                    return;
                }

                ClaimIdentity.SetClaims(context, user);
            }
            catch (Exception ex)
            {
                ExceptionLoggerWrapper.LogException(ex, ExceptionPolicies.ExceptionShielding);
                context.SetError("invalid_grant", GeneralErrorMessage);
                return;
            }
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
    }
}