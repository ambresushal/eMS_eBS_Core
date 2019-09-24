using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.Web.Security;
using System.Security.Principal;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using System.Collections.Generic;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.identitymanagement.Models;
using tmg.equinox.identitymanagement.Authentication;
using tmg.equinox.identitymanagement.Extensions;
using tmg.equinox.identitymanagement.Provider;
using tmg.equinox.identitymanagement.Enums;
using tmg.equinox.identitymanagement.Helper;
using tmg.equinox.applicationservices.viewmodels.FormDesign;

namespace tmg.equinox.identitymanagement
{
    /// <summary>
    /// provides a facade approach for exposing all identity managament functions to outside world
    /// </summary>
    public static class IdentityManager
    {
        #region Private Memebers
        private static bool owinAuthenticationEnabled = false;
        private static readonly object LockObj;
        private static ClaimsProvider claimsProvider;
        private static IAuthProvider authProvider;
        //private static ActiveDirectoryAuthProvider adAuthProvider;
        #endregion Private Members


        #region Public Properties
        public static bool IsHiddenOrDisableSections
        {
            get
            {
                return SecurityConfig.IsHiddenOrDisableSections;
            }
        }
        public static bool IsHiddenContainer
        {
            get
            {
                return SecurityConfig.IsHiddenContainer;
            }
        }
        public static bool IsFolderLockEnable
        {
            get
            {
                return SecurityConfig.IsEnableFolderLockSettings;
            }
        }
        public static bool IsStopScrollFloatingHeaders
        {
            get
            {
                return SecurityConfig.IsStopScrollFloatingHeaders;
            }
        }
        //public static TimeSpan FormsAuthenticationTimeout
        //{
        //    get
        //    {
        //        return authProvider.FormsAuthenticationTimeout;
        //    }
        //}
        public static bool OwinAuthenticationEnabled
        {
            set
            {

                owinAuthenticationEnabled = value;
                authProvider = AuthProviderFactory.Get(owinAuthenticationEnabled);
            }
        }

        //public static bool ADAuthenticationEnabled
        //{
        //    set
        //    {
        //        authProvider.ADAuthenticationEnabled = value;
        //    }
        //}
        //public static string ADDomainName
        //{
        //    set
        //    {
        //        authProvider.ADDomainName = value;
        //    }
        //}

        #endregion Public Properties

        #region Constructor
        static IdentityManager()
        {
            claimsProvider = new ClaimsProvider();
            //   authProvider = AuthProviderFactory.Get(owinAuthenticationEnabled);
            //adAuthProvider = new ActiveDirectoryAuthProvider();
            LockObj = new object();
        }

        #endregion Constructor

        #region Public Methods

        public static bool IsADAuthentication()
        {
            return AuthProviderFactory.IsADAuthentication();
        }

        public static bool IsSOActiveForAD()
        {
            return AuthProviderFactory.IsSOActiveForAD();
        }

        /// <summary>
        /// Provides aysnchronous way of signing in the user with identity constructs
        /// by SK on 9/16/2014
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool SignInAsync(string userName, string password)
        {
            return authProvider.SignInAsync(userName, password);
        }

        public static bool SignInAsync(ClaimsIdentity claimsIdentity)
        {
            return authProvider.SignInAsync(claimsIdentity);
        }
        public static bool SignInAsync(string userName)
        {
            return authProvider.SignInAsync(userName);
        }

        public static List<Claim> GetClaimsFromIdentity(ClaimsIdentity claimsIdentity)
        {
            List<Claim> retval = null;
            var identity = (ClaimsIdentity)claimsIdentity;
            retval = identity.Claims.ToList<Claim>();
            return retval;
        }

        /// <summary>
        /// This method is used to get all roles using ApplicationRoleManager
        /// </summary>
        /// <returns></returns>
        public static IQueryable<ApplicationRole> GetRoles()
        {
            return authProvider.GetRoles();
        }


        /// <summary>
        /// synchronous Method of Sign Out
        /// </summary>
        /// <param name="appendResponseHeader"></param>
        public static void SignOut(bool appendResponseHeader = true)
        {
            authProvider.SignOut(appendResponseHeader);
        }

        /// <summary>
        /// Sets principal and identity for the dummy user when setting "UseAuthentication" is set to false
        /// </summary>
        /// <param name="dummyUserName"></param>
        /// <returns></returns>
        public static bool SignInDummy(string dummyUserName)
        {
            return authProvider.SignInDummy(dummyUserName);
        }

