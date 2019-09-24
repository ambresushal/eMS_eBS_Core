using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.identitymanagement;
using tmg.equinox.identitymanagement.Enums;
using tmg.equinox.identitymanagement.Models;
using tmg.equinox.web.Framework;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.mapper;

namespace tmg.equinox.web.Controllers
{
    public class IdentityController : AuthenticatedController
    {

        #region Private Variables
        IdentityAccessMapper _IdentityAccessMapper;
        #endregion

        #region Constructor

        public IdentityController()
        {
            _IdentityAccessMapper = new IdentityAccessMapper();
        }

        #endregion

        #region Action Methods
        #region Public Methods
        /// <summary>
        /// To Add user role access for section
        /// </summary>
        /// <param name="sectionAccessLevelRows"></param>
        /// <returns>Json result</returns>
        public JsonResult AddElementAccessPermissionSet(List<ElementAccessViewModel> sectionaccessRows)
        {
            JsonResult retJsonResult = null;
            try
            {
                var sectionRows = _IdentityAccessMapper.MapToApplicationRoleClaims(sectionaccessRows, -1, ResourceType.SECTION);
                IdentityManager.AddIdentityClaim(sectionRows, -1, ResourceType.SECTION);
                retJsonResult = Json(JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return retJsonResult;
        }

        /// <summary>
        /// This memthod is used to retrive list of claims using elementId(section id)
        /// </summary>
        /// <param name="elementId"></param>
        /// <returns></returns>
        public JsonResult GetElementAccessPermissionSet(int elementId)
        {
            JsonResult retJsonResult = null;
            try
            {
                //  var sectionRows = _IdentityAccessMapper.MapToApplicationRoleClaims(sectionaccessRows, -1, ResourceType.SECTION);
                IEnumerable<ElementAccessViewModel> sectionWiseUserRoleAccessList = _IdentityAccessMapper.MapToElementAcessViewModel(IdentityManager.GetClaims(elementId), IdentityManager.GetApplicationRoles());
                retJsonResult = Json(sectionWiseUserRoleAccessList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return retJsonResult;

        }
        #endregion
        #endregion

        #region private Methods

        #endregion
    }
}