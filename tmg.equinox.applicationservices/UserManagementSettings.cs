using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.infrastructure.exceptionhandling;
using System.Data.SqlClient;
using tmg.equinox.applicationservices.viewmodels.EmailNotitication;
using tmg.equinox.emailnotification;
using tmg.equinox.emailnotification.Model;
using System.Configuration;
using tmg.equinox.emailnotification.model;
using System.Net.Mail;
using System.Linq.Expressions;

namespace tmg.equinox.applicationservices
{
    public class UserManagementSettings : IUserManagementService
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private EmailSetting emailSettings = new EmailSetting();
        private EmailLogger emailLoggerElements;
        private SendGridEmailNotification sendGridInvocation;
        private SmtpEmailNotification smtpInvocation;
        private EmailResponseData emailAcknowledgement;

        #region Constructor
        public UserManagementSettings(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion

        public List<UserManagementSettingsViewModel> GetUsersDetails(int tenantId, string currentUserName)
        {
            List<UserManagementSettingsViewModel> userManagementsettingsList = null;
            ServiceResult result = new ServiceResult();
            List<UserManagementSettingsViewModel> userRolesDetalis = GetUserRolesDetailsByName(currentUserName);

            try
            {
                if (userRolesDetalis[0].UserRole != "Simplify SuperUser")
                {
                    userManagementsettingsList = (from userMap in this._unitOfWork.RepositoryAsync<User>().Get()

                                                  join userRoleMap in this._unitOfWork.RepositoryAsync<UserRoleAssoc>().Get()

                                                  on userMap.UserID equals userRoleMap.UserId
                                                  join role in this._unitOfWork.RepositoryAsync<UserRole>().Get()
                                                  on userRoleMap.RoleId equals role.RoleID
                                                  where userMap.IsActive == true && userMap.IsVisible == true                                                
                                                  select new UserManagementSettingsViewModel
                                                  {
                                                      UserId = userRoleMap.UserId,
                                                      UserName = userMap.UserName,
                                                      UserRole = role.Name,
                                                      RoleId = userRoleMap.RoleId,
                                                      IsCurrentUser = currentUserName == userMap.UserName ? true : false,
                                                      CreatedBy = userMap.CreatedBy,
                                                      CreatedDate = userMap.CreatedDate,
                                                      UpdatedBy = userMap.UpdatedBy,
                                                      UpdatedDate = userMap.UpdatedDate,
                                                      FirstName = userMap.FirstName,
                                                      LastName = userMap.LastName,
                                                      Email = userMap.Email

                                                  }).ToList();
                }
                else
                {
                    userManagementsettingsList = (from userMap in this._unitOfWork.RepositoryAsync<User>().Get()

                                                  join userRoleMap in this._unitOfWork.RepositoryAsync<UserRoleAssoc>().Get()

                                                  on userMap.UserID equals userRoleMap.UserId
                                                  join role in this._unitOfWork.RepositoryAsync<UserRole>().Get()
                                                  on userRoleMap.RoleId equals role.RoleID
                                                  where userMap.IsActive == true
                                                  select new UserManagementSettingsViewModel
                                                  {
                                                      UserId = userRoleMap.UserId,
                                                      UserName = userMap.UserName,
                                                      UserRole = role.Name,
                                                      RoleId = userRoleMap.RoleId,
                                                      IsCurrentUser = currentUserName == userMap.UserName ? true : false,
                                                      CreatedBy = userMap.CreatedBy,
                                                      CreatedDate = userMap.CreatedDate,
                                                      UpdatedBy = userMap.UpdatedBy,
                                                      UpdatedDate = userMap.UpdatedDate,
                                                      LastName = userMap.LastName,
                                                      Email = userMap.Email,
                                                      FirstName = userMap.FirstName

                                                  }).ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
            }
            return userManagementsettingsList;
        }
        public List<UserManagementSettingsViewModel> GetUserDetails(int tenantId, string currentUserName)
        {
            List<UserManagementSettingsViewModel> userManagementsettingsList = null;
            ServiceResult result = new ServiceResult();
            List<UserManagementSettingsViewModel> userRolesDetalis = GetUserRolesDetailsByName(currentUserName);

            try
            {
                if (userRolesDetalis.Count > 0 && userRolesDetalis[0].UserRole != "Simplify SuperUser")
                {
                    userManagementsettingsList = (from userMap in this._unitOfWork.RepositoryAsync<User>().Get()

                                                  join userRoleMap in this._unitOfWork.RepositoryAsync<UserRoleAssoc>().Get()

                                                  on userMap.UserID equals userRoleMap.UserId
                                                  join role in this._unitOfWork.RepositoryAsync<UserRole>().Get()
                                                  on userRoleMap.RoleId equals role.RoleID
                                                  where userMap.IsActive == true && userMap.IsVisible == true
                                                  && userMap.UserName == currentUserName
                                                  select new UserManagementSettingsViewModel
                                                  {
                                                      UserId = userRoleMap.UserId,
                                                      UserName = userMap.UserName,
                                                      UserRole = role.Name,
                                                      RoleId = userRoleMap.RoleId,
                                                      IsCurrentUser = currentUserName == userMap.UserName ? true : false,
                                                      CreatedBy = userMap.CreatedBy,
                                                      CreatedDate = userMap.CreatedDate,
                                                      UpdatedBy = userMap.UpdatedBy,
                                                      UpdatedDate = userMap.UpdatedDate,
                                                      FirstName = userMap.FirstName,
                                                      LastName = userMap.LastName,
                                                      Email = userMap.Email

                                                  }).ToList();
                }
                else
                {
                    userManagementsettingsList = (from userMap in this._unitOfWork.RepositoryAsync<User>().Get()

                                                  join userRoleMap in this._unitOfWork.RepositoryAsync<UserRoleAssoc>().Get()

                                                  on userMap.UserID equals userRoleMap.UserId
                                                  join role in this._unitOfWork.RepositoryAsync<UserRole>().Get()
                                                  on userRoleMap.RoleId equals role.RoleID
                                                  where userMap.IsActive == true
                                                       && userMap.UserName == currentUserName
                                                  select new UserManagementSettingsViewModel
                                                  {
                                                      UserId = userRoleMap.UserId,
                                                      UserName = userMap.UserName,
                                                      UserRole = role.Name,
                                                      RoleId = userRoleMap.RoleId,
                                                      IsCurrentUser = currentUserName == userMap.UserName ? true : false,
                                                      CreatedBy = userMap.CreatedBy,
                                                      CreatedDate = userMap.CreatedDate,
                                                      UpdatedBy = userMap.UpdatedBy,
                                                      UpdatedDate = userMap.UpdatedDate,
                                                      LastName = userMap.LastName,
                                                      Email = userMap.Email,
                                                      FirstName = userMap.FirstName

                                                  }).ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
            }
            return userManagementsettingsList;
        }
        public List<UserManagementSettingsViewModel> GetUsers()
        {
            List<UserManagementSettingsViewModel> userManagementsettingsList = null;
            ServiceResult result = new ServiceResult();
            
            try
            {
               
                    userManagementsettingsList = (from userMap in this._unitOfWork.RepositoryAsync<User>().Get()

                                                  join userRoleMap in this._unitOfWork.RepositoryAsync<UserRoleAssoc>().Get()

                                                  on userMap.UserID equals userRoleMap.UserId
                                                  join role in this._unitOfWork.RepositoryAsync<UserRole>().Get()
                                                  on userRoleMap.RoleId equals role.RoleID
                                                  where userMap.IsActive == true
                                                  select new UserManagementSettingsViewModel
                                                  {
                                                      UserId = userRoleMap.UserId,
                                                      UserName = userMap.UserName,
                                                      UserRole = role.Name,
                                                      RoleId = userRoleMap.RoleId,
                                                      CreatedBy = userMap.CreatedBy,
                                                      CreatedDate = userMap.CreatedDate,
                                                      UpdatedBy = userMap.UpdatedBy,
                                                      UpdatedDate = userMap.UpdatedDate,
                                                      LastName = userMap.LastName,
                                                      Email = userMap.Email,
                                                      FirstName = userMap.FirstName

                                                  }).ToList();

                return userManagementsettingsList;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
            }
            return userManagementsettingsList;
        }
        public ServiceResult IsUserAlreadyExist(string userName)
        {
            ServiceResult result = new ServiceResult();

            var ifUserActive = this._unitOfWork.RepositoryAsync<User>()
               .Get().Where(c => c.UserName == userName).FirstOrDefault();

            if (ifUserActive == null)
                result.Result = ServiceResultStatus.Success;
            else
            {
                result.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "User already Exist" } });

            }
            return result;
        }

