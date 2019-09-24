using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web.Mvc;
using tmg.equinox.applicationservices.viewmodels.Account;
using tmg.equinox.web.Framework;
using System.Configuration;
using tmg.equinox.web.Framework.ActionFilters;
using tmg.equinox.identitymanagement;
using tmg.equinox.identitymanagement.Models;
using System.Web.Configuration;
using tmg.equinox.web.App_Start;
using tmg.equinox.applicationservices.viewmodels.Report;
using System.Collections.Generic;
using tmg.equinox.applicationservices.interfaces;
using System.Web;
using tmg.equinox.web.FormInstance;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace tmg.equinox.web.Controllers
{
    [NoCache]
    public class ReportController : AuthenticatedController
    {
        #region Private Members
        private IReportService _reportService;

        #endregion Private Members

        #region Constructor
        public ReportController(IReportService reportService)
        {
            this._reportService = reportService;
        }
        #endregion Constructor

        public ActionResult SBCReport(int accountId, int formInstanceId, int tenantId, int adminFormInstanceId)
        {
            return RedirectToRoute(RouteConfig.SBCREPORT, new { accountId = accountId, formInstanceId = formInstanceId, tenantId = tenantId, adminFormInstanceId = adminFormInstanceId });
        }

        /// <summary>
        /// Generic Method to route the Request to Report Page based on ReportName parameter.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="formInstanceId"></param>
        /// <param name="tenantId"></param>
        /// <param name="adminFormInstanceId"></param>
        /// <param name="ReportName"></param>
        /// <param name="folderVersionId"></param>
        /// <returns></returns>
        public ActionResult Reports(int accountId, int formInstanceId, int tenantId, int adminFormInstanceId, string ReportName, int folderVersionId = 0)
        {
            return RedirectToRoute(ReportName, new { accountId = accountId, formInstanceId = formInstanceId, tenantId = tenantId, adminFormInstanceId = adminFormInstanceId, folderVersionId = folderVersionId });
        }

        #region Audit Checklist Report
        public ActionResult AuditChecklistReport()
        {
            return View("~/Views/Report/AuditChecklistReport.cshtml");
        }

        public JsonResult GetAuditChecklistDataList(int tenantID, string fromDate, string toDate)
        {
            DateTime sourceDate = Convert.ToDateTime(fromDate);
            DateTime targetDate = Convert.ToDateTime(toDate);
            IEnumerable<AuditChecklistReportViewModel> dataList = null;
            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

     
        #endregion Audit Checklist Report
    }
}