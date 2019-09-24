using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.identitymanagement.Enums;
using tmg.equinox.identitymanagement.Models;

namespace tmg.equinox.mapper
{
    public class IdentityAccessMapper
    {
        public List<ApplicationRoleClaim> MapToApplicationRoleClaims(List<ElementAccessViewModel> sectionaccessRows, int userId, ResourceType rsType) //here userId is temporary parameter
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

        public List<ElementAccessViewModel> MapToElementAcessViewModel(List<ApplicationRoleClaim> applicationRoleClaims, List<ApplicationRole> roles) //here userId is teprary parameter
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

    }
}
