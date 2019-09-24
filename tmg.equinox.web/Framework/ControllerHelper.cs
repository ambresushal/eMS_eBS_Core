using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using tmg.equinox.applicationservices.viewmodels;

namespace tmg.equinox.web.Framework
{
    public class ControllerHelper
    {
        public static List<RoleClaimModel> SetClaimsInView(List<Claim> claims)
        {
            List<RoleClaimModel> roleClaims = new List<RoleClaimModel>();
            claims.ToList().ForEach(c =>
               roleClaims.Add(new RoleClaimModel
               {
                   Resource = c.Type.Trim(),
                   Action = c.Value.Trim(),
                   ResourceType = c.ValueType.Trim()
               }));
            return roleClaims;
        }

        public static List<RoleClaimModel> FilterClaimsByMethod(List<Claim> claims, string methodName)
        {
            List<RoleClaimModel> roleClaims = new List<RoleClaimModel>();
            claims.Where(c => c.Type.ToLower().Trim().Contains(methodName.ToLower().Trim())).ToList().ForEach(c =>
               roleClaims.Add(new RoleClaimModel
               {
                   Resource = c.Type.Trim(),
                   Action = c.Value.Trim(),
                   ResourceType = c.ValueType.Trim()
               }));
            return roleClaims;
        }
    }
}