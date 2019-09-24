using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using tmg.equinox.identitymanagement.Models;
using System.Collections.Generic;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using System;
using tmg.equinox.identitymanagement.Enums;

namespace tmg.equinox.identitymanagement.Helper
{
    internal static class IdentityHelper
    {
        #region Private Memebers
        #endregion Private Members

        #region Public Properties
        #endregion Public Properties

        #region Constructor
        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Async Method to generate User Identity
        /// </summary>
        /// <param name="applicationUser"></param>
        /// <param name="applicationUserManager"></param>
        /// <returns></returns>
        public static async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUser applicationUser, ApplicationUserManager applicationUserManager)
        {
            var claimsIdentity = await applicationUserManager.CreateIdentityAsync(applicationUser, DefaultAuthenticationTypes.ApplicationCookie);

            return claimsIdentity;
        }

        /// <summary>
        /// Map Element Access View Model to ApplicationRoleClaims
        /// </summary>
        /// <param name="sectionaccessRows"></param>
        /// <param name="userId"></param>
        /// <param name="rsType"></param>
        /// <returns></returns>
        public static List<ApplicationRoleClaim> MapToApplicationRoleClaims(List<ElementAccessViewModel> sectionaccessRows, int userId, ResourceType rsType) //here userId is temporary parameter
        {
            string resourceType = rsType.ToString().ToLower();
            List<ApplicationRoleClaim> applicationRoleClaim = new List<ApplicationRoleClaim>();
            if (sectionaccessRows != null)
            {
                foreach (var row in sectionaccessRows)
                {
                    bool isEditable = row.IsEditable;
                    bool isVisible = row.IsVisible;
                    if (isEditable)
                    {
                        applicationRoleClaim.Add(
                        new ApplicationRoleClaim
                        {
                            ClaimType = "",
                            ClaimValue = ClaimAction.Edit.ToString(),
                            ResourceID = Convert.ToInt32(row.ResourceID),
                            ResourceType = resourceType,
                            RoleID = Convert.ToInt32(row.RoleID),
                            UserId = Convert.ToInt32(userId)
                        }
                        );
                    }
                    if (isVisible)
                    {
                        applicationRoleClaim.Add(
                   new ApplicationRoleClaim
                   {
                       ClaimType = "",
                       ClaimValue = ClaimAction.View.ToString(),
                       ResourceID = Convert.ToInt32(row.ResourceID),
                       ResourceType = resourceType,
                       RoleID = Convert.ToInt32(row.RoleID),
                       UserId = Convert.ToInt32(userId)
                   }
                   );
                    }

                }

            }
            return applicationRoleClaim;
        }

        /// <summary>
        /// Map ApplicationRoleClaims to Element Access View Model 
        /// </summary>
        /// <param name="applicationRoleClaims"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static List<ElementAccessViewModel> MapToElementAcessViewModel(List<ApplicationRoleClaim> applicationRoleClaims, List<ApplicationRole> roles) //here userId is teprary parameter
        {
            List<ElementAccessViewModel> elementAccessList = new List<ElementAccessViewModel>();
            if (applicationRoleClaims.Count > 0)
            {
                foreach (ApplicationRole role in roles)
                {
                    ElementAccessViewModel objElementAccess = new ElementAccessViewModel();
                    objElementAccess.RoleID = role.Id;
                    objElementAccess.RoleName = role.Name;
                    objElementAccess.IsVisible = false;
                    objElementAccess.IsEditable = false;

                    elementAccessList.Add(objElementAccess);
                }

                foreach (ApplicationRoleClaim roleclaim in applicationRoleClaims)
                {
                    if (elementAccessList.Count > 0)
                    {
                        var objElementAccess = elementAccessList.Where(x => x.RoleID == roleclaim.RoleID).FirstOrDefault();
                        if (roleclaim.ClaimValue == ClaimAction.View.ToString())
                            objElementAccess.IsVisible = true;
                        if (roleclaim.ClaimValue == ClaimAction.Edit.ToString())
                            objElementAccess.IsEditable = true;
                        objElementAccess.ResourceID = roleclaim.ResourceID;
                    }
                }
            }            
            return elementAccessList;
        }

        /// <summary>
        /// Method to Get Default Application Role Claims
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static List<ApplicationRoleClaim> GetDefaultApplicationRoleClaims(ResourceType resourceType, int resourceId, List<ApplicationRole> roles)
        {
            List<ApplicationRoleClaim> defaultApplicationRoleClaims = new List<ApplicationRoleClaim>();
            foreach (ApplicationRole role in roles)
            {
                defaultApplicationRoleClaims.Add(
                        new ApplicationRoleClaim
                        {
                            ClaimType = "",                           
                            ClaimValue = SecurityConfig.IsDefaultPermissionsApplicable? ClaimAction.View.ToString()  :  ClaimAction.NA.ToString(),
                            ResourceID = resourceId,
                            ResourceType = resourceType.ToString().ToLower(),
                            RoleID = Convert.ToInt32(role.Id),
                            UserId = -1
                        }
                        );

                //set default Edit permissions to 'Super User' Role
                if (role.Id == (int)UserRole.WCSuperUser && SecurityConfig.IsDefaultPermissionsApplicable)
                    defaultApplicationRoleClaims.Add(
                       new ApplicationRoleClaim
                       {
                           ClaimType = "",
                           ClaimValue = ClaimAction.Edit.ToString(),
                           ResourceID = resourceId,
                           ResourceType = resourceType.ToString().ToLower(),
                           RoleID = Convert.ToInt32(role.Id),
                           UserId = 1228
                       }
                       );                
            }
            return defaultApplicationRoleClaims;
        }
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods



    }
}
