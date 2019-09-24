using Newtonsoft.Json.Linq;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.ruleprocessor;

namespace tmg.equinox.ruleengine
{
    public class ElementRuleProcessor : RuleProcessor
    {
        public ElementRuleProcessor(JObject formData, FormInstanceDataManager formDataInstanceManager, int formInstanceId, FormDesignVersionDetail detail, string sectionName)
        {
            this._formData = formData;
            this._formDataInstanceManager = formDataInstanceManager;
            this._formInstanceId = formInstanceId;
            this._detail = detail;
            this._sectionName = sectionName;
        }

        //Process a rule object
        public override bool ProcessRule(RuleDesign rule)
        {
            //Pass a root expression to ProcessNode method
            bool result = this.ProcessNode(rule.RootExpression, this._formData);
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
                    dataPart[nameParts[nameParts.Length - 1]] = value;
                }
            }
        }
    }
}