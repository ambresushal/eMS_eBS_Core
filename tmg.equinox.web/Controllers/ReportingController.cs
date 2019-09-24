using System.Collections.Generic;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.WCReport;
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

namespace tmg.equinox.web.Controllers
{
    public class ReportingController : AuthenticatedController
    {
        #region Private Members
        private IReportMasterService reportService;
        #endregion Private Members

        public ReportingController(IReportMasterService reportService)
        {
            this.reportService = reportService;
        }

        [HttpGet]
        public JsonResult GetReportList()
        {
            try
            {
                List<ReportViewModel> reportList = null;

                reportList = reportService.GetReportList().ToList();

                return Json(reportList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
        }

        // GET: Reporting
        public ActionResult Index()
        {
            return View();
        }
    }
}