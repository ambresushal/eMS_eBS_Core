using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormContent;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.Framework;
using tmg.equinox.web.Framework.Caching;
using tmg.equinox.web.FormDesignManager.ConfigView;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpreter.rulecompiler;
using tmg.equinox.web.RuleEngine.RuleDescription;
using System.IO;
using tmg.equinox.web.FormInstance;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.ruleinterpreter.RuleCompiler;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.sourcehandler;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.FormInstanceProcessor.ExpressionBuilder;
using tmg.equinox.web.FormInstanceProcessor;
using tmg.equinox.applicationservices.viewmodels;

namespace tmg.equinox.web.Controllers
{
    public class UIElementController : AuthenticatedController
    {
        private IUIElementService service;
        private IDataSourceService dataSourceservice;
        private IFormDesignService _fdService;

        private IConsumerAccountService _accountService { get; set; }

        private IFolderVersionServices _folderVersionServices;
        private IFormInstanceDataServices _formInstanceDataServices;
        private IMasterListService _masterListService;
        private IFormInstanceService _formInstanceService;
        public UIElementController(IUIElementService service, IDataSourceService dsService, IFormDesignService fdService, IConsumerAccountService accountService,
             IFolderVersionServices folderVersionServices, IFormInstanceDataServices formInstanceDataServices, IMasterListService masterListService, IFormInstanceService formInstanceService)
        {
            this.service = service;
            this.dataSourceservice = dsService;
            this._fdService = fdService;
            this._accountService = accountService;
            this._folderVersionServices = folderVersionServices;
            this._formInstanceDataServices = formInstanceDataServices;
            this._masterListService = masterListService;
            this._formInstanceService = formInstanceService;
        }

