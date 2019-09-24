using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FolderVersionReport;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.FormInstance;
using tmg.equinox.web.Framework;
using tmg.equinox.web.Framework.Caching;

namespace tmg.equinox.web.Controllers
{
    public class JournalReportController : AuthenticatedController
    {
        #region Private Members

        private IJournalReportService _journalReportService;
        private IWorkFlowStateServices workflowStateService;
        private IFolderVersionServices _folderVersionServices;
        private IConsumerAccountService _consumerAccountService;
        private IFormDesignService _formDesignServices;

        #endregion Private Members

        #region Constructor
        public JournalReportController(IJournalReportService journalReportService, IWorkFlowStateServices workflowStateService, IFolderVersionServices folderVersionServices, IConsumerAccountService consumerAccountService, IFormDesignService formDesignServices)
        {
            this._journalReportService = journalReportService;
            this.workflowStateService = workflowStateService;
            this._folderVersionServices = folderVersionServices;
            this._consumerAccountService = consumerAccountService;
            this._formDesignServices = formDesignServices;
        }
        #endregion Constructor

        #region PublicMethod


        /// <summary>
        /// Gets All Journal List for current formInstance with all folderversions
        /// </summary>
        /// <param name="formInstanceId"></param>
        /// <param name="folderVersionId"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public JsonResult GetAllJournalsList(int formInstanceId, int folderVersionId, int folderId, int formDesignVersionId, int tenantId)
        {
            IEnumerable<JournalViewModel> journalsList = null;
            FormDesignVersionDetail detail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(tenantId, formDesignVersionId, _formDesignServices);

            journalsList = _journalReportService.GetAllJournalsList(formInstanceId, folderVersionId, folderId, detail);

            if (journalsList == null)
            {
                journalsList = new List<JournalViewModel>();
            }
            return Json(journalsList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets All Journal List for current formInstance with current folderversion
        /// </summary>
        /// <param name="formInstanceId"></param>
        /// <param name="folderVersionId"></param>
        /// <returns></returns>
        public JsonResult GetCurrentVersionJournalsList(int formInstanceId, int folderVersionId, int formDesignVersionId, int tenantId)
        {
            IEnumerable<JournalViewModel> journalsList = null;
            FormDesignVersionDetail detail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(tenantId, formDesignVersionId, _formDesignServices);

            journalsList = _journalReportService.GetCurrentVersionJournalsList(formInstanceId, folderVersionId, formDesignVersionId, tenantId, detail);

            if (journalsList == null)
            {
                journalsList = new List<JournalViewModel>();
            }
            return Json(journalsList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets all responses for Journal using JournalID
        /// </summary>
        /// <param name="journalId"></param>
        /// <returns></returns>
        public JsonResult GetAllJournalResponsesList(int journalId)
        {
            IEnumerable<JournalResponseViewModel> journalResponsesList = null;

            journalResponsesList = _journalReportService.GetAllJournalResponsesList(journalId);

            if (journalResponsesList == null)
            {
                journalResponsesList = new List<JournalResponseViewModel>();
            }
            return Json(journalResponsesList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets current Journal Status using JournalID
        /// </summary>
        /// <param name="journalId"></param>
        /// <returns></returns>
        public JsonResult GetCurrentJournal(int journalId)
        {
            IEnumerable<JournalViewModel> journal = null;

            journal = _journalReportService.GetCurrentJournal(journalId);

            if (journal == null)
            {
                journal = new List<JournalViewModel>();
            }
            return Json(journal, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveJournalEntry(int formInstanceID, int folderVersionID, string description, string fieldName, string fieldPath, int actionID, int tenantId)
        {
            ServiceResult result = new ServiceResult();
            int addedWFStateID = workflowStateService.GetFolderVersionWorkFlowId(tenantId, folderVersionID);
            this._journalReportService.SaveJournalEntry(formInstanceID, folderVersionID, description, fieldName, fieldPath, actionID, addedWFStateID, null, User.Identity.Name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This function is to save journal responses in Fldr.JournalResponse table
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveJournalResponse(int folderVersionID, string response, int actionID, int tenantId, int journalId, int GlobalCloseActionValue)
        {
            ServiceResult result = new ServiceResult();
            if (actionID == GlobalCloseActionValue)
            {
                int closedWFStateID = workflowStateService.GetFolderVersionWorkFlowId(tenantId, folderVersionID);
                this._journalReportService.UpdateJournalEntry(actionID, closedWFStateID, User.Identity.Name, journalId);
            }
            this._journalReportService.SaveJournalResponse(response, User.Identity.Name, journalId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJournalActionList()
        {
            IEnumerable<JournalActionViewModel> actionList = _journalReportService.GetAllActionList();

            if (actionList == null)
            {
                actionList = new List<JournalActionViewModel>();
            }
            return Json(actionList, JsonRequestBehavior.AllowGet);
        }
        //bool CheckIsAllJournalEntryClosed(int folderVersionId)
        public JsonResult CheckAllJournalEntryIsClosed(int folderVersionId, int formInstanceId)
        {
            bool isClosedAllJournal = _journalReportService.CheckAllJournalEntryIsClosed(folderVersionId, formInstanceId);
            return Json(isClosedAllJournal, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsAllJournalEntryIsClosed(int tenantId, int folderVersionId)
        {
            List<FormInstanceViewModel> formInstances = new List<FormInstanceViewModel>();

            //formInstances = this._folderVersionServices.GetFormList(tenantId, folderVersionId);
            formInstances = this._folderVersionServices.GetProductList(tenantId, folderVersionId);

            bool isClosedAllJournal = false;
            foreach(var formInstance in formInstances)
            {
                isClosedAllJournal = _journalReportService.CheckAllJournalEntryIsClosed(folderVersionId, formInstance.FormInstanceID);
                if (isClosedAllJournal == true)
                {
                    break;
                }
            }
            return Json(isClosedAllJournal, JsonRequestBehavior.AllowGet);
        }
        //[NonAction]
        public ActionResult JournalEntryReport()
        {
            return View();
        }

        //Generate Report on Excel sheet
        public ActionResult ExportJournalReport(int tenantId, string accountName, int formInstanceId, int folderId, string folderName, int folderVersionId, string formDesignName, bool isPreviousVersion, int formDesignVersionId)
        {
            bool isGroupHeader = false;
            int noOfColInGroup = 0;
            bool isChildGrid = true;
            FormDesignVersionDetail detail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(tenantId, formDesignVersionId, _formDesignServices);

            string header = string.Empty;
            FolderVersionViewModel folderVersionViewModel = _folderVersionServices.GetFolderVersion(CurrentUserId, CurrentUserName, tenantId, folderVersionId, folderId);


            if (folderVersionViewModel != null)
            {
                if (folderVersionViewModel.AccountId.HasValue && folderVersionViewModel.AccountId.Value > 0)
                {
                    header = "Account:" + _consumerAccountService.GetAccountName(1, folderVersionViewModel.AccountId.Value);
                }
                header = header + "\r\nFolder: " + folderName;
                header = header + "\r\nForm Name: " + formDesignName;
            }

            string csv = string.Empty;
            int journalId;
            if (isPreviousVersion)
            {
                var journalsList = _journalReportService.GetAllJournalsList(formInstanceId, folderVersionId, folderId, detail);
                csv = ToCsvHeader("\t", journalsList);

                foreach (var journal in journalsList)
                {
                    csv += ToCsvRow("\t", journalsList, journal);
                    journalId = journal.JournalID;
                    var journalResponsesList = _journalReportService.GetAllJournalResponsesList(journalId);

                    if (journalResponsesList != null)
                    {
                        csv += "\r\ndummyColumn\t" + ToCSV("\t", journalResponsesList);
                    }
                }
            }
            else
            {
                var journalsList = _journalReportService.GetCurrentVersionJournalsList(formInstanceId, folderVersionId, tenantId, formDesignVersionId, detail);
                csv = ToCsvHeader("\t", journalsList);

                foreach (var journal in journalsList)
                {
                    csv += ToCsvRow("\t", journalsList, journal);
                    journalId = journal.JournalID;
                    var journalResponsesList = _journalReportService.GetAllJournalResponsesList(journalId);

                    if (journalResponsesList != null)
                    {
                        csv += "\r\ndummyColumn\t" + ToCSV("\t", journalResponsesList);
                    }
                }
            }

            ExcelBuilder excelBuilder = new ExcelBuilder();
            MemoryStream fileStream = excelBuilder.ExportToExcel(csv, isGroupHeader, noOfColInGroup, isChildGrid, header);

            var fileDownloadName = "JournalReport - " + formDesignName + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var fsr = new FileStreamResult(fileStream, contentType);
            fsr.FileDownloadName = fileDownloadName;

            return fsr;
        }

        public static string ToCsvHeader<T>(string separator, IEnumerable<T> objectlist)
        {
            Type t = typeof(T);
            PropertyInfo[] props = t.GetProperties();

            PropertyInfo[] filterprops = props.Where(p => p.Name != "JournalID" && p.Name != "FormInstanceID"
                                                        && p.Name != "FolderVersionID" && p.Name != "ActionID"
                                                        && p.Name != "AddedWFStateID" && p.Name != "ClosedWFStateID"
                                                        && p.Name != "RoleClaim").ToArray();

            string header = String.Join(separator, filterprops.Select(f => f.Name).ToArray());

            StringBuilder csvdata = new StringBuilder();
            csvdata.AppendLine(header);

            return csvdata.ToString();
        }

        public static string ToCsvRow<T>(string separator, IEnumerable<T> objectlist, object o)
        {
            Type t = typeof(T);
            PropertyInfo[] props = t.GetProperties();
            PropertyInfo[] filterprops = props.Where(p => p.Name != "JournalID" && p.Name != "FormInstanceID"
                                                        && p.Name != "FolderVersionID" && p.Name != "ActionID"
                                                        && p.Name != "AddedWFStateID" && p.Name != "ClosedWFStateID"
                                                        && p.Name != "RoleClaim").ToArray();

            StringBuilder linie = new StringBuilder();

            foreach (var f in filterprops)
            {
                if (linie.Length > 0)
                    linie.Append(separator);

                var x = f.GetValue(o);

                if (x != null)
                {
                    if (f.Name == "AddedDate" || f.Name == "UpdatedDate")
                    {
                        linie.Append(x.ToString().Substring(0, 10));
                    }
                    else
                    {
                        linie.Append(x.ToString());
                    }
                }
            }

            return linie.ToString();
        }

        public static string ToCSV<T>(string separator, IEnumerable<T> objectlist)
        {
            Type t = typeof(T);
            PropertyInfo[] props = t.GetProperties();
            PropertyInfo[] filterprops = props.Where(p => p.Name != "JournalResponseID" && p.Name != "JournalID"
                                                        && p.Name != "UpdatedDate" && p.Name != "UpdatedBy"
                                                        && p.Name != "RoleClaim").ToArray();

            string header = String.Join(separator, filterprops.Select(f => f.Name).ToArray());

            StringBuilder csvdata = new StringBuilder();
            csvdata.AppendLine(header);

            foreach (var o in objectlist)
            {
                csvdata.Append("\t");
                csvdata.AppendLine(ToCsvFields(separator, filterprops, o));
            }

            return csvdata.ToString();
        }

        public static string ToCsvFields(string separator, PropertyInfo[] props, object o)
        {
            StringBuilder linie = new StringBuilder();

            foreach (var f in props)
            {
                if (linie.Length > 0)
                    linie.Append(separator);

                var x = f.GetValue(o);


                if (x != null)
                {
                    if (f.Name == "AddedDate" || f.Name == "UpdatedDate")
                    {
                        linie.Append(x.ToString().Substring(0, 10));
                    }
                    else
                    {
                        linie.Append(x.ToString());
                    }
                }
            }

            return linie.ToString();
        }

        public ActionResult ProductShareReport()
        {
            return View("ProductShareReport");
        }

        #endregion PublicMethod

    }
}