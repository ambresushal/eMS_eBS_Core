using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.model;

namespace tmg.equinox.ruleinterpreter.ruleprocessor
{
    public class RuleProcessor
    {
        CompiledDocumentRule _compiledRule;
        Dictionary<string, JToken> _sources;

        public RuleProcessor(CompiledDocumentRule compiledRule, Dictionary<string, JToken> sources)
        {
            _compiledRule = compiledRule;
            _sources = sources;
        }

        public Dictionary<string, JToken> ProcessSources()
        {
            Dictionary<string, JToken> processedSources;
            RuleSourceProcessor sourceProcessor = new RuleSourceProcessor(_compiledRule.SourceContainer, _sources);
            processedSources = sourceProcessor.ProcessRuleSources();
            return processedSources;
        }
    }
}
