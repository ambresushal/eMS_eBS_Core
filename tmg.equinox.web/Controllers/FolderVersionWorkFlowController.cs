using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.web.Framework.Caching;

namespace tmg.equinox.web.Controllers
{
    public class FolderVersionWorkFlowController : AuthenticatedController
    {
        #region Private Members

        private IFolderVersionServices folderVersionService;
        private IWorkFlowStateServices workflowStateService;
        private ICCRTranslatorService _mDMTranslatorService;
        private string sendGridUserName = string.Empty;
        private string sendGridPassword = string.Empty;

        #endregion

        #region Constructor

        public FolderVersionWorkFlowController(IFolderVersionServices folderVersionService, IWorkFlowStateServices workflowStateService, ICCRTranslatorService mDMTranslatorService)
        {
            this.folderVersionService = folderVersionService;
            this.workflowStateService = workflowStateService;
            this._mDMTranslatorService = mDMTranslatorService;
        }

        #endregion Constructor

        #region Action Methods

        public JsonResult GetFolderVersionWorkFlowList(int tenantId, int folderVersionId)
        {
            IEnumerable<FolderVersionWorkFlowViewModel> folderVersionWorkFlowList = workflowStateService.GetFolderVersionWorkFlowList(tenantId, folderVersionId);
            if (folderVersionWorkFlowList == null)
            {
                folderVersionWorkFlowList = new List<FolderVersionWorkFlowViewModel>();
            }
            return Json(folderVersionWorkFlowList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWorkFlowStateList(int tenantId, int folderVersionId)
        {
            IEnumerable<WorkFlowVersionStatesViewModel> workflowStateList = workflowStateService.GetWorkFlowStateList(tenantId, folderVersionId);
            if (workflowStateList == null)
            {
                workflowStateList = new List<WorkFlowVersionStatesViewModel>();
            }
            return Json(workflowStateList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCurrentWorkFlowState(int tenantId, int folderVersionId)
        {
            List<KeyValue> workflowState = workflowStateService.GetCurrentWorkFlowStateForStatusUpdate(tenantId, folderVersionId);
            if (workflowState == null)
            {
                workflowState = new List<KeyValue>();
            }
            return Json(workflowState, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateWorkflowStatus(int tenantId, int folderVersionId, int workflowStateId, int approvalStatusId,
                                                    string commenttext, int userId, string majorFolderVersionNumber)
        {
            InitializeEmailSettings();

            //Need to Save all forms in folder on WorkFlowState ProductReview and Audit as we are updating Audit section in all product forms on approving those workflowstate
            //if ((workflowStateId == 3 && approvalStatusId == 1) || workflowStateId == 4)
            //{
            //    List<FormInstanceViewModel> formInstances = new List<FormInstanceViewModel>();
            //    //formInstances = folderVersionService.GetFormList(tenantId, folderVersionId);
            //    formInstances = folderVersionService.GetProductList(tenantId, folderVersionId);

            //    IDataCacheHandler cacheHandler = DataCacheHandlerFactory.GetHandler();
            //    List<JToken> formsData = new List<JToken>();
            //    List<int> formInstanceIDs = new List<int>();
            //    foreach (var form in formInstances)
            //    {
            //        JObject obj = JObject.Parse("{'FormInsatnceID':'','FormData':''}");
            //        formInstanceIDs.Add(form.FormInstanceID);
            //        var data = cacheHandler.IsExists(tenantId, form.FormInstanceID, CurrentUserId);
            //        obj["FormInsatnceID"] = form.FormInstanceID;
            //        obj["FormData"] = data;
            //        formsData.Add(obj);
            //    }

            //    List<FormInstanceViewModel> multipleFormInstances = folderVersionService.GetMultipleFormInstancesData(tenantId, formInstanceIDs);

            //    folderVersionService.SaveMultipleFormInstancesData(tenantId, folderVersionId, multipleFormInstances, formsData, CurrentUserName, userId);

            //}

            ServiceResult result = workflowStateService.UpdateWorkflowState(tenantId, folderVersionId, workflowStateId, approvalStatusId,
                                                                        commenttext, userId, CurrentUserName, majorFolderVersionNumber, sendGridUserName, sendGridPassword);
            //Translation should be considered when we move from “Product config” to “Product review” workflow state for HMHS
            if (ConfigurationManager.AppSettings["clientName"] != null && ConfigurationManager.AppSettings["clientName"].ToLower() == "hmhs")
            {
                //Get Workflow State Name
                var wfStateName = workflowStateService.GetWorkflowStateName(workflowStateId);
                if (wfStateName.ToLower() == "product config" || wfStateName.ToLower() == "product config- renewal custom product" || wfStateName.ToLower() == "product config- ntb custom product")
                {
                    var formInstances = folderVersionService.GetProductList(tenantId, folderVersionId);
                    //Queue all formInstances for Translation
                    QueueFormInstancesFortranslation(formInstances);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void QueueFormInstancesFortranslation(List<FormInstanceViewModel> formInstances)
        {
            foreach (var formInstance in formInstances)
            {
                if (folderVersionService.IsDataForFormInstanceChanged(formInstance.FormInstanceID))
                {
                    _mDMTranslatorService.AddProducttoTranslate(formInstance, CurrentUserName);
                }
            }

        }

        [HttpPost]
        public JsonResult UpdateWorkflowFolderMember(int tenantId, int folderId, int folderVersionId, string userIdList, int accountId)
        {
            InitializeEmailSettings();
            List<int> folderMemberList = JsonConvert.DeserializeObject<List<int>>(userIdList);
            ServiceResult result = workflowStateService.UpdateWorkflowStateFolderMember(tenantId, folderId, folderVersionId, folderMemberList, CurrentUserName, accountId, (int)CurrentUserId, sendGridUserName, sendGridPassword);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// SH This method is used to retrieve team members list corresponding to current workflow.
        /// </summary>
        /// <param name="workflowStateId"></param>
        /// <returns></returns>
        public JsonResult GetWorkflowTeamMembersList(int tenantId, int folderId)
        {
            IList<KeyValue> workflowTeamMemberList = workflowStateService.GetWorkFlowTeamMembers(tenantId, folderId, CurrentUserId);
            return Json(workflowTeamMemberList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWorkFlowStateByProductID(string productId)
        {
            int result = this.workflowStateService.GetWorkFlowStateByProductID(productId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        private void InitializeEmailSettings()
        {
            sendGridUserName = ConfigurationManager.AppSettings["SendGridUserName"] ?? string.Empty;
            sendGridPassword = ConfigurationManager.AppSettings["SendGridPassword"] ?? string.Empty;
        }
    }
}