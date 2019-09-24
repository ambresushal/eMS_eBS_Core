using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.CustomRule;
using System.Text.RegularExpressions;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.web.FormInstanceManager;

namespace tmg.equinox.web.Validator
{
    public class RepeaterValidator : Validator
    {
        RepeaterDesign repeaterDesign { get; set; }
        JArray repeaterData { get; set; }
        JToken _masterListData { get; set; }

        private FormInstanceDataManager _formDataInstanceManager;

        private IFormInstanceService _formInstanceService;
        private IFormDesignService _formDesignServices;
        private IFolderVersionServices _folderVersionServices;
        private int _formInstanceId;
        public RepeaterValidator(JObject formData, RepeaterDesign repeaterDesign, JArray repeaterData, List<ValidationDesign> validations, string sectionPath, List<RuleDesign> validationRules, List<DuplicationDesign> duplicationChecks, JToken masterListData, bool isAnchor, int formInstanceId, FormInstanceDataManager formDataInstanceManager, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, IFormInstanceService formInstanceService)
            : base(formData, sectionPath, validations, validationRules, duplicationChecks, isAnchor)
        {
            this._formData = formData;
            this.repeaterDesign = repeaterDesign;
            this.repeaterData = repeaterData;
            this.validations = validations;
            this.sectionPath = string.IsNullOrEmpty(sectionPath) ? repeaterDesign.Label : (sectionPath + " => " + repeaterDesign.Label);
            this.validationRules = validationRules;
            this.duplicationChecks = duplicationChecks;
            this._masterListData = masterListData;
            this._formInstanceId = formInstanceId;
            this._formDataInstanceManager = formDataInstanceManager;
            this._folderVersionServices = folderVersionServices;
            this._formDesignServices = formDesignServices;
            this._formInstanceService = formInstanceService;
        }

        public override void Validate(ref ErrorSection errorSection, ref int maxAllowedErrorCount, bool isCBCMasterList)
        {
            {
                int i = 1;
                if (repeaterData != null && !this.CheckIsDefaultRow())
                {
                    var childDataSources = getChildDataSources();
                    var duplicationCheckElements = GetDuplicationObjects(childDataSources);
                    var duplicateRows = GetDuplicateRows(repeaterData, duplicationCheckElements);
                    //Check repeater IsDataRequired bit 
                    if (this.repeaterDesign.IsDataRequired.Equals(true))
                        RepeaterDataRequired(this.repeaterDesign, validations, repeaterData, ref errorSection);

                    foreach (JObject repeaterRow in repeaterData)
                    {
                        if (maxAllowedErrorCount == 0) break;
                        var repeaterRowValidator = new RepeaterRowValidator(_formData, repeaterDesign, repeaterRow, validations, i, sectionPath, childDataSources, validationRules, duplicationChecks, _masterListData, _isAnchor, _formInstanceId, _formDataInstanceManager, _folderVersionServices, _formDesignServices, _formInstanceService);
                        repeaterRowValidator.Validate(ref errorSection, ref maxAllowedErrorCount, isCBCMasterList);

                        if (duplicateRows.Contains(i))
                        {
                            string keyValue = repeaterRowValidator.GetRepeaterKey();
                            AddDuplicationErrors(repeaterRow, ref errorSection, ref maxAllowedErrorCount, duplicationCheckElements, i, keyValue);
                        }
                        i++;
                    }
                }
            }
        }

        #region Helper Methods
        private Dictionary<string, List<string>> getChildDataSources()
        {
            Dictionary<string, List<string>> childDatasourceElementnames = new Dictionary<string, List<string>>();
            if (repeaterDesign.ChildDataSources != null)
            {
                foreach (var childDataSource in repeaterDesign.ChildDataSources)
                {
                    var dataSourceElements = new List<string>();
                    foreach (var mappping in childDataSource.Mappings)
                    {
                        dataSourceElements.Add(mappping.TargetElement);
                    }
                    if (dataSourceElements.Any())
                    {
                        childDatasourceElementnames.Add(childDataSource.DataSourceName, dataSourceElements);
                    }
                }
            }
            return childDatasourceElementnames;
        }

