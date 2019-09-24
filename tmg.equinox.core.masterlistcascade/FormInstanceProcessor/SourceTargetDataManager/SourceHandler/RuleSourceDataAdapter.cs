using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.forminstanceprocessor.expressionbuilder;

namespace tmg.equinox.expressionbuilder
{
    public class RuleSourceDataAdapter
    {
        public Dictionary<string, JToken> GetRuleSourceData(int folderVersionId, Dictionary<string, int> ruleAliseformInstances, RuleSourcesContainer sourceContailer, SourceHandlerDBManager dbHandlerManager, FormDesignVersionDetail sourceDesign, CurrentRequestContext requestContext)
        {
            Dictionary<string, string> sources = sourceContailer.RuleSources.Select(sel => sel).ToDictionary(x => x.SourceName, x => x.SourcePath);

            SourceGroupingWrapper sourceGrouping = new SourceGroupingWrapper(sources, ruleAliseformInstances);
            List<SourceHandlerInput> handlerInputs = sourceGrouping.GetSourceHandlerInput();
            Dictionary<string, JToken> sourceOutputs = new Dictionary<string, JToken>();

            foreach (var handlerInput in handlerInputs)
            {
                IRuleSourceHandler sourceHandler = RuleSourceFactory.GetSourceHandler(folderVersionId, handlerInput.FormInstanceId, handlerInput, dbHandlerManager, sourceDesign, requestContext);
                Dictionary<string, JToken> sourceData = sourceHandler.GetSourceData();
                var sourceKeys = sourceData.Keys.ToList();
                sourceKeys.Sort();

                foreach (var item in sourceKeys)
                {
                    if (!sourceOutputs.ContainsKey(item))
                    {
                        sourceOutputs.Add(item, sourceData[item]);
                    }
                }
            }
            return sourceOutputs;
        }
    }
}
