using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Linq;
using tmg.equinox.identitymanagement.Models;

namespace tmg.equinox.identitymanagement.Extensions
{
    public static class ClaimsIdentityExtensions
    {
        public static int GetTenantId(this ClaimsIdentity claimsIdentity)
        {
            if (claimsIdentity == null) throw new ArgumentNullException("user");
            if (claimsIdentity.HasClaim(ClaimTypes.GroupSid))
            {
                var id = claimsIdentity.Claims.GetValue(ClaimTypes.GroupSid);

                int groupSid;

                if (int.TryParse(id, out groupSid))
                {
                    return groupSid;
                }
            }
            else
            {
                throw new Exception("Invalid GroupSidIdentifier");
            }
            return 0;
        }

        /// <summary>
        /// Provides method to get RoleId of logged in user through ClaimsIdentity
        /// </summary>
        /// <param name="claimsIdentity"></param>
        /// <returns></returns>
        public static int GetRoleId(this ClaimsIdentity claimsIdentity)
        {
            if (claimsIdentity == null) throw new ArgumentNullException("user");
            if (claimsIdentity.HasClaim(ClaimTypes.Role))
            {
                var fetchdRoleId = claimsIdentity.Claims.Where(x => x.Type == ClaimTypes.Role).LastOrDefault().Value;

                int roleId;

                if (int.TryParse(fetchdRoleId, out roleId))
                {
                    return roleId;
                }
            }
            //else
            //{
            //    throw new Exception("Invalid RoleId");
                
            //}
            return 0;
        }

        public static int GetRoleSSOId(this ClaimsIdentity claimsIdentity, string ssoEnvironment)
        {
            if (claimsIdentity == null) throw new ArgumentNullException("user");
            if (claimsIdentity.HasClaim("ebsRole"))
            {
                var fetchdRoleId = claimsIdentity.Claims.Where(x => x.Type.ToLower() == "ebsrole").LastOrDefault().Value;
                var role = fetchdRoleId.ToLower().Substring(0, fetchdRoleId.IndexOf(",OU"));
                //var rolename = role.Substring(role.IndexOf("-") + 1);  

                var rolename = string.Empty;
                if (role.Contains(","))
                {
                    string[] rolelist = role.Split(',');
                    foreach (string s in rolelist)
                        if (s.Contains(ssoEnvironment.ToLower() + "-"))
                        {
                            rolename = s.Substring(role.IndexOf("-") + 1);
                            break;
                        }
                }
                else
                    rolename = role.Substring(role.IndexOf("-") + 1);

                int roleId = 0;
                List<ApplicationRole> roles = IdentityManager.GetRoles().ToList();

                foreach(ApplicationRole ap in roles)
                {
                    if(rolename.ToLower().Trim() == ap.Name.ToLower().Trim() || rolename.ToLower().Trim() == ap.Name.Replace(" ", "").ToLower().Trim())
                    {
                        roleId = ap.Id;
                        break;
                    }
                }

                if (roleId == 0)
                    throw new Exception("Invalid RoleId");
                return roleId; 
            }
            else
            {
                throw new Exception("Invalid RoleId");
            }
        }

        /// <summary>
        /// Provides specific method to add claim for tenantId as GroupSid
        /// </summary>
        /// <param name="claimsIdentity"></param>
        /// <param name="tenantId"></param>
        public static void AddTenantIdClaim(this ClaimsIdentity claimsIdentity, int tenantId)
        {
            if (claimsIdentity == null) throw new ArgumentNullException("user");

            claimsIdentity.AddClaim(new Claim(ClaimTypes.GroupSid, tenantId.ToString()));

        }
        /// <summary>
        /// Provides method to add claim for roleId as Role property in ClaimTypes
        /// </summary>
        /// <param name="claimsIdentity"></param>
        /// <param name="roleId"></param>
        public static void AddRoleIdClaim(this ClaimsIdentity claimsIdentity, int roleId)
        {
            if (claimsIdentity == null) throw new ArgumentNullException("user");

            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleId.ToString()));
            List<ApplicationRoleClaim> claims = IdentityManager.GetRolesClaims(roleId);

            foreach (ApplicationRoleClaim claim in claims)
                claimsIdentity.AddClaim(new Claim(claim.ClaimType, claim.ClaimValue));

        }

        public static void AddRoleIdClaim(this ClaimsIdentity claimsIdentity, string ssoEnvironment)
        {
            if (claimsIdentity == null) throw new ArgumentNullException("user");

            int roleId = GetRoleSSOId(claimsIdentity, ssoEnvironment);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleId.ToString()));

            List<ApplicationRoleClaim> claims = IdentityManager.GetRolesClaims(roleId);

            foreach (ApplicationRoleClaim claim in claims)
                claimsIdentity.AddClaim(new Claim(claim.ClaimType, claim.ClaimValue));
        }

        public static bool HasClaim(this ClaimsIdentity user, string type)
        {
            if (user != null)
            {
                return user.HasClaim(x => x.Type.ToLower() == type.ToLower());
            }
            return false;
        }
    }
    public static class ClaimsExtensions
    {
        public static string GetValue(this IEnumerable<Claim> claims, string type)
        {
            if (claims == null) throw new ArgumentNullException("claims");

            if (claims != null)
            {
                var claim = claims.FirstOrDefault(x => x.Type == type);
                if (claim != null) return claim.Value;
            }

            return null;
        }
    }
}
