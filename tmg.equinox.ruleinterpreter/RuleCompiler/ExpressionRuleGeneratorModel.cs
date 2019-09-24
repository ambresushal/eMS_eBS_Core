using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpreter.RuleCompiler
{
    public class ExpressionRuleGeneratorModel
    {
        public class documentRuleModel
        {
            public documentRuleModel()
            {
                documentrule = new DocumentRule();
            }

            public DocumentRule documentrule { get; set; }
        }
        public class DocumentRule
        {
            public DocumentRule()
            {
                ruleconditions = new RuleConditions();
            }
            public string targetelement { get; set; }
            public string ruletype { get; set; }
            public string targetelementtype { get; set; }
            public RuleConditions ruleconditions { get; set; }
        }

        public class RuleConditions
        {
            public RuleConditions()
            {
                sources = new List<Sources>();
                sourcemergelist = new SourceMergeList();
            }
            public List<Sources> sources { get; set; }
            public SourceMergeList sourcemergelist { get; set; }
        }

        public class Sources
        {
            public Sources()
            {
                sourcename = string.Empty;
                sourceelement = string.Empty;
                sourceelementtype = "repeater";
                sourcedocumentfilter = string.Empty;
                sourceformdesignid = 0;
                sourceelementlabel = string.Empty;
            }
            public string sourcename { get; set; }
            public string sourceelement { get; set; }
            public string sourceelementlabel { get; set; }
            public string sourceelementtype { get; set; }
            public string sourcedocumentfilter { get; set; }
            public int sourceformdesignid { get; set; }
        }

       
        public class SourceMergeList
        {
            public SourceMergeList()
            {
                sourcemergeactions = new List<SourcemMergeActions>();
               
            }
            public List<SourcemMergeActions> sourcemergeactions { get; set; }
        }

        public class SourcemMergeActions
        {
            public SourcemMergeActions()
            {
                sourcemergetype = "script";
                sourcemergeexpression = string.Empty;
            }
            public string sourcemergetype { get; set; }
            public string sourcemergeexpression { get; set; }
        }

        

    }


}