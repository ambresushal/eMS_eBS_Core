using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.forminstanceprocessor.expressionbuilder;

namespace tmg.equinox.expressionbuilder
{
    public  class RuleTargetDataAdapter
    {
        public JToken GetTargetJSONToken(int folderVersionId, Dictionary<string, int> ruleAliseformInstances, string targetPath, SourceHandlerDBManager sourceHandlerManager, FormDesignVersionDetail sourceDesign, CurrentRequestContext requestContext)
        {
            Dictionary<string, string> targetPartDic = new Dictionary<string, string>();
            targetPartDic.Add(targetPath, targetPath);

            JToken targetToken = null;
            SourceGroupingWrapper sourceGrouping = new SourceGroupingWrapper(targetPartDic, ruleAliseformInstances);
            List<SourceHandlerInput> handlerInputs = sourceGrouping.GetSourceHandlerInput();

            foreach (var handlerInput in handlerInputs)
            {
                SourceType sourceType = RuleSourceFactory.GetSourceType(handlerInput.FormInstanceId, handlerInput.RuleAlias, sourceHandlerManager, requestContext);
                if (sourceType == SourceType.MasterList)
                {
                    handlerInput.SourceType = "MasterList";
                }
                else
                {
                    handlerInput.SourceType = "NonMasterList";
                }
                IRuleSourceHandler sourceHandler = RuleSourceFactory.GetSourceHandler(folderVersionId, handlerInput.FormInstanceId, handlerInput, sourceHandlerManager, sourceDesign,requestContext);
                Dictionary<string, JToken> sourceData = sourceHandler.GetSourceData();
                targetToken = sourceData.First().Value;
            }
            return targetToken;
        }
    }
}
