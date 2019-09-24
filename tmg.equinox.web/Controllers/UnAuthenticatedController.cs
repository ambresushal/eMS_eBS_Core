using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using tmg.equinox.applicationservices.viewmodels.Account;
using tmg.equinox.caching.client;
using tmg.equinox.web.Framework;
using System.Configuration;
using tmg.equinox.web.Framework.ActionFilters;
using tmg.equinox.identitymanagement;
using tmg.equinox.identitymanagement.Models;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.web.Framework.Caching;
using tmg.equinox.applicationservices.viewmodels.Settings;
using System.Collections.Generic;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.FormDesignManager;
using System.Web;
using System.Security.Claims;

namespace tmg.equinox.web.Controllers
{
    //[NoCache]

    public class UnAuthenticatedController : BaseController
    {
        #region Private Members
        private IFolderVersionServices folderVersionService;
        private IUserManagementService userManagementService;
        private IFormDesignService _formDesignServices;
        private IFolderLockService _folderLockService;
        private IResourceLockService _resourceLockService;
        private IPlanTaskUserMappingService _planTaskUserMappingService;
        bool UseAuthentication = false;
        bool UseADAuthentication = false;
        string domainName = string.Empty;
        bool DummyAuthenticationEnabled = false;
        string UserName = string.Empty;
        bool UseOwinAuthentication = false;
        string _controller = string.Empty;
        string _action = string.Empty;
        const string Account = "account";
        const string Logon = "logon";
        const string PasswordStatus = "Your password has been changed successfully !";
        private bool _inMemoryFolderLock = false;

        #endregion Private Members

        #region Constructor

        public UnAuthenticatedController()
        {
            InitializeIdentityManager();
        }

        /// <summary>
        /// parametrised cocnstructor for releasing lock.
        /// </summary>
        /// <param name="folderVersionService"></param>
        public UnAuthenticatedController(IFolderVersionServices folderVersionService, IUserManagementService userManagementService, IFormDesignService formDesignServices, IFolderLockService folderLockService, IResourceLockService resourceLockService, IPlanTaskUserMappingService planTaskUserMappingService)
        {
            this.folderVersionService = folderVersionService;
            this.userManagementService = userManagementService;
            this._formDesignServices = formDesignServices;
            this._folderLockService = folderLockService;
            _resourceLockService = resourceLockService;
            _planTaskUserMappingService = planTaskUserMappingService;
            this._inMemoryFolderLock = Convert.ToString(ConfigurationManager.AppSettings["FolderLockToUse"]) == "InMemory" ? true : false;
            InitializeIdentityManager();
        }
        #endregion


        #region Public Methods
        /// <summary>
        /// called LogOff method to return back to login page
        /// when back button is pressed.
        /// </summary>
        /// <param name="landingPageEnabled"></param>
        /// <returns></returns>
        [NoCache]
        [HttpGet]
        //[AllowAnonymous]
        [Route("~/")]
        [Route("Account/LogOn")]
        public ActionResult LogOn(bool landingPageEnabled = false)
        {

            var identity = CurrentUserClaimsIdentity;

            if (IdentityManager.IsADAuthentication() && IdentityManager.IsSOActiveForAD())
            {

                List<UserManagementSettingsViewModel> userDetails;// = this.userManagementService.GetUserRolesDetailsByName(viewModel.UserName);
                bool success = false;
                string userName = string.Empty;
                try
                {
                    userName = identity.Name.Split('\\').Last();

                    success = IdentityManager.SignInAsync(identity.Name, "");

                    userDetails = this.userManagementService.GetUserRolesDetailsByName(userName);

                    if (success && userDetails != null && userDetails[0].IsActive == true)
                    {
                        CachingManager.RemoveClaimsFromCache(CurrentUserId.ToString());
                        CachingManager.SetClaimsCache(CurrentUserId.ToString(), CurrentUserClaimsIdentity.Claims.ToList());
                        _planTaskUserMappingService.ExecuteNotifyTaskDueDateOverPushNotification(userName);
                        return RedirectToAction("Index", "DashBoard");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password...Try Again!!!");
                    }

                }
                catch (Exception ex)
                {
                    if (ex.Message == "role not assigned")
                        return RedirectToAction("RoleNotFoundPage", "Error");

                    return RedirectToAction("LoginFailPage", "Error");
                }
            }

            //else if (identity.IsAuthenticated)
            //{
            //    try
            //    {
            //        var ssoEnvironment = Convert.ToString(ConfigurationManager.AppSettings["SSOEnvironment"]);
            //        IdentityManager.GenerateUserIdentity(CurrentUserClaimsIdentity, ssoEnvironment);
            //        CachingManager.RemoveClaimsFromCache(CurrentUserId.ToString());
            //        CachingManager.SetClaimsCache(CurrentUserId.ToString(), CurrentUserClaimsIdentity.Claims.ToList());
            //        return RedirectToAction("Index", "DashBoard");
            //    }
            //    catch (Exception ex)
            //    {
            //        if (ex.Message == "Invalid RoleId")
            //            return RedirectToAction("RoleNotFoundPage", "Error");

            //        return RedirectToAction("LoginFailPage", "Error");
            //    }
            //}
            else if (UseOwinAuthentication == false)
                return RedirectToAction("LoginFailPage", "Error");


            //if authentication is to be employed, load the LogIn page
            if (this.UseAuthentication)
            {
                //await IdentityManager.CreateUser("admin", "123");
                LogOff();
                LoginViewModel viewModel = new LoginViewModel();
                if (ConfigurationManager.AppSettings.AllKeys.Contains("SingleLandingPageUrl")) {
                    string SingleLandingPageUrl = ConfigurationManager.AppSettings["SingleLandingPageUrl"].ToString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(SingleLandingPageUrl))
                    {
                        return Redirect(SingleLandingPageUrl);
                    }
                    else
                    {
                        return View(viewModel);
                    }
                }
                return View(viewModel);
            }
            else if (this.DummyAuthenticationEnabled)
            {
                if (_controller.ToLower().Trim() == Account && _action.ToLower().Trim() == Logon)
                {
                    LoginViewModel viewModel = new LoginViewModel();

                    return View(viewModel);
                }
                IdentityManager.SignInDummy(UserName);

                return RedirectToAction(_action, _controller);
            }
            return null;
        }

