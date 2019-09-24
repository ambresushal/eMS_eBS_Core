using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.web.Framework;
using tmg.equinox.identitymanagement;
using tmg.equinox.caching.client;
using tmg.equinox.applicationservices.viewmodels.Settings;

namespace tmg.equinox.web.Controllers
{
    public class AuthenticatedController : BaseController
    {
        #region Private Memebers
        #endregion Private Members

        #region Public Properties
        #endregion Public Properties

        #region Constructor
        #endregion Constructor

        #region Public Methods
        /// <summary>
        /// Added a condition to check user has LoggedIn or not for back button issues.
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                base.OnActionExecuting(filterContext);
                if ((CurrentUserId == 0 || CurrentUserId == null)
                    && filterContext.ActionDescriptor.ActionName != "PreviewFormInstance"
                    && filterContext.ActionDescriptor.ActionName != "GetOwnerList"
                    && filterContext.ActionDescriptor.ActionName != "GetFormInstanceDesignData"
                    && filterContext.ActionDescriptor.ActionName != "PreviewAllInstances"
                    && filterContext.ActionDescriptor.ActionName != "GetFormInstanceList"
                    && filterContext.ActionDescriptor.ActionName != "GetFormInstanceRepeaterDesignData")
                {
                    filterContext.Result = new RedirectResult(Url.Action("LogOn", "Account"));
                }
                //else
                //{
                //    var identity = CurrentUserClaimsIdentity;

                //    if (IdentityManager.IsADAuthentication())
                //    {
                //        List<UserManagementSettingsViewModel> userDetails;// = this.userManagementService.GetUserRolesDetailsByName(viewModel.UserName);
                //        bool success = false;
                //        //string userName = identity.Name;
                //        string userName = "slawand";

                //        success = IdentityManager.SignInAsync(identity.Name, "");
                //        //userDetails = this.userManagementService.GetUserRolesDetailsByName(userName);
                //        //if (success && userDetails[0].IsActive == true)
                //        //{
                //        CachingManager.RemoveClaimsFromCache(CurrentUserId.ToString());
                //        CachingManager.SetClaimsCache(CurrentUserId.ToString(), CurrentUserClaimsIdentity.Claims.ToList());
                //        //}
                //        //else
                //        //{
                //        //    ModelState.AddModelError("", "Invalid username or password...Try Again!!!");
                //        //}
                //    }
                //}
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
        }
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods

    }
}