        private List<int> GetDuplicateRows(JArray repeaterData, List<DuplicationDesign> ducplicationCheckElements)
        {
            Dictionary<int, string> hashedRepeaterData = new Dictionary<int, string>();
            List<int> duplicateRows = new List<int>();
            int i = 1;
            foreach (JObject repeaterRow in repeaterData)
            {
                string rowHashData = GetRepeaterRowHash(repeaterRow, ducplicationCheckElements);

                if (!string.IsNullOrEmpty(rowHashData.Replace("#", "")))
                {
                    var duplicates = hashedRepeaterData.Where(s => s.Value == rowHashData);
                    if (duplicates.Any())
                    {
                        if (!duplicateRows.Contains(duplicates.FirstOrDefault().Key))
                        {
                            duplicateRows.Add(duplicates.FirstOrDefault().Key);
                        }
                        duplicateRows.Add(i);
                    }
                }
                hashedRepeaterData.Add(i, rowHashData);
                i++;
            }
            return duplicateRows.Distinct().ToList();
        }

        private string GetRepeaterRowHash(JObject repeateRowData, List<DuplicationDesign> ducplicationCheckElements)
        {
            string hashString = "";
            IDictionary<string, JToken> rowData = repeateRowData;
            foreach (var duplicationCheckElement in ducplicationCheckElements)
            {
                if (rowData.ContainsKey(duplicationCheckElement.GeneratedName))
                {
                    string value = rowData[duplicationCheckElement.GeneratedName].ToString().Trim();

                    if (duplicationCheckElement.Type == "select" || duplicationCheckElement.Type == "SelectInput" || duplicationCheckElement.Type == "Dropdown List" || duplicationCheckElement.Type == "Dropdown TextBox")
                    {
                        if (value == "[Select One]" || value == "[selectone]")
                        {
                            value = "";
                        }
                    }
                    hashString += value + "#";
                }
            }
            return hashString.ToLower();
        }

        private List<DuplicationDesign> GetDuplicationObjects(Dictionary<string, List<string>> childDataSources)
        {
            var childElements = childDataSources.Values.SelectMany(s => s);
            var primaryElements = repeaterDesign.Elements.Where(e => !childElements.Contains(e.GeneratedName)).Select(e => e.GeneratedName);
            var duplicationCheckElements = duplicationChecks.Where(e => e.ParentUIElementID.HasValue && e.ParentUIElementID.Value == repeaterDesign.ID && primaryElements.Contains(e.GeneratedName)).ToList();
            return duplicationCheckElements;
        }

        private void AddDuplicationErrors(JObject repeaterRow, ref ErrorSection errorSection, ref int maxAllowedErrorCount, List<DuplicationDesign> duplicationCheckElements, int rowNumber, string keyValue)
        {
            var elements = repeaterDesign.Elements.Where(e => duplicationCheckElements.Where(s => s.GeneratedName == e.GeneratedName).Any());
            foreach (var element in elements)
            {
                if (maxAllowedErrorCount == 0) break;

                string rowIDProperty = "";
                JToken rowIDPropertyObject = (JToken)repeaterRow["RowIDProperty"];
                if (rowIDPropertyObject != null)
                {
                    rowIDProperty = rowIDPropertyObject.ToString();
                }
                else if (rowNumber > 0)
                {
                    rowIDProperty = (rowNumber - 1).ToString();
                }
                string elemmentGenValue = string.Empty;

                if (repeaterRow[element.GeneratedName] != null)
                {
                    elemmentGenValue = repeaterRow[element.GeneratedName].ToString();
                    AddErrorRow(element, ref errorSection, elemmentGenValue, ValidationMessage.duplicateMsg, rowIDProperty, rowNumber, null, keyValue);
                }
            }
        }

