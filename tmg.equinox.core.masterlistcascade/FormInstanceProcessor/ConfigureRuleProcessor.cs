using System.Collections.Generic;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.ruleengine;
using tmg.equinox.ruleprocessor;

namespace tmg.equinox.expressionbuilder
{
    public class ConfigureRuleProcessor
    {
        private FormDesignVersionDetail _detail;        
        private FormInstanceDataManager _formDataInstanceManager;                
        private string _sectionName;
        private int _formInstanceId;
        private List<RuleDesign> _sectionRuleDesigns;

        public ConfigureRuleProcessor(FormDesignVersionDetail detail, FormInstanceDataManager formDataInstanceManager, string sectionName, int formInstanceId, List<RuleDesign> rule)
        {                        
            this._formDataInstanceManager = formDataInstanceManager;
            this._sectionName = sectionName;
            this._formInstanceId = formInstanceId;
            this._detail = detail;
            this._sectionRuleDesigns = rule;
        }

        public void Run()
        {
            ExecuteConfigueRuleProcessor();
        }

        private void ExecuteConfigueRuleProcessor()
        {
            RuleManager ruleMgr = new RuleManager(_sectionRuleDesigns, _detail.JSONData, _formDataInstanceManager, _formInstanceId,_detail, _sectionName);
            ruleMgr.ExecuteRules();
        }
    }
}