        public ServiceResult UpdateUserDetails(int UserID, string CurrentUserName)
        {
            ServiceResult result = new ServiceResult();
            result.Result = ServiceResultStatus.Failure;
            var user = (from us in this._unitOfWork.RepositoryAsync<User>().Get()
                        where us.UserID == UserID
                        select us).FirstOrDefault();
            if (user != null)
            {
                user.UpdatedBy = CurrentUserName;
                user.UpdatedDate = DateTime.Now;
                this._unitOfWork.RepositoryAsync<User>().Update(user);
                this._unitOfWork.Save();
                result.Result = ServiceResultStatus.Success;
            }

            return result;
        }

        public ServiceResult UpdateUserDetails(string currentUserName, bool isPasswordChanged)
        {
            ServiceResult result = new ServiceResult();
            result.Result = ServiceResultStatus.Failure;
            var user = (from us in this._unitOfWork.RepositoryAsync<User>().Get()
                        where us.UserName == currentUserName
                        select us).FirstOrDefault();
            if (user != null)
            {
                user.ChangeInitialPassword = isPasswordChanged;
                user.UpdatedBy = currentUserName;
                user.UpdatedDate = DateTime.Now;
                this._unitOfWork.RepositoryAsync<User>().Update(user);
                this._unitOfWork.Save();
                result.Result = ServiceResultStatus.Success;
            }

            return result;
        }

