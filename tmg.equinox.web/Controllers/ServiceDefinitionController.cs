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
using tmg.equinox.applicationservices.viewmodels.FormContent;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.Helper;
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
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;

namespace tmg.equinox.web.Controllers
{
    public class ServiceDefinitionController : AuthenticatedController
    {
        #region Private Members
        private IServiceDefinitionService _serviceDefinitionService { get; set; }
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public ServiceDefinitionController(IServiceDefinitionService serviceDefinitionService)
        {
            _serviceDefinitionService = serviceDefinitionService;

            if (_serviceDefinitionService == null)
                throw new NullReferenceException("ServiveDefinitionService");
        }
        #endregion Constructor

        #region Action Methods
        #region Service Definition Methods
        public ActionResult GetServiceDefinitionElementList(int tenantId, int serviceDesignVersionId)
        {
            IEnumerable<ServiceDefinitionRowModel> serviceDefintionElementList = _serviceDefinitionService.GetServiceDefinitionListForServiceDesignVersion(tenantId, serviceDesignVersionId);
            if (serviceDefintionElementList == null)
            {
                serviceDefintionElementList = new List<ServiceDefinitionRowModel>();
            }
            return Json(serviceDefintionElementList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add(int tenantId, int formDesignVersionId, int serviceDesignVersionId, int uiElementId, int parentServiceDefinitionId, bool isKey, bool addChildKeys, bool addChildElements)
        {
            ServiceResult result = _serviceDefinitionService.AddServiceDefinition(tenantId, formDesignVersionId, serviceDesignVersionId, uiElementId, parentServiceDefinitionId, base.CurrentUserName, isKey, addChildKeys, addChildElements);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(ServiceDefinitionViewModel viewModel)
        {
            ServiceResult result = _serviceDefinitionService.UpdateServiceDefinition(viewModel, base.CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(int tenantId, int serviceDesignVersionId, int serviceDefinitionId)
        {
            ServiceResult result = _serviceDefinitionService.DeleteServiceDefinition(tenantId, serviceDesignVersionId, serviceDefinitionId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDetails(int tenantId, int serviceDefinitionId)
        {
            ServiceDefinitionViewModel viewModel = _serviceDefinitionService.GetServiceDefinitionDetails(tenantId, serviceDefinitionId);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
        #endregion Service Definition Methods

        #region Service Parameters Methods
        public ActionResult GetServiceParameterList(int tenantId, int serviceDesignId, int serviceDesignVersionId)
        {
            IEnumerable<ServiceParameterRowModel> serviceParameterList = _serviceDefinitionService.GetServiceParameterList(tenantId, serviceDesignId, serviceDesignVersionId);
            return Json(serviceParameterList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddServiceParameter(int tenantId, int serviceDesignId, int serviceDesignVersionId, int dataTypeId, string name, bool isRequired, int uielementId)
        {
            ServiceResult result = _serviceDefinitionService.AddServiceParameter(tenantId, serviceDesignId, serviceDesignVersionId, dataTypeId, name, isRequired, uielementId, base.CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddDefaultFormInstanceIDServiceParameter(int tenantId, int serviceDesignId, int serviceDesignVersionId, int dataTypeId, string name, bool isRequired)
        {
            ServiceResult result = _serviceDefinitionService.AddDefaultServiceParameter(tenantId, serviceDesignId, serviceDesignVersionId, dataTypeId, name, isRequired, base.CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateServiceParameter(int tenantId, int serviceDesignId, int serviceDesignVersionId, int serviceParameterId, int dataTypeId, string name, bool isRequired, int uielementId)
        {
            ServiceResult result = _serviceDefinitionService.UpdateServiceParameter(tenantId, serviceParameterId, serviceDesignId, serviceDesignVersionId, dataTypeId, name, isRequired, uielementId, base.CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteServiceParameter(int tenantId, int serviceParameterId, int serviceDesignId, int serviceDesignVersionId)
        {
            ServiceResult result = _serviceDefinitionService.DeleteServiceParameter(tenantId, serviceParameterId, serviceDesignId, serviceDesignVersionId, base.CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUIElementList(int tenantID, int formDesignVersionID, int formDesignID)
        {
            IEnumerable<UIElementModel> elementList = this._serviceDefinitionService.GetUIElementList(tenantID, formDesignVersionID, formDesignID);
            return Json(elementList, JsonRequestBehavior.AllowGet);
        }
        #endregion Service Parameters Methods

        #endregion Action Methods

        #region Private Methods

        #endregion Private Methods
    }
}