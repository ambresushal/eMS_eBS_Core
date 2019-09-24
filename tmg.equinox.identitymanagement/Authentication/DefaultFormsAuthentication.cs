using System;
using System.Web;
using System.Web.Security;
using tmg.equinox.identitymanagement.Models;

namespace tmg.equinox.identitymanagement.Authentication
{
    public class DefaultFormsAuthentication : IFormsAuthentication
    {
        public void IssueCookie(ApplicationUser applicationUser)
        {
            var formsAuthenticationTicket = UserAuthenticationTicketBuilder.CreateAuthenticationTicket(applicationUser);

            this.SetAuthCookie(HttpContext.Current, formsAuthenticationTicket);
        }

        public void SetAuthCookie(string userName, bool persistent)
        {
            FormsAuthentication.SetAuthCookie(userName, persistent);
        }

        public void Signout()
        {
            FormsAuthentication.SignOut();
            this.DeleteAuthCookie();
        }

        /// <summary>
        /// Adds the encrypted <see cref="FormsAuthenticationTicket"/> to the response stream with an expiration of 20 minutes from <see cref="DateTime.Now"/>.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="authenticationTicket"></param>
        public void SetAuthCookie(HttpContextBase httpContext, FormsAuthenticationTicket authenticationTicket)
        {
            var encryptedTicket = FormsAuthentication.Encrypt(authenticationTicket);
            httpContext.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket) { Expires = CalculateCookieExpirationDate() });
        }

        /// <summary>
        /// Adds the encrypted <see cref="FormsAuthenticationTicket"/> to the response stream with an expiration of 20 minutes from <see cref="DateTime.Now"/>.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="authenticationTicket"></param>
        public void SetAuthCookie(HttpContext httpContext, FormsAuthenticationTicket authenticationTicket)
        {
            var encryptedTicket = FormsAuthentication.Encrypt(authenticationTicket);
            httpContext.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket) { Expires = CalculateCookieExpirationDate() });
        }

        private static DateTime CalculateCookieExpirationDate()
        {
            return DateTime.Now.Add(FormsAuthentication.Timeout);
        }

        public void DeleteAuthCookie(string authCookieName = null)
        {
            HttpCookie authCookie = null;

            if (string.IsNullOrEmpty(authCookieName))
            {
                authCookie = FormsAuthentication.GetAuthCookie(FormsAuthentication.FormsCookieName, true);
            }
            else
            {
                authCookie = FormsAuthentication.GetAuthCookie(authCookieName, true);

            }
            authCookie.Expires = DateTime.Now.AddDays(-1D);
        }

        public TimeSpan GetFormsAuthenticationTimeout()
        {
            return FormsAuthentication.Timeout;
        }

        public FormsAuthenticationTicket Decrypt(string encryptedTicket)
        {
            return FormsAuthentication.Decrypt(encryptedTicket);
        }
    }
}