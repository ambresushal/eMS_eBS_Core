using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.globalUtility;
using tmg.equinox.document.rulebuilder.model;
using tmg.equinox.document.rulebuilder.jsonhelper;
using tmg.equinox.document.rulebuilder.executor;

namespace tmg.equinox.document.rulebuilder.ruleprocessor
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
                sourceDictionary.Add(sourceItem.SourceName, processedSourceToken);
            }
            return sourceDictionary;
        }

        private JToken ProcessSource(RuleSourceItem sourceItem)
        {
            JToken filterMergeOutput = null;
            {
                filterMergeOutput = _sources[sourceItem.SourceName];
            }
            return filterMergeOutput;
        }
    }
}
