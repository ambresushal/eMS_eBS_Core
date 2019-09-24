using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Security;
using System.Web;
using System.Security.Claims;
using System.Configuration;
using System.Security.Principal;
using Newtonsoft.Json;
using tmg.equinox.identitymanagement.Models;
using tmg.equinox.identitymanagement.Extensions;

namespace tmg.equinox.identitymanagement.Authentication
{
    public class UserAuthenticationTicketBuilder
    {
        /// <summary>
        /// Creates a new <see cref="FormsAuthenticationTicket"/> from a user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <remarks>
        /// Encodes the <see cref="UserProfileInfo"/> into the <see cref="FormsAuthenticationTicket.UserData"/> property
        /// of the authentication ticket.  This can be recovered by using the <see cref="UserProfileInfo.FromString"/> method.
        /// </remarks>     
       
        public static FormsAuthenticationTicket CreateAuthenticationTicket(UserProfileInfo userInfo)
        {
            var serializeObject = JsonConvert.SerializeObject(userInfo);
            var ticket = new FormsAuthenticationTicket(
               Convert.ToInt32(ConfigurationManager.AppSettings["FormAuthTicketVersion"]),
                userInfo.UserName,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                Convert.ToBoolean(ConfigurationManager.AppSettings["FormAuthTicketIsPersistent"]),
                serializeObject);
            return ticket;
        }


        public static FormsAuthenticationTicket CreateAuthenticationTicket(ApplicationUser applicationUser)
        {
            UserProfileInfo userProfileInfo = CreateUserContextFromUser(applicationUser);

            var serializeObject = JsonConvert.SerializeObject(userProfileInfo);

            var ticket = new FormsAuthenticationTicket(
               Convert.ToInt32(ConfigurationManager.AppSettings["FormAuthTicketVersion"]),
                applicationUser.UserName,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                Convert.ToBoolean(ConfigurationManager.AppSettings["FormAuthTicketIsPersistent"]),
                serializeObject);
            return ticket;
        }

        //public static FormsAuthenticationTicket CreateAuthenticationTicket(GenericPrincipal principal)
        //{
        //    UserProfileInfo userProfileInfo = CreateUserContextFromUser(principal.ApplicationClaimIdentity());

        //    FormsAuthenticationTicket formsAuthenticationTicket= CreateAuthenticationTicket(userProfileInfo);          
          
        //    string userData = SerializationHelper.SerializeWithBinaryFormatter<GenericPrincipal>(principal);

        //    UserDataDummy.Data = userData;

        //    return formsAuthenticationTicket;
        //}
       
        public static UserProfileInfo CreateUserContextFromUser(ApplicationUser user)
        {
            var userContext = new UserProfileInfo
            {
                Id = user.Id,
                DisplayName = user.UserName,
                /*ClaimsIdentifier = user.AuthorizationId,*/
                UserName = user.UserName
            };
            return userContext;
        }        
    }    
}