        /// <summary>
        /// This method added here for case of session out as it dosen't requires Authentication.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public JsonResult ReleaseFolderLock(int tenantId, int folderId, string isNavigate)
        {
            ServiceResult result = _inMemoryFolderLock ? _resourceLockService.ReleaseFolderLock(folderId, CurrentUserId) : folderVersionService.ReleaseFolderLock(CurrentUserId, tenantId = 1, folderId);
            //Remove from cache
            var folderVerion = folderVersionService.GetLatestFolderVersion(tenantId, folderId);

            FormInstanceSectionDataCacheHandler secCacheHandler = new FormInstanceSectionDataCacheHandler();

            if (folderVerion != null)
            {
                var formInstances = folderVersionService.GetFormInstanceList(tenantId, folderVerion.FolderVersionId, folderId);
                if (formInstances.Count > 0)
                {
                    IDataCacheHandler cacheHandler = DataCacheHandlerFactory.GetHandler();
                    ErrorGridCacheHandler errorCacheHanlder = new ErrorGridCacheHandler();
                    foreach (var item in formInstances)
                    {
                        FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, item.FormDesignVersionID, _formDesignServices);
                        FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(true);
                        if (detail != null)
                        {
                            foreach (SectionDesign secdesign in detail.Sections)
                            {
                                secCacheHandler.RemoveSectionData(item.FormInstanceID, secdesign.FullName, CurrentUserId);
                            }
                        }
                        secCacheHandler.RevomeSectionListFromCache(item.FormInstanceID, CurrentUserId);
                        secCacheHandler.RemoveTargetFormInstanceFromCache(item.FormInstanceID, CurrentUserId);
                        cacheHandler.Remove(item.FormInstanceID, CurrentUserId);
                        errorCacheHanlder.ErrorGridDataRemove(item.FormInstanceID, CurrentUserId);
                    }
                }
            }
            //End

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AllowAnonymous]
        [UserActivityLog(Priority = (int)Priority.Low, Category = (int)Category.Audit, Message = "Log in", Severity = (int)Severity.Information, ActivityEvent = (int)Event.LogOn)]
        [Route("~/")]
        [Route("Account/LogOn")]
        public ActionResult LogOn(LoginViewModel viewModel)
        {
            List<UserManagementSettingsViewModel> userDetails;// = this.userManagementService.GetUserRolesDetailsByName(viewModel.UserName);
            bool success = false;
            if (ModelState.IsValid)
            {
                success = IdentityManager.SignInAsync(viewModel.UserName, viewModel.Password);
                userDetails = this.userManagementService.GetUserRolesDetailsByName(viewModel.UserName);
                if (success && userDetails[0].IsActive == true)
                {
                    int passwordExpiry = 45;
                    int.TryParse(Convert.ToString(ConfigurationManager.AppSettings["PasswordExpireDays"] ?? "45"), out passwordExpiry);

                    if (IdentityManager.IsLockedOut(userDetails[0].UserId))
                    {
                        ModelState.AddModelError("", string.Format("Your account has been locked out due to multiple failed login attempts."));
                    }
                    else if (userDetails[0].ChangeInitialPassword == true || (userDetails[0].UpdatedDate != null && Convert.ToDateTime(userDetails[0].UpdatedDate).AddDays(passwordExpiry) < DateTime.UtcNow))
                    {
                        TempData["userName"] = viewModel.UserName;
                        return RedirectToAction("ChangePassword", "Account");
                    }
                    else if (userDetails[0].UserRole.ToUpper() != "SECURITY")
                    {
                        IdentityManager.ResetAccessFailedCount(userDetails[0].UserId);

                        _planTaskUserMappingService.ExecuteNotifyTaskDueDateOverPushNotification(viewModel.UserName);
                        string controller = string.Empty;
                        string action = string.Empty;
                        string startin = string.Empty;

                        if (viewModel.StartIN != null && viewModel.StartIN.Count > 0)
                        {
                            startin = viewModel.StartIN[0];
                        }

                        tmg.equinox.web.App_Start.StartupHelper.GetUrl(startin, out action, out controller);

                        CachingManager.RemoveClaimsFromCache(CurrentUserId.ToString());
                        CachingManager.SetClaimsCache(CurrentUserId.ToString(), CurrentUserClaimsIdentity.Claims.ToList());
                        return RedirectToAction(action, controller);
                    }
                    else
                    {
                        CachingManager.RemoveClaimsFromCache(CurrentUserId.ToString());
                        CachingManager.SetClaimsCache(CurrentUserId.ToString(), CurrentUserClaimsIdentity.Claims.ToList());
                        return RedirectToAction("UserManagementSettings", "Settings");


                    }
                }
                else if (userDetails.Count > 0)
                {
                    if (IdentityManager.IsLockedOut(userDetails[0].UserId))
                    {
                        ModelState.AddModelError("", string.Format("Your account has been locked out due to multiple failed login attempts."));
                    }
                    else
                    {
                        int accessFailedCount = IdentityManager.AccessFailed(userDetails[0].UserId);
                        int maxFailedCount = 5;
                        if (int.TryParse(Convert.ToString(ConfigurationManager.AppSettings["MaxFailedAccessAttemptsBeforeLockout"] ?? "5"), out maxFailedCount))
                        {
                            int attemptsLeft = maxFailedCount - accessFailedCount;
                            if (attemptsLeft <= 0)
                            {
                                IdentityManager.LockUser(userDetails[0].UserId);
                                ModelState.AddModelError("", string.Format("Invalid credentials. You have {0} more attempt(s) before your account gets locked out.", attemptsLeft));
                            }
                            else
                            {
                                ModelState.AddModelError("", "Invalid password...Try Again!!!");
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password...Try Again!!!");
                }
            }
            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        [UserActivityLog(Priority = (int)Priority.Medium, Category = (int)Category.Audit, Message = "Log off", Severity = (int)Severity.Information, ActivityEvent = (int)Event.LogOff)]
        [Route("Account/LogOff")]
        public ActionResult LogOff()
        {
            if (base.CurrentUserId.HasValue)
            {
                _resourceLockService.ReleaseDocumentAndSectionLock(base.CurrentUserId.Value);
            }
            IdentityManager.SignOut();
            //remove cookies 
            if (Request.Cookies["AuthCookieSAML"] != null)
            {
                HttpCookie authSAMLCookie = new HttpCookie("AuthCookieSAML");
                authSAMLCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(authSAMLCookie);
            }
            if (Request.Cookies["AuthCookieSAML1"] != null)
            {
                HttpCookie authSAMlCookie1 = new HttpCookie("AuthCookieSAML1");
                authSAMlCookie1.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(authSAMlCookie1);
            }

            var identity = (User.Identity as System.Security.Claims.ClaimsIdentity);
            var claims = identity.Claims;
            var claimPrinciple = (ClaimsPrincipal)User;
            foreach (var claimIdentity in claimPrinciple.Identities)
            {
                if (!(claimIdentity is System.Security.Principal.WindowsIdentity))
                {
                    if (claimIdentity is ClaimsIdentity)
                    {
                        claims = claimIdentity.Claims;
                        identity = claimIdentity;
                    }
                }
            }
            if (claims != null)
            {
                if (claims.Any(c => c.Value.Trim().ToUpper() == "ALLOWSINGLELOGIN"))
                {
                    if (ConfigurationManager.AppSettings.AllKeys.Contains("SingleLandingPageUrl"))
                    {
                        string SingleLandingPageUrl = ConfigurationManager.AppSettings["SingleLandingPageUrl"].ToString() ?? string.Empty;
                        if (!string.IsNullOrEmpty(SingleLandingPageUrl))
                        {
                            return Redirect(SingleLandingPageUrl);
                        }
                        else
                        {
                            Session.Abandon();
                            return View("LogOff");
                        }
                    }
                    Session.Abandon();
                    return View("LogOff");
                }
            }
            Session.Abandon();
            return View("LogOff");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Account/changepassword")]
        public ActionResult ChangePassword(string message)
        {
            ChangePasswordModel model = new ChangePasswordModel();
            model.UserName = Convert.ToString(TempData["userName"]);
            return View(model);
        }

        /// <summary>
        /// Method to change password of User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        [Route("Account/changepassword")]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            ViewBag.ReturnUrl = Url.Action("ChangePassword");

            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                if (model.UserName != null)
                {
                    bool isUserValid = IdentityManager.IsUserNameExistAsync(model.UserName);
                    if (isUserValid == false)
                        ModelState.AddModelError("", "UserName is invalid.");

                    else if (model.OldPassword == model.NewPassword)
                        ModelState.AddModelError("", "Current Password and New Password should be different.");
                    else
                    {
                        bool isChangePasswordSucceeded = IdentityManager.ChangePasswordAsync(model.UserName, model.OldPassword, model.NewPassword);
                        if (isChangePasswordSucceeded)
                        {
                            this.userManagementService.UpdateUserDetails(model.UserName, false);
                            TempData["notice"] = PasswordStatus;
                            return RedirectToAction("ChangePassword", "Account");
                        }
                        else
                        {
                            ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The current user is invalid or session expired.");
                }
            }
            return View(model);
        }

        /// <summary>
        /// This method returns the list of all roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAllRoles()
        {

            var roleNames = IdentityManager.GetRoles();
            return Json(roleNames, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [Route("Account/SingleLandingPage")]
        public ActionResult SingleLandingPage(string username, string startIn)
        {
            var identity = CurrentUserClaimsIdentity;
            // if (IdentityManager.IsADAuthentication() && IdentityManager.IsSOActiveForAD())
            {
                List<UserManagementSettingsViewModel> userDetails;// = this.userManagementService.GetUserRolesDetailsByName(viewModel.UserName);
                bool success = false;
                string userName = string.Empty;
                LogOff();
                try
                {
                    userDetails = this.userManagementService.GetUserRolesDetailsByName(username);
                    success = IdentityManager.SignInAsync(username);
                    if (userDetails != null && success)
                    {
                        if (userDetails != null && userDetails[0].IsActive == true)
                        {
                            int passwordExpiry = 45;
                            int.TryParse(Convert.ToString(ConfigurationManager.AppSettings["PasswordExpireDays"] ?? "45"), out passwordExpiry);

                            if (IdentityManager.IsLockedOut(userDetails[0].UserId))
                            {
                                ModelState.AddModelError("", string.Format("Your account has been locked out due to multiple failed login attempts."));
                            }
                            else if (userDetails[0].ChangeInitialPassword == true || (userDetails[0].UpdatedDate != null && Convert.ToDateTime(userDetails[0].UpdatedDate).AddDays(passwordExpiry) < DateTime.UtcNow))
                            {
                                TempData["userName"] = username;
                                return RedirectToAction("ChangePassword", "Account");
                            }
                            else if (userDetails[0].UserRole.ToUpper() != "SECURITY")
                            {
                                IdentityManager.ResetAccessFailedCount(userDetails[0].UserId);

                                _planTaskUserMappingService.ExecuteNotifyTaskDueDateOverPushNotification(username);
                                string controller = string.Empty;
                                string action = string.Empty;
                                string startin = string.Empty;
                                if (!string.IsNullOrEmpty(startIn))
                                {
                                    tmg.equinox.web.App_Start.StartupHelper.GetUrl(startIn, out action, out controller);

                                    CachingManager.RemoveClaimsFromCache(CurrentUserId.ToString());
                                    CachingManager.SetClaimsCache(CurrentUserId.ToString(), CurrentUserClaimsIdentity.Claims.ToList());
                                    return RedirectToAction(action, controller);
                                }
                                else
                                {
                                    return RedirectToAction("Index", "DashBoard");
                                }
                            }
                            else
                            {
                                CachingManager.RemoveClaimsFromCache(CurrentUserId.ToString());
                                CachingManager.SetClaimsCache(CurrentUserId.ToString(), CurrentUserClaimsIdentity.Claims.ToList());
                                return RedirectToAction("UserManagementSettings", "Settings");


                            }
                        }
                        else if (userDetails.Count > 0)
                        {
                            if (IdentityManager.IsLockedOut(userDetails[0].UserId))
                            {
                                ModelState.AddModelError("", string.Format("Your account has been locked out due to multiple failed login attempts."));
                            }
                            else
                            {
                                int accessFailedCount = IdentityManager.AccessFailed(userDetails[0].UserId);
                                int maxFailedCount = 5;
                                if (int.TryParse(Convert.ToString(ConfigurationManager.AppSettings["MaxFailedAccessAttemptsBeforeLockout"] ?? "5"), out maxFailedCount))
                                {
                                    int attemptsLeft = maxFailedCount - accessFailedCount;
                                    if (attemptsLeft <= 0)
                                    {
                                        IdentityManager.LockUser(userDetails[0].UserId);
                                        ModelState.AddModelError("", string.Format("Invalid credentials. You have {0} more attempt(s) before your account gets locked out.", attemptsLeft));
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("", "Invalid password...Try Again!!!");
                                    }
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid username or password...Try Again!!!");
                        }
                    }
                    else
                    {
                        return RedirectToAction("LoginFailPage", "Error");
                    }

                }
                catch (Exception ex)
                {
                    if (ex.Message == "role not assigned")
                        return RedirectToAction("RoleNotFoundPage", "Error");

                    return RedirectToAction("LoginFailPage", "Error");
                }
            }


            if (UseOwinAuthentication == false)
                return RedirectToAction("LoginFailPage", "Error");

            //if authentication is to be employed, load the LogIn page
            if (this.UseAuthentication)
            {
                //await IdentityManager.CreateUser("admin", "123");
                LogOff();
                LoginViewModel viewModel = new LoginViewModel();

                return View(viewModel);
            }
            else if (this.DummyAuthenticationEnabled)
            {
                if (_controller.ToLower().Trim() == Account && _action.ToLower().Trim() == Logon)
                {
                    LoginViewModel viewModel = new LoginViewModel();

                    return View(viewModel);
                }
                IdentityManager.SignInDummy(UserName);

                return RedirectToAction(_action, _controller);
            }
            return null;
        }

        [AllowAnonymous]
        [Route("Account/SwitchToApplication")]
        public ActionResult SwitchToApplication(string appName)
        {
            bool eMSKey = ConfigurationManager.AppSettings.AllKeys.Contains("emedicaresync") ? true : false;
            bool eBSKey = ConfigurationManager.AppSettings.AllKeys.Contains("ebenefitsync") ? true : false;
            string applicationUrl = string.Empty;
            if (appName == "eMedicareSync" && eMSKey)
            {
                applicationUrl = ConfigurationManager.AppSettings["emedicaresync"].ToString() ?? string.Empty;
            }
            else if (appName == "eBenefitSync" && eBSKey)
            {
                applicationUrl = ConfigurationManager.AppSettings["ebenefitsync"].ToString() ?? string.Empty;
            }
            applicationUrl = applicationUrl + "?username=" + CurrentUserName;
            return Redirect(applicationUrl);
        }
        #endregion

        #region Private Methods
        [NonAction]
        private void InitializeIdentityManager()
        {
            string _landingPage = string.Empty;
            NameValueCollection authenticationSectionCollection = (NameValueCollection)ConfigurationManager.GetSection("authenticationSection");
            if (Convert.ToBoolean(authenticationSectionCollection["UseAuthentication"])) this.UseAuthentication = true;
            if (Convert.ToBoolean(authenticationSectionCollection["DummyAuthenticationEnabled"])) this.DummyAuthenticationEnabled = true;
            UserName = authenticationSectionCollection["UserName"];
            if (ConfigurationManager.AppSettings["DomainName"] != null)
            {
                domainName = ConfigurationManager.AppSettings["DomainName"].ToString();
            }
            else
            {
                domainName = "TMG";
            }

            UseOwinAuthentication = Convert.ToBoolean(authenticationSectionCollection["UseOwinAuthentication"]);

            _landingPage = authenticationSectionCollection["LandingPage"];

            string[] controllerActionPair = _landingPage.Split('/');
            _controller = controllerActionPair[0];
            _action = controllerActionPair[1];

            IdentityManager.OwinAuthenticationEnabled = UseOwinAuthentication;
        }
        #endregion
    }
}






