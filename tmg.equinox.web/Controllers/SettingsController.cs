using Microsoft.Owin.Security.Cookies;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.identitymanagement;
using tmg.equinox.web.FormInstance;
using tmg.equinox.web.Framework;
using tmg.equinox.setting.Interface;
using tmg.equinox.setting.Common;

namespace tmg.equinox.web.Controllers
{
    public class SettingsController : AuthenticatedController
    {

        #region Variables
        const string ResetPasswordStatus = "Your password has been reset successfully !";
        private IAutoSaveSettingsService _autoSaveSettingsService;
        private IWorkFlowSettingsService _workFlowSettingsService;
        private IUserManagementService _userManagementService;
        private IFolderVersionServices _folderVersionServices;
        private IWorkFlowVersionStatesService _workFlowVersionStatesService;
        private IWorkFlowCategoryMappingService _workFlowCategoryMappingService;
        private IWorkFlowMasterService _workFlowMasterService;
        private IWorkFlowVersionStatesAccessService _workFlowVersionStatesAccessService;
        private IWFVersionStatesApprovalTypeService _wFVersionStatesApprovalTypeService;
        private string sendGridUserName = string.Empty;
        private string sendGridPassword = string.Empty;
        private string smtpUserName = string.Empty;
        private string smtpPassword = string.Empty;
        private string smtpPort = string.Empty;
        private string smtpHostServerName = string.Empty;
        private ISettingManager _settingManager;
        #endregion


        #region Constructor

        public SettingsController()
        {

        }
        public SettingsController(IAutoSaveSettingsService autoSaveSettingsService, IWorkFlowSettingsService workFlowSettingsService, IUserManagementService userManagementService, IFolderVersionServices folderVersionServices, IWorkFlowCategoryMappingService workFlowCategoryMappingService, IWorkFlowVersionStatesService workFlowVersionStatesService, IWorkFlowMasterService workFlowMasterService, IWorkFlowVersionStatesAccessService workFlowVersionStatesAccessService, IWFVersionStatesApprovalTypeService wFVersionStatesApprovalTypeService, ISettingManager settingManager)
        {
            this._autoSaveSettingsService = autoSaveSettingsService;
            this._workFlowSettingsService = workFlowSettingsService;
            this._userManagementService = userManagementService;
            this._folderVersionServices = folderVersionServices;
            this._workFlowCategoryMappingService = workFlowCategoryMappingService;
            this._workFlowVersionStatesService = workFlowVersionStatesService;
            this._workFlowMasterService = workFlowMasterService;
            this._wFVersionStatesApprovalTypeService = wFVersionStatesApprovalTypeService;
            this._workFlowVersionStatesAccessService = workFlowVersionStatesAccessService;
            this._settingManager = settingManager;
        }

        #endregion Constructor


        #region Action Methods

        // GET: /Settings/
        public ActionResult Index()
        {
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);

            return View();
        }

        public ActionResult WorkFlowSettings()
        {
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            ViewBag.RoleId = RoleID;
            return View();
        }
        //[NonAction]
        public ActionResult UserManagementSettings()
        {
            ViewBag.RoleId = RoleID;
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            return View();
        }

