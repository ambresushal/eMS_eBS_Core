using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.Framework;
using tmg.equinox.identitymanagement.Models;
using tmg.equinox.identitymanagement;
using tmg.equinox.identitymanagement.Enums;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.web.Framework.Caching;
using tmg.equinox.web.PBPView;

using tmg.equinox.web.FormDesignManager;
using tmg.equinox.ruleinterpreter;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpreter.rulecompiler;
using tmg.equinox.web.FormInstance;
using System.IO;
using tmg.equinox.mapper;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.web.RuleEngine.RuleDescription;

namespace tmg.equinox.web.Controllers
{
    public class FormDesignController : AuthenticatedController
    {
        #region Private Variables

        private IFormDesignService _formDesignService;
        private IUIElementService uiElementService;
        private IFolderVersionServices _folderVersionServices;
        private IFormInstanceRepeaterService _formInstanceRepeaterService;
        IdentityAccessMapper _IdentityAccessMapper;

        #endregion

        #region Constructor

        public FormDesignController(IFormDesignService service, IUIElementService uiElementService, IFolderVersionServices folderVersionServices, IFormInstanceRepeaterService formInstanceRepeaterService)
        {
            this._formDesignService = service;
            this._folderVersionServices = folderVersionServices;
            this.uiElementService = uiElementService;
            this._formInstanceRepeaterService = formInstanceRepeaterService;
            _IdentityAccessMapper = new IdentityAccessMapper();
        }

        #endregion

        #region Action Methods

        #region FormDesign Methods

        //
        // GET: /FormDesign/
        //public ActionResult Index()
        //{
        //    return View();
        //}



        public ActionResult Index()
        {
            //CookieExpirationTime = DefaultFormsAuthentication.FormsAuthenticationTimeout.Minutes;
            ViewData["CookieExpirationTime"] = CookieExpirationTime;
            ViewData["SessionTimeoutWarning"] = ConfigurationManager.AppSettings["LoginWarning"];

            //List<RoleClaimModel> roleClaims = new List<RoleClaimModel>();
            //base.Claims.ToList().ForEach(c =>
            //   roleClaims.Add(new RoleClaimModel
            //   {
            //       Resource = c.Type,
            //       Action = c.Value,
            //       ResourceType = c.ValueType
            //   }));

            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            ViewBag.RoleId = RoleID;
            return View();
        }

        public int CookieExpirationTime { get; set; }

        public int SessionTimeoutWarning { get; set; }

