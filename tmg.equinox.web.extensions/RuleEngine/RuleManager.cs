using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.Framework.Caching;

namespace tmg.equinox.web.RuleEngine
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
        private IFormInstanceService _formInstanceService;
        private IFormDesignService _formDesignServices;
        private IFolderVersionServices _folderVersionServices;
        private IFormInstanceRuleExecutionLogService _ruleExecutionLogService;
        private int _ruleExecutionLogParentRowID;
        #endregion

        #region Constructor
        public RuleManager(List<RuleDesign> ruleDesigns, string formData, FormInstanceDataManager formDataInstanceManager, int formInstanceId, FormDesignVersionDetail detail, string sectionName, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, IFormInstanceService formInstanceService, IFormInstanceRuleExecutionLogService ruleExecutionLogService,int ruleExecutionLogParentRowID)
        {
            this._ruleDesigns = ruleDesigns;
            this._formData = JObject.Parse(formData);
            this._formDataInstanceManager = formDataInstanceManager;
            this._formInstanceId = formInstanceId;
            this._detail = detail;
            this._sectionName = sectionName;
            this._folderVersionServices = folderVersionServices;
            this._formDesignServices = formDesignServices;
            this._formInstanceService = formInstanceService;
            this._ruleProcessor = new ElementRuleProcessor(this._formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName,_folderVersionServices,_formDesignServices,_formInstanceService);
            this._repeaterRuleProcessor = new RepeaterRuleProcessor(this._formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName, _folderVersionServices, _formDesignServices, _formInstanceService);
            this._ruleExecutionLogService = ruleExecutionLogService;
            _ruleExecutionLogParentRowID = ruleExecutionLogParentRowID;
        }

        public RuleManager(int formDesignId, string formData)
        {
            this._ruleDesigns = this.GetRules(formDesignId);
            this._formData = JObject.Parse(formData);
            this._ruleProcessor = new ElementRuleProcessor(this._formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName, _folderVersionServices, _formDesignServices, _formInstanceService);
            this._repeaterRuleProcessor = new RepeaterRuleProcessor(this._formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName, _folderVersionServices, _formDesignServices, _formInstanceService);
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

        public string ExecuteVisibleRules()
        {
            //string updatedJson = null;
            if (this._formData != null)
            {
                if (this._ruleDesigns.Count > 0)
                {
                    this.ProcessSectionVisibleRule();

                }
                _detail.JSONData = JsonConvert.SerializeObject(_formData);
            }
            return _detail.JSONData;
        }

        public string ExecuteVisibleElementRules()
        {
            //string updatedJson = null;
            if (this._formData != null)
            {
                if (this._ruleDesigns.Count > 0)
                {
                    this.ProcessElementsVisibleRule();

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
                        try
                        {
                            ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        //Process a single rule object
        private void ProcessRuleOnData(RuleDesign rule)
        {
            var result = _ruleProcessor.ProcessRule(rule);

            //if (_ruleExecutionLogService != null)
            //    _ruleExecutionLogService.SaveFormInstanceRuleExecutionServerlogData(_formInstanceId, _ruleExecutionLogParentRowID,rule, result);

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

        private void ProcessSectionVisibleRule()
        {
            if (_ruleDesigns.Count > 0)
            {
                var rules = _ruleDesigns;

                for (var idx = 0; idx < rules.Count; idx++)
                {
                    var rule = rules[idx];
                    try
                    {
                        this.ProcessVisibleRule(rule);
                    }
                    catch (System.Exception ex)
                    {
                        string customMsg = "An error occurred while excuting rule ID: '" + rule.RuleID + "'";
                        Exception customException = new Exception(customMsg, ex);
                        try
                        {
                            ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        private void ProcessElementsVisibleRule()
        {
            if (_ruleDesigns.Count > 0)
            {
                var rules = _ruleDesigns;

                for (var idx = 0; idx < rules.Count; idx++)
                {
                    var rule = rules[idx];
                    try
                    {
                        this.ProcessElementVisibleRule(rule);
                    }
                    catch (System.Exception ex)
                    {
                        string customMsg = "An error occurred while excuting rule ID: '" + rule.RuleID + "'";
                        Exception customException = new Exception(customMsg, ex);
                        try
                        {
                            ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        //Process a single rule object
        private void ProcessVisibleRule(RuleDesign rule)
        {
            var result = _ruleProcessor.ProcessRule(rule);

            //if (_ruleExecutionLogService != null)
            //    _ruleExecutionLogService.SaveFormInstanceRuleExecutionServerlogData(_formInstanceId, _ruleExecutionLogParentRowID, rule, result);

            if (result == false)
                ProcessVisible(rule.UIElementFullName);
        }

        private void ProcessVisible(string uiElementFullName)
        {
            var childElements = _formData.SelectToken(uiElementFullName);
            if (childElements.Children().Count() > 0)
            {
                if (childElements.Type == JTokenType.Array)
                {
                    _formData.SelectToken(uiElementFullName).Replace(new JArray());
                }
                else
                {
                    foreach (var ele in childElements.Children())
                    {
                        var subsection = _formData.SelectToken(ele.Path);
                        if (subsection.Type == JTokenType.Array)
                            _formData.SelectToken(ele.Path).Replace(new JArray());
                        else
                            ProcessVisible(ele.Path);
                    }
                }
            }
            else
                _ruleProcessor.SetElementPropertyValue(uiElementFullName, "");
        }

        private void ProcessElementVisibleRule(RuleDesign rule)
        {
            var result = _ruleProcessor.ProcessRule(rule);

            //if (_ruleExecutionLogService != null)
            //    _ruleExecutionLogService.SaveFormInstanceRuleExecutionServerlogData(_formInstanceId, _ruleExecutionLogParentRowID, rule, result);

            if (result == false)
                ProcessVisible(rule.UIElementFullName);
        }

        public bool ProcessSectionVisibleRule(RuleDesign rule)
        {
            bool result = _ruleProcessor.ProcessRule(rule);
            return result;
        }

        #endregion
    }
}