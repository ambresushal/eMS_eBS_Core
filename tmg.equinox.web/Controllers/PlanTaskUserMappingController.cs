using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.DPF;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.domain.entities;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;
using tmg.equinox.web.FormDesignManager;

namespace tmg.equinox.web.Controllers
{
    public class PlanTaskUserMappingController : AuthenticatedController
    {
        #region Private Variables
        private IPlanTaskUserMappingService planTaskUserMappingService;
        private IFormDesignService _formDesignServices;
        #endregion
        #region Constructor
        public PlanTaskUserMappingController(IPlanTaskUserMappingService planTaskUserMappService, IFormDesignService formDesignServices)
        {
            this.planTaskUserMappingService = planTaskUserMappService;
            this._formDesignServices = formDesignServices;
        }
        #endregion

        public ActionResult GetTasksTeamMemberList(string strUserName)
        {
            IEnumerable<KeyValue> objList;
            try
            {
                objList = this.planTaskUserMappingService.GetTeamMemberList(base.CurrentUserName);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(objList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetWatchTaskStatusList()
        {
            List<DPFTaskStatusViewModel> TastStatsVMList;

            try
            {
                TastStatsVMList = new List<DPFTaskStatusViewModel>();
                TastStatsVMList.Add(new DPFTaskStatusViewModel() { TaskStatusID = (int)WatchTaskStatus.Assigned, TaskStatusDescription = WatchTaskStatus.Assigned.ToString() });
                TastStatsVMList.Add(new DPFTaskStatusViewModel() { TaskStatusID = (int)WatchTaskStatus.InProgress, TaskStatusDescription = WatchTaskStatus.InProgress.ToString() });
                //TastStatsVMList.Add(new DPFTaskStatusViewModel() { TaskStatusID = (int)WatchTaskStatus.Completed, TaskStatusDescription = WatchTaskStatus.Completed.ToString() });
                TastStatsVMList.Add(new DPFTaskStatusViewModel() { TaskStatusID = (int)WatchTaskStatus.CompletedFail, TaskStatusDescription = GlobalVariables.CompletedFail });
                TastStatsVMList.Add(new DPFTaskStatusViewModel() { TaskStatusID = (int)WatchTaskStatus.CompletedPass, TaskStatusDescription = GlobalVariables.CompletedPass });

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(TastStatsVMList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetWatchTaskPriorityList()
        {
            List<DPFTaskPriorityViewModel> TastStatsVMList;

            try
            {
                TastStatsVMList = new List<DPFTaskPriorityViewModel>();
                TastStatsVMList.Add(new DPFTaskPriorityViewModel() { ID = (int)WatchTaskPriority.Critical, Description = WatchTaskPriority.Critical.ToString() });
                TastStatsVMList.Add(new DPFTaskPriorityViewModel() { ID = (int)WatchTaskPriority.High, Description = WatchTaskPriority.High.ToString() });
                TastStatsVMList.Add(new DPFTaskPriorityViewModel() { ID = (int)WatchTaskPriority.Medium, Description = WatchTaskPriority.Medium.ToString() });
                TastStatsVMList.Add(new DPFTaskPriorityViewModel() { ID = (int)WatchTaskPriority.Low, Description = WatchTaskPriority.Low.ToString() });

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(TastStatsVMList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveWatchPlanTaskUserMapping(DPFPlanTaskUserMappingViewModel objPlanTaskModel)
        {
            bool isSuccess = false;
            ServiceResult saveResult = new ServiceResult();

            try
            {
                objPlanTaskModel.AddedBy = base.CurrentUserName;
                objPlanTaskModel.UpdatedBy = base.CurrentUserName;
                objPlanTaskModel.ManagerUserName = base.CurrentUserName;
                saveResult = this.planTaskUserMappingService.SavePlanTaskUserMapping(objPlanTaskModel);
                if (saveResult.Result == ServiceResultStatus.Success)
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

        public ActionResult GetPlanTaskUserMappingList(int PlanTaskUserMappingId)
        {
            List<DPFPlanTaskUserMappingViewModel> objTaskMapList;

            try
            {
                objTaskMapList = this.planTaskUserMappingService.GetPlanTaskUserMappingList(PlanTaskUserMappingId);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(objTaskMapList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateWatchPlanTaskUserMapping(DPFPlanTaskUserMappingViewModel objPlanTaskModel)
        {
            bool isSuccess = false;
            ServiceResult saveResult = new ServiceResult();

            try
            {
                objPlanTaskModel.UpdatedDate = DateTime.Now;
                objPlanTaskModel.UpdatedBy = base.CurrentUserName;
                objPlanTaskModel.ManagerUserName = base.CurrentUserName;
                saveResult = this.planTaskUserMappingService.UpdatePlanTaskUserMapping(objPlanTaskModel);
                // If save successfull then send Email
                if (saveResult.Result == ServiceResultStatus.Success && (objPlanTaskModel.Status != WatchTaskStatus.Completed.ToString() && objPlanTaskModel.Status != GlobalVariables.CompletedFail && objPlanTaskModel.Status != GlobalVariables.CompletedPass))
                {
                    isSuccess = true;
                    //this.planTaskUserMappingService.SendEmailForChangesInPlanAndTaskAssignment(objPlanTaskModel);
                }
                // If save successfull and Status is completed then send Task completion mail
                if (saveResult.Result == ServiceResultStatus.Success && (objPlanTaskModel.Status == WatchTaskStatus.Completed.ToString() || objPlanTaskModel.Status == GlobalVariables.CompletedFail || objPlanTaskModel.Status == GlobalVariables.CompletedPass))
                {
                    isSuccess = true;
                    //this.planTaskUserMappingService.SendEmailForTaskCompletion(objPlanTaskModel);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateQueuePlanTaskUserMapping(DPFPlanTaskUserMappingViewModel objPlanTaskModel)
        {
            bool isSuccess = false;
            ServiceResult saveResult = new ServiceResult();

            try
            {
                objPlanTaskModel.UpdatedDate = DateTime.Now;
                objPlanTaskModel.UpdatedBy = base.CurrentUserName;
                saveResult = this.planTaskUserMappingService.UpdateQueuePlanTaskUserMapping(objPlanTaskModel);
                // If save successfull then send Email
                if (saveResult.Result == ServiceResultStatus.Success && (objPlanTaskModel.Status == WatchTaskStatus.Completed.ToString() || objPlanTaskModel.Status == GlobalVariables.CompletedFail || objPlanTaskModel.Status == GlobalVariables.CompletedPass))
                {
                    //this.planTaskUserMappingService.SendEmailForTaskCompletion(objPlanTaskModel);
                }
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
        public ActionResult ValidateTaskCompletedForWorkFlow(int FolderVersionID, int WorkFlowVersionStateID)
        {
            ServiceResult saveResult = new ServiceResult();

            try
            {
                bool isValidated = this.planTaskUserMappingService.ValidateTaskCompletedForWorkFlow(FolderVersionID, WorkFlowVersionStateID);
                if (isValidated)
                {
                    ServiceResultItem objResult = new ServiceResultItem();
                    objResult.Status = ServiceResultStatus.Success;
                    List<ServiceResultItem> resultslist = new List<ServiceResultItem>();
                    resultslist.Add(objResult);
                    saveResult.Items = resultslist;
                }
                saveResult.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(saveResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFormDesignVersionList(int folderVersionId)
        {
            IEnumerable<FormDesignVersionRowModel> formDesignVersionList;
            try
            {
                formDesignVersionList = this.planTaskUserMappingService.GetFormDesignVersionList(1, folderVersionId);
                if (formDesignVersionList == null)
                {
                    formDesignVersionList = new List<FormDesignVersionRowModel>();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(formDesignVersionList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTasksFormInstanceListForFolderVersion(int folderVersionId, int folderId, string formDesignVersionId)
        {

            List<FormInstanceViewModel> objList;
            try
            {
                objList = this.planTaskUserMappingService.GetFormInstanceListForFolderVersion(base.TenantID, folderVersionId, folderId, Convert.ToInt32(formDesignVersionId));
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(objList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSectionsList(int tenantId, string formDesignVersionId)
        {
            List<SectionDesign> sectionDesignList;
            try
            {
                sectionDesignList = this.planTaskUserMappingService.GetSectionsList(tenantId, formDesignVersionId, this._formDesignServices);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(sectionDesignList, JsonRequestBehavior.AllowGet);
        }
    }
}