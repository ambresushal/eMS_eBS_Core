using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.infrastructure.exceptionhandling;
using System.Xml;
using System.Collections;
using System.Text;
using tmg.equinox.dependencyresolution;
using Newtonsoft.Json;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.Collateral;
using System.Configuration;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.generatecollateral;
using System.Web.UI;
using tmg.equinox.web.CollateralHelper;
using tmg.equinox.web.FormInstanceProcessor;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.web.Framework;
using tmg.equinox.setting.Interface;
using System.Web;
using System.Web.Hosting;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.web.Controllers
{
    public class DocumentCollateralController : AuthenticatedController
    {
        #region Private Variabls Declaration
        private ICollateralService _reportDesignService;
        //private IDocumentCollateralService _documentService { get; set; }
        #endregion

        public int CookieExpirationTime { get; set; }
        private IUIElementService _uiElementService;
        private IFormDesignService _formDesignService;
        private IFolderVersionServices _folderVersionService;
        private IFormInstanceService _formInstanceService;
        private IFormInstanceDataServices _formInstanceDataService;
        private IMasterListService _masterListService;
        ISettingManager _settingManager;
        public DocumentCollateralController(IUIElementService uiElementService, IFormDesignService formDesignService, IFolderVersionServices folderVersionService, IFormInstanceService formInstanceService, IFormInstanceDataServices formInstanceDataService, IMasterListService masterListService, ISettingManager settingManager)
        {
            //this._documentService = documentCollateralService;
            _reportDesignService = UnityConfig.Resolve<ICollateralService>();
            _uiElementService = uiElementService;
            _formDesignService = formDesignService;
            _folderVersionService = folderVersionService;
            _formInstanceService = formInstanceService;
            _formInstanceDataService = formInstanceDataService;
            _masterListService = masterListService;
            _settingManager = settingManager;
        }

        public ActionResult Index()
        {
            //ViewData["CookieExpirationTime"] = CookieExpirationTime;
            //ViewData["SessionTimeoutWarning"] = ConfigurationManager.AppSettings["LoginWarning"];

            //ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            return View("ViewReportTemplate");
        }

        public JsonResult FormDesignList(int tenantId)
        {
            IEnumerable<FormDesignRowModel> formDesignList = null;
            formDesignList = _reportDesignService.GetFormDesignList(tenantId);

            if (formDesignList == null)
            {
                formDesignList = new List<FormDesignRowModel>();
            }
            return Json(formDesignList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FormDesignVersionList(int tenantId, int formDesignId)
        {
            IEnumerable<FormDesignVersionRowModel> formDesignVersionList = _reportDesignService.GetFormDesignVersionList(tenantId, formDesignId);
            if (formDesignVersionList == null)
            {
                formDesignVersionList = new List<FormDesignVersionRowModel>();
            }
            return Json(formDesignVersionList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewReportTemplate()
        {
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            ViewBag.RoleId = RoleID;
            return View("ViewReportTemplate");
        }


        public JsonResult AddReportTemplate(int TenantID, int ReportTemplateID, string ReportName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                string username = User.Identity.Name;
                result = _reportDesignService.AddReportTemplate(TenantID, ReportTemplateID, ReportName, username);
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateReportTemplate(int TenantID, int ReportTemplateID, string ReportName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                string username = User.Identity.Name;
                result = _reportDesignService.UpdateReportTemplate(TenantID, ReportTemplateID, ReportName, username);
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddReportTemplateVersion(int TenantID, int ReportTemplateID, string EffectiveDate, int reportTemplateVersionID)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                string username = User.Identity.Name;
                result = _reportDesignService.AddReportTemplateVersion(TenantID, ReportTemplateID, EffectiveDate, username, reportTemplateVersionID);
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditReportTemplateVersion(int TenantID, string ReportTemplateVersionID, string EffectiveDate)
        {
            //ViewData["ReportTemplateVersionID"] = ReportTemplateVersionID;
            //return View("EditReportTemplateVersion");

            ServiceResult result = new ServiceResult();
            try
            {
                string username = User.Identity.Name;
                int templateRepostVersionId = int.Parse(ReportTemplateVersionID);

                result = _reportDesignService.UpdateReportTemplateVersion(TenantID, templateRepostVersionId, EffectiveDate, username);
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
            }
            return Json(result, JsonRequestBehavior.AllowGet);

            //ServiceResult result = new ServiceResult();
            //try
            //{
            //    string username = User.Identity.Name;
            //    result = _reportDesignService.UpdateReportTemplate(TenantID, ReportTemplateVersionID, username);
            //}
            //catch (Exception ex)
            //{
            //    result.Result = ServiceResultStatus.Failure;
            //}
            //return null;
        }

        public JsonResult DeleteReportTemplateVersion(int TenantId, int ReportTemplateVersionID)
        {
            ServiceResult result = _reportDesignService.DeleteReportTemplateVersion(TenantId, ReportTemplateVersionID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReportTemplateVersions(int TenantID, int ReportTemplateID, GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<ReportTemplateVersionModel> data = _reportDesignService.GetReportTemplateVersionList(TenantID, ReportTemplateID, gridPagingRequest);
            return Json(data);
        }


        // GET: DocumentCollateral
        public ActionResult Index1()
        {
            return View();
        }

        //public JsonResult GetDocumentList(int tenantId)
        //{
        //    IEnumerable<DocumentCollateralViewModel> documentList = this._documentService.GetDocumentList(tenantId);
        //    return Json(documentList, JsonRequestBehavior.AllowGet);
        //}

        //public FileResult GenerateReport(int formInstanceId)
        //{
        //    try
        //    {
        //        string templateFile = Request.PhysicalApplicationPath + "files\\BenefitMatrix_Final.docx";
        //        FileStream template = new FileStream(templateFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        //        FileStream adminDataSource = new FileStream(Request.PhysicalApplicationPath + "files\\AdminJson.xml", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        //        FileStream medDataSource = new FileStream(Request.PhysicalApplicationPath + "files\\MedicalXML.xml", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        //        DocumentGenerator generator = new DocumentGenerator(adminDataSource, medDataSource, template, 0);
        //        return File(generator.GenerateReport(), "application/pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
        //        if (reThrow)
        //            throw;
        //    }
        //    return null;
        //}

        public JsonResult CheckDSFileDownloadPossibility(int tenantId, int formDesignVersionId)
        {
            try
            {
                DownloadDocumentDesignVersionXML(tenantId, formDesignVersionId);
            }
            catch (Exception ex)
            {
                return Json("Failure");
            }
            return Json("Success");
        }

        public ActionResult DownloadDocumentDesignVersionXML(int tenantId, int formDesignVersionId)
        {
            byte[] data = null;
            string FileName = string.Empty;
            try
            {
                XmlDocument Jsondata = null;

                Jsondata = _reportDesignService.GetFormDesignVersionXML(tenantId, formDesignVersionId, ref FileName);
                if (Jsondata != null)
                {
                    data = Encoding.UTF8.GetBytes(Jsondata.OuterXml);
                    ViewData["downloadStatus"] = "Success";
                }
            }
            catch (Exception ex)
            {
                ViewData["downloadStatus"] = "Failure";
                //return View("Index");
            }
            return File(data, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
        }

        [HttpGet]
        public ActionResult UploadReportTemplate(int ReportTemplateID, string ReportTemplateName)
        {
            ViewData["ReportTemplateID"] = ReportTemplateID;
            ViewData["ReportTemplateName"] = ReportTemplateName;
            return View("UploadReportTemplate");
        }

        public JsonResult GetDocumentDesignVersion(int ReportTemplateVersionID)  //, GridPagingRequest gridPagingRequest)
        {
            IEnumerable<ReportTemplateModel> data = _reportDesignService.GetDocumentVersionInfo(ReportTemplateVersionID);//, gridPagingRequest);
            //if (data == null)
            //{
            //    data = new GridPagingResponse<ReportTemplateModel>(gridPagingRequest.page, 0, gridPagingRequest.rows, new List<ReportTemplateModel>());
            //}
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]     // int tenantId, int formDesignId, int formDesignVersionId
        public JsonResult UpdateReportTemplateVersion(string ReportVersionID, string reportTemplateName, string VersionNumber, string templateDocMappings, string templateProperties, string Parameters)
        {
            string uploadedFileName = string.Empty;
            ServiceResult result = new ServiceResult();
            string username = User.Identity.Name;
            try
            {
                int ReportVersionId = 0;

                Int32.TryParse(ReportVersionID, out ReportVersionId);
                int tenantid = 1;
                result.Result = ServiceResultStatus.Failure;
                string path = string.Empty;
                uploadedFileName = (string)Request.Form["uploadedFileName"];
                string uploadedFileNameExtension = "";
                if (!string.IsNullOrEmpty(uploadedFileName))
                {
                    uploadedFileNameExtension = uploadedFileName.Substring(uploadedFileName.LastIndexOf('.'));
                    reportTemplateName += "_" + VersionNumber;
                    path = Server.MapPath(@"~\App_Data") + @"\" + reportTemplateName + uploadedFileNameExtension;
                    Request.Files[0].SaveAs(path);
                }

                result = _reportDesignService.UpdateReportTemplateVersion(tenantid, username, path, ReportVersionId, reportTemplateName, templateDocMappings, templateProperties, Parameters);
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
            }
            if (result.Result == ServiceResultStatus.Success)
            {
                if (!string.IsNullOrEmpty(uploadedFileName))
                {
                    if (((List<ServiceResultItem>)result.Items).Count == 0)
                        ((List<ServiceResultItem>)result.Items).Add(new ServiceResultItem() { Messages = new string[] { "" } });

                    string reportName = reportTemplateName.Split('_')[0];
                    string description = "File " + uploadedFileName + " has been uploaded against template " + reportName + " of version " + VersionNumber;
                    _reportDesignService.SaveTemplateActivityLog(Convert.ToInt32(ReportVersionID), description, username);
                    ((List<ServiceResultItem>)result.Items).Add(new ServiceResultItem() { Messages = new string[] { "Template File Uploaded Successfully" } });//uploadedFileName
                }
                return Json(result);
            }
            else
            {
                return Json(result);
            }
        }

        public ActionResult ReportSettings()
        {
            return View();
        }

        public JsonResult GetCollateralNames(int tenantId, string ReportLocation = "InMenu")
        {
            IEnumerable<ReportDesignViewModel> reportNames = new List<ReportDesignViewModel>();
            reportNames = _reportDesignService.GetReportNamesForGeneration(tenantId, ReportLocation);
            return Json(reportNames, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProperties(int tenantId, int TemplateReportVersionID)
        {
            //List<ReportPropertiesViewModel> list = new List<ReportPropertiesViewModel>();
            ReportPropertiesViewModel reportProperties = null;

            //ReportPropertiesViewModel> reportProperties = new List<ReportPropertiesViewModel>();
            reportProperties = _reportDesignService.GetProperties(tenantId, TemplateReportVersionID);

            if (reportProperties != null)
            {
                reportProperties.HelpText = reportProperties.HelpText ?? "";
                reportProperties.ReportDescription = reportProperties.ReportDescription ?? "";
            }
            else
            {
                reportProperties = new ReportPropertiesViewModel();
                reportProperties.HelpText = "";
                reportProperties.Visible = false;
                reportProperties.ReportDescription = "";
            }
            return Json(reportProperties, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckTemplateDownloadFilePossibility(string ReportTemplateVersionID)
        {
            try
            {
                DownloadDocument(ReportTemplateVersionID);
            }
            catch (Exception ex)
            {
                return Json("Failure");
            }
            return Json("Success");
        }

        public FileContentResult DownloadDocument(string ReportTemplateVersionID)
        {
            byte[] fileBytes = null;
            byte[] bytes = null;
            string fileName = GetTemplateNameById(Convert.ToInt32(ReportTemplateVersionID));
            string fileNameToDowmloadFile = "";
            string fileExtention = Path.GetExtension(fileName);

            string savePath = Server.MapPath("~" + "\\App_Data\\" + fileName);
            try
            {
                byte[] data = null;
                string fullPath = Server.MapPath("~" + "\\App_Data\\" + fileName);

                if (System.IO.File.Exists(fullPath))
                {
                    FileStream fs = System.IO.File.OpenRead(fullPath);
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
            //var fileName1 = fileName.Split('\\');
            //fileNameToDowmloadFile = fileName1[1];

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

        }

        public JsonResult UpdateReportProperties(ReportPropertiesViewModel model)
        {
            ServiceResult result = new ServiceResult();

            result = _reportDesignService.UpdateReportPropertiesParameters(new List<ParameterViewModel>(), model);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerateReports()
        {
            string collateralOption = ConfigurationManager.AppSettings["CollateralOption"];
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            ViewBag.RoleId = RoleID;
            return View("GenerateReports", null, collateralOption);
        }

        public ActionResult ViewReport(int accountID, int folderID, string formInstanceID, int folderVersionID, string reportTemplateName, int reportTemplateID, string folderVersionEffDt, string description, int formDesignVersionID, string[] collateralOptions, bool isPDF, string documentName)
        {
            string reportTemplateLocation = string.Empty;
            string status = string.Empty;
            string[] formInstanceIDList = formInstanceID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int formInstanceId = Int32.Parse(formInstanceIDList[0]);

            try
            {
                FormInstanceViewModel viewDetails = _reportDesignService.GetFormInstanceIdForView(formInstanceId, reportTemplateID, formDesignVersionID, formInstanceIDList, folderVersionEffDt);
                FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(1, formDesignVersionID, _formDesignService);
                FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(true);
                FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(1, base.CurrentUserId, _formInstanceDataService, base.CurrentUserName, _folderVersionService);
                FormInstanceDataProcessor dataProcessor = new FormInstanceDataProcessor(_folderVersionService, formInstanceId,
                                                                                        folderVersionID, viewDetails.FormDesignVersionID,
                                                                                        false,
                                                                                        base.CurrentUserId, formInstanceDataManager, _formInstanceDataService,
                                                                                        _uiElementService, detail,
                                                                                        base.CurrentUserName, _formDesignService, _masterListService, _formInstanceService);


                dataProcessor.RunViewProcessorsOnCollateralGeneration();
                formInstanceDataManager.SaveTargetSectionsData(formInstanceId, _folderVersionService, _formDesignService);


                List<string> lst = _reportDesignService.GetGenerateReportInputs(formInstanceId, reportTemplateID, formDesignVersionID, formInstanceIDList, folderVersionEffDt);

                //string _path = "C:\\Users\\myadav\\Desktop\\HNE\\chap4.docx";
                GenerateCollateral gn;
                string outputpath = string.Empty;
                ColleteralFilePath colleteralFilePath;
                string imageFilePath = Server.MapPath(@"~\Content") + @"\tinyMce\";
                string collateralFolderPath = ConfigurationManager.AppSettings["CollateralFolderPath"];
                outputpath = collateralFolderPath + "Default.pdf";
                if (lst != null && lst.Count > 0)
                {
                    gn = new GenerateCollateral(lst[0], lst[1], lst[2], collateralFolderPath, imageFilePath, _settingManager, _formInstanceService,_reportDesignService);
                    gn.FormInstanceDetails = _formInstanceService.GetFormInstanceDetails(formInstanceId);
                    outputpath = gn.Generate();
                    outputpath = gn.GenerateWord(outputpath);
                    if (isPDF)
                    {
                        colleteralFilePath = gn.GeneratePDF(outputpath, formInstanceId, User.Identity.Name, 0);
                        outputpath = colleteralFilePath.PDF;
                    }
                }

                FileStream result = System.IO.File.OpenRead(outputpath); // (Server.MapPath(@"~\App_Data") + @"\" + datasource.ReportName + ".pdf");
                byte[] dataBytes = new byte[result.Length];
                result.Read(dataBytes, 0, (int)result.Length);
                string fileName = "";
                if (isPDF && !String.IsNullOrEmpty(documentName))
                {
                    fileName = documentName + ".Pdf";
                }
                else if (!isPDF && !String.IsNullOrEmpty(documentName))
                {
                    fileName = documentName + ".Docx";
                }
                else fileName = System.IO.Path.GetFileName(outputpath);
                return File(dataBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);
            }
            catch (Exception ex)
            {

                return Json("true");
            }

        }

        public ActionResult GenerateCollateral(string accountIDs, string folderIDs, string formInstanceIDs, string formNames, string folderVersionIDs, string reportTemplateName, int reportTemplateID, string folderVersionEffDts, string formDesignVersionIDs, string[] collateralOptions, int rowCount, bool isPDF)
        {
            dynamic output = "";
            List<SBConfig> sbConfig = new List<SBConfig>();
            string[] formInstances = formInstanceIDs.TrimEnd(',').Split(new char[] { ',' });
            string[] folderVersions = folderVersionIDs.TrimEnd(',').Split(new char[] { ',' });
            string[] effectiveDate = folderVersionEffDts.TrimEnd(',').Split(new char[] { ',' });
            string[] formDesignVersions = formDesignVersionIDs.TrimEnd(',').Split(new char[] { ',' });
            if (formInstances.Length == 1)
            {
                int accountId = 0;
                if (accountIDs.TrimEnd(',').Length > 0) // accountIDs will be blank for portfolio folders
                    accountId = int.Parse(accountIDs.TrimEnd(',').Split(new char[] { ',' })[0]);
                int folderId = int.Parse(folderIDs.TrimEnd(',').Split(new char[] { ',' })[0]);
                string formInstanceId = formInstances[0];
                int folderVersionId = int.Parse(folderVersions[0]);
                string effDate = effectiveDate[0];
                int formDesignVersionId = int.Parse(formDesignVersions[0]);
                output = this.ViewReport(accountId, folderId, formInstanceId, folderVersionId, reportTemplateName, reportTemplateID, effDate, string.Empty, formDesignVersionId, collateralOptions, isPDF, ""); // By default word report to be generated.
            }
            else
            {
                if (formInstances.Length > 0 && folderVersions.Length > 0 && folderVersions.Length == formInstances.Length)
                {
                    int index = 0;
                    foreach (string fi in formInstances)
                    {
                        sbConfig.Add(new SBConfig() { FormInstanceID = Convert.ToInt32(fi), FolderVersionID = Convert.ToInt32(folderVersions[index]) });
                        index = index + 1;
                    }
                }
                if (sbConfig.Count > 0)
                {
                    var formInstance = _folderVersionService.GetFormInstance(TenantID, sbConfig[0].FormInstanceID);
                    int _formDesignVersionID = formInstance == null ? 1258 : formInstance.FormDesignVersionID;//

                    List<string> lst = _reportDesignService.GetGenerateReportInputs(sbConfig[0].FormInstanceID, reportTemplateID, _formDesignVersionID, formInstances, effectiveDate[0]);

                    SBDesignDataHelper dataHelper = new SBDesignDataHelper(sbConfig[0].FormInstanceID, sbConfig[0].FolderVersionID, _formDesignVersionID, Convert.ToInt32(CurrentUserId), CurrentUserName,
                        _uiElementService, _formDesignService, _folderVersionService, _formInstanceService, _formInstanceDataService, _masterListService);
                    string sbDesingJsonData = dataHelper.ProcessSBDesign(sbConfig);

                    GenerateCollateral gn;
                    string outputpath = string.Empty;
                    ColleteralFilePath colleteralFilePath = null;
                    string imageFilePath = Server.MapPath(@"~\Content") + @"\tinyMce\";
                    string collateralFolderPath = ConfigurationManager.AppSettings["CollateralFolderPath"];
                    outputpath = collateralFolderPath + "Default.pdf";
                    {
                        gn = new GenerateCollateral(lst[0], sbDesingJsonData, lst[2], collateralFolderPath, imageFilePath, _settingManager, _formInstanceService,  _reportDesignService);
                        outputpath = gn.Generate();
                        outputpath = gn.GenerateWord(outputpath);
                        if (isPDF)
                        {
                            colleteralFilePath = gn.GeneratePDF(outputpath, sbConfig[0].FormInstanceID, User.Identity.Name, 0);
                            outputpath = colleteralFilePath.PDF;
                        }
                    }

                    FileStream result = System.IO.File.OpenRead(outputpath); // (Server.MapPath(@"~\App_Data") + @"\" + datasource.ReportName + ".pdf");
                    byte[] dataBytes = new byte[result.Length];
                    result.Read(dataBytes, 0, (int)result.Length);
                    return File(dataBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, System.IO.Path.GetFileName(outputpath));
                }
            }
            return output;
        }

        public FileResult ViewReport1(int accountID, int folderID, string formInstanceID, int folderVersionID, string reportTemplateName, int reportTemplateID, string folderVersionEffDt, string description, int formDesignVersionID)
        {
            int TemplateReportVersionID = 0;
            string reportTemplateLocation = string.Empty;
            string status = string.Empty;
            ReportDataSource datasource = null;
            IEnumerable<string> formInstanceIDList = formInstanceID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, int> dataSource = _reportDesignService.GetReportTemplateFromReportID(reportTemplateID, folderVersionID, formInstanceIDList, ref TemplateReportVersionID, ref reportTemplateLocation, ref status, folderVersionEffDt);

            //List<ReportDocumentModel> data = _reportDesignService.GetDocumentsforReportGeneration(reportTemplateVersionID, accountID, 0, folderVersionID, formInstanceIDList);
            datasource = _reportDesignService.GetDataSourceForReport(dataSource, TemplateReportVersionID, reportTemplateName, reportTemplateLocation);
            string fileNamedoc = Server.MapPath(@"~\App_Data") + @"\" + datasource.ReportName + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + ".docx";
            string fileName = Server.MapPath(@"~\App_Data") + @"\" + datasource.ReportName + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + ".pdf";

            //FileStream output = System.IO.File.Create(fileName);
            //output.Close();

            FileStream result = System.IO.File.OpenRead(fileName); // (Server.MapPath(@"~\App_Data") + @"\" + datasource.ReportName + ".pdf");
            byte[] dataBytes = new byte[result.Length];
            result.Read(dataBytes, 0, (int)result.Length);
            _reportDesignService.SaveTemplateActivityLog(TemplateReportVersionID, description, User.Identity.Name);
            return File(dataBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);

            //  application/pdf         
        }

        public JsonResult GetAccountFolderFolderVersionDocuments(int reportTemplateID, int AccountID, int FolderID, int FolderVersionID)
        {
            IEnumerable<ReportDocumentModel> data = _reportDesignService.GetDocumentsforReportGeneration(reportTemplateID, AccountID, FolderID, FolderVersionID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAccountAndFolders(int reportTemplateID, int accountID, int folderID, int folderVersionID, string reportName, string planFamilyName, GridPagingRequest gridPagingRequest, bool shouldSelectFormInstance = false)
        {
            GridPagingResponse<ReportDocumentModel> result = _reportDesignService.GetAccountAndFolders(reportTemplateID, accountID, folderID, folderVersionID, reportName, planFamilyName, gridPagingRequest, shouldSelectFormInstance);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetDocumentsForReportGeneration(int ReportTemplateVersionID, int AccountID, int FolderID, int FolderVersionID)
        //{
        //    IEnumerable<ReportDocumentModel> data = _reportDesignService.GetDocumentsforReportGeneration(ReportTemplateVersionID, AccountID, FolderID, FolderVersionID);

        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetReportNames(GridPagingRequest gridPagingRequest)
        {
            int tenantid = 1;
            GridPagingResponse<ReportDesignViewModel> reportNames = _reportDesignService.GetReportNames(tenantid, gridPagingRequest);
            return Json(reportNames, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetComplianceValidationlog(int formInstanceId, int collateralQueueId)
        {


            var validations = _formInstanceService.GetComplianceValidationlog(formInstanceId, User.Identity.Name, collateralQueueId);
            return Json(validations, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetReportNamesForGeneration(string ReportLocation = "InMenu")
        {
            int tenantid = 1;
            IEnumerable<ReportDesignViewModel> reportNames = _reportDesignService.GetReportNamesForGeneration(tenantid, ReportLocation);
            return Json(reportNames, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRoleNames(int tenantId, int reportVersionId)
        {
            IEnumerable<RoleViewModel> roleNames = new List<RoleViewModel>();
            roleNames = _reportDesignService.GetRoleNames(tenantId, reportVersionId);
            return Json(roleNames, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddViewPermissionSet(List<RoleViewModel> sectionAccessRows)
        {
            ServiceResult result = new ServiceResult();
            result = _reportDesignService.AddViewPermissionSet(sectionAccessRows);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string GetTemplateNameById(int TemplateReportVersionID)
        {
            var result = _reportDesignService.GetTemplateNameById(TemplateReportVersionID);
            return result;//Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetParameters(int reportVersionID)
        {
            var result = _reportDesignService.GetParameters(reportVersionID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteCollateralDesign(int tenantId, int reportDesignId)
        {
            ServiceResult result = _reportDesignService.DeleteReportDesign(base.CurrentUserName, tenantId, reportDesignId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FinalizeReportVersion(int tenantId, int reportVersionId, string comments)
        {
            ServiceResult result = _reportDesignService.FinalizeReportVersion(tenantId, reportVersionId, comments);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult QueueCollateral(int accountID, string accountName, int folderID, string folderName, string formInstanceIDs, int folderVersionID, string folderVersionNumbers, string productIds, int reportTemplateID, string folderVersionEffDt, DateTime? runDate, string reportName)
        {
            IEnumerable<string> formInstanceIDList = formInstanceIDs.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<string> folderVersionNumberArr = folderVersionNumbers.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<string> productIdArr = productIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string username = User.Identity.Name;
            string collateralFolderPath = ConfigurationManager.AppSettings["CollateralFolderPath"];
            ServiceResult result = _reportDesignService.QueueCollateral(accountID, accountName, folderID, folderName, folderVersionID, formInstanceIDList, folderVersionNumberArr, productIdArr, reportTemplateID, folderVersionEffDt, username, runDate, collateralFolderPath, reportName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult QueueSBDesignCollateral(string accountIDs, string accountNames, string folderIDs, string folderNames, string formInstanceIDs, string folderVersionIDs, string folderVersionNumbers, string productIds, string formDesignIds, string formDesignVersionIds, string folderVersionEffDts, int reportTemplateID, string reportName)
        {
            string username = User.Identity.Name;
            string collateralFolderPath = ConfigurationManager.AppSettings["CollateralFolderPath"];
            ServiceResult result = _reportDesignService.QueueSBDesignCollateral(accountIDs, accountNames, folderIDs, folderNames, formInstanceIDs, folderVersionIDs, folderVersionNumbers, productIds, formDesignIds, formDesignVersionIds, reportTemplateID, folderVersionEffDts, reportName, username, collateralFolderPath);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserActivity()
        {
            return View("UserActivity");
        }
        public ActionResult CollateralImages()
        {
            return View("CollateralImages");
        }

        public JsonResult GetReportUserActivity(GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<ReportUserActivityViewModel> result = _reportDesignService.GetReportUserActivity(gridPagingRequest);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckRolePermission(int reportTemplateID, string folderVersionEffDt)
        {
            string username = User.Identity.Name;
            bool hasAccess = _reportDesignService.IsReportTemplateVersionAccessiable(reportTemplateID, folderVersionEffDt, username);
            return Json(hasAccess, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsTemplateUploaded(int reportTemplateVersionId)
        {
            bool isTemplateUploaded = _reportDesignService.IsTemplateUploaded(reportTemplateVersionId);
            return Json(isTemplateUploaded, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewQueuedCollaterals()
        {
            return View("ViewQueuedCollaterals");
        }

        public ActionResult ViewUploadedCollaterals()
        {
            return View("ViewUploadedCollaterals");
        }

        public JsonResult GetUploadedCollateralLogsList(GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<UploadReportModel> result = _reportDesignService.GetCollateralProcessUploadList(gridPagingRequest);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult DownloadManualFile(int id, string fileFormat)
        {
            var filePath = _reportDesignService.GetManualUploadedFilePath(id, fileFormat);

            return File(filePath.FileContent, System.Net.Mime.MediaTypeNames.Application.Octet, filePath.FileName);

        }
        public JsonResult GetQueuedCollateralsList(GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<QueuedReportModel> result = _reportDesignService.GetQueuedCollateralsList(gridPagingRequest);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckIfFileExistsToDownload(int processQueue1Up, string fileFormat)
        {
            bool doesFileExist = false;
            string filePath = _reportDesignService.GetFilePath(processQueue1Up, fileFormat);
            var filefolderPath = filePath.Split('|')[0];
            if (System.IO.File.Exists(filefolderPath))
                doesFileExist = true;
            return Json(doesFileExist, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult DownloadCollateralFile(int processQueue1Up, string reportName, int templateReportVersionID, string fileFormat, string docType)
        {
            string filePath = _reportDesignService.GetFilePath(processQueue1Up, fileFormat);
            var fileAndFolderPath = filePath.Split('|');
            FileStream result = System.IO.File.OpenRead(fileAndFolderPath[0]);

            byte[] dataBytes = new byte[result.Length];
            result.Read(dataBytes, 0, (int)result.Length);
            string description = "File has been downloaded against template " + reportName;
            string username = User.Identity.Name;
            _reportDesignService.SaveTemplateActivityLog(templateReportVersionID, description, username);
            string documentName = "CollateralFile";
            if (fileAndFolderPath != null && fileAndFolderPath.Length > 1 && !String.IsNullOrEmpty(fileAndFolderPath[1]))
            {
                documentName = fileAndFolderPath[1];
            }
            if (fileFormat == "word")
            {
                return File(dataBytes, System.Net.Mime.MediaTypeNames.Application.Octet, documentName + ".Docx");
            }
            else
            {
                return File(dataBytes, System.Net.Mime.MediaTypeNames.Application.Octet, documentName + ".Pdf");
            }
        }

        public JsonResult IsReportAlreadyGenerated(int formInstanceId, int reportTemplateId)
        {
            QueuedReportModel processedProduct = _reportDesignService.IsReportAlreadyGenerated(formInstanceId, reportTemplateId);
            return Json(processedProduct, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCollateralTemplatesForReportGeneration(int formInstanceId, string reportLocation = "InFolder")
        {
            IEnumerable<ReportDesignViewModel> lst = _reportDesignService.GetCollateralTemplatesForProduct(formInstanceId, reportLocation);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ViewCollateralsAtFolder(int formInstanceId, GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<QueuedReportModel> result = _reportDesignService.ViewCollateralsAtFolder(formInstanceId, gridPagingRequest);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTemplateName(int templateReportVersionID)
        {
            string fileName = GetTemplateNameById(templateReportVersionID);
            return Json(fileName, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckIfCollateralIsOfSBDesignType(int templateReportID)
        {
            string result = "false";
            bool isSBDesignProduct = _reportDesignService.CheckIfCollateralIsOfSBDesignType(templateReportID);
            if (isSBDesignProduct)
            {
                result = "SB";
            }

            if (isSBDesignProduct == false)
            {
                isSBDesignProduct = _reportDesignService.CheckIfCollateralsOfEOCDesignType(templateReportID);

                if (isSBDesignProduct)
                {
                    result = "EOC";
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPlanFamilyDropdownList(int templateReportID, string reportName)
        {
            string[] arr = { };
            List<string> updatedArr = new List<string>();
            bool isEOCDesignProduct = _reportDesignService.CheckIfCollateralsOfEOCDesignType(templateReportID);
            if (isEOCDesignProduct)
            {
                arr = _reportDesignService.GetPlanFamily(reportName);
                foreach (string item in arr)
                {
                    if (item[0] != '_') {
                        updatedArr.Add(item);
                    }
                }
            }
            return Json(updatedArr, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSettingInfo(string name)
        {

            var SettingModel = _settingManager.GetSettingValue(name);

            return Json(SettingModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCollateralImageList(GridPagingRequest gridPagingRequest)
        {
            List<CollateralImageView> list = new List<CollateralImageView>();
            // var list = new List<CollateralImageView>();




            GridPagingResponse<CollateralImageView> result = _reportDesignService.GetCollateralImages(gridPagingRequest);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SaveImage(string imageDesp,string imagName,  HttpPostedFileBase file)
        {
            string path = string.Format(@"{0}\Content\CollateralImages\{1}", HostingEnvironment.ApplicationPhysicalPath, file.FileName);
            var filname = string.Format("../Content/CollateralImages/{0}", file.FileName);
            var result = _reportDesignService.SaveCollateralImages(imageDesp, filname, file.FileName, imagName);
            if (result.Result == ServiceResultStatus.Success)
            {
                file.SaveAs(path);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MakeCompliance(HttpPostedFileBase file)
        {
            var fileName = string.Format("{0}{1}", DateTime.Now.ToString("yy-MM-dd-THH-mm-ss"), file.FileName);
            string outputpath = string.Format(@"{0}\Content\CollateralImages\", HostingEnvironment.ApplicationPhysicalPath);
            string path = string.Format(@"{0}{1}", outputpath, fileName);
            var originalNewFileNamePDf = string.Format("{0}.pdf", fileName);
            file.SaveAs(path);

            string licpath = string.Format(@"{0}\bin\", HostingEnvironment.ApplicationPhysicalPath);
            GenerateCollateral gc = new GenerateCollateral(path, outputpath, _settingManager, _reportDesignService);

            var result = gc.GeneratePDF(path, licpath);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MakeComplianceWithData(FormCollection formCollection, HttpPostedFileBase file, HttpPostedFileBase file1)
        {
            UploadReportViewModel uploadReportViewModel = new UploadReportViewModel();
            uploadReportViewModel.FolderName = formCollection["folderName"];
            uploadReportViewModel.FolderVersionNumber = formCollection["folderVersionnumber"];
            uploadReportViewModel.ProcessedDate = DateTime.Now;
            Int32 accountID = 0;
            Int32.TryParse(formCollection["accountID"], out accountID);
            uploadReportViewModel.AccountID = accountID;
            uploadReportViewModel.AccountName = formCollection["accountName"];
            uploadReportViewModel.FolderVersionNumber = formCollection["versionNumber"];

            uploadReportViewModel.CollateralName = formCollection["collateralName"];
            Int32 instanceID = 0;
            Int32.TryParse(formCollection["FormInstanceID"], out instanceID);
            uploadReportViewModel.FormInstanceID = instanceID;
            uploadReportViewModel.FormInstanceName = formCollection["FormName"];
            uploadReportViewModel.ProductID = uploadReportViewModel.FormInstanceName;

            Int32 designId = 0;
            Int32.TryParse(formCollection["FormDesignID"], out designId);
            uploadReportViewModel.FormDesignID = designId;

            Int32 designversionId = 0;
            Int32.TryParse(formCollection["FormDesignVersionID"], out designversionId);
            uploadReportViewModel.FormDesignVersionID = designversionId;

            Int32 folderId = 0;
            Int32.TryParse(formCollection["folderID"], out folderId);
            uploadReportViewModel.FolderID = folderId;

            Int32 folderVersionId = 0;
            Int32.TryParse(formCollection["folderVersionID"], out folderVersionId);
            uploadReportViewModel.FolderVersionId = folderVersionId;

            uploadReportViewModel.CreatedBy = CurrentUserName;
            string alreadyConverted508 = formCollection["alreadyConverted508"];
            uploadReportViewModel.AlreadyConverted508 = false;
            if (!string.IsNullOrEmpty(alreadyConverted508))
            {
                if (alreadyConverted508.Contains("true"))
                {
                    uploadReportViewModel.AlreadyConverted508 = true;
                }
            }


            var fileName = string.Format("{0}{1}", DateTime.Now.ToString("yy-MM-dd-THH-mm-ss"), file.FileName);
            string outputpath = string.Format(@"{0}\Content\CollateralImages\", HostingEnvironment.ApplicationPhysicalPath);
            string path = string.Format(@"{0}{1}", outputpath, fileName);
            var originalNewFileNamePDf = string.Format("{0}.pdf", fileName);
            file.SaveAs(path);

            string licpath = string.Format(@"{0}\bin\", HostingEnvironment.ApplicationPhysicalPath);
            string rootpath = string.Format(@"{0}\", HostingEnvironment.ApplicationPhysicalPath);
            GenerateCollateral gc = new GenerateCollateral(path, outputpath, _settingManager, _reportDesignService);

            byte[] msWord = null;
            byte[] pdfx = null;
            //converting file to byte array to save in DB
            using (MemoryStream target = new MemoryStream())
            {
                file.InputStream.CopyTo(target);
                msWord = target.ToArray();
            }
            using (MemoryStream target = new MemoryStream())
            {
                file1.InputStream.CopyTo(target);
                pdfx = target.ToArray();
            }
            var result = gc.GeneratePDFInDatasBase(path, licpath, rootpath, msWord, pdfx, uploadReportViewModel);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        FileResult GetUploadedCollateralFile(int ID, int type)
        {
            //Code to pull file data
            return null;
        }


        [HttpPost]
        public ActionResult DeleteImage(int id)
        {
            var images = _reportDesignService.GetCollateralImages(id);
            var result = _reportDesignService.DeleteCollateralImages(id);
            if (result.Result == ServiceResultStatus.Success)
            {
                if (images != null)
                {
                    if (images.Count > 0)
                    {
                        string path = string.Format(@"{0}\Content\CollateralImages\{1}", HostingEnvironment.ApplicationPhysicalPath, images[0].FileName);

                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public ActionResult UploadPrintXPDFFile(int processQueue1Up, string fileFormat)
        {
            string CollatralFilePathSave = string.Empty;
            if (Request.Files.Count > 0)
            {
                string filePath = _reportDesignService.GetFilePath(processQueue1Up, "pdfx");
                string filefolderPath = filePath.Split('|')[0];
                string existFileName = filefolderPath.Split('/')[1];
                if (existFileName.Contains("_"))
                {
                    existFileName = existFileName.Split('.')[0];
                    var increment = Convert.ToInt32(existFileName.Split('_')[1]);
                    increment = increment + 1;
                    existFileName = existFileName.Split('_')[0] + "_" + increment + ".pdf";
                }
                else
                {
                    existFileName = existFileName.Split('.')[0] + "_1";
                    existFileName = existFileName + ".pdf";
                }

                filefolderPath = filefolderPath.Split('/')[0];
                List<string> FileNameList = SaveFiles(Request, filefolderPath, existFileName);
                filefolderPath = string.Concat(filefolderPath, "/");
                string CollatralFilePath = string.Concat(filefolderPath, FileNameList[0]);
                CollatralFilePathSave = _reportDesignService.SaveCollatralPrintXFormatedPDF(processQueue1Up, CollatralFilePath);
                printxLogTableEntry(processQueue1Up, CollatralFilePathSave);
            }

            return Json("true", JsonRequestBehavior.AllowGet);
        }

        public void printxLogTableEntry(int processQueue1Up, string CollatralFilePathSave)
        {
            FormInstanceComplianceValidationlog obj = new FormInstanceComplianceValidationlog();
            obj.FormInstanceID = Convert.ToInt32(CollatralFilePathSave);
            obj.Error = "Print x";
            obj.AddedDate = DateTime.UtcNow;
            obj.CollateralProcessQueue1Up = processQueue1Up;
            obj.AddedBy = CurrentUserName;
            obj.ComplianceType = "pdfx";
            obj.ValidationType = "Resolved";
            _formInstanceService.UpdateFormInstanceComplianceValidationlogForPrintX(Convert.ToInt32(CollatralFilePathSave), obj, processQueue1Up);

        }
        private List<string> SaveFiles(HttpRequestBase Request, string filefolderPath, string existFileName)
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
                UniqueName = existFileName;
                string fname = Path.Combine(filefolderPath, UniqueName);
                file.SaveAs(fname);
                FileNameList.Add(UniqueName);
                // Get the complete folder path and store the file inside it.  
            }
            return FileNameList;
        }
    }
}

