using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.DashBoard;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.web.Framework;

namespace tmg.equinox.web.Controllers
{
    public class WorkFlowController : AuthenticatedController
    {
        #region Private Variables

        private IWorkFlowStateServices workFlowStateservice;
        private IWorkFlowCategoryMappingService workFlowCategoryMappingService;
        private IWorkFlowVersionStatesService workFlowVersionStatesService;
        private IWorkFlowMasterService workFlowMasterService;
        private IWorkFlowVersionStatesAccessService workFlowVersionStatesAccessService;
        private IWFVersionStatesApprovalTypeService wFVersionStatesApprovalTypeService;
        private string sendGridUserName = string.Empty;
        private string sendGridPassword = string.Empty;
        private string smtpUserName = string.Empty;
        private string smtpPassword = string.Empty;
        private string smtpPort = string.Empty;
        private string smtpHostServerName = string.Empty;

        #endregion

        #region Constructor

        public WorkFlowController(IWorkFlowStateServices workFlowStateservice, IWorkFlowCategoryMappingService workFlowCategoryMappingService, IWorkFlowVersionStatesService workFlowVersionStatesService, IWorkFlowMasterService workFlowMasterService, IWorkFlowVersionStatesAccessService workFlowVersionStatesAccessService, IWFVersionStatesApprovalTypeService wFVersionStatesApprovalTypeService)
        {
            this.workFlowStateservice = workFlowStateservice;
            this.workFlowCategoryMappingService = workFlowCategoryMappingService;
            this.workFlowVersionStatesService = workFlowVersionStatesService;
            this.workFlowMasterService = workFlowMasterService;
            this.workFlowVersionStatesAccessService = workFlowVersionStatesAccessService;
            this.wFVersionStatesApprovalTypeService = wFVersionStatesApprovalTypeService;
        }

        #endregion

        #region Action Methods

