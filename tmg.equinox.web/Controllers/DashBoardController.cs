using System.Collections.Generic;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.DashBoard;
using tmg.equinox.applicationservices.viewmodels.DPF;
using tmg.equinox.web.Framework;
using System.Linq;
using System;
using tmg.equinox.infrastructure.exceptionhandling;
using System.Threading.Tasks;
using tmg.equinox.identitymanagement;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.web.FormInstance;
using System.IO;
using System.Data;
using tmg.equinox.applicationservices.viewmodels.Settings;
using System.Configuration;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.notification;
using System.Web;
using Newtonsoft.Json;

namespace tmg.equinox.web.Controllers
{
    public class DashBoardController : AuthenticatedController
    {
        #region Private Variables

        private readonly IDashboardService service;
        private IUserManagementService _userManagementService;
        private string sendGridUserName = string.Empty;
        private string sendGridPassword = string.Empty;
        private string smtpUserName = string.Empty;
        private string smtpPassword = string.Empty;
        private string smtpPort = string.Empty;
        private string smtpHostServerName = string.Empty;
        private INotificationService _notificationService;
        #endregion
        IResourceLockService _ResourceLockService;
        ISectionLockService _sectionLockService;

        public DashBoardController(IDashboardService service, IUserManagementService userManagementService, INotificationService notificationService, IResourceLockService ResourceLockService, ISectionLockService sectionLockService)
        {
            this.service = service;
            this._userManagementService = userManagementService;
            _notificationService = notificationService;
            _ResourceLockService = ResourceLockService;
            _sectionLockService = sectionLockService;
        }

        // GET: /DashBoard/
        public ActionResult Index()
        {


            ViewBag.IsFolderLockEnable = IdentityManager.IsFolderLockEnable.ToString().ToLower();
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            ViewBag.RoleId = RoleID;
            ViewBag.LastActivity = service.GetLastUserActivity(base.CurrentUserName);
            ViewData["activity"] = service.GetActivity(base.CurrentUserName);
            return View();
        }



