using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using tmg.equinox.identitymanagement.Models;

namespace tmg.equinox.identitymanagement.Provider
{
    // <summary>
    /// Provides an interface to identity based authentication and authorization  functions
    /// </summary>
    public interface IAuthProvider
    {
        bool SignInAsync(string userName, string password);

        bool SignInDummy(string dummyUserName);

        void SignOut(bool appendResponseHeader = true);

        IQueryable<ApplicationRole> GetRoles();

        bool CreateUserAsync(string userName, string password);

        bool ChangePasswordAsync(int userId, string currentPassword, string newPassword);

        bool ResetPasswordAsync(int userId, string userName, string newPassword);
        FormsAuthenticationTicket Decrypt(string encryptedTicket);

        ApplicationUser FindById(int userId);
        GenericPrincipal GenerateUserPrincipal(ClaimsIdentity claimsIdentity, string[] roles);
        bool SignInAsync(ClaimsIdentity claimsIdentity);
        bool SignInAsync(string userName);
        ApplicationUser FindByName(string name);

        ClaimsIdentity GenerateUserIdentity(ApplicationUser applicationUser);
        ClaimsIdentity GenerateUserIdentity(ClaimsIdentity userclaimsIdentity, string ssoEnvironment);
        int AccessFailed(int userId);
        bool ResetAccessFailedCount(int userId);
        int GetAccessFailedCount(int userId);
        bool IsLockedOut(int userId);
        void SetLockoutEndDate(int userId);
    }
}