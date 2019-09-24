using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using tmg.equinox.identitymanagement.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using System.Security.Claims;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using tmg.equinox.identitymanagement.Extensions;
using System.Security.Principal;
using System.Threading;

namespace tmg.equinox.identitymanagement.Provider
{

    public interface IADAuthProvider
    {
        ApplicationUser GetUser(string userName, string password);
        string GetRoleName(string userName);
    }


    public class ActiveDirectoryAuthProvider : BaseAuthProvider, IAuthProvider, IADAuthProvider
    {
        public string GetRoleName(string userName)
        {
            List<string> groups = null;
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, Environment.UserDomainName))
            {
                var user = UserPrincipal.FindByIdentity(pc, userName);
                groups = GetGroupsFromLDAP(groups, user);
            }
            return ActiveDirectoryStore.GetRoleName(groups);
        }

        public ApplicationUser GetUser(string userName, string password)
        {
            bool isValid = false;
            List<string> groups = null;
            UserPrincipal user = null;
            ApplicationUser applicationUser = null;
            try
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain))
                {
                    // validate the credentials
                    if (!string.IsNullOrEmpty(password))
                    {
                        isValid = pc.ValidateCredentials(userName, password);
                    }
                    else
                    {
                        isValid = true;
                    }

                    if (isValid)
                    {
                        //int roleId;
                        user = UserPrincipal.FindByIdentity(pc, userName);
                        if (user == null)
                            return applicationUser;
                    }
                    else
                        return applicationUser;

                    //groups = user.GetAuthorizationGroups().ToList<Principal>();
                    //groups = user.GetGroups().ToList<Principal>();// GetAuthorizationGroups().ToList<Principal>();
                    groups = user.GetGroups().ToList<Principal>().Select(m => m.Name.ToLower()).ToList<string>();
                }
                // get the authorization groups - those are the "roles" 
                //var dbUserName = user.SamAccountName;
                var dbUserName = userName.Split('\\').Last();
                var emailAddress = user.EmailAddress;
                applicationUser = FindByName(dbUserName);
                if (applicationUser == null)
                {
                    if (CreateUserAssignRole(dbUserName, "1234", emailAddress, groups))
                    {
                        applicationUser = FindByName(dbUserName);
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (!VerifyAndReAssignRole(applicationUser, groups, emailAddress))
                {
                    return null;
                }
            }

            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return applicationUser;
        }
        private static List<string> GetGroupsFromLDAP(List<string> groups, UserPrincipal user)
        {
            groups = user.GetGroups().Where(x => x.Name != "Domain Users").ToList<Principal>().Select(m => m.Name.ToLower()).ToList<string>(); ;
            return groups;
        }

        public bool SignInAsync(string userName, string password)
        {
            var retvalue = false;
            try
            {
                var applicationUser = GetUser(userName, password);
                if (applicationUser != null)
                {
                    retvalue = true;

                    ClaimsIdentity claimsIdentity = GenerateUserIdentity(applicationUser);

                    SetHttpContextCurrentPrincipal(claimsIdentity);

                    owinAuthentication.IssueCookie(claimsIdentity, null, false);
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

        public int AccessFailed(int userId)
        {
            applicationUserManager.AccessFailed(userId);
            return applicationUserManager.GetAccessFailedCount(userId);
        }

        public bool ResetAccessFailedCount(int userId)
        {
            var result = applicationUserManager.ResetAccessFailedCount(userId);
            return result.Succeeded;
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
    }
}

