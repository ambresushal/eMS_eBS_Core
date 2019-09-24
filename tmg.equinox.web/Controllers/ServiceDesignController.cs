using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;
using tmg.equinox.identitymanagement.Authentication;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.web.Framework;
using tmg.equinox.web.DataSource;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.identitymanagement.Provider;
using tmg.equinox.identitymanagement.Models;
using tmg.equinox.identitymanagement;
using tmg.equinox.identitymanagement.Enums;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.web.Framework.Caching;

namespace tmg.equinox.web.Controllers
{
    public class ServiceDesignController : AuthenticatedController
    {
        #region Private Members
        private IFormDesignService _formDesignService { get; set; }
        private IServiceDesignService _serviceDesignService { get; set; }
        #endregion Private Members

        #region Constructor
        public ServiceDesignController(IFormDesignService formDesignService, IServiceDesignService serviceDesignService)
        {
            this._formDesignService = formDesignService;
            this._serviceDesignService = serviceDesignService;
        }
        #endregion Constructor

        #region Action Methods
        public ActionResult Index()
        {
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            return View();
        }

        #region Service Design Methods
        public JsonResult ServiceDesignList(int tenantId)
        {
            IEnumerable<ServiceDesignRowModel> serviceDesignList = null;
            serviceDesignList = _serviceDesignService.GetServiceDesignList(tenantId);

            if (serviceDesignList == null)
            {
                serviceDesignList = new List<ServiceDesignRowModel>();
            }
            return Json(serviceDesignList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Add(int tenantId, string serviceName, string serviceMethodName, bool doesReturnAList)
        {
            string serviceReturnType = ConfigurationManager.AppSettings["ServiceDesignDataType"];
            bool IsReturnJSON = serviceReturnType == "JSON" ? true : false;
            ServiceResult result = _serviceDesignService.AddServiceDesign(base.CurrentUserName, tenantId, serviceName, serviceMethodName, doesReturnAList, IsReturnJSON);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Update(int tenantId, int serviceDesignId, string serviceName, string serviceMethodName)
        {
            ServiceResult result = _serviceDesignService.UpdateServiceDesign(base.CurrentUserName, tenantId, serviceDesignId, serviceName, serviceMethodName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(int tenantId, int serviceDesignId)
        {
            ServiceResult result = _serviceDesignService.DeleteServiceDesign(base.CurrentUserName, tenantId, serviceDesignId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion Service Design Methods

        #region Service Design Version Methods
        public JsonResult ServiceDesignVersionList(int tenantId, int serviceDesignId)
        {
            IEnumerable<ServiceDesignVersionRowModel> serviceDesignVersionList = _serviceDesignService.GetServiceDesignVersionList(tenantId, serviceDesignId);
            if (serviceDesignVersionList == null)
            {
                serviceDesignVersionList = new List<ServiceDesignVersionRowModel>();
            }
            return Json(serviceDesignVersionList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddVersion(int tenantId, int serviceDesignId, int serviceDesignVersionId, bool isFirstVersion, DateTime effectiveDate, int formDesignID, int formDesignVersionID)
        {
            ServiceResult result = new ServiceResult();
            if (isFirstVersion == true)
            {
                result = _serviceDesignService.AddServiceDesignVersion(base.CurrentUserName, tenantId, serviceDesignId, effectiveDate, "", formDesignID, formDesignVersionID);
            }
            else
            {
                ServiceDesignVersionRowModel model = _serviceDesignService.GetServiceDesignVersionList(tenantId, serviceDesignId).FirstOrDefault();
                if (serviceDesignVersionId != 0)
                {
                    result = _serviceDesignService.CopyServiceDesignVersion(User.Identity.Name, tenantId, serviceDesignVersionId, serviceDesignId, effectiveDate, model.VersionNumber);
                }
                else
                {
                    result = _serviceDesignService.CopyServiceDesignVersion(User.Identity.Name, tenantId, model.ServiceDesignVersionId, serviceDesignId, effectiveDate, model.VersionNumber);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateVersion(int tenantId, int serviceDesignVersionId, int formDesignVersionId, DateTime effectiveDate)
        {
            ServiceResult result = _serviceDesignService.UpdateServiceDesignVersion(base.CurrentUserName, tenantId, serviceDesignVersionId, formDesignVersionId, effectiveDate);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FinalizeVersion(int tenantId, int serviceDesignVersionId)
        {
            ServiceResult result = new ServiceResult();
            result = _serviceDesignService.FinalizeServiceDesignVersion(base.CurrentUserName, tenantId, serviceDesignVersionId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteVersion(int tenantId, int serviceDesignVersionId, int serviceDesignId)
        {
            ServiceResult result = _serviceDesignService.DeleteServiceDesignVersion(base.CurrentUserName, tenantId, serviceDesignVersionId, serviceDesignId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetPreview(int tenantID, int serviceDesignID, int serviceDesignVersionID)
        {
            ServiceDesignPreviewViewModel viewModel = this._serviceDesignService.GetServiceDesignPreview(tenantID, serviceDesignID, serviceDesignVersionID);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
        #endregion Service Design Version Methods

        #endregion Action Methods

        #region Private Methods

        #endregion Private Methods
    }
}