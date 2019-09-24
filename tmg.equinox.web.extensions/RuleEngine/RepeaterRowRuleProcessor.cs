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
    public class RepeaterRowRuleProcessor : RuleProcessor
    {
        JObject _rowData { get; set; }
        RuleDesign _rule { get; set; }

        public RepeaterRowRuleProcessor(RuleDesign rule, string containerName, JObject rowData, JObject formData, FormInstanceDataManager formDataInstance, int formInstanceId, FormDesignVersionDetail detail, string sectionName, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, IFormInstanceService formInstanceService)
        {
            this._rule = rule;
            this._containerName = containerName;
            this._rowData = rowData;
            this._formData = formData;
            this._formDataInstanceManager = formDataInstance;
            this._formInstanceId = formInstanceId;
            this._detail = detail;
            this._sectionName = sectionName;                        
            this._folderVersionServices = folderVersionServices;
            this._formDesignServices = formDesignServices;
            this._formInstanceService = formInstanceService;
        }

        public Dictionary<int, bool> ProcessRulesForChildRows(int? childRowID)
        {
            Dictionary<int, bool> results = new Dictionary<int, bool>();
            JObject parentRow = (JObject)_rowData.DeepClone();

            string childName = _rule.UIElementFullName.Replace(_containerName + ".", "");
            if (childName.IndexOf(".") > 0)
            {
                childName = childName.Split('.')[0];
            }
            var data = _rowData[childName];

            if (data != null)
            {
                if (childRowID.HasValue)
                {
                    if (_rule.ParentRepeaterType == "In Line")
                    {
                        data = data[childRowID];
                    }
                    else
                    {
                        data = (JObject)data.Where(d => (string)d["RowIDProperty"] == childRowID.ToString());
                    }
                }
                for (var idx = 0; idx < data.Count(); idx++)
                {
                    var childRowData = data[idx];
                    parentRow[childName] = childRowData;
                    bool result = this.ProcessNode(_rule.RootExpression, parentRow, _rule);
                    results.Add(idx, result);
                }
            }
            return results;
        }

        public override bool ProcessRule(RuleDesign rule)
        {
            return this.ProcessNode(rule.RootExpression, _rowData, rule);
        }

        public void SetValueForRuleTarget(bool result, JObject rowToFind)
        {
            if (_rule.TargetPropertyTypeId == (int)TargetPropertyTypes.Value)
            {
                if (result)
                {
                    if (_rule.IsResultSuccessElement)
                    {
                        if (rowToFind == null)
                        {
                            _rowData[_rule.UIElementName] = this.GetOperandValue(_rule.SuccessValueFullName, "", _rowData);
                        }
                        else
                        {
                            _rowData[_rule.UIElementName] = this.GetOperandValue(_rule.SuccessValueFullName, "", rowToFind);
                        }
                    }
                    else
                    {
                        _rowData[_rule.UIElementName] = _rule.SuccessValue;
                    }
                }
                else
                {
                    if (_rule.IsResultFailureElement)
                    {
                        if (rowToFind == null)
                        {
                            _rowData[_rule.UIElementName] = this.GetOperandValue(_rule.FailureValueFullName, "", _rowData);
                        }
                        else
                        {
                            _rowData[_rule.UIElementName] = this.GetOperandValue(_rule.FailureValueFullName, "", rowToFind);
                        }
                    }
                    else
                    {
                        _rowData[_rule.UIElementName] = _rule.FailureValue;
                    }
                }
            }
        }

        public void SetTargetForRow(JObject targetRow, bool result)
        {
            if (_rule.TargetPropertyTypeId == (int)TargetPropertyTypes.Value)
            {
                this.SetValueForRuleTarget(result, null);
            }
        }

        public void SetTargetForChildRows(JObject row, int? parentRowId, int? childRowId, Dictionary<int, bool> results)
        {
            string childName = _rule.UIElementFullName.Replace(_containerName + ".", "");
            if (childName.Contains("."))
            {
                childName = childName.Split('.')[0];
            }
            JArray dataPart = (JArray)row[childName];
            if (dataPart != null)
            {
                if (childRowId.HasValue)
                {
                    if (_rule.ParentRepeaterType == "In Line")
                    {
                        dataPart = (JArray)dataPart[childRowId];
                    }
                    else
                    {
                        dataPart = (JArray)(dataPart.Where(d => (string)d["RowIDProperty"] == childRowId.Value.ToString()));
                    }
                }
                for (int childIdx = 0; childIdx < dataPart.Count; childIdx++)
                {
                    bool result = results.FirstOrDefault().Value;
                    if (results.Count > 1)
                    {
                        result = results[childIdx];
                    }
                    if (_rule.TargetPropertyTypeId == (int)TargetPropertyTypes.Value)
                    {
                        this.SetValueForRuleTarget(result, (JObject)dataPart[childIdx]);
                    }
                }
            }
        }

    }
}