        [HttpGet]
        public JsonResult GetUsersDetails(int tenantId)
        {
            List<UserManagementSettingsViewModel> userList = _userManagementService.GetUsersDetails(tenantId, CurrentUserName);
            return Json(userList, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult CreateUser(string userName, string userRole, string email, string firstName, string lastName)
        {
            ServiceResult result = this._userManagementService.CreateUser(userName, userRole, email, firstName, lastName, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateRole(string userName, string userRole, string newUserRole, int userId)
        {

            ServiceResult result = this._userManagementService.UpdateRole(userName, newUserRole, userId, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteUser(string userName, int userId)
        {
            ServiceResult result = this._userManagementService.DeleteUser(userName, userId);
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult UnlockUser(string userName, int userId)
        {
            ServiceResult result = this._userManagementService.UnlockUser(userName, userId);
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ExportToExcel(string csv, int noOfColInGroup, string repeaterName)
        {
            string header = string.Empty;

            header = "User Management";

            ExcelBuilder excelBuilder = new ExcelBuilder();
            MemoryStream fileStream = excelBuilder.ExportToExcel(csv, false, noOfColInGroup, false, header);

            var fileDownloadName = repeaterName + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var fsr = new FileStreamResult(fileStream, contentType);
            fsr.FileDownloadName = fileDownloadName;

            return fsr;
        }

        [HttpGet]
        public ActionResult GetUserRolesDetails()
        {

            List<UserManagementSettingsViewModel> userRoleList = _userManagementService.GetUserRolesDetails(CurrentUserName);
            return Json(userRoleList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ResetPassword(int userID, string userName, string password)
        {
            ServiceResult result = new ServiceResult();

            bool isResetPasswordSuccessful = IdentityManager.ResetPasswordAsync(userID, userName, password);
            if (isResetPasswordSuccessful && CurrentUserName != userName)
            {
                result = _userManagementService.UpdateUserDetails(userID, CurrentUserName);
                //result.Result = ServiceResultStatus.Success;
            }
            else if (isResetPasswordSuccessful && CurrentUserName == userName)
            {
                result.Result = ServiceResultStatus.Warning;
            }
            else
            {
                result.Result = ServiceResultStatus.Failure;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetApplicableTeamList(int tenantId)
        {

            IList<KeyValue> workflowTeamMemberList = _workFlowSettingsService.GetApplicableTeamList(tenantId);
            return Json(workflowTeamMemberList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateApplicableTeamUserMap(int tenantId, int teamId, string selectedUserListData)
        {
            ServiceResult result = null;
            List<ApplicableTeamMapModel> MemberList = JsonConvert.DeserializeObject<List<ApplicableTeamMapModel>>(selectedUserListData);
            result = _workFlowSettingsService.UpdateApplicableTeamUserMap(tenantId, teamId, MemberList, base.CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetApplicableTeamUserList(int tenantId, int teamId)
        {
            List<WorkFlowSettingsViewModel> list = _workFlowSettingsService.GetApplicableTeamUserMap(tenantId, teamId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSessionTimeOut()
        {

            NameValueCollection owinAuthenticationConfigurationSection = (NameValueCollection)
             ConfigurationManager.GetSection("owinAuthentication");
            CookieAuthenticationOptions cookieAuthenticationOptions = new CookieAuthenticationOptions
            {
                ExpireTimeSpan = TimeSpan.FromMinutes(Convert.ToDouble(owinAuthenticationConfigurationSection["TimeOut"]))
            };

            return Json(cookieAuthenticationOptions.ExpireTimeSpan.TotalMinutes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveAutoSaveSettings(AutoSaveSettingsViewModel settingsViewModel)
        {

            var result = _autoSaveSettingsService.SaveAutoSaveSettings(settingsViewModel,
                CurrentUserId, CurrentUserName);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAutoSaveSettingsForTenant(int tenantID)
        {
            var result = _autoSaveSettingsService.GetAutoSaveSettingsForTenant(tenantID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAutoSaveDuration(int tenantID)
        {
            var result = _autoSaveSettingsService.GetAutoSaveDuration(tenantID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ActivateUser(string userName)
        {
            ServiceResult result = new ServiceResult();
            result = _userManagementService.ActivateUser(userName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFolderVersionCategoryList(int tenantID, GridPagingRequest gridPagingRequest)
        {

            GridPagingResponse<FolderVersionCategoryViewModel> consortiumList = this._folderVersionServices.GetFolderVersionCategoryList(tenantID, gridPagingRequest);

            ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims, GetActionName);
            return Json(consortiumList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddFolderVersionCategory(int tenantID, string folderVersionCategoryName, int folderVersionGroupID)
        {
            ServiceResult result = this._folderVersionServices.AddFolderVersionCategory(tenantID, folderVersionCategoryName, folderVersionGroupID, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateFolderVersionCategory(int tenantID, int folderVersionCategoryID, string folderVersionCategoryName, int folderVersionGroupID)
        {
            ServiceResult result = this._folderVersionServices.UpdateFolderVersionCategory(tenantID, folderVersionCategoryID, folderVersionCategoryName, folderVersionGroupID, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteFolderVersionCategory(int tenantID, int folderVersionCategoryID, string folderVersionCategoryName)
        {
            ServiceResult result = this._folderVersionServices.DeleteFolderVersionCategory(tenantID, folderVersionCategoryID, folderVersionCategoryName, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFolderVersionCategoryForDropdown(int tenantID, bool? isPortfolio = null, int folderVersionID = 0, bool? isFinalized = null)
        {
            //bool isPortfolio = false;
            IEnumerable<FolderVersionCategoryViewModel> categoryList = this._folderVersionServices.GetFolderVersionCategoryForDropdown(tenantID, isPortfolio, folderVersionID, isFinalized);
            if (categoryList == null)
            {
                categoryList = new List<FolderVersionCategoryViewModel>();
            }
            return Json(categoryList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFolderVersionGroupList(int tenantID, GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<FolderVersionGroupViewModel> consortiumList = this._folderVersionServices.GetFolderVersionGroupList(tenantID, gridPagingRequest);
            ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims, GetActionName);
            return Json(consortiumList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddFolderVersionGroup(int tenantID, string folderVersionGroupName)
        {
            ServiceResult result = this._folderVersionServices.AddFolderVersionGroup(tenantID, folderVersionGroupName, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateFolderVersionGroup(int tenantID, int folderVersionGroupID, string folderVersionGroupName)
        {
            ServiceResult result = this._folderVersionServices.UpdateFolderVersionGroup(tenantID, folderVersionGroupID, folderVersionGroupName, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteFolderVersionGroup(int folderVersionGroupID, string folderVersionGroupName)
        {
            ServiceResult result = this._folderVersionServices.DeleteFolderVersionGroup(folderVersionGroupID, folderVersionGroupName, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFolderVersionGroupForDropdown(int tenantID)
        {
            IEnumerable<FolderVersionGroupViewModel> groupList = this._folderVersionServices.GetFolderVersionGroupForDropdown(tenantID);
            if (groupList == null)
            {
                groupList = new List<FolderVersionGroupViewModel>();
            }
            return Json(groupList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendMailNotification(string UserDetails)
        {

            List<UserManagementSettingsViewModel> userDetailList = JsonConvert.DeserializeObject<List<UserManagementSettingsViewModel>>(UserDetails);
            InitializeEmailSettings();
            ServiceResult result = new ServiceResult();

            result = _userManagementService.sendEmailNotification(userDetailList, sendGridUserName, sendGridPassword, smtpUserName, smtpPort, smtpHostServerName);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void InitializeEmailSettings()
        {
            sendGridUserName = ConfigurationManager.AppSettings["SendGridUserName"] ?? string.Empty;
            sendGridPassword = ConfigurationManager.AppSettings["SendGridPassword"] ?? string.Empty;
            smtpUserName = ConfigurationManager.AppSettings["SmtpUserName"] ?? string.Empty;
            smtpPort = ConfigurationManager.AppSettings["SmtpPort"] ?? string.Empty;
            smtpHostServerName = ConfigurationManager.AppSettings["SmtpHostServerName"] ?? string.Empty;
        }


        [HttpGet]
        public JsonResult GetFormDesignUserSetting(FormDesignUserSettingInputModel input)
        {

            input.UserId = Convert.ToInt32(base.CurrentUserId);

            var formDesignUserSettingModel = _userManagementService.GetFormDesignUserSetting(input);

            return Json(formDesignUserSettingModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveFormDesignUserSetting(FormDesignUserSettingModel input)
        {

            input.UserId = Convert.ToInt32(base.CurrentUserId);
            input.UpdatedBy = base.CurrentUserName;
            input.UpdatedDate = DateTime.Now;

            if (input.FormDesignUserSettingID == 0)
            {
                input.AddedBy = input.UpdatedBy;
                input.AddedDate = DateTime.Now;
            }

            var result = _userManagementService.SaveFormDesignUserSetting(input);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveUnLockTimeOutSetting(string unlockTimeOutInMin, string tenantId)
        {
            _settingManager.SaveLockSetting(unlockTimeOutInMin);

            return Json(new ServiceResult { Result = ServiceResultStatus.Success }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUnLockTimeOutSetting(string tenantId)
        {
            var unlockSetting = _settingManager.GetSettingValue(SettingConstant.UNLOCK_TIME_OUT);

            if (string.IsNullOrEmpty(unlockSetting))
            {
                unlockSetting = "60";
                SaveUnLockTimeOutSetting(unlockSetting, "1");
            }
            return Json(unlockSetting, JsonRequestBehavior.AllowGet);
        }



        #endregion Action Method
    }
}