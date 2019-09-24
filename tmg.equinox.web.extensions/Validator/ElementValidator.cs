using Force.DeepCloner;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.RuleEngine;

namespace tmg.equinox.web.Validator
{
    public class ElementValidator : Validator
    {
        ElementDesign element { get; set; }
        JObject elementData { get; set; }
        int rowNumber { get; set; }
        string rowIDProperty { get; set; }
        JObject _rowData { get; set; }
        ValidationRuleProcessor ruleProcessor { get; set; }
        JToken _masterListData { get; set; }

        public string _keyValue { get; set; }
        bool _isSOTView = false;
        private FormInstanceDataManager _formDataInstanceManager;
        private IFormInstanceService _formInstanceService;
        private IFormDesignService _formDesignServices;
        private IFolderVersionServices _folderVersionServices;
        private int _formInstanceId;

        public ElementValidator(JObject formData, ElementDesign element, JObject elementData, List<ValidationDesign> validations, int rowNumber, string sectionPath, string rowIDProperty, List<RuleDesign> validationRules, JObject rowData, List<DuplicationDesign> duplicationChecks, JToken masterListData, bool isSOTView, bool isAnchor, int formInstanceId, FormInstanceDataManager formDataInstanceManager, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, IFormInstanceService formInstanceService)
            : base(formData, sectionPath, validations, validationRules, duplicationChecks, isAnchor)
        {
            this._formData = formData;
            this.element = element;
            this.elementData = elementData;
            this.validations = validations;
            this.rowNumber = rowNumber;
            this.sectionPath = sectionPath;
            this.rowIDProperty = rowIDProperty;
            this.validationRules = validationRules;
            this._rowData = rowData;
            this.duplicationChecks = duplicationChecks;
            this._masterListData = masterListData;
            this._isSOTView = isSOTView;
            this._formInstanceId = formInstanceId;
            this._formDataInstanceManager = formDataInstanceManager;
            this._folderVersionServices = folderVersionServices;
            this._formDesignServices = formDesignServices;
            this._formInstanceService = formInstanceService;
        }

        public override void Validate(ref ErrorSection errorSection, ref int maxAllowedErrorCount, bool isCBCMasterList)
        {
            ///Check for section IsVisible and Enable. Also Execute Visible and Enable rules for section.
            var ruleList = validationRules.Where(r => r.UIELementID == element.ElementID).ToList();
            ruleProcessor = new ValidationRuleProcessor(element.Name, element.FullName, ruleList, _formData, _rowData, sectionPath, _formInstanceId, _formDataInstanceManager, _folderVersionServices, _formDesignServices, _formInstanceService);

            if (element.Section != null)
            {
                if (!ruleProcessor.EvaluateVisibleEnableRules(element.Visible, element.Enabled, _isAnchor)) { return; }
                JObject subSectionData = elementData != null ? (elementData[element.Section.GeneratedName] is JObject) ? (JObject)elementData[element.Section.GeneratedName] : null : null;
                var subSectionValidator = new SectionValidator(_formData, element.Section, subSectionData, validations, sectionPath, validationRules, duplicationChecks, _masterListData, _isSOTView, _isAnchor, _formInstanceId, _formDataInstanceManager, _folderVersionServices, _formDesignServices, _formInstanceService);
                subSectionValidator.Validate(ref errorSection, ref maxAllowedErrorCount, isCBCMasterList);
            }
            else if (element.Repeater != null)
            {
                if (!ruleProcessor.EvaluateVisibleEnableRules(element.Visible, element.Enabled, _isAnchor)) { return; }
                JArray repeaterData = elementData != null ? elementData[element.Repeater.GeneratedName] is JArray ? (JArray)elementData[element.Repeater.GeneratedName] : null : null;
                var repeaterValidator = new RepeaterValidator(_formData, element.Repeater, repeaterData, validations, sectionPath, validationRules, duplicationChecks, _masterListData, _isAnchor, _formInstanceId, _formDataInstanceManager, _folderVersionServices, _formDesignServices, _formInstanceService);
                repeaterValidator.Validate(ref errorSection, ref maxAllowedErrorCount, isCBCMasterList);
            }
            else
            {
                try
                {
                    if (!String.Equals(element.Op, "I"))
                    {
                        var childRowIDProperty = elementData != null ? (JToken)elementData["RowIDProperty"] : null;

                        if (ruleProcessor.CheckForValidationEvaluationRequired())
                        {
                            if (ruleList.Count == 0)
                            {
                                ValidationDesign elementValidation = validations.Where(v => v.FullName.Equals(element.FullName)).FirstOrDefault();
                                JToken elementValue = elementData != null ? (JToken)elementData[element.GeneratedName] : null;
                                if (elementValidation != null)
                                    this.EvaluateValidation(elementValue, elementValidation, ref errorSection, ref maxAllowedErrorCount, element, rowIDProperty, rowNumber, ruleProcessor, _keyValue);
                                else if (element.DataType == "date")
                                {
                                    elementValidation = new ValidationDesign() { FullName = element.FullName, UIElementName = element.Name, IsRequired = false, IsError = false, Regex = "", ValidationMessage = "", HasMaxLength = false, MaxLength = 0, DataType = "date", IsActive = true };
                                    this.EvaluateValidation(elementValue, elementValidation, ref errorSection, ref maxAllowedErrorCount, element, rowIDProperty, rowNumber, ruleProcessor, _keyValue);
                                }
                            }

                            foreach (var rule in ruleList)
                            {
                                ValidationDesign elementValidation = validations.Where(v => v.FullName.Equals(element.FullName)).FirstOrDefault().DeepClone();
                                var elementValidationRules = this.validationRules.Where(v => v.UIELementID == element.ElementID).ToList();

                                ruleProcessor.EvaluateRequiredRules(ref elementValidation);
                                ruleProcessor.EvaluateErrorRules(ref elementValidation, rule);
                                ruleProcessor.EvaluateCustomRules(ref elementValidation);

                                JToken elementValue = elementData != null ? (JToken)elementData[element.GeneratedName] : null;
                                if (elementValidation != null)
                                    this.EvaluateValidation(elementValue, elementValidation, ref errorSection, ref maxAllowedErrorCount, element, rowIDProperty, rowNumber, ruleProcessor, _keyValue);
                                else if (element.DataType == "date")
                                {
                                    elementValidation = new ValidationDesign() { FullName = element.FullName, UIElementName = element.Name, IsRequired = false, IsError = false, Regex = "", ValidationMessage = "", HasMaxLength = false, MaxLength = 0, DataType = "date", IsActive = true };
                                    this.EvaluateValidation(elementValue, elementValidation, ref errorSection, ref maxAllowedErrorCount, element, rowIDProperty, rowNumber, ruleProcessor, _keyValue);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string customMsg = "An error occurred while validating document for Element ID: '" + element.ElementID + "'" + " For Path '" + element.FullName + "'";
                    Exception customException = new Exception(customMsg, ex);
                    ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
                }
            }
        }
    }
}
