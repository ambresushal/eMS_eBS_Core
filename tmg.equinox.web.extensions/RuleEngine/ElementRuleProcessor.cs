using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.FormInstanceManager;

namespace tmg.equinox.web.RuleEngine
{
    public class ElementRuleProcessor : RuleProcessor
    {
        public ElementRuleProcessor(JObject formData, FormInstanceDataManager formDataInstanceManager, int formInstanceId, FormDesignVersionDetail detail, string sectionName, IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, IFormInstanceService formInstanceService)
        {
            this._formData = formData;
            this._formDataInstanceManager = formDataInstanceManager;
            this._formInstanceId = formInstanceId;
            this._detail = detail;
            this._sectionName = sectionName;
            this._folderVersionServices = folderVersionServices;
            this._formDesignServices = formDesignServices;
            this._formInstanceService = formInstanceService;
        }

        //Process a rule object
        public override bool ProcessRule(RuleDesign rule)
        {
            if (rule.TargetPropertyTypeId == (int)TargetPropertyTypes.CustomRule)
                return this.ProcessNode(rule, rule.RootExpression, this._formData);
            //Pass a root expression to ProcessNode method
            bool result = this.ProcessNode(rule.RootExpression, this._formData, rule);
            return result;
        }

        //Set an expression evaluated value to an element in form data
        public void SetElementPropertyValue(string elementFullName, string value)
        {
            JToken dataPart = this._formData;
            var nameParts = elementFullName.Split('.');
            for (var idx = 0; idx < nameParts.Length - 1; idx++)
            {
                if (idx == 0)
                {
                    dataPart = this._formData[nameParts[idx]];
                }
                else
                {
                    if (dataPart[nameParts[idx]] != null)
                    {
                        dataPart = dataPart[nameParts[idx]];
                    }
                }
            }
            if (value == null)
            {
                value = "";
            }

            if (value != " " && dataPart.HasValues)
            {
                if (dataPart[nameParts[nameParts.Length - 1]] != null)
                {
                    // If value is multiselct dropdown
                    if (value.StartsWith("[") && value.EndsWith("]") && !string.Equals(value, "[Select One]"))
                    {
                        dataPart[nameParts[nameParts.Length - 1]] = JToken.Parse(value);
                    }
                    else
                    {
                        dataPart[nameParts[nameParts.Length - 1]] = value;
                    }
                }
            }
        }

    }
}