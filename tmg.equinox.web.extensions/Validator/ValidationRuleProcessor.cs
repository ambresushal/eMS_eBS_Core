using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.RuleEngine;
using System.Text.RegularExpressions;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.applicationservices.interfaces;

namespace tmg.equinox.web.Validator
{
    public class ValidationRuleProcessor
    {
        private List<RuleDesign> _rules { get; set; }
        private JObject _formData { get; set; }
        private JObject _rowData { get; set; }
        private string _name { get; set; }
        private string _fullName { get; set; }
        private string _parentSectionPath { get; set; }
        private FormInstanceDataManager _formDataInstanceManager;
        private IFormInstanceService _formInstanceService;
        private IFormDesignService _formDesignServices;
        private IFolderVersionServices _folderVersionServices;
        private int _formInstanceId;

        public ValidationRuleProcessor(string name, string fullName, List<RuleDesign> validationRules, JObject formData, JObject rowData, string parentSectionPath, int formInstanceId, FormInstanceDataManager formDataInstanceManager, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, IFormInstanceService formInstanceService)
        {
            this._name = name;
            this._fullName = fullName;
            this._rules = validationRules;
            this._formData = formData;
            this._rowData = rowData;
            this._parentSectionPath = parentSectionPath;
            this._formInstanceId = formInstanceId;
            this._formDataInstanceManager = formDataInstanceManager;
            this._folderVersionServices = folderVersionServices;
            this._formDesignServices = formDesignServices;
            this._formInstanceService = formInstanceService;
        }

        public void EvaluateRequiredRules(ref ValidationDesign elementValidation)
        {
            bool isRequired = false;
            var requiredRules = _rules.Where(e => e.TargetPropertyTypeId == (int)RuleProcessor.TargetPropertyTypes.IsRequired).ToList();

            if (requiredRules.Any())
            {
                isRequired = ProcessRules(requiredRules, false);
                if (elementValidation != null)
                {
                    elementValidation.IsRequired = isRequired;
                }
                else if (isRequired)
                {
                    elementValidation = new ValidationDesign() { FullName = _fullName, UIElementName = _name, IsRequired = true, IsError = false, Regex = "", ValidationMessage = "", HasMaxLength = false, MaxLength = 0, DataType = "", IsActive = true };
                }
            }
        }

        public void EvaluateErrorRules(ref ValidationDesign elementValidation, RuleDesign rule)
        {
            bool hasValidationError = false;
            if (rule.TargetPropertyTypeId == (int)RuleProcessor.TargetPropertyTypes.Error)
            {
                hasValidationError = !ProcessRules(new List<RuleDesign>() { rule }, true);
                if (elementValidation != null)
                {
                    elementValidation.IsError = hasValidationError;
                    if (elementValidation.IsError)
                        elementValidation.ValidationMessage = string.IsNullOrEmpty(rule.Message) ? elementValidation.ValidationMessage : rule.Message;
                }
                else if (hasValidationError)
                {
                    elementValidation = new ValidationDesign() { FullName = _fullName, UIElementName = _name, IsRequired = false, IsError = true, Regex = "", ValidationMessage = "", HasMaxLength = false, MaxLength = 0, DataType = "", IsActive = true };
                    elementValidation.ValidationMessage = string.IsNullOrEmpty(rule.Message) ? elementValidation.ValidationMessage : rule.Message;

                }
            }
        }

        public void EvaluateCustomRules(ref ValidationDesign elementValidation)
        {
            bool hasValidationError = false;
            var errorRules = _rules.Where(e => e.TargetPropertyTypeId == (int)RuleProcessor.TargetPropertyTypes.CustomRule).ToList();
            if (errorRules.Any())
            {
                hasValidationError = !ProcessRules(errorRules, false);
                if (elementValidation != null)
                {
                    elementValidation.IsError = hasValidationError;
                    elementValidation.ValidationMessage = string.IsNullOrEmpty(errorRules[0].Message) ? elementValidation.ValidationMessage : errorRules[0].Message;
                }
                else if (hasValidationError)
                {
                    elementValidation = new ValidationDesign() { FullName = _fullName, UIElementName = _name, IsRequired = false, IsError = true, Regex = "", ValidationMessage = "", HasMaxLength = false, MaxLength = 0, DataType = "", IsActive = true };
                    elementValidation.ValidationMessage = string.IsNullOrEmpty(errorRules[0].Message) ? elementValidation.ValidationMessage : errorRules[0].Message;

                }
            }
        }