        /// <summary>
        /// Creates a user in identity DB 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool CreateUserAsync(string userName, string password)
        {
            return authProvider.CreateUserAsync(userName, password);
        }

        /// <summary>
        /// Check whether Username is valid or not.
        /// </summary>
        /// <returns></returns>
        public static bool IsUserNameExistAsync(string userName)
        {
            var applicationUser = authProvider.FindByName(userName);
            if (applicationUser != null)
                return true;
            return false;
        }

        /// <summary>
        /// Changes the password of existing user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public static bool ChangePasswordAsync(string userName, string currentPassword, string newPassword)
        {
            var applicationUser = authProvider.FindByName(userName);
            if (applicationUser == null)
                return authProvider.ChangePasswordAsync(0, currentPassword, newPassword);
            return authProvider.ChangePasswordAsync(applicationUser.Id, currentPassword, newPassword);
        }

        public static bool ResetPasswordAsync(int userID, string userName, string password)
        {
            var applicationUser = authProvider.FindByName(userName);
            if (applicationUser == null)
                return authProvider.ResetPasswordAsync(0, userName, password);
            return authProvider.ResetPasswordAsync(applicationUser.Id, userName, password);

        }
        /// <summary>
        /// Decrypt Authentication Ticket
        /// </summary>
        /// <param name="encryptedTicket"></param>
        /// <returns></returns>
        public static FormsAuthenticationTicket Decrypt(string encryptedTicket)
        {
            return authProvider.Decrypt(encryptedTicket);
        }

        /// <summary>
        /// Find Application User By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static ApplicationUser FindApplicationUserById(int Id)
        {
            return authProvider.FindById(Id);
        }

        public static ClaimsIdentity GenerateUserIdentity(ClaimsIdentity claimsIdentity, string ssoEnvironment)
        {
            return authProvider.GenerateUserIdentity(claimsIdentity, ssoEnvironment);
        }

