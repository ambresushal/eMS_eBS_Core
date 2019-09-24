using Newtonsoft.Json.Linq;

namespace tmg.equinox.expressionbuilder
{
    public class DocumentSaveRuleOptimizer
    {
        public JObject _formData;
        public DocumentSaveRuleOptimizer(string formData)
        {
            if (!string.IsNullOrEmpty(formData))
            {
                this._formData = JObject.Parse(formData);
            }
        }

        public bool hasPartChanged(string jsonPath, string newSectionData)
        {
            bool result = false;
            if (this._formData != null)
            {
                string preValue = _formData.SelectToken(jsonPath).ToString();
                string newValue = ((JObject)newSectionData).SelectToken(jsonPath).ToString();
                if (!string.Equals(preValue, newValue))
                {
                    result = true;
                }
            }

            return result;
        }

        public bool hasSectionChanged(string sectionName, string newSectionData)
        {
            bool result = false;
            if (this._formData != null)
            {
                string sectionData = _formData[sectionName].ToString();
                newSectionData = JObject.Parse(newSectionData).SelectToken(sectionName).ToString();
                if (!string.Equals(sectionData, newSectionData))
                {
                    result = true;
                }
            }

            return result;
        }
    }
}
