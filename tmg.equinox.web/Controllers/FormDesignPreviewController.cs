using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.DataSource;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.Framework;
using tmg.equinox.web.Framework.Caching;

namespace tmg.equinox.web.Controllers
{
    public class FormDesignPreviewController : AuthenticatedController
    {
        #region Private Members
        private IFormDesignService _formDesignService;
        private IFolderVersionServices _folderVersionServices;
        private IFormDesignService _formDesignServices;
        #endregion Private Members

        #region Consturctor
        public FormDesignPreviewController(IFormDesignService formDesignService, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices)
        {
            this._formDesignService = formDesignService;
            this._folderVersionServices = folderVersionServices;
            this._formDesignServices = formDesignServices;
        }
        #endregion Constructor

        #region Action Methods
        /// <summary>
        /// This method is not yet used.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Preview(int tenantId, int formDesignVersionId, string formName)
        {
            FormDesignVersionPreviewViewModel viewModel = new FormDesignVersionPreviewViewModel();

            viewModel.FormDesignVersionID = formDesignVersionId;
            viewModel.FormName = formName;
            viewModel.TenantID = tenantId;

            return View(viewModel);
        }

        [HttpGet]
        public JsonResult GetFormDesignDataForPreview(int tenantId, int formDesignVersionId)
        {
            FormDesignVersionDetail detail = this._formDesignService.GetFormDesignVersionDetail(tenantId, formDesignVersionId);
            string jsonDesign = "";
            if (detail != null)
            {
                detail.JSONData = detail.GetDefaultJSONDataObject();
                //validate default JSON
                var converter = new ExpandoObjectConverter();
                dynamic jsonObject = JsonConvert.DeserializeObject<ExpandoObject>(detail.JSONData, converter);
                jsonDesign = JsonConvert.SerializeObject(detail);
                this._formDesignService.SaveCompiledFormDesignVersion(tenantId, formDesignVersionId, jsonDesign, User.Identity.Name);
            }

            detail = JsonConvert.DeserializeObject<FormDesignVersionDetail>(jsonDesign);
            string defaultJSONData = detail.GetDefaultJSONDataObject();
            detail.JSONData = defaultJSONData;
            if (detail.DataSources != null && detail.DataSources.Count > 0)
            {
                
               FormInstanceDataManager formDataInstance =new FormInstanceDataManager(tenantId, CurrentUserId, null,base.CurrentUserName, _folderVersionServices);
               DataSourceMapper dm = new DataSourceMapper(tenantId, 0, 0, detail.FormDesignId, formDesignVersionId, false, this._folderVersionServices, detail.JSONData, detail, formDataInstance, detail.Sections[0].FullName, this._formDesignServices);
                dm.AddDataSourceRange(detail.DataSources);
                detail.JSONData = dm.MapDataSources();
            }
            return Json(detail, JsonRequestBehavior.AllowGet);
        }

        #endregion Action Methods
    }
}