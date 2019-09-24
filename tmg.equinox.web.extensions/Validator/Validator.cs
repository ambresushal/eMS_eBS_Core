using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;

namespace tmg.equinox.web.Validator
{
    public abstract class Validator
    {
        public JObject _formData { get; set; }
        public List<ValidationDesign> validations { get; set; }
        public List<RuleDesign> validationRules { get; set; }
        public List<DuplicationDesign> duplicationChecks { get; set; }
        public string sectionPath { get; set; }
        public bool _isAnchor { get; set; }

        public Validator(JObject formData, string sectionPath, List<ValidationDesign> validations, List<RuleDesign> validationRules, List<DuplicationDesign> duplicationChecks, bool isAnchor)
        {
            this._formData = formData;
            this.validations = validations;
            this.validationRules = validationRules;
            this.duplicationChecks = duplicationChecks;
            this.sectionPath = sectionPath;
            this._isAnchor = isAnchor;
        }

        #region Abstract Methods

        public abstract void Validate(ref ErrorSection errorSection, ref int maxAllowedErrorCount, bool isCBCMasterList);

        #endregion

        #region Concrete Methods

        protected void EvaluateValidation(JToken elementValue, ValidationDesign elementValidation, ref ErrorSection errorSection, ref int maxAllowedErrorCount, ElementDesign element, string rowIDProperty, int rowNumber, ValidationRuleProcessor ruleProcessor, string keyValue)
        {
            bool hasValidationError = false;
            string validationMessage = string.IsNullOrEmpty(elementValidation.ValidationMessage) ? "" : elementValidation.ValidationMessage;
            string value = elementValue == null ? "" : elementValue.ToString();

            hasValidationError = CheckForRequired(elementValidation, value, ref validationMessage);

            if (!hasValidationError && !string.IsNullOrEmpty(value))
                hasValidationError = CheckForRegex(elementValidation, value, ref validationMessage);

            if (hasValidationError)
            {
                if (AddErrorRow(element, ref errorSection, value, validationMessage, rowIDProperty, rowNumber, ruleProcessor, keyValue))
                    maxAllowedErrorCount--;
            }
            else if (elementValidation.IsError)
            {
                if (AddErrorRow(element, ref errorSection, value, validationMessage == "" ? ValidationMessage.ruleErrorMsg : validationMessage, rowIDProperty, rowNumber, ruleProcessor, keyValue))
                    maxAllowedErrorCount--;
            }
        }

        protected bool AddErrorRow(ElementDesign element, ref ErrorSection errorSection, string value, string validationMessage, string rowIDProperty, int rowNumber, ValidationRuleProcessor ruleProcessor, string KeyValue)
        {
            bool result = true;
            if (ruleProcessor != null && !ruleProcessor.EvaluateVisibleEnableRules(element.Visible, element.Enabled, _isAnchor)) { return false; }
            ErrorRow errorRow = new ErrorRow()
            {
                ID = errorSection.ErrorRows.Count + 1,
                ElementID = GetElementName(element, rowNumber) + errorSection.FormInstanceID,
                Form = errorSection.Form,
                FormInstance = "tab" + errorSection.FormInstanceID,
                SectionID = errorSection.SectionID,
                Section = errorSection.Section,
                FormInstanceID = errorSection.FormInstanceID,
                SubSectionName = sectionPath,
                Field = element.Label,
                GeneratedName = element.GeneratedName,
                ColumnNumber = 0,
                RowIdProperty = rowIDProperty,
                RowNum = rowNumber > 0 ? rowNumber.ToString() : "",
                Description = String.Format(validationMessage, element.Label),
                Value = value,
                RepeaterGridID = "",
                KeyValue = string.IsNullOrEmpty(KeyValue) ? (rowNumber > 0 ? rowNumber.ToString() : "") : KeyValue
            };

            if (!CheckIfErrorRowExists(errorRow, errorSection))
                errorSection.ErrorRows.Add(errorRow);
            return result;
        }

        protected bool CheckIfErrorRowExists(ErrorRow errorRow, ErrorSection errorSection)
        {
            bool result = false;
            var rowExists = errorSection.ErrorRows.Where(s => s.ElementID == errorRow.ElementID && s.GeneratedName == errorRow.GeneratedName && s.KeyValue == errorRow.KeyValue && s.Description == errorRow.Description).FirstOrDefault();
            if (rowExists != null)
            {
                result = true;
            }
            return result;
        }

        protected bool CheckForRequired(ValidationDesign elementValidation, string value, ref string validationMessage)
        {
            bool hasValidationError = false;
            if (elementValidation.IsRequired.HasValue && elementValidation.IsRequired.Value)
            {
                if (!elementValidation.UIElementName.Contains("CheckBox"))
                {
                    if (string.IsNullOrEmpty(value) || value == "[Select One]")
                    {
                        hasValidationError = true;
                        validationMessage = ValidationMessage.requiredMsg;
                    }
                }
            }
            return hasValidationError;
        }

        protected bool CheckForRegex(ValidationDesign elementValidation, string value, ref string validationMessage)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            bool hasValidationError = false;
            var regexExpression = @elementValidation.Regex;
            if (string.IsNullOrEmpty(regexExpression) && !string.IsNullOrEmpty(elementValidation.DataType))
            {
                if (elementValidation.DataType == "int")
                {
                    regexExpression = @"^\d+$";
                    validationMessage = validationMessage == "" ? ValidationMessage.invalidIntMsg : validationMessage;
                }
                else if (elementValidation.DataType == "float")
                {
                    regexExpression = @"^[-+]?\d*\.?\d*$";
                    validationMessage = validationMessage == "" ? ValidationMessage.invalidFloatMsg : validationMessage;
                }
                else if (elementValidation.DataType == "date")
                {
                    regexExpression = @"^(((0?[1-9]|1[012])/(0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])/(29|30)|(0?[13578]|1[02])/31)/(19|[2-9]\d)\d{2}|0?2/29/((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00)))$";
                    validationMessage = validationMessage == "" ? ValidationMessage.invalidDateMsg : validationMessage;
                }
            }
            else if (!string.IsNullOrEmpty(regexExpression))
            {
                validationMessage = validationMessage == "" ? ValidationMessage.regexMsg : validationMessage;
            }
            if (!string.IsNullOrEmpty(regexExpression))
            {
                hasValidationError = !Regex.IsMatch(value, regexExpression);
            }
            return hasValidationError;
        }

        protected string GetElementName(ElementDesign element, int rowNumber)
        {
            string elementName = element.Name;
            if (rowNumber > 0)
            {
                elementName = String.Format(elementName.Replace("_1_", "_{0}_"), rowNumber);
            }
            return elementName;
        }

        #endregion
    }
}