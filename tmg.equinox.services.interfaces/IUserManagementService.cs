using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.Settings;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IUserManagementService
    {
        List<UserManagementSettingsViewModel> GetUsersDetails(int tenantId,string currentUserName);
        ServiceResult CreateUser(string userName, string role, string email, string firstName, string lastName,string createdBy);
        List<UserManagementSettingsViewModel> GetUserRolesDetails(string userName);
        ServiceResult DeleteUser(string userName, int userId);
        ServiceResult UnlockUser(string userName, int userId);
        ServiceResult UpdateRole(string userName, string newUserRole,int userId,string updatedBy);
        List<UserManagementSettingsViewModel> GetUserRolesDetailsByName(string userName);
        ServiceResult sendEmailNotification(List<UserManagementSettingsViewModel> userNames, string sendGridUserName, string sendGridPassword,string smtpUserName,string smtpPort,string smtpHostServerName);
        ServiceResult ActivateUser(string userName);
        ServiceResult UpdateUserDetails(int UserID, string CurrentUserName);
        ServiceResult UpdateUserDetails(string currentUserName, bool isPasswordChanged);
        List<FormDesignUserSettingModel> GetFormDesignUserSetting(FormDesignUserSettingInputModel input);
        ServiceResult SaveFormDesignUserSetting(FormDesignUserSettingModel input);
        List<RoleViewModel> GetRoles();
        List<UserManagementSettingsViewModel> GetUsers();
        List<UserManagementSettingsViewModel> GetUserDetails(int tenantId, string currentUserName);
    }
}
