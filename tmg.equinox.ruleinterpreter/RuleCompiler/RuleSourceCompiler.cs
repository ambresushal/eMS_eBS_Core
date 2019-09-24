using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.globalUtility;
using tmg.equinox.ruleinterpreter.model;

namespace tmg.equinox.ruleinterpreter.rulecompiler
{
    class RuleConditionCompiler
    {
        RuleConditions _ruleCondition;
        public RuleConditionCompiler(RuleConditions rulecondition)
        {
            _ruleCondition = rulecondition;
        }

        public RuleSourcesContainer Compile()
        {
            RuleSourcesContainer container = new RuleSourcesContainer();
            SourceMergeListCompiler mergeListCompiler = new SourceMergeListCompiler(_ruleCondition.sourcemergelist);
            container.RuleSources = CompileSourceItems(_ruleCondition);
            container.SourceMergeList = mergeListCompiler.GetCompiledSourceMergeList();

            return container;
        }

        private List<RuleSourceItem> CompileSourceItems(RuleConditions mapping)
        {
            List<RuleSourceItem> items = new List<RuleSourceItem>();
            foreach (var source in mapping.sources)
            {
                items.Add(CompileSourceItem(source));
            }
            return items;
        }

        private RuleSourceItem CompileSourceItem(Source source)
        {
            RuleSourceItem item = new RuleSourceItem();
            item.SourceName = source.sourcename;
            item.SourcePath = source.sourceelement;
            item.SourceDocumentFilter = source.sourcedocumentfilter;
            return item;
        }
    }
}
