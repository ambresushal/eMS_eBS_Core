using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using tmg.equinox.identitymanagement.Authentication;
using tmg.equinox.identitymanagement.Extensions;
using tmg.equinox.identitymanagement.Models;
using System.Web.Security;
using System.Security.Principal;
using System.Web;
using System.DirectoryServices.AccountManagement;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.identitymanagement.Provider
{
    public class AuthProvider : BaseAuthProvider, IAuthProvider
    {

        //private bool _owinAuthenticationEnabled = false; 
        public AuthProvider(bool owinAuthenticationEnabled)
        {
            OwinAuthenticationEnabled = owinAuthenticationEnabled;
        }

        public int tenantId = 0;

        public int AccessFailed(int userId)
        {
            var applicationUser = applicationUserManager.AccessFailed(userId);
            return applicationUserManager.GetAccessFailedCount(userId);
        }

        public bool ResetAccessFailedCount(int userId)
        {
            var applicationUser = applicationUserManager.ResetAccessFailedCount(userId);
            return applicationUser.Succeeded;
        }

        public int GetAccessFailedCount(int userId)
        {
            return applicationUserManager.GetAccessFailedCount(userId);
        }

        public bool IsLockedOut(int userId)
        {
            return applicationUserManager.IsLockedOut(userId);
        }

        public void SetLockoutEndDate(int userId)
        {
            applicationUserManager.SetLockoutEnabled(userId, true);
            applicationUserManager.SetLockoutEndDate(userId, DateTimeOffset.Now.AddDays(30));
        }

        //#endregion Public Properties
        public bool SignInAsync(string userName, string password)
        {
            var retvalue = false;
            try
            {

                var applicationUser = applicationUserManager.Find(userName, password);

                if (applicationUser != null)
                {
                    ClaimsIdentity claimsIdentity = GenerateUserIdentity(applicationUser);
                    if (owinAuthenticationEnabled)
                    {
                        SetHttpContextCurrentPrincipal(claimsIdentity);

                        owinAuthentication.IssueCookie(claimsIdentity, null, false);
                    }
                    else
                    {
                        formAuthentication.IssueCookie(applicationUser);
                    }

                    retvalue = true;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                retvalue = false;
            }
            return retvalue;
        }
    }
}