        public bool CheckForValidationEvaluationRequired()
        {
            bool IsValidationEvaluationRequired = true;

            List<RuleDesign> runValidationRules = _rules.Where(e => e.TargetPropertyTypeId == (int)RuleProcessor.TargetPropertyTypes.RunValidation).ToList();

            if (runValidationRules.Any())
            {

                IsValidationEvaluationRequired = ProcessRules(runValidationRules, false);
            }

            return IsValidationEvaluationRequired;
        }

        private bool ProcessRules(List<RuleDesign> rules, bool isAllTrueNeeded)
        {
            bool result = isAllTrueNeeded;
            ElementRuleProcessor ruleProcessor = new ElementRuleProcessor(_formData, _formDataInstanceManager, _formInstanceId, null, null, _folderVersionServices, _formDesignServices, _formInstanceService);
            foreach (var rule in rules)
            {
                bool currentRuleResult = false;
                if (rule.IsParentRepeater == true)
                {
                    string containerName = Regex.Replace(_parentSectionPath, "[^\\^0-9A-Za-z\\=>]", "").Replace("=>", ".");
                    if (rule.TargetKeyFilters != null && rule.TargetKeyFilters.Count > 0)
                    {
                        currentRuleResult = true;
                        bool isRowMatchingTarget = IsRepeaterRowMatchesTargetKeyFilter(rule);
                        if (isRowMatchingTarget)
                        {
                            RepeaterRowRuleProcessor repeaterRuleProcessor = new RepeaterRowRuleProcessor(rule, containerName, _rowData, _formData, _formDataInstanceManager, _formInstanceId, null, null, _folderVersionServices, _formDesignServices, _formInstanceService);
                            currentRuleResult = repeaterRuleProcessor.ProcessRule(rule);
                        }

                    }
                    else
                    {
                        RepeaterRowRuleProcessor repeaterRuleProcessor = new RepeaterRowRuleProcessor(rule, containerName, _rowData, _formData, _formDataInstanceManager, _formInstanceId, null, null, _folderVersionServices, _formDesignServices, _formInstanceService);
                        currentRuleResult = repeaterRuleProcessor.ProcessRule(rule);
                    }
                }
                else
                {
                    currentRuleResult = ruleProcessor.ProcessRule(rule);
                }
                if (isAllTrueNeeded)
                {
                    result = result && currentRuleResult;
                    if (result == false) break;
                }
                else
                {
                    result = result || currentRuleResult;
                    if (result) break;
                }
            }
            return result;
        }

        private bool IsRepeaterRowMatchesTargetKeyFilter(RuleDesign rule)
        {
            bool result = true;

            if (rule.TargetKeyFilters != null && rule.TargetKeyFilters.Count > 0)
            {
                foreach (var filter in rule.TargetKeyFilters)
                {
                    string[] path = filter.RepeaterKeyName.Split('.');
                    if (path.Length > 0)
                    {
                        if (this._rowData[path[path.Length - 1]].ToString() != filter.RepeaterKeyValue)
                        {
                            result = false;
                        }
                    }
                }
            }

            return result;
        }

        public bool EvaluateVisibleEnableRules(bool IsVisible, bool IsEnable)
        {
            List<RuleDesign> visibleRules = _rules.Where(r => r.TargetPropertyTypeId == (int)RuleProcessor.TargetPropertyTypes.Visible).ToList();
            IsVisible = visibleRules.Any() ? this.ProcessRules(visibleRules, true) : IsVisible;
            if (IsVisible)
            {
                IsEnable = true;
                //List<RuleDesign> enableRules = _rules.Where(r => r.TargetPropertyTypeId == (int)RuleProcessor.TargetPropertyTypes.Enabled).ToList();
                //IsEnable = enableRules.Any() ? this.ProcessRules(enableRules,true) : IsEnable;
            }
            return IsVisible && IsEnable;
        }

        public bool EvaluateVisibleEnableRules(bool IsVisible, bool IsEnable, bool isAnchor)
        {
            List<RuleDesign> visibleRules = _rules.Where(r => r.TargetPropertyTypeId == (int)RuleProcessor.TargetPropertyTypes.Visible).ToList();
            IsVisible = visibleRules.Any() ? this.ProcessRules(visibleRules, true) : IsVisible;
            if (IsVisible)
            {
                if (isAnchor)
                {
                    List<RuleDesign> enableRules = _rules.Where(r => r.TargetPropertyTypeId == (int)RuleProcessor.TargetPropertyTypes.Enabled).ToList();
                    IsEnable = enableRules.Any() ? this.ProcessRules(enableRules, true) : IsEnable;
                }
                else
                {
                    IsEnable = true;
                }
            }
            return IsVisible && IsEnable;
        }
    }
}