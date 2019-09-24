using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.identitymanagement.Enums;
using tmg.equinox.identitymanagement.Models;

namespace tmg.equinox.identitymanagement.Provider
{
    public class ClaimsProvider : IClaimsProvider
    {
        #region Public/ Protected / Private Member Variables
        private SecurityDbContext context = null;
        #endregion Member Variables

        #region Constructor/Dispose
        public ClaimsProvider()
        {
            context = new SecurityDbContext();

        }
        #endregion Constructor/Dispose

        #region Public Methods

        public List<Claim> GetIdentityClaims(List<Claim> claims, string controllerName, string actionName, ResourceType resource = ResourceType.ACTION)
        {
            return claims.Where(c =>
            {
                string type = c.Type;
                if (type.Contains('/'))
                {
                    string[] resources = type.Split('/');
                    return resources[0].ToLower().Trim().Equals(controllerName.ToLower().Trim());
                    //&& resources[1].ToLower().Trim().Equals(actionName.ToLower().Trim());
                }
                return type.ToLower().Trim().Equals(controllerName.ToLower().Trim());
            }).ToList();
        }

        public void AddUpdateClaim(List<ApplicationRoleClaim> RoleClaims)
        {
            foreach (var roleclaim in RoleClaims)
            {
                context.RoleClaim.Add(roleclaim);
                context.Entry(roleclaim).State = roleclaim.Id == 0 ?
                                   EntityState.Added :
                                   EntityState.Modified;
            }
            context.SaveChanges();
        }

        public void RemoveClaim(int resourceId, ResourceType resourceType)
        {
            List<ApplicationRoleClaim> roleClaims = context.RoleClaim.Where(rc => rc.ResourceID == resourceId && rc.ResourceType == resourceType.ToString().ToLower()).ToList<ApplicationRoleClaim>();

            foreach (var roleClaim in roleClaims)
            {
                context.RoleClaim.Remove(roleClaim);
                context.Entry(roleClaim).State = EntityState.Deleted;
            }
            context.SaveChanges();
        }

        /// <summary>
        /// Add Update UserName for SSO 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int AddUpdateUser(int tenantId, ApplicationUser user, ApplicationRole role)
        {
            ApplicationUserRole userRoleAssoc;
            userRoleAssoc = new ApplicationUserRole();
            userRoleAssoc.Role = role;
            userRoleAssoc.User = user;

            int userId = 0;

            context.Users.Add(user).Roles.Add(userRoleAssoc);
            context.Entry(userRoleAssoc).State = EntityState.Added;
            context.Entry(user).State = user.Id == 0 ?
                               EntityState.Added :
                               EntityState.Modified;
            context.SaveChangesAsync();
            userId = user.Id;

            return userId;
        }

        public void UpdateRoleAssociation(int tenantId, ApplicationUser user, ApplicationRole role, int roleId)
        {
            if (user.Roles.Count <= 0)
            {
                AddUpdateUser(tenantId, user, role);
            }

            else
            {
                List<ApplicationUserRole> oldRole = user.Roles.Where(u => u.RoleId == roleId).ToList();
                if (oldRole.Count <= 0)
                {
                    AddUpdateUser(tenantId, user, role);
                }
            }

        }
        /// <summary>
        /// Get User ID in SSO login
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int GetUserId(int tenantId, string userName, int roleId)
        {
            int userId = 0;
            ApplicationUser userAddUpdateInsert;
            ApplicationRole userRole = new ApplicationRole();
            List<ApplicationUser> users = context.Users.Where(u => u.UserName == userName && u.TenantID == tenantId).Include(c => c.Roles).OrderByDescending(u => u.Id).ToList<ApplicationUser>();

            if (roleId > 0)
            {
                userRole = context.Roles.Where(u => u.Id == roleId).FirstOrDefault();
            }

            if (users.Count > 0)
            {
                userAddUpdateInsert = users.FirstOrDefault();
                UpdateRoleAssociation(tenantId, userAddUpdateInsert, userRole, roleId);
                userId = userAddUpdateInsert.Id;
            }
            else
            {
                userAddUpdateInsert = new ApplicationUser();

                userAddUpdateInsert.UserName = userName;
                userAddUpdateInsert.TenantID = tenantId;

                userId = AddUpdateUser(tenantId, userAddUpdateInsert, userRole);
            }
            return userId;
        }

        /// <summary>
        /// Get Claims By resource type and resource id
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public List<ApplicationRoleClaim> GetClaims(ResourceType resourceType, int resourceId)
        {
            return context.RoleClaim.Where(x => x.ResourceType == resourceType.ToString().ToLower() && x.ResourceID == resourceId).ToList<ApplicationRoleClaim>();
        }

        /// <summary>
        /// Get Claims By resource type and resource id
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public List<ApplicationRoleClaim> GetClaims(int roleId)
        {
            return context.RoleClaim.Where(x => x.RoleID == roleId).Distinct().ToList<ApplicationRoleClaim>();
        }

        /// <summary>
        /// Get Claims By resource type
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public List<ApplicationRoleClaim> GetClaims(ResourceType resourceType)
        {
            return context.RoleClaim.Where(x => x.ResourceType == resourceType.ToString().ToLower()).ToList<ApplicationRoleClaim>();
        }

        /// <summary>
        /// Get list of Roles
        /// </summary>
        /// <returns></returns>
        public List<ApplicationRole> GetRoles()
        {
            List<ApplicationRole> roles = context.Roles.ToList<ApplicationRole>();
            return roles;
        }

        public ApplicationRole GetRoleByName(string roleName)
        {
            ApplicationRole userRole = context.Roles.Where(x => x.Name == roleName).ToList<ApplicationRole>().FirstOrDefault();
            return userRole;
        }

        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods

        #region Helper Methods
        #endregion Helper Methods
    }
}
