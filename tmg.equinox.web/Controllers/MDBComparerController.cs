using System.Collections.Generic;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.web.Framework;
using System.IO;
using System;
using System.Configuration;
using tmg.equinox.web.ODMExecuteManager;
using tmg.equinox.repository.interfaces;
using tmg.equinox.web.FormInstance;

namespace tmg.equinox.web.Controllers
{
    public class MDBComparerController : AuthenticatedController
    {
        private IOdmService _OdmService;
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private IFolderVersionServices _folderVersionService { get; set; }
        public MDBComparerController(IOdmService OdmService, IUnitOfWorkAsync unitOfWork, IFolderVersionServices folderVersionService)
        {
            this._OdmService = OdmService;
            this._unitOfWork = unitOfWork;
            this._folderVersionService = folderVersionService;
        }

        public ActionResult MDBComparer()
        {
            ViewBag.RoleId = RoleID;
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            return View("~/Views/PBPImport/MDBComparer.cshtml");
        }


        [HttpPost]
        public JsonResult UploadPBPFiles()
        {
            string file = string.Empty;
            string mDBCompareFilePath = ConfigurationManager.AppSettings["MDBCompareFilePath"];
            if (Request.Files.Count > 0)
            {
                try
                {
                    List<string> FileNameList = _OdmService.SaveFiles(Request, mDBCompareFilePath);

                    return Json(FileNameList, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    return Json(0);
                }
            }
            else
            {
                return Json(0);
            }
        }

        public ActionResult ValidateMDBfiles(string importedFileName, string exportedFileName)
        {
            ServiceResult result = new ServiceResult();
            string mDBCompareFilePath = ConfigurationManager.AppSettings["MDBCompareFilePath"];
            MdbComparer mc = new MdbComparer(this._unitOfWork, this._folderVersionService);
            result = mc.ValidateMDBFiles(importedFileName, exportedFileName, mDBCompareFilePath);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CompareMDB(string importGenratedFileName, string exportGenratedFileName, string importFileName, string importedFilePath, string exportFileName, string exportedFilePath)
        {
            ServiceResult result = new ServiceResult();
            string mDBCompareFilePath = ConfigurationManager.AppSettings["MDBCompareFilePath"];
            MdbComparer mc = new MdbComparer(this._unitOfWork, this._folderVersionService);

            ExcelBuilder excelBuilder = new ExcelBuilder();

            var fileDownloadName = "MDB Compare Report.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            MemoryStream fileStream = mc.Comparer(Request, importGenratedFileName, exportGenratedFileName, mDBCompareFilePath, importFileName, importedFilePath, exportFileName, exportedFilePath);
            var fsr = new FileStreamResult(fileStream, contentType);
            fsr.FileDownloadName = fileDownloadName;
            return fsr;
        }
    }
}
