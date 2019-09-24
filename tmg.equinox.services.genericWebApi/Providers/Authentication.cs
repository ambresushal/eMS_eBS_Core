using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Collections.Specialized;
using System.Web.Mvc;
using tmg.equinox.applicationservices.viewmodels.Account;
using System.Configuration;
using tmg.equinox.identitymanagement;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.dependencyresolution;

namespace tmg.equinox.services.genericWebApi.Providers
{
    public class Authentication 
    {
        private IUserManagementService _userManagementService { get; set; }

        public Authentication() { }

        public  bool AuthenticateUser(string username, string password)
        {
            LoginViewModel viewModel = new LoginViewModel();
            viewModel.UserName = username;
            viewModel.Password = password;

            _userManagementService = UnityConfig.Resolve<IUserManagementService>();
            List<UserManagementSettingsViewModel> userDetails = this._userManagementService.GetUserRolesDetailsByName(viewModel.UserName);

             var success = IdentityManager.SignInAsync(viewModel.UserName, viewModel.Password);

             if (success && userDetails[0].IsActive == true)
             {
                 return true;

             }
             else
             {
                 return false;
             }
        }
    }
}