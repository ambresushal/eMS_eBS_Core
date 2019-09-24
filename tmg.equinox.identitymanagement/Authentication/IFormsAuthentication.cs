using System;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using tmg.equinox.identitymanagement.Models;

namespace tmg.equinox.identitymanagement.Authentication
{
    public interface IFormsAuthentication
    {

        //TimeSpan FormsAuthenticationTimeout { get; }
        TimeSpan GetFormsAuthenticationTimeout();
        /// <summary>
        /// Forces signout from the authorization system.
        /// </summary>
        void Signout();

        /// <summary>
        /// Adds the encrypted <see cref="FormsAuthenticationTicket"/> to the response stream with an expiration of 20 minutes from <see cref="DateTime.Now"/>.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="authenticationTicket"></param>
        void SetAuthCookie(HttpContextBase httpContext, FormsAuthenticationTicket authenticationTicket);

        /// <summary>
        /// Adds the encrypted <see cref="FormsAuthenticationTicket"/> to the response stream with an expiration of 20 minutes from <see cref="DateTime.Now"/>.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="authenticationTicket"></param>
        void SetAuthCookie(HttpContext httpContext, FormsAuthenticationTicket authenticationTicket);

        /// <summary>
        /// Decrypts a ticket from a string and returns a <see cref="FormsAuthenticationTicket"/>
        /// </summary>
        /// <param name="encryptedTicket"></param>
        /// <returns></returns>
        FormsAuthenticationTicket Decrypt(string encryptedTicket);
        /// <summary>
        /// Deletes the form authentication cookie by expiring it
        /// </summary>
        void DeleteAuthCookie(string cookieName);

        /*void CreateAndSetAuthCookie(GenericPrincipal principal);*/
        /// <summary>
        /// Creates a ticket with the help of ticket builder and sets the authentication cookie.
        /// </summary>
        /// <summary>
        /// Creates a ticket with the help of ticket builder and sets the authentication cookie.
        /// </summary>
        /// <param name="userProfileInfo"></param>
        void IssueCookie(ApplicationUser applicationUser);

        /// <summary>
        /// Create User in database.
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="password"></param>
        //void CreateUser(ApplicationUserManager userManager, string password);

        //Task<bool> FindAsync(string userName, string password);
    }
}