        [HttpGet]
        public JsonResult GetFormUpdatesList()
        {
            try
            {
                IEnumerable<FormUpdatesViewModel> formUpdatesViewModelList = service.GetFormUpdatesList(base.TenantID);
                if (formUpdatesViewModelList == null)
                {
                    formUpdatesViewModelList = new List<FormUpdatesViewModel>();
                }

                return Json(formUpdatesViewModelList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
        }


        [HttpGet]
        public JsonResult GetWorkQueueListNotReleasedAndApproved(GridPagingRequest gridPagingRequest, string viewMode, int taskFolderVersionId)
        {
            try
            {
                GridPagingResponse<ViewModelForProofingTasks> workQueueViewModelForProofingTasks = service.GetWorkQueueListForProofingTasks(base.TenantID, base.CurrentUserId, base.CurrentUserName, viewMode, taskFolderVersionId, false, gridPagingRequest);
                return Json(workQueueViewModelForProofingTasks, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
        }

        [HttpGet]
        public JsonResult GetWatchList(GridPagingRequest gridPagingRequest, bool isViewInterested, string viewMode, int taskFolderVersionId)
        {
            try
            {
                GridPagingResponse<ViewModelForProofingTasks> watchListViewModelForProofingTasks = service.GetWatchListForProofingTasks(base.TenantID, base.CurrentUserId, base.CurrentUserName, isViewInterested, viewMode, gridPagingRequest, taskFolderVersionId, false);

                return Json(watchListViewModelForProofingTasks, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
        }

        [HttpGet]
        public JsonResult GetUserRoleAssignment(int folderVersionID, string userAssignmentDialogState, GridPagingRequest gridPagingRequest)
        {
            try
            {
                GridPagingResponse<UserRoleAssignmentViewModel> userRoleAssignmentList = service.GetUserRoleAssignment(folderVersionID, base.CurrentUserId, userAssignmentDialogState, gridPagingRequest);
                return Json(userRoleAssignmentList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
        }

        private void InitializeEmailSettings()
        {
            sendGridUserName = ConfigurationManager.AppSettings["SendGridUserName"] ?? string.Empty;
            sendGridPassword = ConfigurationManager.AppSettings["SendGridPassword"] ?? string.Empty;
            smtpUserName = ConfigurationManager.AppSettings["SmtpUserName"] ?? string.Empty;
            smtpPort = ConfigurationManager.AppSettings["SmtpPort"] ?? string.Empty;
            smtpHostServerName = ConfigurationManager.AppSettings["SmtpHostServerName"] ?? string.Empty;
        }

        public ActionResult ExportWorkQueueListToExcel(string csv, int noOfColInGroup, string repeaterName)
        {
            string header = string.Empty;

            header = "Work Queue List";

            ExcelBuilder excelBuilder = new ExcelBuilder();
            MemoryStream fileStream = excelBuilder.ExportToExcel(csv, false, noOfColInGroup, false, header);

            var fileDownloadName = "WorkQueueList" + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var fsr = new FileStreamResult(fileStream, contentType);
            fsr.FileDownloadName = fileDownloadName;

            return fsr;
        }
        public ActionResult ExportWorkQueueListToExcelTable(string viewMode)
        {
            DataTable workQueyeListdt = service.GetWorkQueueListListForProofingTasksDataTable(base.TenantID, base.CurrentUserId, base.CurrentUserName, viewMode, 0);

            workQueyeListdt.Columns.Remove("MappingRowID");
            workQueyeListdt.Columns.Remove("Account");
            workQueyeListdt.Columns.Remove("TenantID");
            workQueyeListdt.Columns.Remove("FolderId");
            workQueyeListdt.Columns.Remove("FolderVersionId");
            workQueyeListdt.Columns.Remove("FormInstanceId");
            workQueyeListdt.Columns.Remove("TaskId");
            workQueyeListdt.Columns.Remove("AddedBy");
            workQueyeListdt.Columns.Remove("AddedDate");
            workQueyeListdt.Columns.Remove("UpdatedBy");
            workQueyeListdt.Columns.Remove("UpdatedDate");
            workQueyeListdt.Columns.Remove("RoleClaim");
            workQueyeListdt.Columns.Remove("MarkInterested");
            workQueyeListdt.Columns.Remove("LastPageNoForGrid");
            workQueyeListdt.Columns.Remove("Attachments");
            workQueyeListdt.Columns.Remove("ID");
            workQueyeListdt.Columns.Remove("PlanTaskUserMappingDetails");
            workQueyeListdt.Columns.Remove("Order");
            workQueyeListdt.Columns.Remove("FolderVersionWFStateID");
            workQueyeListdt.Columns.Remove("TaskWFStateID");

            if (viewMode != "Completed")
                workQueyeListdt.Columns.Remove("Completed");

            if (workQueyeListdt.Rows.Count > 0)
            {
                foreach (DataRow dr in workQueyeListdt.Rows)
                {
                    string[] arrEffectiveDate = dr["EffectiveDate"].ToString().Split(' ');
                    dr["EffectiveDate"] = (arrEffectiveDate.Count() > 0) ? arrEffectiveDate[0] : "";

                    string[] arrStartDate = dr["StartDate"].ToString().Split(' ');
                    dr["StartDate"] = (arrStartDate.Count() > 0) ? arrStartDate[0] : "";

                    string[] arrDueDate = dr["DueDate"].ToString().Split(' ');
                    dr["DueDate"] = (arrDueDate.Count() > 0) ? arrDueDate[0] : "";

                    if (viewMode == "Completed")
                    {
                        string[] arrCompleted = dr["Completed"].ToString().Split(' ');
                        dr["Completed"] = (arrCompleted.Count() > 0) ? arrCompleted[0] : "";
                    }
                }
            }

            ExcelBuilder excelBuilder = new ExcelBuilder();
            MemoryStream fileStream = excelBuilder.DownLoadDataTableToExcel(workQueyeListdt, "Work Queue List", "Work Queue List");

            var fileDownloadName = "WorkQueueList" + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var fsr = new FileStreamResult(fileStream, contentType);
            fsr.FileDownloadName = fileDownloadName;

            return fsr;
        }

        public ActionResult ExportWatchListToExcel(bool isViewInterested, string viewMode)
        {
            //DataTable watchListdt = service.GetWatchListListDataTable(base.TenantID, base.CurrentUserId);
            DataTable watchListdt = service.GetWatchListListForProofingTasksDataTable(base.TenantID, base.CurrentUserId, base.CurrentUserName, isViewInterested, viewMode, 0);
            watchListdt.Columns.Remove("MappingRowID");
            watchListdt.Columns.Remove("Account");
            watchListdt.Columns.Remove("TenantID");
            watchListdt.Columns.Remove("FolderId");
            watchListdt.Columns.Remove("FolderVersionId");
            watchListdt.Columns.Remove("FormInstanceId");
            watchListdt.Columns.Remove("TaskId");
            watchListdt.Columns.Remove("AddedBy");
            watchListdt.Columns.Remove("AddedDate");
            watchListdt.Columns.Remove("UpdatedBy");
            watchListdt.Columns.Remove("UpdatedDate");
            watchListdt.Columns.Remove("RoleClaim");
            watchListdt.Columns.Remove("LastPageNoForGrid");
            watchListdt.Columns.Remove("Attachments");
            watchListdt.Columns.Remove("ID");
            watchListdt.Columns.Remove("PlanTaskUserMappingDetails");
            watchListdt.Columns.Remove("Order");
            watchListdt.Columns.Remove("FolderVersionWFStateID");
            watchListdt.Columns.Remove("TaskWFStateID");

            if (viewMode != "Completed")
                watchListdt.Columns.Remove("Completed");

            if (watchListdt.Rows.Count > 0)
            {
                foreach (DataRow dr in watchListdt.Rows)
                {
                    string[] arrEffectiveDate = dr["EffectiveDate"].ToString().Split(' ');
                    dr["EffectiveDate"] = (arrEffectiveDate.Count() > 0) ? arrEffectiveDate[0] : "";

                    string[] arrStartDate = dr["StartDate"].ToString().Split(' ');
                    dr["StartDate"] = (arrStartDate.Count() > 0) ? arrStartDate[0] : "";

                    string[] arrDueDate = dr["DueDate"].ToString().Split(' ');
                    dr["DueDate"] = (arrDueDate.Count() > 0) ? arrDueDate[0] : "";

                    if (viewMode == "Completed")
                    {
                        string[] arrCompleted = dr["Completed"].ToString().Split(' ');
                        dr["Completed"] = (arrCompleted.Count() > 0) ? arrCompleted[0] : "";
                    }

                    dr["MarkInterested"] = (dr["MarkInterested"].ToString().ToLower() == "true") ? "Yes" : "No";
                }
            }

            ExcelBuilder excelBuilder = new ExcelBuilder();
            MemoryStream fileStream = excelBuilder.DownLoadDataTableToExcel(watchListdt, "Watch List", "Watch List");

            var fileDownloadName = "WatchList" + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var fsr = new FileStreamResult(fileStream, contentType);
            fsr.FileDownloadName = fileDownloadName;

            return fsr;
        }
        public ActionResult SaveInterstedFolderVersion(int folderVersionId, string currentUserName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                result = service.SaveInterestedWatchList(folderVersionId, base.CurrentUserId);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveInterstedTask(int id)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                result = service.SaveInterestedWatchList(id, true);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveInterstedTask(int id)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                result = service.SaveInterestedWatchList(id, false);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveInterstedAllTask(bool isViewInterested, string viewMode, bool value, int taskFolderVersionId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                DataTable watchListdt = service.GetWatchListListForProofingTasksDataTable(base.TenantID, base.CurrentUserId, base.CurrentUserName, isViewInterested, viewMode, taskFolderVersionId);
                int[] ids = watchListdt.AsEnumerable().Select(r => r.Field<int>("MappingRowID")).ToArray();
                result = service.SaveInterestedAllWatchList(ids, value);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteInterestedFolderVersion(int folderVersionId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                result = service.DeleteInterestedWatchList(folderVersionId);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTeamsOfFolderVersion(int tenantId, int folderVersionID)
        {
            try
            {
                IList<KeyValue> watchList = service.GetTeamsOfFolderVersion(base.TenantID, folderVersionID);
                return Json(watchList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
        }

        public ActionResult CheckIsManager()
        {
            bool isManager = false;
            try
            {
                isManager = service.CheckIsManager(base.CurrentUserId);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(isManager, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult NotificationstatusgridData(GridPagingRequest gridPagingRequest, Boolean viewMode)
        {
            GridPagingResponse<NotificationstatusViewModel> obj = service.GetNotificationstatusDataList(viewMode, base.CurrentUserId, gridPagingRequest);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult NotificationisreadDataclr(Boolean viewMode)
        {

            var obj = _notificationService.MarkNotificationToRead(viewMode, base.CurrentUserId.Value, CurrentUserName);
            return Json(obj, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult NotifiacationAddRecord(string Message, int TypeId, int UserId, string AddedBy, string UpdatedBy)
        {
            NotificationstatusViewModel result = new NotificationstatusViewModel();
            //result = service.SaveNotificationDataList(result);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSubworkQueueList(int FolderVersionID, string viewMode)
        {
            try
            {
                List<ViewModelForProofingTasks> workQueueViewModelForProofingTasks = service.GetSubworkQueueList(FolderVersionID, viewMode).ToList();
                return Json(workQueueViewModelForProofingTasks, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;

            }

        }

        [HttpGet]
        public JsonResult GetCommentList(int TaskID, bool isAttachmentsOnly)
        {
            try
            {
                List<CommentViewModel> commentList = service.GetComment(TaskID, isAttachmentsOnly);
                return Json(commentList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
        }

        [HttpGet]
        public JsonResult GetSubwatchQueueList(int FolderVersionID, string viewMode)
        {
            try
            {
                List<ViewModelForProofingTasks> workQueueViewModelForProofingTasks = service.GetSubwatchQueueList(FolderVersionID, viewMode).ToList();
                return Json(workQueueViewModelForProofingTasks, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;

            }
        }

        [HttpPost]
        public ActionResult uploadExcel()
        {
            string FileUniqueName = string.Empty;
            if (Request.Files.Count > 0)
            {
                try
                {
                    List<string> FileNameList = new List<string>();
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    List<string> FileNamList = new List<string>();
                    string FileName, UniqueName = Guid.NewGuid().ToString();
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            FileName = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            FileName = file.FileName;
                        }

                        string[] SplitStr = FileName.Split('.');
                        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(FileName);
                        string fileExt = Path.GetExtension(FileName);
                        FileUniqueName = fileNameWithoutExt + UniqueName + fileExt;

                        string path1 = Server.MapPath("~/App_Data/TempAttachments");
                        if (!Directory.Exists(path1))
                        {
                            Directory.CreateDirectory(path1);
                        }
                        string fname = Path.Combine(path1, FileUniqueName);
                        file.SaveAs(fname);
                    }
                }
                catch (Exception ex)
                {
                    bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                    if (reThrow) throw ex;
                    return null;
                }
            }
            return Json(FileUniqueName);
        }

        public ActionResult DownloadDocument(string attachment, string file)
        {
            byte[] fileBytes = null;
            byte[] bytes = null;
            string fileNameToDowmloadFile = "";
            string fileName = attachment;
            string fileExtention = Path.GetExtension(fileName);

            try
            {
                byte[] data = null;
                string fullPath = Server.MapPath("~/App_Data/PermAttachments/");
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }
                if (System.IO.File.Exists(fullPath + fileName))
                {
                    FileStream fs = System.IO.File.OpenRead(fullPath + fileName);
                    data = new byte[fs.Length];
                    int br = fs.Read(data, 0, data.Length);
                    if (br != fs.Length)
                        throw new System.IO.IOException(fullPath);
                    fs.Close();
                }
                fileBytes = data;
            }
            catch (Exception ex)
            {
                ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file);

        }

        public JsonResult GetSectionLockStatus()
        {
            return Json(_ResourceLockService.GetAllLockIntances(base.CurrentUserId.Value), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ReleaseSectionListBySuperUser(List<ResourceLockInputModel> resourceLockInputModel)
        {
            try
            {
                if (resourceLockInputModel.Count > 0)
                {
                    foreach (var item in resourceLockInputModel)
                    {

                        _ResourceLockService.ReleaseDocumentAndSectionLock(item.FormInstanceID, item.SectionName);
                    }
                }

            }
            catch (Exception e)
            {

            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateNotifyuserList(List<ResourceLockInputModel> resourceLockInputModel)
        {           
                    
                        _ResourceLockService.UpdateNotifyUserList(resourceLockInputModel, base.CurrentUserId.Value, base.CurrentUserName);
                           
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetIsSuperUser()
        {
            return Json(base.IsSuperUser(), JsonRequestBehavior.AllowGet);
        }
    }
}
