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
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.web.Controllers
{
    public class WorkFlowTaskMappingController : AuthenticatedController
    {
        #region Variables
        private IWorkFlowTaskMappingService _wFTaskMappingService;
        private string sendGridUserName = string.Empty;
        private string sendGridPassword = string.Empty;
        private string smtpUserName = string.Empty;
        private string smtpPassword = string.Empty;
        private string smtpPort = string.Empty;
        private string smtpHostServerName = string.Empty;
        #endregion

        #region Constructor

        public WorkFlowTaskMappingController(IWorkFlowTaskMappingService wFTaskMappingService)
        {
            this._wFTaskMappingService = wFTaskMappingService;
        }
        #endregion Constructor

    public JsonResult GetWorkFlowList(int tenantId)
        {

            IList<KeyValue> getworkflowList = _wFTaskMappingService.GetWorkFlowList(tenantId);
            return Json(getworkflowList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateApplicableWfTaskrMap(int tenantId,int WfstateId, string selectedTaskListData)
        {
            ServiceResult result = null;
            List<ApplicableTaskMapModel> MemberList = JsonConvert.DeserializeObject<List<ApplicableTaskMapModel>>(selectedTaskListData);
            result = _wFTaskMappingService.UpdateApplicableWFTaskMap(tenantId, WfstateId, MemberList, base.CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNonSelectedTaskList(int tenantId)
        {
            IEnumerable<KeyValue> getnonselectedtaskList = _wFTaskMappingService.GetNonSelectedTaskList(tenantId);
            if (getnonselectedtaskList == null)
            {
                getnonselectedtaskList = new List<KeyValue>();
            }
            return Json(getnonselectedtaskList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetApplicableWfTaskList(int tenantId, int WfstateId)
        {
            IEnumerable<KeyValue> list = _wFTaskMappingService.GetApplicableWfTaskList(tenantId, WfstateId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveTaskAndWFStateTaskMapping(int wfStateId, string taskDescription)
        {
            bool isSuccess = false;
            try
            {
                ServiceResult result = _wFTaskMappingService.SaveDPFTaskAndWorkflowMappings(wfStateId, taskDescription, DateTime.Now, base.CurrentUserName, base.TenantID);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
    }
}