using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Routing;
using tmg.equinox.identitymanagement.Models;

namespace tmg.equinox.services.api.Models
{
    public class ModelFactory
    {
        private UrlHelper _UrlHelper;
        private ApplicationUserManager _AppUserManager;

        public ModelFactory(HttpRequestMessage request, ApplicationUserManager appUserManager)
        {
            _UrlHelper = new UrlHelper(request);
            _AppUserManager = appUserManager;
        }

        public UserReturnModel Create(ApplicationUser appUser)
        {
          
            return new UserReturnModel
            {
                //Url = _UrlHelper.Link("GetUserById", new { id = appUser.Id }),
                Id = appUser.Id,
                UserName = appUser.UserName,
                FullName = string.Format("{0} {1}", appUser.FirstName, appUser.LastName),
                Email = appUser.Email,
                EmailConfirmed = appUser.EmailConfirmed,
                 Roles = _AppUserManager.GetRolesAsync(appUser.Id).Result,
              //  Claims = _AppUserManager.GetClaimsAsync(appUser.Id).Result
            };
        }
        public ApplicationUser Create(string userName, string role, string email, string firstName, string lastName)
        {

            return new ApplicationUser
            {
                //Url = _UrlHelper.Link("GetUserById", new { id = appUser.Id }),
                UserName = userName,
                Email = email,
                EmailConfirmed = true,
                FirstName= firstName,
                LastName = lastName,
                TenantID = 1
            };
        }
        public RoleReturnModel Create(ApplicationRole appRole)
        {

            return new RoleReturnModel
            {
              //  Url = _UrlHelper.Link("GetRoleById", new { id = appRole.Id }),
                Id = appRole.Id.ToString(),
                Name = appRole.Name
            };

        }
        public List<RoleReturnModel> Create(List<ApplicationRole> appRoles)
        {
            List<RoleReturnModel> roleReturnModels = new List<RoleReturnModel>(); 
            foreach (var appRole in appRoles)
            {
                roleReturnModels.Add(new RoleReturnModel
                {
                    //  Url = _UrlHelper.Link("GetRoleById", new { id = appRole.Id }),
                    Id = appRole.Id.ToString(),
                    Name = appRole.Name
                });
            }

            return roleReturnModels;
        }
    }
    public class RoleReturnModel
    {
         public string Id { get; set; }
        public string Name { get; set; }
    }
    public class UserReturnModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public IList<string> Roles { get; set; }
       
    }
}