        /// <summary>
        /// This method Returns the List of UI Elements for Form Design Version
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <returns>List of UI Elements for Form Design Version</returns>
        public JsonResult FormDesignVersionUIElementList(int tenantId, int formDesignVersionId)
        {
            IEnumerable<UIElementRowModel> uiElementList = service.GetUIElementListForFormDesignVersion(tenantId, formDesignVersionId);
            if (uiElementList == null)
            {
                uiElementList = new List<UIElementRowModel>();
            }
            return Json(uiElementList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUIElementListForAliases(int tenantId, string formDesignName, DateTime effectiveDate)
        {
            List<UIElementSummaryModel> uiElementList = new List<UIElementSummaryModel>();
            int formDesignVersionID = _fdService.GetLatestFormDesignVersionID(formDesignName, effectiveDate);
            List<UIElementSummaryModel> elements = service.GetUIElementListMod(tenantId, formDesignVersionID);
            if (elements != null)
            {
                var repeaterList = from uiElem in elements
                                   where uiElem.ElementType == "Repeater"
                                   select uiElem;
                var elemList = from uiElem in elements where (uiElem.ElementType != "Section" && uiElem.ElementType != "Repeater") select uiElem;
                if (elemList != null)
                {
                    List<int> repeaterIds = new List<int>();
                    if (repeaterList != null)
                    {
                        repeaterIds = repeaterList.Select(a => a.UIElementID).ToList();
                    }
                    uiElementList = elemList.Where(a => a.ParentUIElementId.HasValue && !repeaterIds.Contains(a.ParentUIElementId.Value)).ToList();
                }
            }
            return Json(uiElementList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocumentViewList(int formDesignId)
        {
            List<DocumentViewListViewModel> viewList = this.service.GetDocumentViewListForExpressionRules(formDesignId);
            return Json(viewList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUIElementListForExpressionRule(int tenantId, int formDesignId, DateTime effecttiveDate, string term)
        {
            int formDesignVersionId = service.GetFormDesignVersionID(formDesignId, effecttiveDate);
            IEnumerable<ElementListViewModel> uiElementList = service.GetUIElementListForExpressionRuleBuilder(tenantId, formDesignVersionId, term);
            return Json(uiElementList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUIElementFullPathForExpressionRule(int tenantId, int uiElementId, int formDesignId, DateTime effecttiveDate)
        {
            int formDesignVersionId = service.GetFormDesignVersionID(formDesignId, effecttiveDate);
            string fullName = service.GetUIElementFullPath(uiElementId, formDesignVersionId);
            return Json(fullName, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateExpressionRule(int tenantId, int formDesignID, int formDesignVersionId, int ruleID, GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<DocumentViewModel> documentsList = null;
            if (formDesignID > 0)
            {
                if (tmg.equinox.config.Config.GetApplicationName().ToLower() == "ebenefitsync")
                    documentsList = this._accountService.GetDocumentsList(tenantId, formDesignID, gridPagingRequest, "eBS");
                else
                    documentsList = this._accountService.GetDocumentsList(tenantId, formDesignID, gridPagingRequest);
            }
            else
            {
                if (tmg.equinox.config.Config.GetApplicationName().ToLower() == "ebenefitsync")
                    documentsList = this._accountService.GetDocumentsList(tenantId, formDesignID, gridPagingRequest, "eBS");
                else
                    documentsList = this._accountService.GetDocumentsList(tenantId, gridPagingRequest);
            }

            string result = null;
            DocumentViewModel document = new DocumentViewModel();
            if (documentsList.rows.Count() > 0)
            {
                document = documentsList.rows[0];
                FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(tenantId, 1, _formInstanceDataServices, "superuser", _folderVersionServices);
                SourceHandlerDBManager sourceDBManager = new SourceHandlerDBManager(tenantId, _folderVersionServices, formInstanceDataManager, _fdService, _masterListService);
                FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, formDesignVersionId, _fdService);
                FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(true);

                CurrentRequestContext requestContext = new CurrentRequestContext();
                ExpressionRuleTreeProcessor processor = new ExpressionRuleTreeProcessor(document.FormInstanceID, service, document.FolderVersionID ?? default(int),
                sourceDBManager, _fdService, formInstanceDataManager, detail, _formInstanceService, 1, requestContext);
                try
                {
                    JToken resultData = processor.ProcessRuleTest(ruleID);
                    result = "Rule has been validated successfully.";
                }
                catch (System.Exception ex)
                {
                    result = ex.Message;
                }
            }
            else
            {
                result = "Document doesn't found to validate the expression rule.";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUIElementList(int tenantId, int formDesignVersionId)
        {
            var uiElementList = service.GetUIElementList(tenantId, formDesignVersionId);
            if (uiElementList == null)
            {
                uiElementList = new ConfigViewRowModel();
            }
            return Json(uiElementList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUIElementFullPath(int tenantId, int uiElementId, string formDesignName, DateTime effectiveDate)
        {
            int formDesignVersionID = _fdService.GetLatestFormDesignVersionID(formDesignName, effectiveDate);
            string fullName = service.GetUIElementFullPath(uiElementId, formDesignVersionID);
            return Json(fullName, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetDropDownForElement(int tenantId, int uiElementId, string formDesignName, DateTime effectiveDate)
        {
            int formDesignVersionID = _fdService.GetLatestFormDesignVersionID(formDesignName, effectiveDate);
            return DropDown(tenantId, formDesignVersionID, uiElementId);
        }
        [ValidateInput(false)]
        public JsonResult SaveFormDesignVersion(int tenantId, int formDesignVersionId, int formDesignId, string prevElements, string newElements, string comments, string extendedProperties)
        {
            ServiceResult result = new ServiceResult();
            ConfigViewHelper helper = new ConfigViewHelper();
            List<string> keys = helper.GetKeys();

            ConfigViewValidator validator = new ConfigViewValidator(newElements);
            List<int> errorRows = validator.Validate();
            if (errorRows.Count == 0)
            {
                List<JToken> source = JToken.Parse(prevElements).ToList();
                List<JToken> target = JToken.Parse(newElements).ToList();
                Dictionary<string, List<JToken>> operations = helper.GetCompareResult(source, target, keys);

                //Delete Elements
                var elements = operations["DELETED"];
                elements.ForEach(e =>
                {
                    DeleteElementByType(tenantId, formDesignVersionId, Convert.ToString(e["ElementType"]), Convert.ToInt32(e["UIElementID"]));
                });

                //Add New Elements
                elements = operations["ADDED"];
                if (elements.Count > 0)
                {
                    List<JToken> savedElements = new List<JToken>();
                    //Add container elements, if any
                    var containerElements = elements.Where(s => (string)s["IsContainer"] == "True").ToList();
                    savedElements.AddRange(containerElements);
                    foreach (var cntr in containerElements)
                    {
                        var model = helper.GetUIElementAddModel(tenantId, formDesignId, formDesignVersionId, cntr);
                        result = AddElementByType(model);
                        int containerElementId = Convert.ToInt32(result.Items.FirstOrDefault().Messages[0]);
                        var childElements = elements.Where(s => (string)s["ParentUIElementId"] == (string)cntr["TempUIElementId"]).ToList();
                        savedElements.AddRange(childElements);
                        foreach (var item in childElements)
                        {
                            item["ParentUIElementId"] = containerElementId;
                            if (Convert.ToString(item["IsContainer"]) != "True")
                            {
                                model = helper.GetUIElementAddModel(tenantId, formDesignId, formDesignVersionId, item);
                                result = AddElementByType(model);
                            }
                        }
                    }

                    var pendingElements = elements.Except(savedElements).ToList();
                    foreach (var row in pendingElements)
                    {
                        var model = helper.GetUIElementAddModel(tenantId, formDesignId, formDesignVersionId, row);
                        result = AddElementByType(model);
                    }
                }

                elements = operations["UPDATED"];
                elements.ForEach(e =>
                {
                    var model = helper.GetUIElementUpdateModel(tenantId, formDesignId, formDesignVersionId, e);
                    UpdateElementByType(model);
                });
                result = service.UpdateCommentsForUIElement(formDesignId, formDesignVersionId, comments, CurrentUserName, extendedProperties);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(errorRows, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult SaveConfigRulesTesterData(int tenantId, string designRulesTesterData)
        {
            ServiceResult result = service.SaveConfigurationRuleTesterData(tenantId, CurrentUserName, designRulesTesterData);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getFormDesignVersionUIElementsTestData(int tenantId, int formDesignVersionId, int elementId)
        {
            List<ConfigRulesTesterData> uiElementTestData = service.GetFormDesignVersionUIElementsTestData(tenantId, formDesignVersionId, elementId);
            return Json(uiElementTestData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Export to Excel
        /// </summary>
        /// <param name="csv"></param>
        /// <param name="isGroupHeader"></param>
        /// <param name="noOfColInGroup"></param>
        /// <param name="isChildGrid"></param>
        /// <param name="repeaterName"></param>
        /// <param name="formName"></param>
        /// <param name="folderVersionId"></param>
        /// <param name="folderId"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public ActionResult ruleTesterGridExportToExcel(string csv, string header, int noOfColInGroup, string repeaterName)
        {
            ExcelBuilder excelBuilder = new ExcelBuilder();
            MemoryStream fileStream = excelBuilder.ExportToExcel(csv, false, noOfColInGroup, false, header);

            var fileDownloadName = repeaterName + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var fsr = new FileStreamResult(fileStream, contentType);
            fsr.FileDownloadName = fileDownloadName;

            return fsr;
        }

        public JsonResult GetConfigVieExcelConfiguration(int formDesignVersionId)
        {
            List<ExcelConfiguration> configuration = new List<ExcelConfiguration>();
            var config = service.GetFormDesignExcelConfiguration(formDesignVersionId);
            if (!string.IsNullOrEmpty(config))
            {
                configuration = JsonConvert.DeserializeObject<List<ExcelConfiguration>>(config);
            }

            return Json(configuration, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadConfigView(string formDesignName, int formDesignId, string formDesignVersion, int formDesignVersionId, string data, string extendedProperties, string comments, string configuration)
        {
            ConfigViewHelper helper = new ConfigViewHelper();
            List<Comments> commentText = new List<Comments>();
            string elements = HttpUtility.UrlDecode(data);
            var elementList = helper.GetAllUIElementsForDownload(elements);
            if (!string.IsNullOrEmpty(comments))
            {
                comments = HttpUtility.UrlDecode(comments);
                commentText = JsonConvert.DeserializeObject<List<Comments>>(HttpUtility.UrlDecode(comments));
            }
            if (!string.IsNullOrEmpty(configuration))
            {
                configuration = HttpUtility.UrlDecode(configuration);
                service.UpdateFormDesignExcelConfiguration(formDesignId, formDesignVersionId, configuration, CurrentUserName);
            }
            else
            {
                configuration = service.GetFormDesignExcelConfiguration(formDesignVersionId);
            }
            var ruleList = service.GetAllRulesForDesignVersion(formDesignVersionId);
            var expRules = service.GetAllDocumentRule(formDesignVersionId);
            configuration = HttpUtility.UrlDecode(configuration);
            List<ExcelConfiguration> config = JsonConvert.DeserializeObject<List<ExcelConfiguration>>(configuration);
            ConfigViewExcelGenerator generator = new ConfigViewExcelGenerator(ruleList, expRules.ToList());
            byte[] reportBytes = generator.GenerateConfigExcelReport(elementList, config, formDesignName, formDesignVersion, commentText, HttpUtility.UrlDecode(extendedProperties));
            string fileName = formDesignName + "_Config_View_" + "_" + DateTime.Now.ToString();
            var fileDownloadName = fileName + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(reportBytes, contentType, fileDownloadName);
        }

        public JsonResult CheckIsRepeaterColumnKeyElement(int tenantId, int uiElementID)
        {
            bool result = service.CheckIsRepeaterColumnKeyElement(uiElementID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method Returns the TextBox Element
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <returns>TextBox Element</returns>
        public JsonResult TextBox(int tenantId, int formDesignVersionId, int uiElementId)
        {
            TextBoxElementModel model = service.GetTextBox(tenantId, formDesignVersionId, uiElementId);
            model.IsDataSourceMapped = dataSourceservice.GetDataSourceMappingExistence(uiElementId, model.FormDesignID, formDesignVersionId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method Returns the Calendar Element
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <returns>Calendar Element</returns>
        public JsonResult Calendar(int tenantId, int formDesignVersionId, int uiElementId)
        {
            CalendarElementModel model = service.GetCalendar(tenantId, formDesignVersionId, uiElementId);
            model.IsDataSourceMapped = dataSourceservice.GetDataSourceMappingExistence(uiElementId, model.FormDesignID, formDesignVersionId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method Returns the DropDown Element
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <returns>DropDown Element</returns>
        public JsonResult DropDown(int tenantId, int formDesignVersionId, int uiElementId)
        {
            DropDownElementModel model = service.GetDropDown(tenantId, formDesignVersionId, uiElementId);
            model.IsDataSourceMapped = dataSourceservice.GetDataSourceMappingExistence(uiElementId, model.FormDesignID, formDesignVersionId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method Returns the CheckBox Element
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <returns>CheckBox Element</returns>
        public JsonResult CheckBox(int tenantId, int formDesignVersionId, int uiElementId)
        {
            CheckBoxElementModel model = service.GetCheckBox(tenantId, formDesignVersionId, uiElementId);
            model.IsDataSourceMapped = dataSourceservice.GetDataSourceMappingExistence(uiElementId, model.FormDesignID, formDesignVersionId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method Returns the RadioButton Element
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <returns>RadioButton Element</returns>
        public JsonResult RadioButton(int tenantId, int formDesignVersionId, int uiElementId)
        {
            RadioButtonElementModel model = service.GetRadioButton(tenantId, formDesignVersionId, uiElementId);
            model.IsDataSourceMapped = dataSourceservice.GetDataSourceMappingExistence(uiElementId, model.FormDesignID, formDesignVersionId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method Returns the Repeater Element
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <returns>Repeater Element</returns>
        public JsonResult Repeater(int tenantId, int formDesignVersionId, int uiElementId)
        {
            RepeaterElementModel model = service.GetRepeater(tenantId, formDesignVersionId, uiElementId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method Returns the List of Rules.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <returns>List of Rules</returns>
        public JsonResult Rules(int tenantId, int formDesignVersionId, int uiElementId)
        {
            IEnumerable<RuleRowModel> models = service.GetRulesForUIElement(tenantId, formDesignVersionId, uiElementId);
            if (models == null)
            {
                models = new List<RuleRowModel>();
            }
            return Json(models, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method Returns the Section Element
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <returns>Section Element</returns>
        public JsonResult Section(int tenantId, int formDesignVersionId, int uiElementId)
        {
            SectionElementModel model = service.GetSectionDesignDetail(tenantId, formDesignVersionId, uiElementId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method Returns the Tab Element
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <returns>Section Element</returns>
        public JsonResult Tab(int tenantId, int uiElementId)
        {
            TabElementModel model = service.GetTabDetail(tenantId, uiElementId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method Returns the List of Section Element.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <returns>List of Section Element</returns>
        public JsonResult SectionList(int tenantId, int formDesignVersionId, int uiElementId)
        {
            IEnumerable<SectionElementModel> models = service.GetSectionList(tenantId, formDesignVersionId, uiElementId);
            if (models != null && models.Count() > 0)
            {
                models = models.OrderBy(c => c.Sequence);
            }
            return Json(models, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method Upadate the sequence of Section Elements
        /// </summary>
        /// <param name="SectionElementModelSerializerHelper model"></param>
        [HttpPost]
        public JsonResult UpdateSectionSequences(SectionElementModelSerializerHelper model)
        {
            IDictionary<int, int> elementSequences = new Dictionary<int, int>();
            foreach (SectionElementModel section in model.Models)
            {
                elementSequences.Add(section.UIElementID, section.Sequence);
            }
            service.UpdateSectionSequences(User.Identity.Name, model.TenantID, model.FormDesignVersionID, elementSequences);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //Only some properties of the model parameter are populated for an Add
        public JsonResult AddSection(SectionElementModel model)
        {
            ServiceResult result = service.AddSectionDesign(User.Identity.Name, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, model.Label, "Section", true, 3);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //Only some properties of the model parameter are populated for an Update
        public JsonResult UpdateSectionName(SectionElementModel model)
        {
            SectionElementModel sourceModel = service.GetSectionDesignDetail(model.TenantID, model.FormDesignVersionID, model.UIElementID);
            sourceModel.Label = model.Label;
            sourceModel.FormDesignID = model.FormDesignID;
            ServiceResult result = service.UpdateSectionDesign(User.Identity.Name, sourceModel.TenantID, sourceModel.FormDesignID, sourceModel.FormDesignVersionID, sourceModel.UIElementID, sourceModel.Enabled, sourceModel.HelpText, true, model.HasCustomRule, sourceModel.Label, sourceModel.LayoutTypeID, sourceModel.Visible, sourceModel.IsDataSource, sourceModel.DataSourceName, sourceModel.DataSourceDescription, null, false, false, string.Empty, sourceModel.CustomHtml, model.ViewType, true, model.MDMName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Get the sequence of UIElements
        public JsonResult FieldList(int tenantId, int formDesignVersionId, int parentUIElementId)
        {
            IEnumerable<UIElementSeqModel> result = service.GetChildUIElements(tenantId, formDesignVersionId, parentUIElementId).OrderBy(c => c.Sequence);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //Get the Repeater element of UIElements
        public JsonResult GetRepeaterFieldList(int tenantId, int formDesignVersionId, int parentUIElementId)
        {
            IEnumerable<UIElementSeqModel> result = service.GetApplicableRepeaterChildUIElementsForDuplicationCheck(tenantId, formDesignVersionId, parentUIElementId).OrderBy(c => c.Sequence);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DataSourceFieldList(int tenantId, int formDesignId, int formDesignVersionId, int parentUIElementId, bool isKey)
        {
            IEnumerable<UIElementSeqModel> result = dataSourceservice.GetChildUIElementsForDataSource(tenantId, formDesignId, formDesignVersionId, parentUIElementId, isKey).OrderBy(c => c.Sequence);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Update the Sequence of UIElements
        public JsonResult UpdateFieldSequences(UIElementModelSerializerHelper model)
        {
            IDictionary<int, int> elementSequences = new Dictionary<int, int>();
            foreach (UIElementSeqModel element in model.Models)
            {
                elementSequences.Add(element.UIElementID, element.Sequence);
            }
            ServiceResult result = service.UpdateFieldSequences(User.Identity.Name, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, elementSequences);

            //Insert Repeater Keys for Duplication Check 
            IDictionary<int, bool> primaryKeyElements = new Dictionary<int, bool>();
            foreach (UIElementSeqModel element in model.Models)
            {
                primaryKeyElements.Add(element.UIElementID, element.IsKey);
            }
            ServiceResult duplicationResult = service.UpdateFieldCheckDuplicate(User.Identity.Name, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, primaryKeyElements);

            //Add key element for Repeater UIElement
            IDictionary<int, bool> keyElements = new Dictionary<int, bool>();
            foreach (var item in model.Models)
            {
                keyElements.Add(item.UIElementID, item.IsKey);
            }
            result = service.UpdateRepeaterKeyElement(model.TenantID, model.ParentUIElementID, keyElements);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //Update the CheckDuplicate of UIElements
        public JsonResult UpdateFieldCheckDuplicate(UIElementModelSerializerHelper model)
        {
            IDictionary<int, bool> elementCheckDuplicate = new Dictionary<int, bool>();
            foreach (UIElementSeqModel element in model.Models)
            {
                elementCheckDuplicate.Add(element.UIElementID, element.CheckDuplicate);
            }
            ServiceResult result = service.UpdateFieldCheckDuplicate(User.Identity.Name, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, elementCheckDuplicate);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //Add element in the form Design
        [HttpPost]
        public JsonResult AddElement(UIElementAddModel model)
        {
            ServiceResult result = new ServiceResult();
            List<string> ChkChildElement = new List<string>()
                                    {
                                        "TEXTBOX","CALENDAR",
                                        "LABEL","CHECKBOX","DROPDOWN LIST","DROPDOWN TEXTBOX",
                                        "MULTILINE TEXTBOX"
                                    };
            if (model.ElementType != null)
            {
                switch (model.ElementType.ToUpper())
                {
                    case "SECTION":
                        result = service.AddSectionDesign(User.Identity.Name, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, model.Label, model.ElementType, model.IsStandard, model.ViewType);
                        break;
                    case "REPEATER":
                        result = service.AddRepeater(User.Identity.Name, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, model.Label, "", 0, model.ElementType, model.IsStandard, model.ViewType);
                        break;
                    case "TEXTBOX":
                    case "[BLANK]":
                    case "LABEL":
                    case "MULTILINE TEXTBOX":
                    case "RICH TEXTBOX":
                        bool isBlank = model.ElementType == "[Blank]" ? true : false;
                        bool isLabel = model.ElementType == "Label" || model.ElementType == "[Blank]" ? true : false;
                        bool isMultiline = model.ElementType == "Multiline TextBox" ? true : false;
                        //by default text box needs to have String as defult data type selected. 2 - aligns to the String Value in UI.ApplicationDataType Table in database.
                        result = service.AddTextBox(User.Identity.Name, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, isMultiline, isBlank, isLabel, model.Label, "", 0, model.ElementType, model.IsStandard, model.ViewType);
                        break;
                    case "CHECKBOX":
                        result = service.AddCheckBox(User.Identity.Name, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, model.Label, "", 0, model.ElementType, model.IsStandard, model.ViewType);
                        break;
                    case "DROPDOWN LIST":
                    case "DROPDOWN TEXTBOX":
                        bool isDropDownTextBox = model.ElementType == "Dropdown TextBox" ? true : false;
                        result = service.AddDropDown(User.Identity.Name, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, model.Label, "", 0, model.ElementType, isDropDownTextBox, model.IsStandard, model.ViewType);
                        break;
                    case "RADIO BUTTON":
                        result = service.AddRadioButton(User.Identity.Name, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, model.Label, "", 0, model.ElementType, model.IsStandard, model.ViewType);
                        break;
                    case "CALENDAR":
                        result = service.AddCalendar(User.Identity.Name, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, model.Label, "", 0, model.ElementType, model.IsStandard, model.ViewType);
                        break;
                }
                if (ChkChildElement.Contains(model.ElementType.ToUpper()))
                {
                    service.AddDefaultKeyForRepeater(model.TenantID, model.FormDesignVersionID, model.ParentUIElementID);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ServiceResult AddElementByType(UIElementAddModel model)
        {
            ServiceResult result = new ServiceResult();
            List<string> ChkChildElement = new List<string>()
                                    {
                                        "TEXTBOX","CALENDAR",
                                        "LABEL","CHECKBOX","DROPDOWN LIST","DROPDOWN TEXTBOX",
                                        "MULTILINE TEXTBOX"
                                    };
            if (model.ElementType != null)
            {
                //Update rule description for the rules
                if (model.Rules != null && model.Rules.Count > 0)
                {
                    RuleTextManager ruleMgr = new RuleTextManager(model.Rules.ToList());
                    List<string> elements = ruleMgr.GetLeftOperands();
                    var uielement = service.GetUIElementByNames(model.FormDesignVersionID, elements);
                    model.Rules = ruleMgr.GenerateRuleText(uielement);
                }
                switch (model.ElementType.ToUpper())
                {
                    case "SECTION":
                        result = service.AddSectionDesign(User.Identity.Name, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, model.Label,
                            model.ElementType, model.IsEnable, model.IsVisible, model.Sequence, model.Rules, model.AreRulesModified, model.ExtendedProperties,
                            model.Layout, model.ViewType, model.SourceUIElementId, model.CustomHtml);
                        break;
                    case "REPEATER":
                        result = service.AddRepeater(User.Identity.Name, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, model.Label, model.HelpText,
                            model.Sequence, model.ElementType, model.IsEnable, model.IsVisible, model.Rules, model.AreRulesModified, model.AdvancedConfiguration,
                            model.ExtendedProperties, model.Layout, model.ViewType, model.AllowBulkUpdate, model.SourceUIElementId);
                        break;
                    case "TEXTBOX":
                    case "[BLANK]":
                    case "LABEL":
                    case "MULTILINE TEXTBOX":
                    case "RICH TEXTBOX":
                        bool isBlank = model.ElementType == "[Blank]" ? true : false;
                        bool isLabel = model.ElementType == "Label" || model.ElementType == "[Blank]" ? true : false;
                        bool isMultiline = model.ElementType == "Multiline TextBox" ? true : false;
                        //by default text box needs to have String as defult data type selected. 2 - aligns to the String Value in UI.ApplicationDataType Table in database.
                        result = service.AddTextBox(User.Identity.Name, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, isMultiline, isBlank, isLabel,
                            model.Label, model.HelpText, model.Sequence, model.ElementType, model.ApplicationDataTypeID, model.IsRequired, model.MaxLength, model.IsEnable,
                            model.IsVisible, Convert.ToInt32(model.LibraryRegexID), model.Rules, model.AreRulesModified, model.ExtendedProperties, model.DefaultValue, model.ViewType, model.SourceUIElementId);
                        break;
                    case "CHECKBOX":
                        result = service.AddCheckBox(User.Identity.Name, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, model.Label, model.HelpText,
                            model.Sequence, model.ElementType, model.IsEnable, model.IsVisible, model.Rules, model.AreRulesModified, model.ExtendedProperties, model.ViewType, model.SourceUIElementId);
                        break;
                    case "DROPDOWN LIST":
                    case "DROPDOWN TEXTBOX":
                        bool isDropDownTextBox = model.ElementType == "Dropdown TextBox" ? true : false;
                        result = service.AddDropDown(User.Identity.Name, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, model.Label, model.HelpText,
                            model.Sequence, model.ElementType, isDropDownTextBox, model.IsEnable, model.IsVisible, model.IsRequired, model.IsMultiselect, model.Items,
                            model.Rules, model.AreRulesModified, model.ExtendedProperties, model.DefaultValue, model.ViewType, model.SourceUIElementId);
                        break;
                    case "RADIO BUTTON":
                        result = service.AddRadioButton(User.Identity.Name, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, model.Label, model.HelpText,
                            model.Sequence, model.ElementType, model.IsEnable, model.IsVisible, model.IsRequired, model.OptionYes, model.OptionNo, model.Rules,
                            model.AreRulesModified, model.ExtendedProperties, model.ViewType, model.SourceUIElementId);
                        break;
                    case "CALENDAR":
                        result = service.AddCalendar(User.Identity.Name, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, model.Label, model.HelpText,
                            model.Sequence, model.ElementType, model.IsEnable, model.IsVisible, model.IsRequired, model.Rules, model.AreRulesModified, model.ExtendedProperties, model.DefaultValue, model.ViewType, model.SourceUIElementId);
                        break;
                }
                if (ChkChildElement.Contains(model.ElementType.ToUpper()))
                {
                    service.AddDefaultKeyForRepeater(model.TenantID, model.FormDesignVersionID, model.ParentUIElementID);
                }
            }
            return result;
        }

        //Update element in the form design.
        [HttpPost]
        public JsonResult UpdateElement(string modelStr)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            UIElementUpdateModel model = JsonConvert.DeserializeObject<UIElementUpdateModel>(modelStr, settings);

            ServiceResult result = new ServiceResult();

            //Set UIElementDataType to String for Label and Multiline TextBox
            if (model.ElementType == "Label" || model.ElementType == "Multiline TextBox" || model.ElementType == "Rich TextBox")
            {
                model.UIElementDataTypeID = 2;
            }

            //Update rule description for the rules
            if (model.Rules != null && model.Rules.Count() > 0)
            {
                RuleTextManager ruleMgr = new RuleTextManager(model.Rules.ToList());
                List<string> elements = ruleMgr.GetLeftOperands();
                var uielement = service.GetUIElementByNames(model.FormDesignVersionID, elements);
                model.Rules = ruleMgr.GenerateRuleText(uielement);
            }

            switch (model.ElementType)
            {
                case "Tab":
                    result = service.UpdateTab(User.Identity.Name, model.TenantID, model.UIElementID, model.HasCustomRule, model.IsCustomRulesModified, model.CustomRule);
                    break;
                case "Section":
                    result = service.UpdateSectionDesign(User.Identity.Name, model.TenantID, model.FormDesignID, model.FormDesignVersionID, model.UIElementID, model.Enabled,
                        model.HelpText, model.IsRequired, model.HasCustomRule, model.Label, model.LayoutTypeID,
                        model.Visible, model.IsDataSource, model.DataSourceName, model.DataSourceDescription, model.Rules, model.AreRulesModified, model.IsCustomRulesModified, model.CustomRule, model.CustomHtml, model.ViewType, model.IsStandard, model.MDMName);
                    break;
                case "Repeater":
                    result = service.UpdateRepeater(User.Identity.Name, model.TenantID, model.FormDesignID, model.FormDesignVersionID, model.UIElementID, model.Enabled, model.Visible, model.HasCustomRule, model.HelpText, model.IsRequired,
                        model.Label, model.ChildCount, model.Sequence, model.LayoutTypeID, model.IsDataSource, model.DataSourceName, model.DataSourceDescription, model.Rules, model.AreRulesModified, model.IsCustomRulesModified, model.CustomRule, model.LoadFromServer, model.IsDataRequired, model.AllowBulkUpdate, model.AdvancedConfiguration, model.RepeaterTemplates, model.IsStandard, model.MDMName);
                    break;
                case "Textbox":
                case "Label":
                case "Multiline TextBox":
                case "Rich TextBox":
                    result = service.UpdateTextBox(User.Identity.Name, model.TenantID, model.FormDesignID, model.FormDesignVersionID, model.UIElementID, model.Enabled, model.Visible, model.HasCustomRule, model.HelpText, model.IsLabel, model.IsMultiLine,
                        model.IsRequired, model.UIElementDataTypeID, model.DefaultValue, model.Label, model.MaxLength, model.Sequence, model.SpellCheck, model.AllowGlobalUpdates,
                        model.IsLibraryRegex, model.Regex, model.CustomRegexMessage, model.LibraryRegexID, model.Rules, model.AreRulesModified, model.IsCustomRulesModified, model.CustomRule, model.MaskFlag, model.ViewType, model.IsStandard, model.MDMName);
                    break;
                case "Checkbox":
                    result = service.UpdateCheckBox(User.Identity.Name, model.TenantID, model.FormDesignID, model.FormDesignVersionID, model.UIElementID, model.Enabled, model.Visible,
                        model.HasCustomRule, model.HelpText, model.IsRequired, model.Label, model.OptionLabel, model.Sequence, model.DefaultBoolValue,
                        model.Rules, model.AreRulesModified, model.IsCustomRulesModified, model.CustomRule, model.AllowGlobalUpdates, model.ViewType, model.IsStandard, model.MDMName);
                    break;
                case "Dropdown List":
                case "Dropdown TextBox":
                    result = service.UpdateDropDown(User.Identity.Name, model.TenantID, model.FormDesignID, model.FormDesignVersionID, model.UIElementID, model.Enabled,
                            model.Visible, model.HasCustomRule, model.HelpText, model.IsRequired, model.DefaultValue, model.Label,
                            model.Sequence, model.Items, model.Rules, model.AreRulesModified, model.IsCustomRulesModified, model.CustomRule, model.UIElementDataTypeID, model.IsDropDownTextBox, model.IsSortRequired,
                            model.IsLibraryRegex, model.LibraryRegexID, model.AllowGlobalUpdates, model.IsDropDownFilterable, model.ViewType, model.IsMultiSelect, model.IsStandard, model.MDMName);
                    break;
                case "Radio Button":
                    result = service.UpdateRadioButton(User.Identity.Name, model.TenantID, model.FormDesignID, model.FormDesignVersionID, model.UIElementID, model.Enabled, model.Visible,
                        model.HasCustomRule, model.HelpText, model.IsRequired, model.Label, model.OptionLabel, model.OptionLabelNo, model.IsYesNo, model.Sequence, model.DefaultBoolValueForRadio,
                        model.Rules, model.AreRulesModified, model.IsCustomRulesModified, model.CustomRule, model.AllowGlobalUpdates, model.ViewType, model.IsStandard, model.MDMName);
                    break;
                case "Calendar":
                    result = service.UpdateCalendar(User.Identity.Name, model.TenantID, model.FormDesignID, model.FormDesignVersionID, model.UIElementID, model.Enabled,
                            model.Visible, model.HasCustomRule, model.HelpText, model.IsRequired, model.Label, model.Sequence,
                            model.DefaultDate, model.MinDate, model.MaxDate, model.Rules, model.AreRulesModified, model.IsCustomRulesModified, model.CustomRule, model.AllowGlobalUpdates, model.ViewType, model.IsStandard, model.MDMName);
                    break;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ServiceResult UpdateElementByType(UIElementConfigUpdateModel model)
        {
            ServiceResult result = new ServiceResult();
            //Set UIElementDataType to String for Label and Multiline TextBox
            if (model.ElementType == "Label" || model.ElementType == "Multiline TextBox" || model.ElementType == "Rich TextBox")
            {
                model.ApplicationDataTypeID = 2;
            }

            //Update rule description for the rules
            if (model.Rules != null && model.Rules.Count() > 0)
            {
                RuleTextManager ruleMgr = new RuleTextManager(model.Rules.ToList());
                List<string> elements = ruleMgr.GetLeftOperands();
                var uielement = service.GetUIElementByNames(model.FormDesignVersionID, elements);
                model.Rules = ruleMgr.GenerateRuleText(uielement);
            }

            switch (model.ElementType)
            {
                case "Tab":
                    result = service.UpdateTab(User.Identity.Name, model.TenantID, model.UIElementID, model.HasCustomRule, model.IsCustomRulesModified, model.CustomRule);
                    break;
                case "Section":
                    result = service.UpdateSectionDesign(User.Identity.Name, model.TenantID, model.FormDesignID, model.FormDesignVersionID, model.UIElementID, model.IsEnable,
                        model.HelpText, model.Required, model.Label, model.IsVisible, model.Rules, model.AreRulesModified, model.ViewType, model.ParentUIElementId,
                        model.ExtendedProperties, model.Layout, model.CustomHtml, model.IsStandard);
                    break;
                case "Repeater":
                    result = service.UpdateRepeater(User.Identity.Name, model.TenantID, model.FormDesignID, model.FormDesignVersionID, model.UIElementID, model.IsEnable, model.IsVisible, model.HelpText, model.Required,
                        model.Label, model.Sequence, model.Rules, model.AreRulesModified, model.AdvancedConfiguration, model.ParentUIElementId, model.ExtendedProperties, model.Layout, model.ViewType, model.AllowBulkUpdate, model.IsStandard);
                    break;
                case "Textbox":
                case "Label":
                case "Multiline TextBox":
                case "Rich TextBox":
                    result = service.UpdateTextBox(User.Identity.Name, model.TenantID, model.FormDesignID, model.FormDesignVersionID, model.UIElementID, model.IsEnable, model.IsVisible, model.HelpText, model.IsLabel, model.IsMultiLine,
                        model.Required, model.ApplicationDataTypeID, model.DefaultValue, model.Label, model.MaxLength, model.Sequence,
                        model.LibraryRegexID, model.Rules, model.AreRulesModified, model.ViewType, model.ParentUIElementId, model.ExtendedProperties, model.IsStandard);
                    break;
                case "Checkbox":
                    result = service.UpdateCheckBox(User.Identity.Name, model.TenantID, model.FormDesignID, model.FormDesignVersionID, model.UIElementID, model.IsEnable, model.IsVisible,
                        model.HelpText, model.Required, model.Label, model.Sequence, model.DefaultBoolValue, model.Rules, model.AreRulesModified, model.ViewType, model.ParentUIElementId, model.ExtendedProperties, model.IsStandard);
                    break;
                case "Dropdown List":
                case "Dropdown TextBox":
                    result = service.UpdateDropDown(User.Identity.Name, model.TenantID, model.FormDesignID, model.FormDesignVersionID, model.UIElementID, model.IsEnable,
                            model.IsVisible, model.HelpText, model.Required, model.Label, model.Sequence, model.Items, model.Rules, model.AreRulesModified,
                            model.IsDropDownTextBox, model.LibraryRegexID, model.ViewType, model.IsMultiSelect, model.ParentUIElementId, model.ExtendedProperties, model.DefaultValue, model.IsStandard);
                    break;
                case "Radio Button":
                    result = service.UpdateRadioButton(User.Identity.Name, model.TenantID, model.FormDesignID, model.FormDesignVersionID, model.UIElementID, model.IsEnable, model.IsVisible,
                        model.HelpText, model.Required, model.Label, model.OptionYes, model.OptionNo, model.IsYesNo, model.Sequence, model.DefaultBoolValue,
                        model.Rules, model.AreRulesModified, model.ViewType, model.ParentUIElementId, model.ExtendedProperties, model.IsStandard);
                    break;
                case "Calendar":
                    result = service.UpdateCalendar(User.Identity.Name, model.TenantID, model.FormDesignID, model.FormDesignVersionID, model.UIElementID, model.IsEnable,
                            model.IsVisible, model.HelpText, model.Required, model.Label, model.Sequence, model.DefaultValue, model.Rules, model.AreRulesModified,
                            model.ViewType, model.ParentUIElementId, model.ExtendedProperties, model.IsStandard);
                    break;
            }

            return result;
        }

        //Delete element from form design
        [HttpPost]
        public JsonResult DeleteElement(int tenantId, int formDesignVersionId, string elementType, int uiElementID)
        {
            ServiceResult result = DeleteElementByType(tenantId, formDesignVersionId, elementType, uiElementID);
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ServiceResult DeleteElementByType(int tenantId, int formDesignVersionId, string elementType, int uiElementID)
        {
            ServiceResult result = new ServiceResult();
            switch (elementType)
            {
                case "Section":
                    result = this.service.DeleteSection(tenantId, formDesignVersionId, uiElementID);
                    break;
                case "Tab":
                    throw new NotSupportedException("Can not delete tab.");
                case "Repeater":
                    result = this.service.DeleteRepeater(tenantId, formDesignVersionId, uiElementID);
                    break;
                case "Textbox":
                case "Label":
                case "Multiline TextBox":
                case "[Blank]":
                case "Rich TextBox":
                    result = service.DeleteTextBox(tenantId, formDesignVersionId, uiElementID);
                    break;
                case "Checkbox":
                    result = service.DeleteCheckBox(tenantId, formDesignVersionId, uiElementID);
                    break;
                case "Dropdown List":
                case "Dropdown TextBox":
                    result = service.DeleteDropDown(tenantId, formDesignVersionId, uiElementID);
                    break;
                case "Radio Button":
                    result = service.DeleteRadioButton(tenantId, formDesignVersionId, uiElementID);
                    break;
                case "Calendar":
                    result = service.DeleteCalendar(tenantId, formDesignVersionId, uiElementID);
                    break;
            }

            return result;
        }

        #region DataSource Methods

        /// <summary>
        /// This method Returns the List of DataSources.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="uiElementId"></param>
        /// <param name="uiElementType"></param>
        /// <returns>List of DataSources</returns>
        public JsonResult DataSources(int tenantId, int uiElementId, string uiElementType, int formDesignId, int formDesignVersionId)
        {
            IEnumerable<DataSourceRowModel> models = dataSourceservice.GeDataSourcesForUIElementType(tenantId, uiElementId, uiElementType, formDesignId, formDesignVersionId);
            if (models == null)
            {
                models = new List<DataSourceRowModel>();
            }
            return Json(models, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method Returns the List of UIElements for DataSources.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="uiElementId"></param>
        /// <param name="uiElementType"></param>
        /// <returns>List of UIElements for DataSources</returns>
        public JsonResult DataSourceUIElements(int tenantId, int uiElementId, string uiElementType, int dataSourceId, int formDesignId, int formDesignVersionId)
        {
            IEnumerable<UIElementRowModel> models = dataSourceservice.GetDataSourceUIElements(tenantId, uiElementId, uiElementType, dataSourceId, formDesignId, formDesignVersionId);
            if (models == null)
            {
                models = new List<UIElementRowModel>();
            }
            return Json(models, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllDataSourceFilterOperatorList()
        {
            IEnumerable<KeyValue> models = dataSourceservice.GetDataSourceFilterOperators();
            if (models == null)
            {
                models = new List<KeyValue>();
            }
            return Json(models, JsonRequestBehavior.AllowGet);
        }



        //Checks whether datasource name uniqe or not.
        public JsonResult IsDataSourceNameUnique(int tenantId, int formDesignVersionId, string dataSourceName, int uiElementId, string uiElementType)
        {
            bool isUnique = dataSourceservice.IsDataSourceNameUnique(tenantId, formDesignVersionId, dataSourceName, uiElementId, uiElementType);
            return Json(isUnique, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsMDMNameUnique(int tenantId, int formDesignId, string mdmName, int uiElementId, string uiElementType)
        {
            bool isUnique = service.IsMDMNameUnique(tenantId, formDesignId, mdmName, uiElementId, uiElementType);
            return Json(isUnique, JsonRequestBehavior.AllowGet);
        }

        //Update the datasource
        [HttpPost]
        public JsonResult UpdateDataSource(List<DataSourceUiElementMappingModel> uiElementRows, int uiElementId, int tenantId, bool isEmptyDelete, string uiElementType, int formDesignId, int formDesignVersionId, List<int> existingDataSourceIdList)
        {
            ServiceResult result = dataSourceservice.UpdateDataSource(uiElementRows, uiElementId, tenantId, isEmptyDelete, uiElementType, formDesignId, formDesignVersionId, existingDataSourceIdList, base.CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //add mapping to the datasource UIElement.
        [HttpPost]
        public JsonResult AddDataSourceUIElementMapping(int uiElementId, int dataSourceId, int mappedUIElementId)
        {
            ServiceResult result = dataSourceservice.AddDataSourceUIElementMapping(uiElementId, dataSourceId, mappedUIElementId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Return the List of DataSourceElementDisplayMode for DropDown.
        public JsonResult GetDataSourceElementDisplayMode()
        {
            IEnumerable<DataSourceElementDisplayModeModel> models = dataSourceservice.GetDataSourceElementDisplayMode();

            return Json(models, JsonRequestBehavior.AllowGet);
        }

        //Return the List of DataSourceDisplayMode(Auto/Manual) for DropDown.
        public JsonResult GetDataSourceDisplayMode()
        {
            IEnumerable<DataSourceModeViewModel> models = dataSourceservice.GetDataSourceDisplayMode();
            return Json(models, JsonRequestBehavior.AllowGet);
        }
        //Return the List of DataCopyMode for DropDown.
        public JsonResult GetDataCopyMode()
        {
            IEnumerable<KeyValue> ownerList = dataSourceservice.GetDataCopyMode();
            if (ownerList == null)
            {
                ownerList = new List<KeyValue>();
            }
            return Json(ownerList, JsonRequestBehavior.AllowGet);
        }


        #endregion


        //Retruns List of logical operators.
        public JsonResult GetLogicalOperatorList(int tenantId)
        {
            IEnumerable<LogicalOperatorTypeViewModel> models = service.GetDataSourceElementDisplayMode(tenantId);

            return Json(models, JsonRequestBehavior.AllowGet);
        }

        //Retruns List of operators.
        public JsonResult GetOperatorList(int tenantId)
        {
            IEnumerable<OperatorTypeViewModel> models = service.GetOperatorTypes(tenantId);

            return Json(models, JsonRequestBehavior.AllowGet);
        }

        //Retruns List of target properties.
        public JsonResult GetTargetPropertyList(int tenantId)
        {
            IEnumerable<TargetPropertyViewModel> models = service.GetTargetPropertyTypes(tenantId);

            return Json(models, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeControlType(int tenantId, int formDesignVersionId, string newElementType, int uiElementId, int newuiElementTypeId)
        {
            ServiceResult result = new ServiceResult();
            switch (newElementType.ToUpper())
            {
                case "TEXTBOX":
                case "RICH TEXTBOX":
                case "LABEL":
                case "MULTILINE TEXTBOX":
                    result = service.ChangeControlText(User.Identity.Name, tenantId, formDesignVersionId, uiElementId, newuiElementTypeId, newElementType);
                    break;
                case "DROPDOWN LIST":
                case "DROPDOWN TEXTBOX":
                    bool isDropDownTextBox = newElementType.ToUpper() == "DROPDOWN TEXTBOX" ? true : false;
                    result = service.ChangeControlDrop(User.Identity.Name, tenantId, formDesignVersionId, uiElementId, newuiElementTypeId, newElementType, isDropDownTextBox);
                    break;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Expression Rule Methods

        public JsonResult DocumentRule(int uiElementId, int formDesignId, int formDesignVersionId)
        {
            IEnumerable<DocumentRuleModel> models = service.GetDocumentRule(uiElementId, formDesignId, formDesignVersionId);
            if (models == null)
            {
                models = new List<DocumentRuleModel>();
            }
            return Json(models, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DocumentRuleNew(int uiElementId, int formDesignId, int formDesignVersionId, DateTime effecttiveDate)
        {
            IEnumerable<DocumentRuleModel> models = service.GetDocumentRule(uiElementId, formDesignId, formDesignVersionId);
            if (models == null)
            {
                models = new List<DocumentRuleModel>();
            }
            else
            {
                if (models != null && models.Count() > 0)
                {
                    var ruleSplitData = models.ToList()[0].RuleJSON;
                    var splitSourceData = JsonConvert.DeserializeObject<ExpressionRuleGeneratorModel.documentRuleModel>(ruleSplitData);
                    var sourceData = service.UpdateFormDesignIdInSourceData(splitSourceData.documentrule.ruleconditions.sources.ToList(), effecttiveDate);
                    var SourceMergeExpressionData = splitSourceData.documentrule.ruleconditions.sourcemergelist.sourcemergeactions.ToList()[0];
                    models.ToList()[0].ElementData = JsonConvert.SerializeObject(sourceData);
                    models.ToList()[0].RuleJSON = SourceMergeExpressionData.sourcemergeexpression.Replace("\\", "");
                }

            }
            return Json(models, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDocumentRuleType()
        {
            IEnumerable<DocumentRuleTypeModel> models = service.GetDocumentRuleType();

            return Json(models, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveDocumentRule(int uiElementId, string uiElementType, int formDesignId, int formDesignVersionId, IEnumerable<DocumentRuleModel> nRules)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                foreach (var item in nRules)
                {
                    var getJson = item.RuleJSON;
                    Documentrule documentRule = DocumentRuleSerializer.Deserialize(getJson);
                    DocumentRuleCompiler ruleCompiler = new DocumentRuleCompiler(item.DocumentRuleTypeID, documentRule);
                    CompiledDocumentRule compiledRule = ruleCompiler.CompileRule();
                    item.CompiledRuleJSON = JsonConvert.SerializeObject(compiledRule);
                }

                string username = User.Identity.Name;
                result = service.SaveDocumentRule(username, uiElementId, uiElementType, formDesignId, formDesignVersionId, nRules);
                var docids = result.Items.FirstOrDefault();
                if (docids != null)
                {
                    if (!string.IsNullOrEmpty(docids.Messages[0]))
                    {
                        int docRuleId = Convert.ToInt32(docids.Messages[0]);
                        CompiledDcoumentCacheHandler cache = new CompiledDcoumentCacheHandler();
                        cache.Remove(docRuleId);

                        CompiledDocumentRuleReadOnlyObjectCache.RemoveDoucmentRuleFromCache(docRuleId);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveDocumentRuleNew(int uiElementId, string uiElementType, int formDesignId, int formDesignVersionId, IEnumerable<DocumentRuleModel> nRules, ExpressionRuleGeneratorModel expmodel)
        {
            ServiceResult result = new ServiceResult();
            string targetelementpath = service.GetUIElementFullPath(uiElementId, formDesignVersionId);
            try
            {
                foreach (var item in nRules)
                {
                    var ruleType = service.GetRuleType(item.DocumentRuleTypeID).ToLower();
                    var formDesignName = service.GetFormName(item.FormDesignVersionID);
                    var getJson = ExpressionRuleGenerator.GenerateRuleText(formDesignName, ruleType, uiElementType.ToLower(), targetelementpath, item);
                    item.RuleJSON = getJson;
                    item.Description = item.Description;
                    Documentrule documentRule = DocumentRuleSerializer.Deserialize(getJson);
                    DocumentRuleCompiler ruleCompiler = new DocumentRuleCompiler(item.DocumentRuleTypeID, documentRule);
                    CompiledDocumentRule compiledRule = ruleCompiler.CompileRule();
                    item.CompiledRuleJSON = JsonConvert.SerializeObject(compiledRule);
                }

                string username = User.Identity.Name;
                result = service.SaveDocumentRule(username, uiElementId, uiElementType, formDesignId, formDesignVersionId, nRules);
                var docids = result.Items.FirstOrDefault();
                if (docids != null)
                {
                    if (!string.IsNullOrEmpty(docids.Messages[0]))
                    {
                        int docRuleId = Convert.ToInt32(docids.Messages[0]);
                        CompiledDcoumentCacheHandler cache = new CompiledDcoumentCacheHandler();
                        cache.Remove(docRuleId);

                        CompiledDocumentRuleReadOnlyObjectCache.RemoveDoucmentRuleFromCache(docRuleId);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DownloadDocumentRuleJson(int dRulesID)
        {
            var documentRuleJson = service.GetDocumentRuleJsonData(dRulesID);
            return Json(documentRuleJson, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteDocumentRule(int dRulesID)
        {
            ServiceResult result = service.DeleteDocumentRule(dRulesID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult GetLeftRepeaterKeys(int tenantId, int uiElementId, int expressionId)
        {
            bool rightOnly = false;
            var keyList = service.GetExpressionRepeaterKeyList(TenantID, uiElementId, expressionId, rightOnly);
            return Json(keyList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRightRepeaterKeys(int tenantId, int uiElementId, int expressionId)
        {
            bool rightOnly = true;
            var keyList = service.GetExpressionRepeaterKeyList(TenantID, uiElementId, expressionId, rightOnly);
            return Json(keyList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTargetRepeaterKeys(int tenantId, int uiElementId, int ruleId)
        {
            var keyList = service.GetTargetRepeaterKeyList(TenantID, uiElementId, ruleId);
            return Json(keyList, JsonRequestBehavior.AllowGet);
        }
    }
}