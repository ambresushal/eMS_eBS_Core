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
using System.Configuration;

namespace tmg.equinox.identitymanagement.Provider
{
    public class BaseAuthProvider
    {
        #region Public/ Protected / Private Member Variables
        protected bool owinAuthenticationEnabled = false;
        protected bool adAuthenticationEnabled = false;
        protected string adDomainName = string.Empty;
        protected static IFormsAuthentication formAuthentication = null;
        protected ApplicationUserManager applicationUserManager = null;
        protected ApplicationRoleManager applicationRoleManager = null;
        protected OwinAuthentication owinAuthentication = null;
        protected readonly object LockObj = new object();
        protected static string authenticationType = DefaultAuthenticationTypes.ApplicationCookie;

        #endregion Member Variables

        #region Constructor/Dispose
        public BaseAuthProvider()
        {
            formAuthentication = new DefaultFormsAuthentication();
            applicationUserManager = new ApplicationUserManager(new UserStore<ApplicationUser, ApplicationRole, int, ApplicationUserLogin, ApplicationUserRole, ApplicationRoleClaim>(new SecurityDbContext()));
            applicationRoleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole, int, ApplicationUserRole>(new SecurityDbContext()));
            owinAuthentication = new OwinAuthentication(authenticationType);

            //Enable lockout
            applicationUserManager.UserLockoutEnabledByDefault = true;
            applicationUserManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromDays(30);
            int maxFailedCount = 5;
            if (int.TryParse(Convert.ToString(ConfigurationManager.AppSettings["MaxFailedAccessAttemptsBeforeLockout"] ?? "5"), out maxFailedCount))
            {
                applicationUserManager.MaxFailedAccessAttemptsBeforeLockout = maxFailedCount;
            }
        }
        #endregion Constructor/Dispose

        #region Public Properties
        public TimeSpan FormsAuthenticationTimeout
        {
            get
            {
                return formAuthentication.GetFormsAuthenticationTimeout();
            }
        }
        public bool OwinAuthenticationEnabled
        {
            set
            {
                owinAuthenticationEnabled = value;
            }
        }


        public int tenantId = 0;
        #endregion Public Properties

        #region Public Methods

        public bool SignInAsync(ClaimsIdentity claimsIdentity)
        {
            var retvalue = false;
            try
            {

                if (claimsIdentity.IsAuthenticated)
                {
                    owinAuthentication.IssueCookie(claimsIdentity, null, false);
                }

                retvalue = true;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                retvalue = false;
            }
            return retvalue;
        }

        public IQueryable<ApplicationRole> GetRoles()
        {
            var roles = applicationRoleManager.Roles;
            return roles;
        }

