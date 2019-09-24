using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.RulesManager;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.Framework;
using tmg.equinox.web.Framework.RulesManager;
using tmg.equinox.web.RuleEngine.RuleDescription;

namespace tmg.equinox.web.Controllers
{
    public class RulesManagerController : AuthenticatedController
    {
        private IRulesManagerService _ruleManagerService;
        private IUIElementService _elementService;
        public RulesManagerController(IRulesManagerService service, IUIElementService uiElementService)
        {
            this._ruleManagerService = service;
            this._elementService = uiElementService;
        }

        // GET: RulesManager
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetFormDesignList(int tenantId)
        {
            var formDesigns = _ruleManagerService.GetFormDesigns(tenantId);
            return Json(formDesigns, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFormDesignVersionList(int tenantId, int formDesignId)
        {
            var designversions = _ruleManagerService.GetFormDesignVersions(tenantId, formDesignId);
            return Json(designversions, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRuleList(GridPagingRequest gridPagingRequest, int tenantId, int formDesignVersionId)
        {
            var ruleList = _ruleManagerService.GetRulesByFormDesignVersion(tenantId, formDesignVersionId, gridPagingRequest);
            return Json(ruleList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRuleListByTarget(GridPagingRequest gridPagingRequest, int tenantId, int formDesignVersionId)
        {
            var ruleList = _ruleManagerService.GetRulesByFormDesignVersionByTarget(tenantId, formDesignVersionId, gridPagingRequest);
            return Json(ruleList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRuleListBySource(GridPagingRequest gridPagingRequest, int tenantId, int formDesignVersionId)
        {
            var ruleList = _ruleManagerService.GetRulesByFormDesignVersionBySource(tenantId, formDesignVersionId, gridPagingRequest);
            return Json(ruleList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOperandList(int tenantId, int formDesignVersionId)
        {
            IEnumerable<ElementRowModel> uiElementList = _ruleManagerService.GetElementList(tenantId, formDesignVersionId);
            if (uiElementList == null)
            {
                uiElementList = new List<ElementRowModel>();
            }
            return Json(uiElementList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRule(int tenantId, int formDesignVersionId, int ruleId)
        {
            var rule = _ruleManagerService.GetRuleByID(tenantId, ruleId, formDesignVersionId);
            return Json(rule, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveRule(int tenantId, int formDesignVersionId, string model)
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
            ServiceResult result = new ServiceResult();

            RuleRowModel objRule = JsonConvert.DeserializeObject<RuleRowModel>(model, settings);
            objRule = this.UpdateRuleDescription(objRule, formDesignVersionId);

            if (objRule.RuleId <= 0)
            {
                result = _ruleManagerService.AddRule(tenantId, CurrentUserName, objRule);
            }
            if (objRule.RuleId > 0)
            {
                objRule.RuleId = this.CreateNewElement(tenantId, objRule.RuleId, formDesignVersionId);
                result = _ruleManagerService.UpdateRule(tenantId, CurrentUserName, objRule);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteRule(int tenantId, string ruleIds, int formDesignVersionId)
        {
            int[] rules = JsonConvert.DeserializeObject<int[]>(ruleIds);
            ServiceResult result = _ruleManagerService.DeleteRule(tenantId, rules, formDesignVersionId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSourcesByRule(int tenantId, int formDesignVersionId, int ruleId)
        {
            var sources = _ruleManagerService.GetSourceByRuleID(tenantId, formDesignVersionId, ruleId);
            return Json(sources, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTargetsByRule(int tenantId, int formDesignVersionId, int ruleId)
        {
            var targets = _ruleManagerService.GetTargetByRuleID(tenantId, formDesignVersionId, ruleId);
            return Json(targets, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AssignTargets(int tenantId, int uiElementId, string targetMap, int formDesignVersionID)
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
            var elementMap = JsonConvert.DeserializeObject<List<ElementRuleMap>>(targetMap, settings);
            ServiceResult result = new ServiceResult();
            this.CreateNewElement(tenantId, elementMap[0].RuleID, formDesignVersionID, ref elementMap);
            result = _ruleManagerService.AssignTargets(tenantId, uiElementId, elementMap, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTargetsByRuleID(int tenantId, int formDesignVersionID, int ruleId)
        {
            var elementMap = _ruleManagerService.GetTargetMapByRuleID(tenantId, ruleId);
            return Json(elementMap, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Download(int tenantId, string formDesignName, int formDesignVersionId, string viewType)
        {
            List<RuleRowViewModel> ruleList = new List<RuleRowViewModel>();
            if (string.Equals(viewType, "target", StringComparison.OrdinalIgnoreCase))
            {
                ruleList = _ruleManagerService.GetRulesByFormDesignVersionByTarget(tenantId, formDesignVersionId);
            }
            if (string.Equals(viewType, "source", StringComparison.OrdinalIgnoreCase))
            {
                ruleList = _ruleManagerService.GetRulesByFormDesignVersionBySource(tenantId, formDesignVersionId);
            }
            if (string.Equals(viewType, "rule", StringComparison.OrdinalIgnoreCase))
            {
                ruleList = _ruleManagerService.GetRulesByFormDesignVersion(tenantId, formDesignVersionId);
            }

            ExcelGenerator exGenerator = new ExcelGenerator(ruleList, viewType);
            byte[] reportBytes = exGenerator.GenerateExcelReport();
            string fileName = formDesignName + " - Rule List";
            var fileDownloadName = fileName + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(reportBytes, contentType, fileDownloadName);
        }
        public ActionResult GetRuleHierarchy(int tenantId, int formDesignVersionId, int ruleId)
        {
            List<RuleHierarchyViewModel> ruleHierarchyList = _ruleManagerService.GetRulesHierarchyByFormDesignVersion(tenantId, formDesignVersionId);
            var ruleIDList = from rl in ruleHierarchyList where rl.RuleID == ruleId select rl;
            List<string> groups = new List<string>();
            if (ruleIDList != null && ruleIDList.Count() > 0)
            {
                foreach (var rl in ruleIDList)
                {
                    if (groups.Contains(rl.GroupID) == false)
                    {
                        groups.Add(rl.GroupID);
                    }
                }
            }
            List<RuleHierarchyViewModel> finalListForRule = new List<RuleHierarchyViewModel>();
            if (groups.Count > 0)
            {
                var rls = from rl in ruleHierarchyList where groups.Contains(rl.GroupID) select rl;
                if (rls != null && rls.Count() > 0)
                {
                    finalListForRule = rls.ToList();
                }
            }
            RulesManagerTreeDataBuilder builder = new RulesManagerTreeDataBuilder(finalListForRule, ruleId);
            JArray result = builder.GenerateTreeDataV2();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private RuleRowModel UpdateRuleDescription(RuleRowModel objRule, int formDesignVersionId)
        {
            //Get Left Operand/Right Operand
            foreach (ExpressionRowModel expression in objRule.RootExpression.Expressions)
            {
                expression.LeftKeyFilter = expression.LeftKeyFilter == null ? _ruleManagerService.GetKeyFilter(objRule.RuleId, false) : expression.LeftKeyFilter;
                expression.RightKeyFilter = expression.RightKeyFilter == null ? _ruleManagerService.GetKeyFilter(objRule.RuleId, true) : expression.RightKeyFilter;
            }

            //Generate description
            RuleTextManager ruleMgr = new RuleTextManager();
            List<string> elements = ruleMgr.GetLeftOperands(objRule);
            var uielement = _elementService.GetUIElementByNames(formDesignVersionId, elements);
            objRule = ruleMgr.GenerateRuleText(objRule, uielement);
            return objRule;
        }

        private int CreateNewElement(int tenantId, int ruleId, int formDesignVersionId)
        {
            int newRuleId = ruleId;
            var elementMap = _ruleManagerService.GetTargetMapByRuleID(tenantId, ruleId);

            foreach (var map in elementMap)
            {
                bool newElementRequired = _elementService.IsNewUIElementCreationRequired(map.UIElementID, formDesignVersionId);
                if (newElementRequired)
                {
                    string elementType = _elementService.GetUIElementTypeByID(map.UIElementID);
                    ServiceResult result = _elementService.CopyElement(tenantId, formDesignVersionId, map.UIElementID, elementType, CurrentUserName);
                    if (result.Result == ServiceResultStatus.Success)
                    {
                        var items = result.Items.ToList();
                        if (items.Count > 1)
                        {
                            string ruleMap = items[1].Messages[0];
                            string[] rules = ruleMap.Split(',');
                            foreach (string rMap in rules)
                            {
                                if (rMap.Split(':')[0] == ruleId.ToString())
                                {
                                    newRuleId = Convert.ToInt32(rMap.Split(':')[1]);
                                }
                            }
                        }
                    }
                }
            }

            return newRuleId;
        }

        private void CreateNewElement(int tenantId, int ruleId, int formDesignVersionId, ref List<ElementRuleMap> eleRuleMap)
        {
            foreach (var map in eleRuleMap)
            {
                bool newElementRequired = _elementService.IsNewUIElementCreationRequired(map.UIElementID, formDesignVersionId);
                if (newElementRequired)
                {
                    string elementType = _elementService.GetUIElementTypeByID(map.UIElementID);
                    ServiceResult result = _elementService.CopyElement(tenantId, formDesignVersionId, map.UIElementID, elementType, CurrentUserName);
                    if (result.Result == ServiceResultStatus.Success)
                    {
                        var items = result.Items.ToList();
                        if (items.Count > 1)
                        {
                            string newElement = items[0].Messages[0];
                            map.UIElementID = Convert.ToInt32(newElement);
                        }
                    }
                }
            }
        }

    }
}