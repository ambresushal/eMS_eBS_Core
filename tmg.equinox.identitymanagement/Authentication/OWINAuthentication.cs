using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using tmg.equinox.identitymanagement.Models;
using System.Security.Claims;

namespace tmg.equinox.identitymanagement.Authentication
{
    public class OwinAuthentication
    {
        #region Private Memebers
        private string authenticationType = DefaultAuthenticationTypes.ApplicationCookie;
        #endregion Private Members


        #region Public Properties
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }
        #endregion Public Properties

        #region Constructor
        public OwinAuthentication(string owinAuthenticationType)
        {
            authenticationType = owinAuthenticationType;
        }
        #endregion Constructor

        #region Public Methods
        /// <summary>
        /// This provides the mechanism to create a cookie after the user has been authenticated
        /// </summary>
        /// <param name="isPersistent"></param>
        /// <param name="claimsIdentity"></param>      
        public void IssueCookie(ClaimsIdentity claimsIdentity, TimeSpan? tokenLifetime, bool? persistentCookie)
        {
            SignOut();

            var props = new AuthenticationProperties();
            if (tokenLifetime.HasValue) props.ExpiresUtc = DateTime.UtcNow.Add(tokenLifetime.Value);
            if (persistentCookie.HasValue) props.IsPersistent = persistentCookie.Value;

            AuthenticationManager.SignIn(props, claimsIdentity);
        }
        public void SignOut()
        {
            AuthenticationManager.SignOut(this.authenticationType);
        }
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
