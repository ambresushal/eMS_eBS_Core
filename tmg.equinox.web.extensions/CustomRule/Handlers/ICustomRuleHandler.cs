using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace tmg.equinox.web.CustomRule
{
    public interface ICustomRuleHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="sectionData"></param>
        /// <param name="formData"></param>
        CustomRuleResult RunRulesForSection(string sectionName, JObject sectionData);

        string RunRulesForDocument(JObject oldFormData);

        bool HasCustomRules(string sectionName, JObject sectionData);
        
        void SetMasterListData(JObject masterListData);
    }

    public class CustomRuleResult
    {
        public List<SectionResult> updatedSections { get; set; }
    }

    public class SectionResult
    {
        public SectionResult(string sectionName, string sectionData)
        {
            SectionName = sectionName;
            SectionData = sectionData;
        }

        public string SectionName { get; set; }
        public string SectionData { get; set; }
    }
}
