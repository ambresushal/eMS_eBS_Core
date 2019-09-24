using Newtonsoft.Json;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.CCRIntegration;
using tmg.equinox.domain.entities.Models.CCRTranslator;
using tmg.equinox.web.FormInstance;
using tmg.equinox.web.Framework;

namespace tmg.equinox.web.Controllers
{
    public class CCRIntegrationController : AuthenticatedController
    {

        private IFolderVersionServices _folderVersionServices;
        private ICCRTranslatorService _ccrtanslatorService;
        public CCRIntegrationController(IFolderVersionServices folderVersionServices, ICCRTranslatorService ccrranslatorService)
        {
            this._folderVersionServices = folderVersionServices;
            this._ccrtanslatorService = ccrranslatorService;
        }

        public ActionResult Index()
        {
            ViewBag.RoleId = RoleID;
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);


            return View();
        }

        #region Translation

        [HttpGet]
        public ActionResult TranslationQueue()
        {
            ViewBag.RoleId = RoleID;
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            List<TranslationQueueViewModel> QueueList = _ccrtanslatorService.GetTranslatorQueue();
            ViewBag.data = QueueList;
            return View();
        }

        public ActionResult GetTranslationLog(int processGovernance1Up)
        {
            DataSet logDetails = _ccrtanslatorService.GetTranslationLog(processGovernance1Up, base.RoleID);

            ExcelBuilder excelBuilder = new ExcelBuilder();
            ExcelPackage excelPkg = new ExcelPackage();
            MemoryStream fileStream = new MemoryStream();
            
            for (int i = 0; i < logDetails.Tables.Count; i++)
            {                
                fileStream = excelBuilder.DownLoadDataTablesToExcel(excelPkg, logDetails.Tables[i], "", "Acitivity Log");
            }

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileDownloadName = "TranslationLog -" + processGovernance1Up + ".xlsx";
            var fsr = new FileStreamResult(fileStream, contentType);
            fsr.FileDownloadName = fileDownloadName;
            return fsr;
        }

        #endregion

        #region Research WorkStation
        public ActionResult TranslatedPlan()
        {
            ViewBag.RoleId = RoleID;
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            List<CCRTranslatorQueue> planList = _ccrtanslatorService.GetTranslatorProduct();
            ViewBag.data = JsonConvert.SerializeObject(planList);
            return View();
        }

        public ActionResult TableDetails(string ProductId)
        {
            ViewBag.RoleId = RoleID;
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            ViewBag.ProductId = ProductId;
            ViewBag.data = _ccrtanslatorService.GetTableDetails(ProductId);
            return View();
        }

        public ActionResult ProvisionDetails(string ProductId)
        {
            ViewBag.RoleId = RoleID;            
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            ViewBag.ProductId = ProductId;
            ViewBag.data = _ccrtanslatorService.GetReportDetails(ProductId);
            return View();
        }


        #endregion
    }
}