        public ActionResult UpdateFolderVersionWorkflowStateUser(int tenantId, List<UserRoleAssignmentViewModel> assignedUserList, int folderVersionId)
        {
            ServiceResult result = new ServiceResult();
            ServiceResult exsistingUser = new ServiceResult();
            try
            {
                exsistingUser = workFlowStateservice.ValidateWorkflowStateUser(assignedUserList, folderVersionId);
                if ((new List<ServiceResultItem>(exsistingUser.Items as List<ServiceResultItem>))[0].Messages[0].Length != 0)
                {
                    result = exsistingUser;
                }
                else
                {
                    InitializeEmailSettings();
                    result = workFlowStateservice.UpdateFolderVersionWorkflowStateUser(tenantId, assignedUserList, folderVersionId, base.CurrentUserName, base.CurrentUserId, sendGridUserName, sendGridPassword, smtpUserName, smtpPort, smtpHostServerName);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteFolderVersionWorkflowStateUser(List<UserRoleAssignmentViewModel> assignedUserList, int folderVersionId)
        {
            ServiceResult result = new ServiceResult();
            ServiceResult exsistingUser = new ServiceResult();
            try
            {
                result = workFlowStateservice.DeleteWorkFlowVersionStatesUser(assignedUserList, folderVersionId, base.CurrentUserName);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWorkFlowCategoryMappingList(int tenantID, GridPagingRequest gridPagingRequest)
        {
            List<WorkFlowCategoryMappingViewModel> consortiumList = this.workFlowCategoryMappingService.GetWorkFlowCategoryMappingList(tenantID);
            return Json(consortiumList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateWorkFlowCategoryMapping(int tenantID, int workFlowVersionID, int workFlowType, int accountType, int folderVersionCategoryID)
        {
            ServiceResult result = this.workFlowCategoryMappingService.UpdateWorkFlowCategoryMapping(tenantID, workFlowVersionID, workFlowType, accountType, folderVersionCategoryID, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddWorkFlowCategoryMapping(int tenantID, int workFlowType, int accountType, int folderVersionCategoryID)
        {
            ServiceResult result = this.workFlowCategoryMappingService.AddWorkFlowCategoryMapping(tenantID, workFlowType, accountType, folderVersionCategoryID, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CopyWorkFlowCategoryMapping(int folderVersionCategoryID, int workFlowVersionID)
        {
            ServiceResult result = this.workFlowCategoryMappingService.CopyWorkFlowCategorymapping(folderVersionCategoryID, workFlowVersionID, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteWorkFlowCategoryMapping(int tenantID, int workFlowVersionID)
        {
            ServiceResult result = this.workFlowCategoryMappingService.DeleteWorkFlowCategoryMapping(tenantID, workFlowVersionID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FinalizeWorkFlowVersion(int workFlowVersionID)
        {
            ServiceResult result = this.workFlowCategoryMappingService.FinalizeWorkFlowVersion(workFlowVersionID, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //_workFlowVersionStatesService
        public JsonResult GetWorkFlowVersionStatesList(int workFlowVersionID, GridPagingRequest gridPagingRequest)
        {
            List<WorkFlowVersionStatesViewModel> consortiumList = this.workFlowVersionStatesService.GetWorkFlowVersionStatesList(workFlowVersionID);
            return Json(consortiumList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateWorkFlowVersionStates(int workFlowVersionStatesID, int wFStateID, int sequence)
        {
            ServiceResult result = this.workFlowVersionStatesService.UpdateWorkFlowVersionStates(workFlowVersionStatesID, wFStateID, sequence, CurrentUserName, null);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddWorkFlowVersionStates(int tenantID, int workFlowVersionID, int wFStateID, int sequence)
        {
            ServiceResult result = this.workFlowVersionStatesService.AddWorkFlowVersionStates(tenantID, workFlowVersionID, wFStateID, sequence, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteWorkFlowVersionStates(int workFlowVersionStatesID)
        {
            ServiceResult result = this.workFlowVersionStatesService.DeleteWorkFlowVersionStates(workFlowVersionStatesID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //_workFlowStateMaster
        public JsonResult GetWorkFlowStateMasterList(int tenantID, GridPagingRequest gridPagingRequest)
        {
            List<WorkFlowStateMasterViewModel> consortiumList = this.workFlowMasterService.GetWorkFlowStateMasterList(tenantID);
            return Json(consortiumList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateWorkFlowStateMaster(int wFStateID, string wFStateName)
        {
            ServiceResult result = this.workFlowMasterService.UpdateWorkFlowStateMaster(wFStateID, wFStateName, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddWorkFlowStateMaster(int tenantID, string wFStateName)
        {
            ServiceResult result = this.workFlowMasterService.AddWorkFlowStateMaster(tenantID, wFStateName, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteWorkFlowStateMaster(int wFStateID)
        {
            ServiceResult result = this.workFlowMasterService.DeleteWorkFlowStateMaster(wFStateID, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //_workFlowStateApprovalTypeMaster
        public JsonResult GetWorkFlowStateApprovalTypeMasterList(int tenantID, GridPagingRequest gridPagingRequest)
        {
            List<WorkFlowStateApprovalTypeMasterViewModel> consortiumList = this.workFlowMasterService.GetWorkFlowStateApprovalTypeMasterList(tenantID);
            return Json(consortiumList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateWorkFlowStateApprovalTypeMaster(int workFlowStateApprovalTypeID, string workFlowStateApprovalTypeName)
        {
            ServiceResult result = this.workFlowMasterService.UpdateWorkFlowStateApprovalTypeMaster(workFlowStateApprovalTypeID, workFlowStateApprovalTypeName, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddWorkFlowStateApprovalTypeMaster(int tenantID, string workFlowStateApprovalTypeName)
        {
            ServiceResult result = this.workFlowMasterService.AddWorkFlowStateApprovalTypeMaster(tenantID, workFlowStateApprovalTypeName, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteWorkFlowStateApprovalTypeMaster(int workFlowStateApprovalTypeID)
        {
            ServiceResult result = this.workFlowMasterService.DeleteWorkFlowStateApprovalTypeMaster(workFlowStateApprovalTypeID, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //_workFlowVersionStatesAccessService
        public JsonResult GetWorkFlowVersionStatesAccessList(int workFlowVersionStateID, GridPagingRequest gridPagingRequest)
        {
            List<WorkFlowVersionStatesAccessViewModel> consortiumList = this.workFlowVersionStatesAccessService.GetWorkFlowVersionStatesAccessList(workFlowVersionStateID);
            return Json(consortiumList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateWorkFlowVersionStatesAccess(int workFlowVersionStatesAccessID, int roleID)
        {
            ServiceResult result = this.workFlowVersionStatesAccessService.UpdateWorkFlowVersionStatesAccess(workFlowVersionStatesAccessID, roleID, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddWorkFlowVersionStatesAccess(int workFlowVersionStateID, int roleID)
        {
            ServiceResult result = this.workFlowVersionStatesAccessService.AddWorkFlowVersionStatesAccess(workFlowVersionStateID, roleID, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteWorkFlowVersionStatesAccess(int workFlowVersionStatesAccessID)
        {
            ServiceResult result = this.workFlowVersionStatesAccessService.DeleteWorkFlowVersionStatesAccess(workFlowVersionStatesAccessID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        ////_wFVersionStatesApprovalTypeService
        public JsonResult GetWFVersionStatesApprovalTypeList(int workFlowVersionStateID, GridPagingRequest gridPagingRequest)
        {
            List<WFVersionStatesApprovalTypeViewModel> consortiumList = this.wFVersionStatesApprovalTypeService.GetWFVersionStatesApprovalTypeList(workFlowVersionStateID);
            return Json(consortiumList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateWFVersionStatesApprovalType(int wFVersionStatesApprovalTypeID, int workFlowStateApprovalTypeID)
        {
            ServiceResult result = this.wFVersionStatesApprovalTypeService.UpdateWFVersionStatesApprovalType(wFVersionStatesApprovalTypeID, workFlowStateApprovalTypeID, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddWFVersionStatesApprovalType(int workFlowStateApprovalTypeID, int workFlowVersionStateID)
        {
            ServiceResult result = this.wFVersionStatesApprovalTypeService.AddWFVersionStatesApprovalType(workFlowStateApprovalTypeID, workFlowVersionStateID, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteWFVersionStatesApprovalType(int wFVersionStatesApprovalTypeID)
        {
            ServiceResult result = this.wFVersionStatesApprovalTypeService.DeleteWFVersionStatesApprovalType(wFVersionStatesApprovalTypeID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWFVersionStatesApprovalTypeAction(int wFVersionStatesApprovalTypeID)
        {
            List<WFStatesApprovalTypeActionViewModel> consortiumList = this.wFVersionStatesApprovalTypeService.GetWFStatesApprovalTypeActionList(wFVersionStatesApprovalTypeID);
            return Json(consortiumList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateWFVersionStatesApprovalTypeAction(int wFStatesApprovalTypeActionID, string actionResponse)
        {
            ServiceResult result = this.wFVersionStatesApprovalTypeService.UpdateWFStatesApprovalTypeAction(wFStatesApprovalTypeActionID, actionResponse, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTasksWorkFlowStateListGreaterThanSelected(int wfStateId, int folderversionId)
        {
            List<WorkFlowStateMasterViewModel> objList;
            try
            {
                objList = this.workFlowMasterService.GetWorkFlowStateListGreaterThanSelected(base.TenantID, wfStateId, folderversionId);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(objList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        private void InitializeEmailSettings()
        {
            sendGridUserName = ConfigurationManager.AppSettings["SendGridUserName"] ?? string.Empty;
            sendGridPassword = ConfigurationManager.AppSettings["SendGridPassword"] ?? string.Empty;
            smtpUserName = ConfigurationManager.AppSettings["SmtpUserName"] ?? string.Empty;
            smtpPort = ConfigurationManager.AppSettings["SmtpPort"] ?? string.Empty;
            smtpHostServerName = ConfigurationManager.AppSettings["SmtpHostServerName"] ?? string.Empty;
        }

    }
}