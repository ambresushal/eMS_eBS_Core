using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.FormInstanceManager;

namespace tmg.equinox.web.RuleEngine
{
    public class RepeaterRuleProcessor
    {
        private JObject formData { get; set; }
        private string _currentUserName;
        private FormInstanceDataManager _formDataInstanceManager;
        private int? _currentUserId;
        private IFormInstanceDataServices _formInstanceDataServices;
        private int _formInstanceId;
        private FormDesignVersionDetail _detail;
        private string _sectionName;
        private IFormInstanceService _formInstanceService;
        private IFormDesignService _formDesignServices;
        private IFolderVersionServices _folderVersionServices;
        public RepeaterRuleProcessor(JObject formData, FormInstanceDataManager formDataInstanceManager, int formInstanceId, FormDesignVersionDetail detail, string sectionName, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, IFormInstanceService formInstanceService)
        {
            this.formData = formData;
            this._formDataInstanceManager = formDataInstanceManager;
            this._formInstanceId = formInstanceId;
            this._detail = detail;
            this._sectionName = sectionName;
            this._folderVersionServices = folderVersionServices;
            this._formDesignServices = formDesignServices;
            this._formInstanceService = formInstanceService;
        }

        public void ProcessRepeaterRule(RuleDesign rule, int? parentRowId, int? childRowId)
        {
            var repeaterData = this.GetRepeaterDataFromElementName(rule.UIElementFullName);
            JArray data = repeaterData.data;
            if (rule.IsParentRepeater == true) { rule.RunForRow = true; };
            if (parentRowId.HasValue)
            {
                data = (JArray)data.Where(v => (int)v["RowIDProperty"] == parentRowId);
            }
            string repeaterName = repeaterData.repeaterName;
            if (string.IsNullOrEmpty(rule.ParentRepeaterType) || rule.ParentRepeaterType == "Dropdown")
            {
                if (rule.RunForRow)
                {
                    if (data != null && data.Count > 0)
                    {
                        foreach (JObject rowData in data)
                        {
                            if (rule.TargetKeyFilters != null && rule.TargetKeyFilters.Count > 0)
                            {
                                bool isValidRow = IsValidRowForComplexRule(rule, repeaterName, rowData, data);
                                if (!isValidRow)
                                    continue;
                            }
                            RepeaterRowRuleProcessor rowRuleProcessor = new RepeaterRowRuleProcessor(rule, repeaterName, rowData, this.formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName, _folderVersionServices, _formDesignServices, _formInstanceService);
                            bool result = rowRuleProcessor.ProcessRule(rule);
                            rowRuleProcessor.SetTargetForRow(rowData, result);
                        }
                    }
                }
                else
                {
                    ElementRuleProcessor elementRuleProcessor = new ElementRuleProcessor(formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName, _folderVersionServices, _formDesignServices, _formInstanceService);
                    bool result = elementRuleProcessor.ProcessRule(rule);
                    if (rule.TargetPropertyTypeId != (int)RuleProcessor.TargetPropertyTypes.Visible && rule.TargetPropertyTypeId != (int)RuleProcessor.TargetPropertyTypes.Enabled)
                    {
                        foreach (JObject rowData in data)
                        {
                            RepeaterRowRuleProcessor rowRuleProcessor = new RepeaterRowRuleProcessor(rule, repeaterName, rowData, this.formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName, _folderVersionServices, _formDesignServices, _formInstanceService);
                            rowRuleProcessor.SetTargetForRow(rowData, result);
                        }
                    }
                }
            }
            else if (rule.ParentRepeaterType == "In Line" || rule.ParentRepeaterType == "Child")
            {
                if (rule.RunForRow || rule.RunForParentRow)
                {
                    if (data != null && data.Count > 0)
                    {
                        foreach (JObject rowData in data)
                        {
                            RepeaterRowRuleProcessor rowRuleProcessor = new RepeaterRowRuleProcessor(rule, repeaterName, rowData, this.formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName, _folderVersionServices, _formDesignServices, _formInstanceService);
                            Dictionary<int, bool> results = rowRuleProcessor.ProcessRulesForChildRows(childRowId);
                            rowRuleProcessor.SetTargetForChildRows(rowData, parentRowId, childRowId, results);
                        }
                    }
                }
                else
                {
                    ElementRuleProcessor elementRuleProcessor = new ElementRuleProcessor(formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName, _folderVersionServices, _formDesignServices, _formInstanceService);
                    bool result = elementRuleProcessor.ProcessRule(rule);
                    if (rule.TargetPropertyTypeId != (int)RuleProcessor.TargetPropertyTypes.Visible && rule.TargetPropertyTypeId != (int)RuleProcessor.TargetPropertyTypes.Enabled)
                    {
                        Dictionary<int, bool> results = new Dictionary<int, bool>();
                        results.Add(1, result);
                        foreach (JObject rowData in data)
                        {
                            RepeaterRowRuleProcessor rowRuleProcessor = new RepeaterRowRuleProcessor(rule, repeaterName, rowData, this.formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName, _folderVersionServices, _formDesignServices, _formInstanceService);
                            rowRuleProcessor.SetTargetForChildRows(rowData, parentRowId.Value, childRowId, results);
                        }
                    }
                }
            }
        }

        private bool IsValidRowForComplexRule(RuleDesign rule, string repeaterName, JObject currentRow, JArray data)
        {
            var validRowData = false;
            JArray validRow = this.getFilteredRowData(rule.TargetKeyFilters, data);
            JObject _validRowObj = validRow.Count > 0 ? (JObject)validRow[0] : null;
            if (JToken.DeepEquals(_validRowObj, currentRow))
                validRowData = true;

            return validRowData;
        }
        private JArray getFilteredRowData(List<RepeaterKeyFilterDesign> filterKeys, JArray repeaterData)
        {
            JArray filteredRow = null;
            foreach (var filter in filterKeys)
            {
                string key = filter.RepeaterKeyName.Split('.')[filter.RepeaterKeyName.Split('.').Length - 1];
                string _value = filter.RepeaterKeyValue.ToString();
                repeaterData = new JArray(repeaterData.Where(v => v[key].ToString() == _value.ToString()));
            }
            if (repeaterData != null)
                filteredRow = repeaterData;

            return filteredRow;
        }
        private dynamic GetRepeaterDataFromElementName(string elementName)
        {
            string repeaterName = "";
            string[] nameParts = elementName.Split('.');
            JObject dataPart = formData;
            JArray dataToReturn = null;
            foreach (string partName in nameParts)
            {
                var objData = dataPart[partName];
                repeaterName = string.IsNullOrEmpty(repeaterName) ? partName : repeaterName + "." + partName;
                if (objData is JArray)
                {
                    dataToReturn = (JArray)objData;
                    break;
                }
                dataPart = (JObject)objData;
            }
            return new { data = dataToReturn, repeaterName = repeaterName };
        }
    }
}