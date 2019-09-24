using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;

namespace tmg.equinox.web.FormInstanceProcessor.ExpressionBuilder
{
    public class CurrentRequestContext
    {
        public Dictionary<string, int> RuleAliasesLoadedForSection { get; set; }
        public Dictionary<string, List<int>> MultipleRuleAliasesLoadedForSection { get; set; }
        public Dictionary<string, bool> RuleAliasesMasterListMaps { get; set; }
        public Dictionary<string, Dictionary<string, JToken>> RuleAliasesMasterListDataMaps { get; set; }
        public Dictionary<string, JObject> RuleAliasesNonMasterListDataMaps { get; set; }

        public Dictionary<int, FormDesignVersionDetail> FormDesignVersionMaps { get; set; }

        public Dictionary<int, int> FormInstanceDesignVersionMap { get; set; }
        public Dictionary<string, dynamic> ExpressionRuleActivityLog { get; set; }

        public CurrentRequestContext()
        {
            RuleAliasesLoadedForSection = new Dictionary<string, int>();
            MultipleRuleAliasesLoadedForSection = new Dictionary<string, List<int>>();
            RuleAliasesMasterListMaps = new Dictionary<string, bool>();
            RuleAliasesMasterListDataMaps = new Dictionary<string, Dictionary<string, JToken>>();
            RuleAliasesNonMasterListDataMaps = new Dictionary<string, JObject>();
            FormDesignVersionMaps = new Dictionary<int, FormDesignVersionDetail>();
            FormInstanceDesignVersionMap = new Dictionary<int, int>();
            ExpressionRuleActivityLog = new Dictionary<string, dynamic>();
        }
    }
}