        public void SignOut(bool appendResponseHeader = true)
        {
            //if (owinAuthenticationEnabled || adAuthenticationEnabled)
            //{
            owinAuthentication.SignOut();
            //}
            //else
            //{
            //    formAuthentication.Signout();
            //}

            if (appendResponseHeader)
            {
                HttpContext.Current.Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
                HttpContext.Current.Response.AppendHeader("Expires", "0");
            }
        }
        public bool SignInDummy(string dummyUserName)
        {
            DummyFormsAuthentication dummyFormsAuthentication = new DummyFormsAuthentication();
            var retvalue = false;
            try
            {
                var dummyUser = applicationUserManager.FindByName(dummyUserName);

                if (dummyUser != null)
                {
                    tenantId = dummyUser.TenantID;

                    GenerateUserIdentity(dummyUser);

                    applicationUserManager.CreateIdentity(dummyUser, DefaultAuthenticationTypes.ApplicationCookie);

                    dummyFormsAuthentication.IssueCookie(dummyUser);

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

        public bool SignInAsync(string userName)
        {
            var retvalue = false;
            try
            {

                var applicationUser = applicationUserManager.FindByName(userName);

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

        public bool VerifyAndReAssignRole(ApplicationUser applicationUser, List<string> authorizationgroups, string emailAddress)
        {
            string userRoleName = string.Empty;
            bool isInRole = false;
            IList<string> existingUserRoles = new List<string>();
            try
            {
                //Sync the email address
                if (emailAddress != applicationUser.Email)
                {
                    applicationUserManager.SetEmail(applicationUser.Id, emailAddress);
                }

                userRoleName = ActiveDirectoryStore.GetRoleName(authorizationgroups);
                if (!string.IsNullOrEmpty(userRoleName))
                {
                    isInRole = applicationUserManager.IsInRole(applicationUser.Id, userRoleName);
                    if (!isInRole)
                    {
                        existingUserRoles = applicationUserManager.GetRoles(applicationUser.Id);
                        applicationUserManager.RemoveFromRole(applicationUser.Id, existingUserRoles[0]);
                        applicationUserManager.AddToRole(applicationUser.Id, userRoleName);

                        return true;
                    }
                }
                else
                {
                    throw new Exception("role not assigned");
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return isInRole;
        }
        public bool CreateUserAssignRole(string userName, string password, string emailAddress, List<string> authorizationgroups)
        {
            bool retVal = false;
            string userRoleName = string.Empty;
            try
            {
                userRoleName = ActiveDirectoryStore.GetRoleName(authorizationgroups);
                if (!string.IsNullOrEmpty(userRoleName))
                {
                    ApplicationUser applicationUser = new ApplicationUser { UserName = userName, TenantID = 1, Email = emailAddress, PhoneNumber = "123456789" };

                    IdentityResult result = applicationUserManager.Create(applicationUser, password);

                    if (result.Succeeded)
                    {
                        applicationUserManager.AddToRole(applicationUser.Id, userRoleName);
                        retVal = true;
                    }
                }
                else
                {
                    throw new Exception("role not assigned");
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return retVal;
        }

        public bool CreateUserAsync(string userName, string password)
        {
            bool retVal = true;

            ApplicationUser applicationUser = new ApplicationUser { UserName = userName, TenantID = 1, Email = userName + "@themostgroup.com", PhoneNumber = "123456789" };

            IdentityResult result = applicationUserManager.Create(applicationUser, password);

            if (!result.Succeeded) retVal = false;


            return retVal;
        }
        public bool ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            bool retVal = true;
            try
            {
                if (userId == 0) return false;

                IdentityResult result = applicationUserManager.ChangePassword(userId, currentPassword, newPassword);
                if (!result.Succeeded) retVal = false;
            }
            catch (Exception ex)
            {
                retVal = false;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return retVal;
        }
        public bool ResetPasswordAsync(int userId, string userName, string password)
        {
            bool retVal = true;
            try
            {
                ApplicationUser usr = applicationUserManager.FindByName(userName);
                IdentityResult r = applicationUserManager.RemovePassword(usr.Id);
                string newPassword = applicationUserManager.PasswordHasher.HashPassword(password);
                usr.PasswordHash = newPassword;
                IdentityResult result = applicationUserManager.Update(usr);
                if (!result.Succeeded) retVal = false;
            }
            catch (Exception ex)
            {
                retVal = false;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return retVal;
        }
        public FormsAuthenticationTicket Decrypt(string encryptedTicket)
        {
            FormsAuthenticationTicket formsAuthenticationTicket = formAuthentication.Decrypt(encryptedTicket);
            return formsAuthenticationTicket;
        }

        public ApplicationUser FindById(int Id)
        {
            return applicationUserManager.FindById(Id);
        }

        public ApplicationUser FindByName(string name)
        {
            return applicationUserManager.FindByName(name);
        }
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods

        #region Helper Methods
        public ClaimsIdentity GenerateUserIdentity(ApplicationUser applicationUser)
        {
            var claimsIdentity = applicationUserManager.CreateIdentity(applicationUser, authenticationType);
            claimsIdentity.AddTenantIdClaim(applicationUser.TenantID);
            claimsIdentity.AddRoleIdClaim(applicationUser.Roles.FirstOrDefault().RoleId);
            return claimsIdentity;
        }

        public ClaimsIdentity GenerateUserIdentity(ClaimsIdentity userclaimsIdentity, string ssoEnvironment)
        {
            var claimsIdentity = userclaimsIdentity;

            //Remove NameIdentifier claim
            Claim nameid = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            claimsIdentity.RemoveClaim(nameid);

            //Set NameIdentifier claim value to Name claim
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, nameid.Value));

            claimsIdentity.AddTenantIdClaim(1);
            claimsIdentity.AddRoleIdClaim(ssoEnvironment);
            return claimsIdentity;
        }
        public void SetHttpContextCurrentPrincipal(ClaimsIdentity claimsIdentity)
        {
            GenericPrincipal genericPrincipal = GenerateUserPrincipal(claimsIdentity,
                applicationRoleManager.Roles.Select(role => role.Name).ToArray());

            WindowsPrincipal.Current.Identities.First().AddClaims(genericPrincipal.Claims);

            // WindowsPrincipal.Current.AddIdentity(claimsIdentity);

            if (HttpContext.Current.User != null)
                Thread.CurrentPrincipal = HttpContext.Current.User = genericPrincipal;
        }
        public GenericPrincipal GenerateUserPrincipal(ClaimsIdentity claimsIdentity, string[] roles)
        {
            var principal = new GenericPrincipal(claimsIdentity, roles);
            return principal;
        }
        #endregion Helper Methods        
    }
}