        private void AddNonZeroErrors(JObject repeaterRow, ElementDesign element, ref ErrorSection errorSection, ref int maxAllowedErrorCount, int rowNumber, string keyValue, string generatedName)
        {
            //if (maxAllowedErrorCount == 0) {return;}
            string rowIDProperty = "";
            JToken rowIDPropertyObject = (JToken)repeaterRow["RowIDProperty"];
            if (rowIDPropertyObject != null)
            {
                rowIDProperty = rowIDPropertyObject.ToString();
            }
            else if (rowNumber > 0)
            {
                rowIDProperty = (rowNumber - 1).ToString();
            }
            {
                AddNonZeroErrorRow(element, ref errorSection, element.GeneratedName, ValidationMessage.nonZeroMsg, rowIDProperty, rowNumber, null, keyValue);
                //AddErrorRow(element, ref errorSection, repeaterRow[element.GeneratedName].ToString(), ValidationMessage.nonZeroMsg, rowIDProperty, rowNumber, null, keyValue);
            }
        }

        protected void AddNonZeroErrorRow(ElementDesign element, ref ErrorSection errorSection, string value, string validationMessage, string rowIDProperty, int rowNumber, ValidationRuleProcessor ruleProcessor, string KeyValue)
        {
            if (ruleProcessor != null && !ruleProcessor.EvaluateVisibleEnableRules(element.Visible, element.Enabled, _isAnchor)) { return; }
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
                GeneratedName = value,
                ColumnNumber = 0,
                RowIdProperty = rowIDProperty,
                RowNum = rowNumber > 0 ? rowNumber.ToString() : "",
                Description = String.Format(validationMessage, element.Label),
                Value = value,
                RepeaterGridID = "",
                KeyValue = string.IsNullOrEmpty(KeyValue) ? rowNumber.ToString() : KeyValue
            };
            errorSection.ErrorRows.Add(errorRow);
        }

        //Handle IsDataRequired validation for Repeater
        private void RepeaterDataRequired(RepeaterDesign repeaterDesign, List<ValidationDesign> validations, JArray repeaterData, ref ErrorSection errorSection)
        {
            List<ValidationDesign> RequiredFields = validations.Where(s => s.IsRequired.Equals(true)).ToList();
            ElementDesign element;
            bool HasItem = false;
            string RepeaterName = "";
            if (RequiredFields != null && repeaterData.Count() <= 0)
            {
                foreach (var item in RequiredFields)
                {
                    foreach (var repitem in repeaterDesign.Elements)
                    {
                        RepeaterName = repeaterDesign.Name;
                        if (item.UIElementName.Equals(repitem.Name))
                        {
                            HasItem = true;
                            var RepeaterRequiredFieldDetails = repeaterDesign.Elements.Where(s => s.FullName.Equals(item.FullName));
                            element = new ElementDesign
                            {
                                ElementID = RepeaterRequiredFieldDetails.Select(s => s.ElementID).FirstOrDefault(),
                                GeneratedName = RepeaterRequiredFieldDetails.Select(s => s.GeneratedName).FirstOrDefault(),
                                Name = RepeaterRequiredFieldDetails.Select(s => s.Name).FirstOrDefault(),
                                Label = RepeaterRequiredFieldDetails.Select(s => s.Label).FirstOrDefault(),

                            };
                            AddErrorRow(element, ref errorSection, "False", ValidationMessage.requiredMsg, RepeaterName, 0, null, null);
                        }
                        else
                        {
                            HasItem = false;
                        }
                        if (HasItem)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private bool CheckIsDefaultRow()
        {
            bool defaultRow = true;
            try
            {
                if (repeaterData != null && repeaterData.Count == 1)
                {
                    var elements = repeaterDesign.Elements;
                    if (elements != null && elements.Count > 0)
                    {
                        foreach (var element in elements)
                        {
                            if (!String.IsNullOrEmpty(Convert.ToString(repeaterData[0][element.GeneratedName] ?? String.Empty)))
                            {
                                defaultRow = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    defaultRow = false;
                }
            }
            catch (Exception ex)
            {
                defaultRow = false;
            }
            return defaultRow;
        }
        #endregion
    }
}