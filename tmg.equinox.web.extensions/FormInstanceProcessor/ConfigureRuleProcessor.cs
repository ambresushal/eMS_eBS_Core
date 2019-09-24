using System.Collections.Generic;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.RuleEngine;

namespace tmg.equinox.web.FormInstanceProcessor
{
    public class ConfigureRuleProcessor
    {
        private FormDesignVersionDetail _detail;
        private FormInstanceDataManager _formDataInstanceManager;
        private string _sectionName;
        private int _formInstanceId;
        private List<RuleDesign> _sectionRuleDesigns;
        private IFormInstanceService _formInstanceService;
        private IFormDesignService _formDesignServices;
        private IFolderVersionServices _folderVersionServices;
        private IFormInstanceRuleExecutionLogService _ruleExecutionLogService;
        private int _ruleExecutionLogParentRowID;
        public ConfigureRuleProcessor(FormDesignVersionDetail detail, FormInstanceDataManager formDataInstanceManager, string sectionName, int formInstanceId, List<RuleDesign> rule, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, IFormInstanceService formInstanceService, IFormInstanceRuleExecutionLogService ruleExecutionLogService,int ruleExecutionLogParentRowID)
        {
            this._formDataInstanceManager = formDataInstanceManager;
            this._sectionName = sectionName;
            this._formInstanceId = formInstanceId;
            this._detail = detail;
            this._sectionRuleDesigns = rule;
            this._folderVersionServices = folderVersionServices;
            this._formDesignServices = formDesignServices;
            this._formInstanceService = formInstanceService;
            this._ruleExecutionLogService = ruleExecutionLogService;
            _ruleExecutionLogParentRowID = ruleExecutionLogParentRowID;
        }

        public void Run()
        {
            ExecuteConfigueRuleProcessor();
        }

        public void RunRules()
        {
            ExecuteVisibleRuleProcessor();
        }

        public void RunVisibleRules()
        {
            ExecuteVisibleRuleElementProcessor();
        }

        public bool RunRule(RuleDesign rule)
        {
            RuleManager ruleMgr = new RuleManager(_sectionRuleDesigns, _detail.JSONData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName, _folderVersionServices, _formDesignServices, _formInstanceService, _ruleExecutionLogService, _ruleExecutionLogParentRowID);
            return ruleMgr.ProcessSectionVisibleRule(rule);
        }

        private void ExecuteConfigueRuleProcessor()
        { 
            RuleManager ruleMgr = new RuleManager(_sectionRuleDesigns, _detail.JSONData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName, _folderVersionServices, _formDesignServices, _formInstanceService, _ruleExecutionLogService, _ruleExecutionLogParentRowID);
            ruleMgr.ExecuteRules();
        }

        private void ExecuteVisibleRuleProcessor()
        {
            RuleManager ruleMgr = new RuleManager(_sectionRuleDesigns, _detail.JSONData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName, _folderVersionServices, _formDesignServices, _formInstanceService, _ruleExecutionLogService, _ruleExecutionLogParentRowID);
            ruleMgr.ExecuteVisibleRules();
        }

        private void ExecuteVisibleRuleElementProcessor()
        {
            RuleManager ruleMgr = new RuleManager(_sectionRuleDesigns, _detail.JSONData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName, _folderVersionServices, _formDesignServices, _formInstanceService, _ruleExecutionLogService, _ruleExecutionLogParentRowID);
            ruleMgr.ExecuteVisibleElementRules();
        }
    }
}