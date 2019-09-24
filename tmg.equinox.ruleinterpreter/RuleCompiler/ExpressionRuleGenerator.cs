using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.interpreter;
using static tmg.equinox.ruleinterpreter.RuleCompiler.ExpressionRuleGeneratorModel;

namespace tmg.equinox.ruleinterpreter.RuleCompiler
{
    public static class ExpressionRuleGenerator
    {

        public static string GenerateRuleText(string formDesignName, string ruleType, string uiElementType, string targetelementpath, DocumentRuleModel docRuleModel)
        {
            var expressionRuleTemplate = new documentRuleModel();
            //expressionRuleTemplate.documentRule = new ExpressionRuleGeneratorModel.DocumentRule();
            expressionRuleTemplate.documentrule.targetelement = formDesignName + "[" + targetelementpath + (uiElementType == "section" ? "." : "") + "]";
            expressionRuleTemplate.documentrule.targetelementtype = uiElementType;
            expressionRuleTemplate.documentrule.ruletype = ruleType;
            // docRuleModel.Description=
            var JSONdata = docRuleModel.RuleJSON.Replace("\\", "");
            var ruleConditions = new ExpressionRuleGeneratorModel.RuleConditions();
            List<Sources> sources = JsonConvert.DeserializeObject<List<ExpressionRuleGeneratorModel.Sources>>(docRuleModel.ElementData);
            foreach (var val in sources)
            {
                ruleConditions.sources.Add(new Sources
                {
                    sourcedocumentfilter = val.sourcedocumentfilter.ToLower() == "no" ? null : val.sourcedocumentfilter.ToLower(),
                    sourceelement = val.sourceelement,
                    sourceelementtype = "repeater",
                    sourcename = val.sourcename,
                    sourceelementlabel = val.sourceelementlabel
                });
            }


            ruleConditions.sourcemergelist = new SourceMergeList();
            ruleConditions.sourcemergelist.sourcemergeactions = new List<SourcemMergeActions>();

            List<SourcemMergeActions> sourcemergelist = JsonConvert.DeserializeObject<List<ExpressionRuleGeneratorModel.SourcemMergeActions>>(docRuleModel.ElementData);


            ruleConditions.sourcemergelist.sourcemergeactions.Add(new SourcemMergeActions
            {
                sourcemergeexpression = JSONdata,
                sourcemergetype = "script"
            });

            expressionRuleTemplate.documentrule.ruleconditions = ruleConditions;

            var ExpressionRule = JsonConvert.SerializeObject(expressionRuleTemplate);

            return ExpressionRule;


        }


    }
}
