using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.MLImport;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.viewmodels;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.ruleinterpretercollateral;
using tmg.equinox.web.Framework;
using tmg.equinox.web.masterlist;

namespace tmg.equinox.web.Controllers
{
    public class MasterListController : AuthenticatedController
    {
        private IMasterListService service;
        private IFolderVersionServices _folderVersionServices;
        private IFormInstanceDataServices _formInstanceDataServices;
        private IMLImportHelperService _mlImportHelperService;
        private IBaselineMasterListService _baselineMasterListService;
        private IMasterListCascadeService _masterListCascadeService;
        private static readonly ILog _logger = LogProvider.For<MasterListController>();

        public MasterListController(IMasterListService service, IFolderVersionServices folderVersionServices, IFormInstanceDataServices formInstanceDataServices, IMLImportHelperService mlImportHelperService, IBaselineMasterListService baselineMasterListService, IMasterListCascadeService masterListCascadeService)
        {
            this.service = service;
            this._folderVersionServices = folderVersionServices;
            this._formInstanceDataServices = formInstanceDataServices;
            this._mlImportHelperService = mlImportHelperService;
            this._baselineMasterListService = baselineMasterListService;
            _masterListCascadeService = masterListCascadeService;

        }

        public JsonResult LibraryRegexList(int tenantId)
        {
            IEnumerable<KeyValue> regexList = service.GetLibraryRegexes(tenantId);
            if (regexList == null)
            {
                regexList = new List<KeyValue>();
            }
            return Json(regexList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApprovalStatusTypeList(int tenantId)
        {
            IEnumerable<KeyValue> approvalStatusList = service.GetApprovalStatusTypeList(tenantId);
            if (approvalStatusList == null)
            {
                approvalStatusList = new List<KeyValue>();
            }
            return Json(approvalStatusList, JsonRequestBehavior.AllowGet);
        }

        //[AllowAnonymous]
        public JsonResult GetOwnerList(int tenantId)
        {
            IEnumerable<KeyValue> ownerList = service.GetOwnerList(tenantId);
            if (ownerList == null)
            {
                ownerList = new List<KeyValue>();
            }
            return Json(ownerList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFormInstanceData(int formInstanceID)
        {
            FormInstanceViewModel formInstanceViewModel = new FormInstanceViewModel();
            formInstanceViewModel = _folderVersionServices.GetFormInstance(1, formInstanceID);
            return Json(formInstanceViewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CascadeMasterList()
        {
            ViewBag.RoleId = RoleID;
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            return View();
        }

        public JsonResult GetMasterListCascadeBatch()
        {
            List<MasterListCascadeBatchViewModel> masterListCascadeBatchViewModel = new List<MasterListCascadeBatchViewModel>();
            masterListCascadeBatchViewModel = _masterListCascadeService.GetMasterListCascadeBatch();
            return Json(masterListCascadeBatchViewModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMasterListCascadeBatchDetail(int masterListCascadeBatchID)
        {
            List<MasterListCascadeBatchDetailViewModel> masterListCascadeBatchDetailViewModel = new List<MasterListCascadeBatchDetailViewModel>();
            masterListCascadeBatchDetailViewModel = _masterListCascadeService.GetMasterListCascadeBatchDetail(masterListCascadeBatchID);
            return Json(masterListCascadeBatchDetailViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImportMasterList()
        {
            string FileName, UniqueName = Guid.NewGuid().ToString();
            string IMPORTFILEPATH = ConfigurationManager.AppSettings["MLImportFiles"].ToString();
            string Status = "Processing", Message = "", jsonData = "";
            int FormInstanceId = 0;
            MasterListImportViewModel model = new MasterListImportViewModel();
            try
            {
                //  Get all files from Request object  
                HttpFileCollectionBase files = Request.Files;
                List<string> FileNamList = new List<string>();

                HttpPostedFileBase file = files[0];
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
                string FileUniqueName = SplitStr[0] + UniqueName + "." + SplitStr[1];

                string MLFileName = string.Empty;
                if (FileNamList.Count() > 0)
                {
                    MLFileName = Path.GetFileName(FileNamList[0].ToString());
                }

                string filePath = Path.Combine(IMPORTFILEPATH, FileUniqueName);
                file.SaveAs(filePath);

                _logger.Debug("ml File got uploaded");

                ProcessMLExcelFile processMLExcelFile = new ProcessMLExcelFile();
                //DataTable dt = processMLExcelFile.ConvertExceltoDatatable(filePath, FileName);

                DataTable dt = processMLExcelFile.ConvertExceltoDatatableUsingEPPlus(filePath);

                _logger.Debug("ml datatable created");

                //Add code to save into database
                string comment = Request.Params["MLcomment"].ToString();
                string MLFolderName = Request.Params["MLFolderName"].ToString();
                MLFolderName = MLFolderName.Replace("\r\n", string.Empty);
                string MLSectionName = Request.Params["MLSectionName"].ToString();
                MLSectionName = MLSectionName.Replace("\r\n", string.Empty);
                MLSectionName = MLSectionName.Trim();
                string MLFormInstanceId = Request.Params["MLFormInstanceId"].ToString();
                int IMLFormInstanceId = Convert.ToInt32(MLFormInstanceId);
                //wellcare
                //Check effective date
                FolderVersionViewModel mlViewModel = _folderVersionServices.GetCurrentFolderVersionML(MLFolderName);
                DateTime effectiveDate = mlViewModel.EffectiveDate;

                if (MLSectionName == "Part D Benefit Grid" && effectiveDate.Year == 2019)
                {
                    //MLSectionName = "Part D Benefit Grid 2019";
                }

                bool IsValid = _mlImportHelperService.ProcessMasterListImportTemplateValidation(MLSectionName, dt, IMPORTFILEPATH);
                if (IsValid)
                {
                    //Get existing Json for MasterList

                    _logger.Debug("ml before json processing");
                    switch (MLSectionName)
                    {
                        case "Plan and LIS Premium":
                            jsonData = _mlImportHelperService.PremiumMLJsonFormatting(dt);
                            break;
                        case "Formulary Informations":
                            jsonData = _mlImportHelperService.FormularyInfoMLJsonFormatting(dt);
                            break;
                        case "Benchmark Informations":
                            jsonData = _mlImportHelperService.BenchmarkInfoMLJsonFormatting(dt);
                            break;
                        case "Part D Benefit Grid":
                        case "Part D Benefit Grid 2019":
                            jsonData = _mlImportHelperService.PrescriptionMLJsonFormatting(dt, effectiveDate);
                            break;
                        case "FIPS":
                            jsonData = _mlImportHelperService.FIPSMLJsonFormatting(dt);
                            break;
                        default:
                            Message = "Incorrect template. Please try again by using a correct template.";
                            break;
                    }
                    _logger.Debug("ml after json processing");


                    //Update Full MasterList Data
                    //BASELINING CODE START
                    _logger.Debug("ml before base line");
                    //  FolderVersionViewModel mlViewModel = _folderVersionServices.GetCurrentFolderVersionML(MLFolderName);
                    var serviceResult = _baselineMasterListService.CreateBaseLineFolderBeforeUpdate(mlViewModel, comment, CurrentUserName, false);
                    string NewFolderVersionId = (serviceResult.Items).FirstOrDefault().Messages[0];
                    int INewFolderVersionId = 0;
                    if (NewFolderVersionId != null)
                    {
                        INewFolderVersionId = Convert.ToInt32(NewFolderVersionId);
                    }
                    if (mlViewModel.FolderVersionStateID == 1)
                    {
                        AddToActivityLog(IMLFormInstanceId, mlViewModel.FolderId, "Folder Version Update", "Baseline Completed");
                    }
                    FormInstanceId = service.GetFormInstanceIds(INewFolderVersionId).FirstOrDefault();
                    _logger.Debug("ml after base line");
                    //END BASELINING CODE
                    _folderVersionServices.UpdateFormInstanceData(1, FormInstanceId, jsonData);
                    Status = "Completed";

                    //Insert in Masterlist import table
                    service.SaveMasterListImportData(FileUniqueName, IMPORTFILEPATH, FormInstanceId, comment, CurrentUserName, DateTime.UtcNow, Status);
                    _logger.Debug("ml after Insert in Masterlist import table");
                    AddToActivityLog(FormInstanceId, mlViewModel.FolderId, "Folder Version Update", "Import Completed");
                    Message = "Success";

                    model.FolderVersionId = INewFolderVersionId;
                    model.FolderId = mlViewModel.FolderId;
                    model.FolderType = mlViewModel.FolderType;

                    model.Message = Message;
                    model.FormInstanceId = FormInstanceId;
                    model.ViewName = MLFolderName;
                    model.SectionName = MLSectionName;

                    //return Json({ model: model,  isValid: true }, JsonRequestBehavior.AllowGet);
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model.Message = "Template is not valid for Master List : -" + MLSectionName;
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("ml after json processing" + ex.Message);
                Message = "Master List import failed.";
                model.Message = Message + " " + ex.Message;
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        private void AddToActivityLog(int formInstanceId, int folderId, string elementPath, string description)
        {
            try
            {
                FormInstanceViewModel fiModel = _folderVersionServices.GetFormInstance(1, formInstanceId);
                List<ActivityLogModel> activityModels = new List<ActivityLogModel>();
                ActivityLogModel activityModel = new ActivityLogModel();
                activityModel.ElementPath = elementPath;
                activityModel.Description = description + " - Version " + fiModel.FolderVersionNumber;
                activityModel.Field = "";
                activityModel.FolderVersionName = "";
                activityModel.FormInstanceID = 0;
                activityModel.IsNewRecord = true;
                activityModel.RowNum = "";
                activityModel.SubSectionName = "";
                activityModel.UpdatedBy = CurrentUserName;
                activityModel.UpdatedLast = DateTime.Now;
                activityModels.Add(activityModel);
                _folderVersionServices.SaveFormInstanceAvtivitylogData(1, formInstanceId, folderId, fiModel.FolderVersionID, fiModel.FormDesignID, fiModel.FormDesignVersionID, activityModels);
            }
            catch (Exception ex)
            {

            }

        }
        public JsonResult GetDesignAliases(int tenantId, DateTime effectiveDate)
        {
            //get folderVersionId / formInstanceId
            int formDesignVersionId = service.GetAliasMasterListForEffectiveDate(tenantId, effectiveDate);
            JObject data = service.GetAliasMasterListDataForDesignVersion(tenantId, formDesignVersionId);
            AliasListGenerator gen = new AliasListGenerator(data);
            List<MasterListAliasViewModel> aliases = gen.GetAliases();
            return Json(aliases, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetQueryBuilderRules(int tenantId, string key)
        {
            IReportExpressionResolver resolver = new ReportExpressionResolver();
            ExpressionTemplate template = resolver.GetExpressionTemplate(key);
            string logicalOp = "";
            switch (template.LogicalOperator)
            {
                case "&":
                    logicalOp = "AND";
                    break;
                case "|":
                    logicalOp = "OR";
                    break;
                default:
                    logicalOp = "AND";
                    break;

            }
            JObject result = JObject.Parse("{'condition': '" + logicalOp + "' ,rules: []}");
            JArray rulesArray = (JArray)result.SelectToken("rules");

            if (!key.StartsWith("STATIC", StringComparison.OrdinalIgnoreCase) && !key.StartsWith("CUSTOM", StringComparison.OrdinalIgnoreCase) && !key.StartsWith("OPTIONAL", StringComparison.OrdinalIgnoreCase))
            {
                if (template != null)
                {
                    foreach (DocumentAlias alias in template.Variables)
                    {
                        JToken rule = JToken.Parse("{ 'id': '', 'operator' : '','value' : ''}");
                        rule["id"] = alias.DocumentDesignName + "[" + alias.Alias + "]";
                        string op = "";
                        switch (alias.Operator)
                        {
                            case "=":
                                op = "ebs_equals";
                                break;
                            case "!=":
                                op = "ebs_ne";
                                break;
                            case ">":
                                op = "ebs_gt";
                                break;
                            case ">=":
                                op = "ebs_gte";
                                break;
                            case "<":
                                op = "ebs_lt";
                                break;
                            case "<=":
                                op = "ebs_lte";
                                break;
                            case "₳":
                                op = "ebs_all";
                                break;
                            case "Ɐ":
                                op = "ebs_any";
                                break;
                            case "!Ɐ":
                                op = "ebs_notany";
                                break;
                            case "^":
                                op = "ebs_contains";
                                break;
                        }
                        rule["operator"] = op;
                        if (alias.ValueType == ruleinterpretercollateral.ValueType.Single)
                        {
                            rule["value"] = alias.Value;
                        }
                        else
                        {

                            rule["value"] = GetArray(alias.Values);
                        }
                        rulesArray.Add(rule);
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetKey(string key)
        {
            string keyResult = "";
            JObject keyData = JObject.Parse(key);
            JToken rules = keyData.SelectToken("rules");
            string condition = keyData["condition"].ToString();
            switch (condition)
            {
                case "AND":
                    condition = "&";
                    break;
                case "OR":
                    condition = "|";
                    break;
            }
            string op = "";
            if (rules != null)
            {

                JArray ruleArray = (JArray)rules;
                int ruleCount = ruleArray.Count;
                int idx = 0;
                foreach (var rule in rules)
                {
                    op = rule["operator"].ToString();
                    switch (op)
                    {
                        case "ebs_equals":
                            op = "=";
                            break;
                        case "ebs_ne":
                            op = "!=";
                            break;
                        case "ebs_gt":
                            op = ">";
                            break;
                        case "ebs_gte":
                            op = ">=";
                            break;
                        case "ebs_lt":
                            op = "<";
                            break;
                        case "ebs_lte":
                            op = "<=";
                            break;
                        case "ebs_all":
                            op = "₳";
                            break;
                        case "ebs_any":
                            op = "Ɐ";
                            break;
                        case "ebs_notany":
                            op = "!Ɐ";
                            break;
                        case "ebs_contains":
                            op = "^";
                            break;

                    }

                    keyResult = keyResult + rule["id"].ToString() + op + rule["value"].ToString();
                    if (idx < ruleCount - 1)
                    {
                        keyResult = keyResult + condition;
                    }
                    idx++;
                }
            }
            return Json(keyResult, JsonRequestBehavior.AllowGet);
        }

        private JArray GetArray(string[] values)
        {
            JArray arr = new JArray();
            if (values != null)
            {
                foreach (var val in values)
                {
                    arr.Add(val);
                }
            }
            return arr;
        }
    }
}