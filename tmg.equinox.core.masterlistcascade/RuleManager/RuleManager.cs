using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.caching;
using tmg.equinox.ruleprocessor;
using tmg.equinox.infrastructure.exceptionhandling;
using System;

namespace tmg.equinox.ruleengine
{
    public class RuleManager
    {
        #region Private Member
        private List<RuleDesign> _ruleDesigns;
        private ElementRuleProcessor _ruleProcessor;
        private RepeaterRuleProcessor _repeaterRuleProcessor;
        private JObject _formData;
        private FormInstanceDataManager _formDataInstanceManager;
        private int _formInstanceId;
        private FormDesignVersionDetail _detail;
        private string _sectionName;
        #endregion

        #region Constructor
        public RuleManager(List<RuleDesign> ruleDesigns, string formData, FormInstanceDataManager formDataInstanceManager, int formInstanceId, FormDesignVersionDetail detail, string sectionName)
        {
            this._ruleDesigns = ruleDesigns;
            this._formData = JObject.Parse(formData);
            this._formDataInstanceManager = formDataInstanceManager;
            this._formInstanceId = formInstanceId;
            this._detail = detail;
            this._sectionName = sectionName;
            this._ruleProcessor = new ElementRuleProcessor(this._formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName);
            this._repeaterRuleProcessor = new RepeaterRuleProcessor(this._formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName);
        }

        public RuleManager(int formDesignId, string formData)
        {
            this._ruleDesigns = this.GetRules(formDesignId);
            this._formData = JObject.Parse(formData);
            this._ruleProcessor = new ElementRuleProcessor(this._formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName);
            this._repeaterRuleProcessor = new RepeaterRuleProcessor(this._formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName);
        }
        #endregion

        #region Public Method
        public string ExecuteRules()
        {
            //string updatedJson = null;
            if (this._formData != null)
            {
                if (this._ruleDesigns.Count > 0)
                {
                    this.ProcessValueRulesOnData("DOCUMENTROOT");

                }
                _detail.JSONData = JsonConvert.SerializeObject(_formData);
            }
            return _detail.JSONData;
        }
        #endregion

        #region Private Methods
        //Pass Section Name for which rules to be executed
        private void ProcessValueRulesOnData(string containerName)
        {
            //If rule is related to repeater then loop and call the same method
            //else call ProcessRuleOnData for a particular rule

            if (_ruleDesigns.Count > 0)
            {
                var rules = _ruleDesigns.Where(a => a.TargetPropertyTypeId == 3 && (a.UIElementFullName.IndexOf(containerName) == 0 || containerName == "DOCUMENTROOT")).ToList();

                for (var idx = 0; idx < rules.Count; idx++)
                {
                    var rule = rules[idx];
                    try
                    {
                        if (rule.IsParentRepeater == true)
                        {
                            this.ProcessRepeaterRuleOnData(rule);
                        }
                        else
                        {
                            this.ProcessRuleOnData(rule);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        string customMsg = "An error occurred while excuting rule ID: '" + rule.RuleID + "'";
                        Exception customException = new Exception(customMsg, ex);
                        ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
                    }
                }
            }
        }

        //Process a single rule object
        private void ProcessRuleOnData(RuleDesign rule)
        {
            var result = _ruleProcessor.ProcessRule(rule);
            if (result == true)
            {
                if (rule.IsResultSuccessElement == true)
                {
                    var successValue = _ruleProcessor.GetOperandValue(rule.SuccessValueFullName, null, JObject.Parse(_detail.JSONData));
                    _ruleProcessor.SetElementPropertyValue(rule.UIElementFullName, successValue);
                }
                else
                {
                    if (rule.SuccessValue != null)
                    {
                        _ruleProcessor.SetElementPropertyValue(rule.UIElementFullName, rule.SuccessValue);
                    }
                }
            }
            else
            {
                if (rule.IsResultFailureElement == true)
                {
                    var successValue = _ruleProcessor.GetOperandValue(rule.FailureValueFullName, null, JObject.Parse(_detail.JSONData));
                    _ruleProcessor.SetElementPropertyValue(rule.UIElementFullName, successValue);
                }
                else
                {
                    if (rule.FailureValue != null)
                    {
                        _ruleProcessor.SetElementPropertyValue(rule.UIElementFullName, rule.FailureValue);
                    }
                }
            }
        }

        //Process a single rule object for repeater
        private void ProcessRepeaterRuleOnData(RuleDesign rule)
        {
            _repeaterRuleProcessor.ProcessRepeaterRule(rule, null, null);
        }

        private List<RuleDesign> GetRules(int formDesignId)
        {
            List<RuleDesign> rules = new List<RuleDesign>();
            FormDesignDataCacheHandler cacheHanlder = new FormDesignDataCacheHandler();
            rules = cacheHanlder.GetRuleDesigns(formDesignId);
            return rules;
        }
        #endregion
    }
}