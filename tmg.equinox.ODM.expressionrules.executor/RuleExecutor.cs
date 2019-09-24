using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.pbp;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.masterListCascade;
using tmg.equinox.dependencyresolution;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpreter.rulecompiler;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.FormInstanceProcessor;
using tmg.equinox.web.FormInstanceProcessor.ExpressionBuilder;
using tmg.equinox.web.Framework.Caching;
using tmg.equinox.web.sourcehandler;

namespace tmg.equinox.ODM.expressionrules.executor
{
    public class RuleExecutor
    {
        private IFolderVersionServices _folderVersionServices;
        private IFormDesignService _formDesignServices;
        private IFormInstanceDataServices _formInstanceDataServices;
        private IMasterListService _masterListService;
        private IUIElementService _uiElementService;
        private IFormInstanceService _formInstanceService;
        private IMasterListCascadeService _mlcService;
        private List<ElementDocumentRuleViewModel> _rules;

        // Construcor Initialization
        public RuleExecutor()
        {
            UnityConfig.RegisterComponents();
            _uiElementService = UnityConfig.Resolve<IUIElementService>();
            _mlcService = UnityConfig.Resolve<IMasterListCascadeService>();
            _formDesignServices = UnityConfig.Resolve<IFormDesignService>();
            _masterListService = UnityConfig.Resolve<IMasterListService>();
            _formInstanceService = UnityConfig.Resolve<IFormInstanceService>();
            _formInstanceDataServices = UnityConfig.Resolve<IFormInstanceDataServices>();
            _folderVersionServices = UnityConfig.Resolve<IFolderVersionServices>();
            _rules = _mlcService.GetODMElementDocumentRules(0);
        }
        public JObject ProcessExpressionRules(MigrationPlanItem plan, JObject formInstanceData)
        {
            int tenantId = 1;
            int? userId = 1228;
            string currentUserName = "superuser";
            try
            {
                if (formInstanceData != null && plan != null)
                {
                    // Add formdata to section-wise cache to run expression rules as expression rule uses data from cache or db, but we have not stored the data in db before rule execution.
                    FormInstanceSectionDataCacheHandler handler = new FormInstanceSectionDataCacheHandler();
                    List<string> sectionList = formInstanceData.Properties().Select(p => p.Name).ToList();
                    foreach (var sectionName in sectionList)
                    {
                        JObject objSectionData = JObject.Parse("{'" + sectionName + "':[]}");
                        objSectionData[sectionName] = formInstanceData[sectionName];
                        handler.AddSectionData(plan.FormInstanceId, sectionName, JsonConvert.SerializeObject(objSectionData), userId);
                    }

                    CurrentRequestContext requestContext = new CurrentRequestContext();
                    FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(tenantId, userId, _formInstanceDataServices, currentUserName, _folderVersionServices);
                    SourceHandlerDBManager sourceDBManager = new SourceHandlerDBManager(tenantId, _folderVersionServices, formInstanceDataManager, _formDesignServices, _masterListService);
                    FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, plan.FormDesignVersionId, _formDesignServices);
                    FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(true);
                    if (_rules == null || _rules.Count == 0)
                    {
                        // Get All Element Rules
                        _rules = _mlcService.GetODMElementDocumentRules(plan.FormDesignVersionId);
                    }
                    ExpressionRuleTreeProcessor processor = new ExpressionRuleTreeProcessor(plan.FormInstanceId, _uiElementService, plan.FolderVersionId,
                        sourceDBManager, _formDesignServices, formInstanceDataManager, detail, _formInstanceService, userId, requestContext);
                    foreach (var rule in _rules)
                    {
                        try
                        {
                            string sectionName = rule.TargetFieldPaths.Split('.')[0];
                            var getJson = rule.RuleJSON;
                            Documentrule documentRule = DocumentRuleSerializer.Deserialize(getJson);
                            DocumentRuleCompiler ruleCompiler = new DocumentRuleCompiler(0, documentRule);
                            CompiledDocumentRule compiledRule = ruleCompiler.CompileRule();
                            string sectionData = processor.ProcessCompiledRuleForODM(compiledRule);
                            if (!String.IsNullOrEmpty(sectionData))
                            {
                                JObject data = (JObject)JObject.Parse((sectionData));
                                if (data != null)
                                {
                                    formInstanceData.SelectToken(sectionName).Replace(data[sectionName]);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            string customMsg = "An error occurred while processing Rule For Target Path '" + rule.TargetFieldPaths + "'";
                            Exception customException = new Exception(customMsg, ex);
                            //bool reThrow = ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Rules For Forminstance ID: '" + plan.FormInstanceId + "', QID: '" + plan.QID + "'";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
            return formInstanceData;
        }
    }
}