        public List<UserManagementSettingsViewModel> GetUserRolesDetails(string userName)
        {
            List<UserManagementSettingsViewModel> userRoleList = null;
            List<UserManagementSettingsViewModel> userRolesDetalis = GetUserRolesDetailsByName(userName);
            try
            {

                if (userRolesDetalis[0].UserRole == "Simplify SuperUser")
                {

                    userRoleList = (from userRole in this._unitOfWork.RepositoryAsync<UserRole>()
                                        .Query().Get()
                                    select new UserManagementSettingsViewModel
                                    {
                                        UserRole = userRole.Name,
                                        RoleId = userRole.RoleID,
                                    }).ToList();

                }
                else
                {
                    userRoleList = (from userRole in this._unitOfWork.RepositoryAsync<UserRole>()
                                        .Query().Get().Where(c => c.Name != "Simplify SuperUser")
                                    select new UserManagementSettingsViewModel
                                    {
                                        UserRole = userRole.Name,
                                        RoleId = userRole.RoleID,
                                    }).ToList();

                }


            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(new Exception("GetUserDetail - " + ex.Message), ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return userRoleList;
        }
        public List<RoleViewModel> GetRoles()
        {
            List<RoleViewModel> userRoleList = null;
            try
            {

               
                    userRoleList = (from userRole in this._unitOfWork.RepositoryAsync<UserRole>()
                                        .Query().Get().Where(c => c.Name != "TMG Super User")
                                    select new RoleViewModel
                                    {
                                        Name = userRole.Name,
                                        Id = userRole.RoleID,
                                    }).ToList();

               
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(new Exception("GetUserDetail - " + ex.Message), ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return userRoleList;
        }
        public List<UserManagementSettingsViewModel> GetUserRolesDetailsByName(string userName)
        {
            List<UserManagementSettingsViewModel> userManagementsettingsList = null;
            ServiceResult result = new ServiceResult();
            try
            {
                userManagementsettingsList = (from userMap in this._unitOfWork.RepositoryAsync<User>().Get()

                                              join userRoleMap in this._unitOfWork.RepositoryAsync<UserRoleAssoc>().Get()

                                              on userMap.UserID equals userRoleMap.UserId
                                              join role in this._unitOfWork.RepositoryAsync<UserRole>().Get()
                                              on userRoleMap.RoleId equals role.RoleID
                                              where userMap.UserName == userName
                                              select new UserManagementSettingsViewModel
                                              {
                                                  UserId = userRoleMap.UserId,
                                                  UserName = userMap.UserName,
                                                  UserRole = role.Name,
                                                  RoleId = userRoleMap.RoleId,
                                                  IsActive = userMap.IsActive,
                                                  ChangeInitialPassword = userMap.ChangeInitialPassword,
                                                  UpdatedDate = userMap.UpdatedDate
                                              }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
            }
            return userManagementsettingsList;
        }

        public ServiceResult UpdateRole(string userName, string newUserRole, int userId, string updatedBy)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();
                SqlParameter paramUserName = new SqlParameter("@userName", userName);
                SqlParameter paramNewUserRole = new SqlParameter("@roleName", newUserRole);

                UserRole userRoles = this._unitOfWork.Repository<UserRole>().ExecuteSql("exec [dbo].[Change_User_Role] @userName,@roleName",
                             paramUserName, paramNewUserRole).ToList().FirstOrDefault();
                User user = _unitOfWork.RepositoryAsync<User>().FindById(userId);

                if (user != null)
                {

                    user.UpdatedBy = updatedBy;
                    user.UpdatedDate = System.DateTime.Now;

                    this._unitOfWork.RepositoryAsync<User>().Update(user);
                    this._unitOfWork.Save();
                }

                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(new Exception("UpdateUserRole - " + ex.Message), ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        public ServiceResult DeleteUser(string userName, int userId)
        {
            ServiceResult result = new ServiceResult();

            User user = _unitOfWork.RepositoryAsync<User>().FindById(userId);

            if (user != null)
            {
                user.IsActive = false;

                this._unitOfWork.RepositoryAsync<User>().Update(user);
                this._unitOfWork.Save();

                result.Result = ServiceResultStatus.Success;
            }
            else
            {
                result.Result = ServiceResultStatus.Failure;
                throw new Exception("User Does Not  exists");
            }
            return result;
        }

        public ServiceResult UnlockUser(string userName, int userId)
        {
            ServiceResult result = new ServiceResult();

            User user = _unitOfWork.RepositoryAsync<User>().FindById(userId);

            if (user != null)
            {
                user.LockoutEndDateUtc = null;
                user.AccessFailedCount = 0;

                this._unitOfWork.RepositoryAsync<User>().Update(user);
                this._unitOfWork.Save();

                result.Result = ServiceResultStatus.Success;
            }
            else
            {
                result.Result = ServiceResultStatus.Failure;
                throw new Exception("User Does Not  exists");
            }
            return result;
        }

        public ServiceResult CreateUser(string userName, string role, string email, string firstName, string lastName, string createdBy)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                List<UserManagementSettingsViewModel> userDetails = GetUserRolesDetailsByName(userName).Where(c => c.IsActive == true).ToList();

                result = IsUserAlreadyExist(userName);
                if (result.Result == ServiceResultStatus.Success)
                {
                    result = IsUserEmailAlreadyExist(email);
                }
                else
                {
                    return result;
                }

                if (result.Result == ServiceResultStatus.Success)
                {

                    User user = new User();

                    SqlParameter paramUserName = new SqlParameter("@username", userName);
                    SqlParameter paramUserRole = new SqlParameter("@role", role);
                    SqlParameter paramUserEmail = new SqlParameter("@email", email);
                    SqlParameter paramUserFirstName = new SqlParameter("@firstName", firstName);
                    SqlParameter paramUserLastName = new SqlParameter("@lastName", lastName);
                    SqlParameter paramUserCreatedBy = new SqlParameter("@createdBy", createdBy);

                    user = this._unitOfWork.Repository<User>().ExecuteSql("exec [dbo].[Create_New_User] @username,@role,@email,@firstName ,@lastName,@createdBy",
                                 paramUserName, paramUserRole, paramUserEmail, paramUserFirstName, paramUserLastName, paramUserCreatedBy).ToList().FirstOrDefault();

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                    ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "User already Exist" } });
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(new Exception("NewUserDetails - " + ex.Message), ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        public ServiceResult IsUserEmailAlreadyExist(string email)
        {
            ServiceResult result = new ServiceResult();

            var ifUserActive = this._unitOfWork.RepositoryAsync<User>()
               .Get().Where(c => c.Email == email).FirstOrDefault();

            if (ifUserActive == null)
            {
                result.Result = ServiceResultStatus.Success;

            }
            else
            {
                result.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Email Already Exist" } });

            }
            return result;

        }

        public ServiceResult ActivateUser(string userName)
        {
            List<UserManagementSettingsViewModel> getUserDetails = GetUserRolesDetailsByName(userName);
            ServiceResult result = new ServiceResult();
            User user = _unitOfWork.RepositoryAsync<User>().FindById(getUserDetails.FirstOrDefault().UserId);
            if (user != null)
            {
                user.IsActive = true;
                this._unitOfWork.RepositoryAsync<User>().Update(user);
                this._unitOfWork.Save();

                result.Result = ServiceResultStatus.Success;
            }
            else
            {
                result.Result = ServiceResultStatus.Failure;
                throw new Exception("Problem occurred while activating user, Please try again!!");
            }
            return result;
        }

        public ServiceResult sendEmailNotification(List<UserManagementSettingsViewModel> UserDetails, string sendGridUserName, string sendGridPassword, string smtpUserName, string smtpPort, string smtpServerHostName)
        {
            ServiceResult emailNotificationResult = new ServiceResult();
            emailLoggerElements = new EmailLogger();
            // sendGridInvocation = new SendGridEmailNotification();
            smtpInvocation = new SmtpEmailNotification();

            emailAcknowledgement = new EmailResponseData();
            try
            {

                foreach (var userdetail in UserDetails)
                {
                    List<string> sendMailToList = new List<string>();
                    sendMailToList.Add(userdetail.Email);

                    emailLoggerElements.Name = userdetail.FirstName + ' ' + userdetail.LastName;
                    emailLoggerElements.EmailID = userdetail.Email;
                    emailLoggerElements.UserName = userdetail.UserName;
                    emailLoggerElements.Password = ConfigurationManager.AppSettings["DefaultPassword"];
                    emailLoggerElements.URL = ConfigurationManager.AppSettings["URL"];
                    emailSettings.To = sendMailToList;
                    //emailSettings.SendGridUserName = sendGridUserName;
                    //emailSettings.SendGridPassword = sendGridPassword;
                    //emailSettings.SendGridFrom = ((!string.IsNullOrEmpty(sendGridUserName)) && (!string.IsNullOrEmpty(emailSettings.DisplayName))) ? new MailAddress(sendGridUserName, emailSettings.DisplayName) : null;

                    emailSettings.SmtpFrom = ((!string.IsNullOrEmpty(smtpUserName)) && (!string.IsNullOrEmpty(emailSettings.DisplayName))) ? new MailAddress(smtpUserName, emailSettings.DisplayName) : null;
                    emailSettings.SmtpPort = Convert.ToInt32(smtpPort);
                    emailSettings.SmtpServerHostName = smtpServerHostName;

                    ServiceResult emailValidationResult = ValidateEmailSettings(emailSettings);
                    if (emailValidationResult.Result == ServiceResultStatus.Success)
                    {
                        string appName = config.Config.GetApplicationName();
                        emailSettings.SubjectLine = string.Format(UserEmailNotificationConstants.SubjectLineFormat, appName);

                        emailSettings.Text = string.Format(UserEmailNotificationConstants.CreateNewEmailHtml, emailLoggerElements.Name, emailLoggerElements.EmailID,
                                                            emailLoggerElements.UserName, emailLoggerElements.Password, emailLoggerElements.URL, appName);
                        emailLoggerElements.EmailContent = emailSettings.Text;
                        //emailAcknowledgement = sendGridInvocation.SendMessage(emailSettings);
                        emailAcknowledgement = smtpInvocation.SendMessage(emailSettings);
                        emailLoggerElements.Comment = emailAcknowledgement.Message;
                        emailNotificationResult.Result = ServiceResultStatus.Success;
                    }
                    else
                    {
                        emailNotificationResult.Result = ServiceResultStatus.Failure;
                        emailLoggerElements.Comment = string.Join(", ", emailValidationResult.Items.Select(x => x.Messages.FirstOrDefault().ToString()).ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                //Return Failure result
                emailLoggerElements.Comment = UserEmailNotificationConstants.UnknownExceptionErrorMessage;
                emailNotificationResult = ex.ExceptionMessages();
            }

            return emailNotificationResult;

        }

        internal class UserEmailNotificationConstants
        {
            public const string SubjectLineFormat = "{0} Production credentials";
            public const string EmailNotificationErrorMessage = "EmailSettings contain an insufficient information, hence unable delivered an email.Please contact support team.";
            public const string CreateNewEmailHtml = " <p style='font-family:Calibri;font-size:15px;'>Hello,<br /><br />Please see below for your eBenefitSync Production credentials.</p>" +
                                                                    "<table style='font-family:Calibri;font-size:15px;padding-left:2%'>" +
                                                                        "<tbody>" +
                                                                            "<tr><td>Name</td><td>  : {0}</td> </tr>" +
                                                                            "<tr><td>Email</td><td>  : {1}</td></tr>" +
                                                                            "<tr><td>User Name:  </td><td>  : {2}</td></tr>" +
                                                                            "<tr><td>Password</td><td>  : {3}</td></tr>" +
                                                                            "<tr><td> {5} URL</td><td>  :<a href='{4}'> {4}</a></td></tr>" +
                                                                        "</tbody>" +
                                                                    "</table>" +
                                                                "<p style='font-family:Calibri;font-size:15px;'>Regards,<br />CBC Support Team.</p><br />" +
                                                                "<p style='font-style:italic;font-family:Calibri;font-size:15px;'>This is Autogenerated Mail. Please do not reply.</p> ";

            public const string UnknownExceptionErrorMessage = "Unbale to process email notification request.Please contact support team.";

            public const string EmptyEmailSentToListMessage = "Email Sent To list is Empty.Hence unable delivered an email.Please contact support team.";
        }

        private List<string> GetUserDetailsForEmailNotification(List<UserManagementSettingsViewModel> UserDetails)
        {
            List<string> sendMailToList = new List<string>();
            foreach (var userdetail in UserDetails)
            {
                sendMailToList.Add(userdetail.Email);
            }
            sendMailToList = sendMailToList.Distinct().ToList();
            return sendMailToList;
        }

        private ServiceResult ValidateEmailSettings(EmailSetting emailSettings)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();

            try
            {
                result.Result = ServiceResultStatus.Success;
                //if (emailSettings.SendGridFrom == null || string.IsNullOrEmpty(emailSettings.SendGridPassword))
                //{
                //    items.Add(new ServiceResultItem() { Messages = new string[] { EmailNotificationConstants.EmailNotificationErrorMessage } });
                //}
                if (emailSettings.SmtpFrom == null || string.IsNullOrEmpty(emailSettings.SmtpServerHostName))
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { EmailNotificationConstants.EmailNotificationErrorMessage } });
                }
                if (emailSettings.To == null || emailSettings.To.Count() <= 0)
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { EmailNotificationConstants.EmptyEmailSentToListMessage } });
                }

                if (items.Count() > 0)
                {
                    result.Result = ServiceResultStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                items.Add(new ServiceResultItem() { Messages = new string[] { EmailNotificationConstants.UnknownExceptionErrorMessage } });
                result.Result = ServiceResultStatus.Failure;
            }
            result.Items = items;
            return result;
        }

        public List<FormDesignUserSettingModel> GetFormDesignUserSetting(FormDesignUserSettingInputModel input)
        {


            Expression<Func<FormDesignUserSetting, bool>> filterCriteria = c => c.FormDesignVersionID == input.FormDesignVersionID && c.UserId == input.UserId && (c.Key == input.Key || c.Key == input.Key1);


            if (string.IsNullOrEmpty(input.Key))
            {
                filterCriteria = c => c.FormDesignVersionID == input.FormDesignVersionID && c.UserId == input.UserId;
            }

            var formDesignUserSettings = (from ver in this._unitOfWork.RepositoryAsync<FormDesignUserSetting>()
                                          .Query()
                                          .Filter(filterCriteria)
                                          .Get()
                                          select new FormDesignUserSettingModel
                                          {
                                              FormDesignUserSettingID = ver.FormDesignUserSettingID,
                                              FormDesignVersionID = ver.FormDesignVersionID,
                                              Key = ver.Key,
                                              UserId = ver.UserId,
                                              Data = ver.Data,
                                              LevelAt = ver.LevelAt
                                          }).ToList();

            return formDesignUserSettings;
        }
        private FormDesignUserSetting GetFormDesignUserSetting(FormDesignUserSettingModel input)
        {
            var formDesignUserSettings = (from ver in this._unitOfWork.RepositoryAsync<FormDesignUserSetting>()
                                          .Query()
                                          .Filter(c => c.FormDesignVersionID == input.FormDesignVersionID && c.UserId == input.UserId && c.Key == input.Key && c.LevelAt == input.LevelAt)
                                          .Get()
                                          select ver).FirstOrDefault();

            return formDesignUserSettings;
        }
        public ServiceResult SaveFormDesignUserSetting(FormDesignUserSettingModel input)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            var formDesignUserSetting = new FormDesignUserSetting();
            try
            {
                formDesignUserSetting = GetFormDesignUserSetting(input);
                if (formDesignUserSetting == null)
                {
                    formDesignUserSetting = new FormDesignUserSetting();
                    formDesignUserSetting.AddedBy = input.AddedBy;
                    formDesignUserSetting.AddedDate = input.AddedDate;
                }

                formDesignUserSetting.Key = input.Key;
                formDesignUserSetting.Data = input.Data;
                formDesignUserSetting.LevelAt = input.LevelAt;
                formDesignUserSetting.FormDesignVersionID = input.FormDesignVersionID;
                formDesignUserSetting.UpdatedBy = input.UpdatedBy;
                formDesignUserSetting.UpdatedDate = input.UpdatedDate;
                formDesignUserSetting.UserId = input.UserId;

                if (formDesignUserSetting.FormDesignUserSettingID == 0)
                {
                    formDesignUserSetting.AddedBy = input.AddedBy;
                    formDesignUserSetting.AddedDate = input.AddedDate;
                    this._unitOfWork.RepositoryAsync<FormDesignUserSetting>().Insert(formDesignUserSetting);
                }
                else
                {
                    this._unitOfWork.RepositoryAsync<FormDesignUserSetting>().Update(formDesignUserSetting);
                }

                this._unitOfWork.Save();
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;

                items.Add(new ServiceResultItem() { Messages = new string[] { SettingConstants.UnknownExceptionErrorMessage } });
                result.Result = ServiceResultStatus.Failure;
            }
            result.Items = items;
            return result;
        }
    }
}
