using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.FormInstanceManager;

namespace tmg.equinox.web.Validator
{
    public class RepeaterRowValidator : Validator
    {
        RepeaterDesign repeaterDesign { get; set; }
        JObject repeaterRowData { get; set; }
        int rowNumber { get; set; }
        Dictionary<string, List<string>> childDataSources { get; set; }
        public string _keyValue { get; set; }
        JToken _masterListData { get; set; }


        private FormInstanceDataManager _formDataInstanceManager;

        private IFormInstanceService _formInstanceService;
        private IFormDesignService _formDesignServices;
        private IFolderVersionServices _folderVersionServices;
        private int _formInstanceId;
        public RepeaterRowValidator(JObject formData, RepeaterDesign repeaterDesign, JObject repeaterRowData, List<ValidationDesign> validations, int rowNumber, string sectionPath, Dictionary<string, List<string>> childDataSources, List<RuleDesign> validationRules, List<DuplicationDesign> duplicationChecks, JToken masterListData, bool isAnchor, int formInstanceId, FormInstanceDataManager formDataInstanceManager, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, IFormInstanceService formInstanceService)
            : base(formData, sectionPath, validations, validationRules, duplicationChecks, isAnchor)
        {
            this._formData = formData;
            this.repeaterDesign = repeaterDesign;
            this.repeaterRowData = repeaterRowData;
            this.validations = validations;
            this.rowNumber = rowNumber;
            this.sectionPath = sectionPath;
            this.childDataSources = childDataSources;
            this.validationRules = validationRules;
            this._masterListData = masterListData;
            this._formInstanceId = formInstanceId;
            this._formDataInstanceManager = formDataInstanceManager;
            this._folderVersionServices = folderVersionServices;
            this._formDesignServices = formDesignServices;
            this._formInstanceService = formInstanceService;
        }

        public override void Validate(ref ErrorSection errorSection, ref int maxAllowedErrorCount, bool isCBCMasterList)
        {
            if (repeaterDesign != null)
            {
                string rowIDProperty = "";
                JToken rowIDPropertyObject = (JToken)repeaterRowData["RowIDProperty"];
                if (rowIDPropertyObject != null)
                    rowIDProperty = rowIDPropertyObject.ToString();
                else if (rowNumber > 0)
                    rowIDProperty = (rowNumber - 1).ToString();

                string keyValues = string.Empty;
                if (isCBCMasterList)
                {
                    foreach (var ele in repeaterDesign.Elements)
                    {
                        if (ele.IsKey)
                        {
                            JToken elementValue = repeaterRowData != null ? (JToken)repeaterRowData[ele.GeneratedName] : null;
                            if (elementValue != null && elementValue.ToString() != string.Empty)
                                keyValues += elementValue.ToString() + "#";
                        }
                    }

                    if (keyValues.Length > 0)
                        this._keyValue = keyValues.Substring(0, keyValues.Length - 1);
                    else
                        this._keyValue = keyValues;
                }

                foreach (var ele in repeaterDesign.Elements)
                {
                    if (ele.IsKey)
                    {
                        JToken elementValue = repeaterRowData != null ? (JToken)repeaterRowData[ele.GeneratedName] : null;
                        if (elementValue != null && elementValue.ToString() != string.Empty)
                            keyValues += elementValue.ToString() + "^";
                    }
                }

                this._keyValue = keyValues.TrimEnd(new char[] { '^' });

                var childElements = childDataSources.Values.SelectMany(s => s);
                var primaryElements = repeaterDesign.Elements.Where(e => !childElements.Contains(e.GeneratedName));

                foreach (var element in primaryElements)
                {
                    if (maxAllowedErrorCount == 0) break;
                    var elementValidator = new ElementValidator(_formData, element, repeaterRowData, validations, rowNumber, sectionPath, rowIDProperty, validationRules, repeaterRowData, duplicationChecks, _masterListData, false, _isAnchor, _formInstanceId, _formDataInstanceManager, _folderVersionServices, _formDesignServices, _formInstanceService);
                    elementValidator._keyValue = keyValues.TrimEnd(new char[] { '^' });
                    elementValidator.Validate(ref errorSection, ref maxAllowedErrorCount, isCBCMasterList);
                }

                JObject rowData = (JObject)repeaterRowData.DeepClone();

                foreach (var childDataSource in childDataSources)
                {
                    if (maxAllowedErrorCount == 0) break;
                    JArray dataSourceElementData = null;
                    if (repeaterRowData[childDataSource.Key] is JArray)
                        dataSourceElementData = (JArray)repeaterRowData[childDataSource.Key];

                    var childDesignElements = repeaterDesign.Elements.Where(e => childDataSource.Value.Contains(e.GeneratedName));
                    if (dataSourceElementData != null)
                    {
                        foreach (JObject childDataSourceRow in dataSourceElementData)
                        {
                            rowData[childDataSource.Key] = childDataSourceRow;
                            if (maxAllowedErrorCount == 0) break;
                            foreach (var element in childDesignElements)
                            {
                                if (maxAllowedErrorCount == 0) break;
                                var elementValidator = new ElementValidator(_formData, element, childDataSourceRow, validations, rowNumber, sectionPath, rowIDProperty, validationRules, rowData, duplicationChecks, _masterListData, false, _isAnchor, _formInstanceId, _formDataInstanceManager, _folderVersionServices, _formDesignServices, _formInstanceService);
                                elementValidator._keyValue = keyValues;
                                elementValidator.Validate(ref errorSection, ref maxAllowedErrorCount, isCBCMasterList);
                            }
                        }
                    }
                }
            }
        }

        public string GetRepeaterKey()
        {
            string keyValues = string.Empty;
            foreach (var ele in repeaterDesign.Elements)
            {
                if (ele.IsKey)
                {
                    JToken elementValue = repeaterRowData != null ? (JToken)repeaterRowData[ele.GeneratedName] : null;
                    if (elementValue != null && elementValue.ToString() != string.Empty)
                        keyValues += elementValue.ToString() + "^";
                }
            }

            return keyValues.TrimEnd(new char[] { '^' });
        }
    }
}