        /// <summary>
        /// Execute Post Authenticate
        /// </summary>
        /// <param name="authCookie"></param>
        public static void ExecutePostAuthenticate(HttpCookie authCookie)
        {
            try
            {
                if (!String.IsNullOrEmpty(authCookie.Value) && !owinAuthenticationEnabled)
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                    if (authTicket != null)
                    {
                        lock (LockObj)
                        {
                            string userData = authTicket.UserData;

                            if (!String.IsNullOrEmpty(userData))
                            {
                                UserProfileInfo userProfileInfo = JsonConvert.DeserializeObject<UserProfileInfo>(userData);

                                ApplicationUser applicationUser = FindApplicationUserById(userProfileInfo.Id);

                                GenericPrincipal genericPrincipal = authProvider.GenerateUserPrincipal(authProvider.GenerateUserIdentity(applicationUser), authProvider.GetRoles().Select(role => role.Name).ToArray());

                                Thread.CurrentPrincipal = HttpContext.Current.User = genericPrincipal;
                            }
                            else
                            {
                                SignOut();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
        }

        /// <summary>
        ///Filter claims based on controller name and action
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public static List<Claim> GetIdentityClaims(List<Claim> claims, string controllerName, string actionName)
        {
            if (claims != null)
                return claimsProvider.GetIdentityClaims(claims, controllerName, actionName);
            return null;
        }

        /// <summary>
        ///Filter claims based on controller name and action
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        //public static void AddIdentityClaim(List<ElementAccessViewModel> claimDataRows, int userId, ResourceType resourceType)//here user Id is temprary.
        //{
        //    if (claimDataRows.Count > 0)
        //    {
        //        int resourceId = 0;
        //        List<ApplicationRoleClaim> applicationRoleClaim = IdentityHelper.MapToApplicationRoleClaims(claimDataRows, userId, resourceType);

        //        resourceId = Convert.ToInt32(applicationRoleClaim.FirstOrDefault().ResourceID);
        //        claimsProvider.RemoveClaim(resourceId, resourceType);
        //        claimsProvider.AddUpdateClaim(applicationRoleClaim);
        //    }
        //}
        public static void AddIdentityClaim(List<ApplicationRoleClaim> applicationRoleClaim, int userId, ResourceType resourceType)//here user Id is temprary.
        {
            if (applicationRoleClaim.Count > 0)
            {
                int resourceId = 0;
                //  List<ApplicationRoleClaim> applicationRoleClaim = IdentityHelper.MapToApplicationRoleClaims(claimDataRows, userId, resourceType);

                resourceId = Convert.ToInt32(applicationRoleClaim.FirstOrDefault().ResourceID);
                claimsProvider.RemoveClaim(resourceId, resourceType);
                claimsProvider.AddUpdateClaim(applicationRoleClaim);
            }
        }

        /// <summary>
        /// Add Default claim 
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public static void AddDefaultResourceClaims(ResourceType resourceType, int resourceId)
        {
            List<ApplicationRoleClaim> DefaultResourceClaims = new List<ApplicationRoleClaim>();
            List<ApplicationRole> currentRoles = GetRoles().ToList();
            ClaimsProvider claimsProvider = new ClaimsProvider();
            DefaultResourceClaims = IdentityHelper.GetDefaultApplicationRoleClaims(resourceType, resourceId, currentRoles);

            claimsProvider.RemoveClaim(resourceId, resourceType);
            claimsProvider.AddUpdateClaim(DefaultResourceClaims);
        }

        /// <summary>
        /// Remove Resource Claims
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        public static void RemoveResourceClaims(ResourceType resourceType, int resourceId)
        {
            claimsProvider.RemoveClaim(resourceId, resourceType);
        }

        /// <summary>
        /// Method to get claims by user Id
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        //public static List<ElementAccessViewModel> GetClaims(int resourceId)
        //{
        //    List<ApplicationRoleClaim> applicationRoleClaims = claimsProvider.GetClaims(ResourceType.SECTION, resourceId);
        //    List<ApplicationRole> applicationRoles = claimsProvider.GetRoles();

        //    List<ElementAccessViewModel> elementAccessList = IdentityHelper.MapToElementAcessViewModel(applicationRoleClaims, applicationRoles);
        //    return elementAccessList;
        //}
        public static List<ApplicationRoleClaim> GetClaims(int resourceId)
        {
            List<ApplicationRoleClaim> applicationRoleClaims = claimsProvider.GetClaims(ResourceType.SECTION, resourceId);
            // List<ElementAccessViewModel> elementAccessList = IdentityHelper.MapToElementAcessViewModel(applicationRoleClaims, applicationRoles);
            // return elementAccessList;
            return applicationRoleClaims;
        }
        public static List<ApplicationRole> GetApplicationRoles()
        {
            List<ApplicationRole> applicationRoles = claimsProvider.GetRoles();
            return applicationRoles;
        }

        public static List<ApplicationRoleClaim> GetRolesClaims(int roleId)
        {
            List<ApplicationRoleClaim> claims = claimsProvider.GetClaims(roleId);
            return claims;
        }

        public static string GetUserRole(int roleId)
        {
            var roles = IdentityManager.GetApplicationRoles();
            return roles.Where(x => x.Id == roleId).FirstOrDefault().Name;
        }

        
        /// <summary>
        /// Get Resource Claims by Resource Type
        /// </summary>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        public static List<ApplicationRoleClaim> GetResourceClaims(ResourceType resourceType)
        {
            List<ApplicationRoleClaim> applicationRoleClaims = new List<ApplicationRoleClaim>();
            applicationRoleClaims = claimsProvider.GetClaims(ResourceType.FORM);
            return applicationRoleClaims;
        }

        /// <summary>
        /// Get User ID in SSO login
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static int GetUserId(int tenantId, string userName, int roleId)
        {
            int userId = 0;
            userId = claimsProvider.GetUserId(tenantId, userName, roleId);
            return userId;
        }


        public static int AddUpdateUser(int tenantId, ApplicationUser user, ApplicationRole role)
        {
            int userId = 0;
            userId = claimsProvider.AddUpdateUser(tenantId, user, role);
            return userId;
        }

        public static ApplicationRole GetRoleByName(string roleName)
        {
            ApplicationRole userRole = claimsProvider.GetRoleByName(roleName);
            return userRole;
        }

        public static int AccessFailed(int userId)
        {
            return authProvider.AccessFailed(userId);
        }

        public static bool ResetAccessFailedCount(int userId)
        {
            return authProvider.ResetAccessFailedCount(userId);
        }

        public static int GetAccessFailedCount(int userId)
        {
            return authProvider.GetAccessFailedCount(userId);
        }

        public static void LockUser(int userId)
        {
            authProvider.SetLockoutEndDate(userId);
        }

        public static bool IsLockedOut(int userId)
        {
            return authProvider.IsLockedOut(userId);
        }

        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}

