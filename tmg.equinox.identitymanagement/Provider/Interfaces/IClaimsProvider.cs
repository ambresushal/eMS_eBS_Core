using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.identitymanagement.Enums;
using tmg.equinox.identitymanagement.Models;

namespace tmg.equinox.identitymanagement.Provider
{
    /// <summary>
    /// Provides an interface to claims related database CRUD operations
    /// </summary>
    public interface IClaimsProvider
    {
        List<Claim> GetIdentityClaims(List<Claim> claims,string controllerName, string actionName, ResourceType resource = ResourceType.ACTION);                
        //void AddClaim(List<Claim> claims);
        void AddUpdateClaim(List<ApplicationRoleClaim> RoleClaims);
        void RemoveClaim(int resourceId, ResourceType resourceType);
        //void RemoveClaim(List<Claim> claims);   
    }
}
