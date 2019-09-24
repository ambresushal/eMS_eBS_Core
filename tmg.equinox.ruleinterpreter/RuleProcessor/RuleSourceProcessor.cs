using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.globalUtility;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpreter.jsonhelper;
using tmg.equinox.ruleinterpreter.executor;

namespace tmg.equinox.ruleinterpreter.ruleprocessor
{
    public class RuleSourceProcessor
    {
        RuleSourcesContainer _ruleSourceContainer;
        Dictionary<string, JToken> _sources;

        public RuleSourceProcessor(RuleSourcesContainer ruleSourceContainer, Dictionary<string, JToken> sources)
        {
            _ruleSourceContainer = ruleSourceContainer;
            _sources = sources;
        }

        public Dictionary<string, JToken> ProcessRuleSources()
        {
            Dictionary<string, JToken> sourceDictionary = new Dictionary<string, JToken>();

            foreach (RuleSourceItem sourceItem in _ruleSourceContainer.RuleSources)
            {
                JToken processedSourceToken = ProcessSource(sourceItem);
                if(processedSourceToken!=null)
                sourceDictionary.Add(sourceItem.SourceName, processedSourceToken);
            }
            return sourceDictionary;
        }

        private JToken ProcessSource(RuleSourceItem sourceItem)
        {
            JToken filterMergeOutput = null;
            {
                if(_sources.ContainsKey(sourceItem.SourceName))
                filterMergeOutput = _sources[sourceItem.SourceName];
            }
            return filterMergeOutput;
        }
    }
}