        public JsonResult FormDesignList(int tenantId)
        {
            IEnumerable<FormDesignRowModel> formDesignList = null;
            formDesignList = _formDesignService.GetFormDesignList(tenantId);

            if (formDesignList == null)
            {
                formDesignList = new List<FormDesignRowModel>();
            }
            return Json(formDesignList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AnchorDesignList(int tenantId)
        {
            IEnumerable<FormDesignRowModel> formDesignList = null;
            formDesignList = _formDesignService.GetAnchorDesignList(tenantId);

            if (formDesignList == null)
            {
                formDesignList = new List<FormDesignRowModel>();
            }
            return Json(formDesignList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FormDesignListByDocType(int tenantId, int docDesignType)
        {
            IEnumerable<FormDesignRowModel> formDesignList = null;
            formDesignList = _formDesignService.GetFormDesignListByDocType(tenantId, docDesignType);

            if (formDesignList == null)
            {
                formDesignList = new List<FormDesignRowModel>();
            }
            return Json(formDesignList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocumentDesignType()
        {
            IEnumerable<DocumentDesignTypeRowModel> documentType = null;
            documentType = _formDesignService.GetDocumentDesignType();

            if (documentType == null)
            {
                documentType = new List<DocumentDesignTypeRowModel>();
            }

            return Json(documentType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MasterListFormDesignList(int tenantId)
        {
            IEnumerable<FormDesignRowModel> formDesignList = null;
            formDesignList = _formDesignService.GetMasterListFormDesignList(tenantId);

            if (formDesignList == null)
            {
                formDesignList = new List<FormDesignRowModel>();
            }
            return Json(formDesignList, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult Add(int tenantId, string displayText, bool isMasterList, int docType, int srcDesign, bool isAliasDesign, bool usesAliasDesign, bool IsSectionLock)
        {
            ServiceResult result = _formDesignService.AddFormDesign(base.CurrentUserName, tenantId, displayText.Replace(" ", ""), displayText, "", isMasterList, docType, srcDesign, isAliasDesign, usesAliasDesign, IsSectionLock);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Update(int tenantId, int formDesignId, string displayText, int srcDesign, bool IsSectionLock)
        {
            ServiceResult result = _formDesignService.UpdateFormDesign(base.CurrentUserName, tenantId, formDesignId, displayText.Replace(" ", ""), displayText, srcDesign, IsSectionLock);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(int tenantId, int formDesignId)
        {
            ServiceResult result = _formDesignService.DeleteFormDesign(base.CurrentUserName, tenantId, formDesignId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddType(string displayText)
        {
            ServiceResult result = _formDesignService.AddDesignType(displayText.Replace(" ", ""));
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region FormDesignVersion Methods


        public JsonResult FormDesignVersionList(int tenantId, int formDesignId)
        {
            IEnumerable<FormDesignVersionRowModel> formDesignVersionList = _formDesignService.GetFormDesignVersionList(tenantId, formDesignId);
            if (formDesignVersionList == null)
            {
                formDesignVersionList = new List<FormDesignVersionRowModel>();
            }
            return Json(formDesignVersionList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddVersion(int tenantId, int formDesignId, int formDesignVersionId, bool isFirstVersion, DateTime effectiveDate)
        {
            ServiceResult result = new ServiceResult();
            if (isFirstVersion == true)
            {
                result = _formDesignService.AddFormDesignVersion(base.CurrentUserName, tenantId, formDesignId, effectiveDate, "", "");
                FormDesignRowModel formDesign = _formDesignService.GetFormDesignList(tenantId).Where(c => c.FormDesignId == formDesignId).FirstOrDefault();
                FormDesignVersionRowModel model = _formDesignService.GetFormDesignVersionList(tenantId, formDesignId).FirstOrDefault();
                uiElementService.AddTab(base.CurrentUserName, tenantId, model.FormDesignVersionId, formDesign.DisplayText, formDesign.DisplayText);
            }
            else
            {
                FormDesignVersionRowModel model = _formDesignService.GetFormDesignVersionList(tenantId, formDesignId).FirstOrDefault();
                if (formDesignVersionId != 0)
                {
                    result = _formDesignService.CopyFormDesignVersion(User.Identity.Name, tenantId, formDesignVersionId, effectiveDate, model.Version, "");
                }
                else
                {
                    result = _formDesignService.CopyFormDesignVersion(User.Identity.Name, tenantId, model.FormDesignVersionId, effectiveDate, model.Version, "");
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateVersion(int tenantId, int formDesignVersionId, DateTime effectiveDate)
        {
            ServiceResult result = _formDesignService.UpdateFormDesignVersion(base.CurrentUserName, tenantId, formDesignVersionId, effectiveDate, "");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FinalizeVersion(int tenantId, int formDesignVersionId, string comments)
        {
            ServiceResult result = new ServiceResult();
            result = CompileFormDesign(tenantId, formDesignVersionId);
            if (result.Result == ServiceResultStatus.Success)
            {
                //Compile Document Rule JSON
                CompileFormDesignVersionRule(tenantId, formDesignVersionId);

                if (result.Result == ServiceResultStatus.Success)
                {

                    //Check if Finalizing form has any DataSources Mapped with other forms that are still not finalized.

                    result = _formDesignService.CheckDataSourceMappings(base.CurrentUserName, tenantId, formDesignVersionId);

                    if (result != null && result.Items.Any())
                    {
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    // This method is used to check whether a formdesign version Form Name is Master list or not 
                    // as Load repeater from Server functionality is only applicable for Master List form.
                    if (_formDesignService.IsMasterList(formDesignVersionId))
                    {
                        result = _formInstanceRepeaterService.SaveRepeaterFormInstanceDataOnFinalize(tenantId, formDesignVersionId, base.CurrentUserName);
                        if (result.Result == ServiceResultStatus.Success)
                        {
                            result = _formDesignService.FinalizeFormDesignVersion(base.CurrentUserName, tenantId, formDesignVersionId, comments);
                        }
                    }
                    else
                    {
                        result = _formDesignService.FinalizeFormDesignVersion(base.CurrentUserName, tenantId, formDesignVersionId, comments);
                    }

                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteVersion(int tenantId, int formDesignVersionId, int formDesignId)
        {
            ServiceResult result = _formDesignService.DeleteFormDesignVersion(base.CurrentUserName, tenantId, formDesignVersionId, formDesignId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// SH Validation for compilation: Atleast one user role should be selected for 'Edit'
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CompileFormDesignVersion(int tenantId, int formDesignVersionId)
        {
            ServiceResult result = new ServiceResult();
            var compileFlag = false;
            List<int> uiElementIdList = uiElementService.GetSectionListByFormDesignVersionId(formDesignVersionId);
            var roles = IdentityManager.GetApplicationRoles();
            var AccessPermissions = uiElementIdList.Select(sectionId => _IdentityAccessMapper.MapToElementAcessViewModel(IdentityManager.GetClaims(sectionId), roles));
            //  foreach (List<ElementAccessViewModel> sectionWiseUserRoleAccessList in  uiElementIdList.Select(sectionId => IdentityManager.GetClaims(sectionId)))
            foreach (List<ElementAccessViewModel> sectionWiseUserRoleAccessList in AccessPermissions)
            {
                compileFlag = sectionWiseUserRoleAccessList.Any(claimList => claimList.IsEditable == true);
                if (compileFlag == false)
                    break;
            }
            if (compileFlag == true)
            {
                result = CompileFormDesign(tenantId, formDesignVersionId);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVersionNumber(int formDesignVersionId, int tenantId)
        {
            string versionNumber = this._formDesignService.GetVersionNumber(formDesignVersionId, tenantId);
            return Json(versionNumber, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CompileFormDesignVersionRule(int tenantId, int formDesignVersionId)
        {
            ServiceResult result = new ServiceResult();

            IEnumerable<DocumentRuleModel> documentRules = this.uiElementService.GetAllDocumentRule(formDesignVersionId);
            //Compile Document Rule JSON
            result = CompileDocumentRule(tenantId, formDesignVersionId);

            //if (result.Result == ServiceResultStatus.Success)
            //{
            //Rule Tree
            result = CompileDocumentRuleTree(tenantId, formDesignVersionId, documentRules);
            // }
            if (result.Result == ServiceResultStatus.Success)
            {  //Event Map Tree
                result = CompileDocumentEventTree(tenantId, formDesignVersionId, documentRules);
            }

            CompiledDocumentRuleReadOnlyObjectCache.CleanDocumentRule();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FormDesignVersionRules(int tenantID, int[] formDesignVersionIDS)
        {
            List<FormDesignVersionRuleModel> models = new List<FormDesignVersionRuleModel>();
            if (formDesignVersionIDS != null)
            {
                foreach (var fdvID in formDesignVersionIDS)
                {
                    FormDesignVersionDetail detail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(tenantID, fdvID, _formDesignService);
                    if (detail != null)
                    {
                        FormDesignVersionRuleModel model = new FormDesignVersionRuleModel();
                        model.Rules = detail.Rules;
                        model.Validations = detail.Validations;
                        model.FormDesignVersionId = fdvID;
                        models.Add(model);
                    }
                }
            }
            return Json(models, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFormDesignVersionActivityLog(int formDesignId, int formDesignVersionId)
        {
            FormDesignActivityLog activityLog = new FormDesignActivityLog(this._formDesignService);
            List<ActivityLog> activityLogData = activityLog.GetActivityLogData(formDesignId, formDesignVersionId);
            return Json(activityLogData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FormDesignVersionActivityLogToExcel(string csv, string header, int noOfColInGroup, string repeaterName)
        {
            ExcelBuilder excelBuilder = new ExcelBuilder();
            MemoryStream fileStream = excelBuilder.ExportToExcel(csv, false, noOfColInGroup, false, header);

            var fileDownloadName = repeaterName + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var fsr = new FileStreamResult(fileStream, contentType);
            fsr.FileDownloadName = fileDownloadName;

            return fsr;
        }

        public JsonResult GenerateRuledescription(int formDesignId, int formDesignVersionId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                List<UIElement> elementList = uiElementService.GetUIElementsListByFormDesignId(1, formDesignVersionId);
                foreach (var element in elementList)
                {
                    var rules = uiElementService.GetRulesForUIElement(1, formDesignVersionId, element.UIElementID);
                    if (rules != null && rules.Count() > 0)
                    {
                        RuleTextManager ruleMgr = new RuleTextManager(rules.ToList());
                        List<string> elements = ruleMgr.GetLeftOperands();
                        var uielement = uiElementService.GetUIElementByNames(formDesignVersionId, elements);
                        rules = ruleMgr.GenerateRuleText(uielement);
                        uiElementService.UpdateRuleDescription(rules);

                    }
                }
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion            Action Methods

        #region Private Methods

        private void SetFormPermission(FormDesignVersionDetail formDesignVersionDetails)
        {
            int formDesignVersionId = formDesignVersionDetails.FormDesignVersionId;
            List<int> roleIDs = new List<int>();
            int[] roleIds = new int[10];
            List<ElementAccessViewModel> formClaimsToAdd = new List<ElementAccessViewModel>();
            List<ApplicationRole> roleList = IdentityManager.GetRoles().ToList();
            if (roleList.Count > 0)
            {
                foreach (var role in roleList)
                {
                    foreach (SectionDesign section in formDesignVersionDetails.Sections)
                    {
                        var permissions = section.AccessPermissions;
                        if (permissions.Count > 0)
                        {
                            int visibleSectionCount = permissions.Where(x => x.RoleID == role.Id && x.IsVisible == true).Count();
                            if (visibleSectionCount > 0 && section.Visible == true)
                            {
                                ElementAccessViewModel formClaim = new ElementAccessViewModel();

                                formClaim.IsVisible = true;
                                formClaim.RoleID = role.Id;
                                formClaim.ResourceID = formDesignVersionId;
                                formClaimsToAdd.Add(formClaim);
                                break;
                            }
                        }
                    }
                }
                if (formClaimsToAdd.Count > 0)
                {
                    IdentityManager.AddIdentityClaim(_IdentityAccessMapper.MapToApplicationRoleClaims(formClaimsToAdd, base.CurrentUserId.Value, ResourceType.FORM), base.CurrentUserId.Value, ResourceType.FORM);
                }
                else
                {
                    IdentityManager.RemoveResourceClaims(ResourceType.FORM, formDesignVersionId);
                }

            }


        }

        private ServiceResult CompileFormDesign(int tenantId, int formDesignVersionId)
        {
            FormDesignVersionDetail detail = this._formDesignService.GetFormDesignVersionDetail(tenantId, formDesignVersionId);
            ServiceResult result = new ServiceResult();
            if (detail != null)
            {
                SetFormPermission(detail);
                detail.JSONData = detail.GetDefaultJSONDataObject();
                //validate default JSON
                var converter = new ExpandoObjectConverter();
                dynamic jsonObject = JsonConvert.DeserializeObject<ExpandoObject>(detail.JSONData, converter);
                string jsonData = JsonConvert.SerializeObject(detail);
                FormDesignVersionDetailReadOnlyObjectCache.SetFormDesignVersionDetail(formDesignVersionId, detail);
                //Add to cache on Compile
                FormDesignDataCacheHandler cacheHandler = new FormDesignDataCacheHandler();
                cacheHandler.Add(tenantId, formDesignVersionId, jsonData);
                //End

                result = this._formDesignService.SaveCompiledFormDesignVersion(tenantId, formDesignVersionId, jsonData, base.CurrentUserName);
            }
            return result;
        }

        public JsonResult CompileImpactedField(int tenantId, int formDesignVersionId)
        {
            ServiceResult result = new ServiceResult();
            FormDesignVersionDetail detail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(tenantId, formDesignVersionId, _formDesignService);
            IEnumerable<DocumentRuleExtModel> docRules = this.uiElementService.GetAllExpressionRulesForImpactList(formDesignVersionId);
            var sourceFormDesignVersion = this._formDesignService.GetFormDesignVersionList(1, 2359).LastOrDefault();
            if (sourceFormDesignVersion != null)
            {
                var sourceDocElements = this.uiElementService.GetUIElementListForFormDesignVersion(tenantId, sourceFormDesignVersion.FormDesignVersionId);
                var targetDocElements = this.uiElementService.GetUIElementListForFormDesignVersion(tenantId, formDesignVersionId);

                ViewImpactDesignHelper viewImpact = new ViewImpactDesignHelper();
                string impactList = viewImpact.BuildImpactedFieldList(docRules, sourceDocElements, targetDocElements, detail);

                ViewImpactCacheManager cache = new ViewImpactCacheManager();
                cache.Remove(formDesignVersionId);
                cache.Add(formDesignVersionId, impactList);

                result = this.uiElementService.UpdatePBPViewImpacts(CurrentUserName, formDesignVersionId, impactList);
            }
            else
            {
                result.Result = ServiceResultStatus.Failure;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private ServiceResult CompileDocumentRule(int tenantId, int formDesignVersionId)
        {
            IEnumerable<DocumentRuleModel> docRules = this.uiElementService.GetAllDocumentRule(formDesignVersionId);
            ServiceResult result = new ServiceResult();
            Dictionary<int, string> data = new Dictionary<int, string>();
            string username = User.Identity.Name;

            if (docRules != null)
            {
                CompiledDcoumentCacheHandler compileDocHandler = new CompiledDcoumentCacheHandler();
                foreach (DocumentRuleModel dr in docRules)
                {
                    var getJson = dr.RuleJSON;
                    Documentrule documentRule = DocumentRuleSerializer.Deserialize(getJson);
                    DocumentRuleCompiler ruleCompiler = new DocumentRuleCompiler(dr.DocumentRuleTypeID, documentRule);
                    CompiledDocumentRule compiledRule = ruleCompiler.CompileRule();
                    var compiledJson = DocumentRuleSerializer.Serialize(compiledRule);
                    data.Add(dr.DocumentRuleID, compiledJson);

                    compileDocHandler.Remove(dr.DocumentRuleID);
                }

                result = this.uiElementService.UpdateCompileJSONRule(username, data);
            }

            if (result.Result == ServiceResultStatus.Success)
            {
                ExpressionBuilderTreeCacheHandler treeHandler = new ExpressionBuilderTreeCacheHandler();
                treeHandler.Remove(formDesignVersionId);

                ExpressionBuilderEventMapCacheHandler eventMapHandler = new ExpressionBuilderEventMapCacheHandler();
                eventMapHandler.Remove(formDesignVersionId);
            }

            return result;
        }

        private ServiceResult CompileDocumentRuleTree(int tenantId, int formDesignVersionId, IEnumerable<DocumentRuleModel> documentRules)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                List<DocumentRuleData> docRules = this.uiElementService.GetAllDocumentRuleForTree(formDesignVersionId, documentRules);
                string username = User.Identity.Name;

                if (docRules != null && docRules.Count() > 0)
                {
                    RuleTree formTree = new RuleTree(docRules);
                    var ruleTreeJson = formTree.RuleTreeFormulation();
                    result = this.uiElementService.UpdateRuleTreeJSON(username, formDesignVersionId, ruleTreeJson);
                }
                else
                {
                    result = this.uiElementService.UpdateRuleTreeJSON(username, formDesignVersionId, null);
                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { ex.Message } });

            }
            return result;
        }

        private ServiceResult CompileDocumentEventTree(int tenantId, int formDesignVersionId, IEnumerable<DocumentRuleModel> documentRules)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                List<DocumentRuleData> docRules = this.uiElementService.GetAllDocumentRuleForEventTree(formDesignVersionId, documentRules);
                string username = User.Identity.Name;

                if (docRules != null && docRules.Count() > 0)
                {
                    EventTree formTree = new EventTree(docRules);
                    formTree.formName = this.uiElementService.GetFormName(formDesignVersionId);
                    formTree.formDesignVersionId = formDesignVersionId;
                    var EventTreeJson = formTree.EventTreeFormulation();
                    result = this.uiElementService.UpdateEventTreeJSON(username, formDesignVersionId, EventTreeJson);
                }
                else
                {
                    result = this.uiElementService.UpdateEventTreeJSON(username, formDesignVersionId, null);
                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
            }

            return result;
        }

        #endregion Private Methods
    }


}