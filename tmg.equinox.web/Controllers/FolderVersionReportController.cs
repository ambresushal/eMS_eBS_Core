using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.Reporting;
using tmg.equinox.web.FormInstance;
using tmg.equinox.web.Framework;


namespace tmg.equinox.web.Controllers
{

    public class FolderVersionReportController : AuthenticatedController
    {

        #region Private Members
        private IFolderVersionReportService _folderVersionReportService;
        private IFolderVersionServices _folderVersionServices;
       
        #endregion Private Members

        #region Constructor
        public FolderVersionReportController(IFolderVersionReportService _folderVersionReportService, IFolderVersionServices _folderVersionServices)
        {
            this._folderVersionReportService = _folderVersionReportService;
            this._folderVersionServices = _folderVersionServices;
        }
        #endregion Constructor

        #region ActionMethod
        //[NonAction]
        public ActionResult ChangeSummaryReport()
        {
            return View();
        }
        #endregion ActionMethod

        #region PublicMethod
        /// <summary>
        /// Gets the folder name list form which the form instances will be copied.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="accountType">Type of the account.</param>
        /// <returns></returns>
        public JsonResult GetFolderList(int tenantId, int accountType)
        {           
           IEnumerable<ReportingViewModel> folderList = null;
            if (accountType == 1)
            {
                folderList = _folderVersionReportService.GetFolderList(tenantId, true);
            }
            else if (accountType == 2)
            {
                folderList = _folderVersionReportService.GetFolderList(tenantId, false); ;
            }
            if (folderList == null)
            {
                folderList = new List<ReportingViewModel>();
            }
            return Json(folderList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the folder Version list based on folder .
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="accountType">Type of the account.</param>
        /// /// <param name="FolderId">Type of the folder identifier.</param>
        /// <returns></returns>
        public JsonResult GetFolderVersionList(int tenantId, int accountType, int folderId)
        {
            IEnumerable<ReportingViewModel> folderVersionList = null;
            if (accountType == 1)
            {
                folderVersionList = _folderVersionReportService.GetFolderVersionList(tenantId, folderId, true);
            }
            else if (accountType == 2)
            {
                folderVersionList = _folderVersionReportService.GetFolderVersionList(tenantId, folderId, false); ;
            }
            if (folderVersionList == null)
            {
                folderVersionList = new List<ReportingViewModel>();
            }
            return Json(folderVersionList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the folder form list form which the form instances will be copied.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="folderOneVersionId">Type of the first folder version identifier.</param>
        /// <param name="folderTwoVersionId">Type of the second folder version identifier.</param
        /// <returns></returns>
        public JsonResult GetSourceFormInstanceList(int tenantId, int sourceFolderVersionId)
        {
            IEnumerable<ReportingViewModel> formInstanceList = null;

            formInstanceList = _folderVersionReportService.GetSourceFormInstanceList(tenantId, sourceFolderVersionId); ;

            return Json(formInstanceList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the form list toform based on folder version.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="folderVersionId">The folder version identifier.</param>
        /// <returns></returns>
        public JsonResult GetTargetFormInstanceList(int tenantId, int folderVersionId)
        {
            IEnumerable<ReportingViewModel> formInstanceList = _folderVersionReportService.GetTargetFormInstanceList(tenantId, folderVersionId);
            if (formInstanceList == null)
            {
                formInstanceList = new List<ReportingViewModel>();
            }
            return Json(formInstanceList, JsonRequestBehavior.AllowGet);
        }

        //Genrate Report on Excel sheet
        public ActionResult ExportChangeSummaryReport(int tenantId, int sourceFolderId, int targetFolderId, string sourceFolderName, string targetFolderName, string sourceFolderVersion, string targetFolderVersion, string encodeString)
        {

            //get json string
            string jsonString = HttpUtility.UrlDecode(encodeString);
            //get folder Account Name
            string sourceFolderAccountName = this._folderVersionReportService.GetFolderAccountName(tenantId, sourceFolderId);
            string targetFolderAccountName = this._folderVersionReportService.GetFolderAccountName(tenantId, targetFolderId);

            List<string> coverPageDataList = new List<string>();
            coverPageDataList.Add(sourceFolderAccountName);
            coverPageDataList.Add(targetFolderAccountName);
            coverPageDataList.Add(sourceFolderName);
            coverPageDataList.Add(targetFolderName);
            coverPageDataList.Add(sourceFolderVersion);
            coverPageDataList.Add(targetFolderVersion);


            ChangeSummaryReportBuilder reportBuilder = new ChangeSummaryReportBuilder(_folderVersionServices, _folderVersionReportService);
            string fileName = "ChangeSummary";
            var fileDownloadName = fileName + ".xlsx";
           var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

           return File((reportBuilder.ExportToExcel(tenantId,coverPageDataList, jsonString)), contentType, fileDownloadName);         

             
        }

        
        #endregion PublicMethod
     
    }


}