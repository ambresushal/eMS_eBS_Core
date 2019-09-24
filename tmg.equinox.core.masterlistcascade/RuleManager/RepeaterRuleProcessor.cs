using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.ruleprocessor;

namespace tmg.equinox.ruleengine
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
        public RepeaterRuleProcessor(JObject formData, FormInstanceDataManager formDataInstanceManager, int formInstanceId, FormDesignVersionDetail detail, string sectionName)
        {
            this.formData = formData;
            this._formDataInstanceManager = formDataInstanceManager;
            this._formInstanceId = formInstanceId;
            this._detail = detail;
            this._sectionName = sectionName;
        }

        public void ProcessRepeaterRule(RuleDesign rule, int? parentRowId, int? childRowId)
        {
            var repeaterData = this.GetRepeaterDataFromElementName(rule.UIElementFullName);
            JArray data = repeaterData.data;
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
                            RepeaterRowRuleProcessor rowRuleProcessor = new RepeaterRowRuleProcessor(rule, repeaterName, rowData, this.formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName);
                            bool result = rowRuleProcessor.ProcessRule(rule);
                            rowRuleProcessor.SetTargetForRow(rowData, result);
                        }
                    }
                }
                else
                {
                    ElementRuleProcessor elementRuleProcessor = new ElementRuleProcessor(formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName);
                    bool result = elementRuleProcessor.ProcessRule(rule);
                    if (rule.TargetPropertyTypeId != (int)RuleProcessor.TargetPropertyTypes.Visible && rule.TargetPropertyTypeId != (int)RuleProcessor.TargetPropertyTypes.Enabled)
                    {
                        foreach (JObject rowData in data)
                        {
                            RepeaterRowRuleProcessor rowRuleProcessor = new RepeaterRowRuleProcessor(rule, repeaterName, rowData, this.formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName);
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
                            RepeaterRowRuleProcessor rowRuleProcessor = new RepeaterRowRuleProcessor(rule, repeaterName, rowData, this.formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName);
                            Dictionary<int, bool> results = rowRuleProcessor.ProcessRulesForChildRows(childRowId);
                            rowRuleProcessor.SetTargetForChildRows(rowData, parentRowId, childRowId, results);
                        }
                    }
                }
                else
                {
                    ElementRuleProcessor elementRuleProcessor = new ElementRuleProcessor(formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName);
                    bool result = elementRuleProcessor.ProcessRule(rule);
                    if (rule.TargetPropertyTypeId != (int)RuleProcessor.TargetPropertyTypes.Visible && rule.TargetPropertyTypeId != (int)RuleProcessor.TargetPropertyTypes.Enabled)
                    {
                        Dictionary<int, bool> results = new Dictionary<int, bool>();
                        results.Add(1, result);
                        foreach (JObject rowData in data)
                        {
                            RepeaterRowRuleProcessor rowRuleProcessor = new RepeaterRowRuleProcessor(rule, repeaterName, rowData, this.formData, _formDataInstanceManager, _formInstanceId, _detail, _sectionName);
                            rowRuleProcessor.SetTargetForChildRows(rowData, parentRowId.Value, childRowId, results);
                        }
                    }
                }
            }
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