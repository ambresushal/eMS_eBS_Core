using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tmg.equinox.web.FormInstanceProcessor
{
    public class RuleExecutionTreeSearch
    {
        private JObject _ruleTree;
        
        public RuleExecutionTreeSearch(JObject ruleTree)
        {
            _ruleTree = ruleTree;
        }

        //get the rules node  from ruletreeobject where "id"=ruleId
        public List<JObject> GetChildRules(int ruleId)
        {
            List<JObject> innerRules=new List<JObject>();
            return innerRules;
        }

        private string GetRuleType(int ruleId)
        {
            string ruleType = null;

            return ruleType;
        }
    }
}